using FluentAssertions;
using SolidPrinciples.OCP.Problem.ImageStorage;

namespace SolidPrinciples.OCP.Tests.Problem.ImageStorage;

/// <summary>
/// Tests que demuestran los problemas de la violación de OCP en almacenamiento de imágenes.
/// </summary>
/// <remarks>
/// Estos tests muestran:
/// 1. Testing difícil por acoplamiento a múltiples proveedores
/// 2. Configuración innecesaria para proveedores no usados
/// 3. Tests lentos (I/O real)
/// 4. Difícil hacer mocks
///
/// Comparar con Solution/ImageStorageTests.cs para ver la mejora.
/// </remarks>
public sealed class ImageUploadServiceTests
{
    [Fact]
    public async Task UploadImage_WithLocalStorage_SavesFileToFileSystem()
    {
        // Arrange
        // PROBLEMA: Necesito configuración de Azure aunque solo uso Local
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var service = new ImageUploadService(
            localBasePath: tempPath,
            azureBlobConnectionString: "irrelevant-connection-string", // Ruido
            azureContainerName: "irrelevant-container");              // Ruido

        var communityId = Guid.NewGuid();
        var image = new CommunityImageDto
        {
            CommunityId = communityId,
            FileName = "logo.png",
            ContentType = "image/png",
            ImageData = [1, 2, 3, 4, 5]
        };

        try
        {
            // Act
            var url = await service.UploadImageAsync(StorageType.Local, image);

            // Assert
            url.Should().StartWith("file://");
            url.Should().Contain(communityId.ToString());
            url.Should().EndWith("logo.png");

            // PROBLEMA: Test hace I/O real, es lento
            var filePath = url.Replace("file://", "");
            File.Exists(filePath).Should().BeTrue();
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, recursive: true);
            }
        }
    }

    [Fact]
    public async Task UploadImage_WithAzureBlob_ReturnsAzureBlobUrl()
    {
        // Arrange
        // PROBLEMA: Necesito configuración de Local aunque solo uso Azure
        var service = new ImageUploadService(
            localBasePath: "irrelevant-path",                    // Ruido
            azureBlobConnectionString: "test-connection-string",
            azureContainerName: "test-images");

        var image = new CommunityImageDto
        {
            CommunityId = Guid.NewGuid(),
            FileName = "avatar.jpg",
            ContentType = "image/jpeg",
            ImageData = [10, 20, 30, 40, 50]
        };

        // Act
        var url = await service.UploadImageAsync(StorageType.AzureBlob, image);

        // Assert
        url.Should().Contain("blob.core.windows.net");
        url.Should().Contain("test-images");
        url.Should().Contain(image.CommunityId.ToString());
        url.Should().EndWith("avatar.jpg");
    }

    [Fact]
    public async Task UploadImage_WithUnsupportedType_ThrowsException()
    {
        // Arrange
        var service = new ImageUploadService(
            localBasePath: "C:\\temp",
            azureBlobConnectionString: "conn",
            azureContainerName: "container");

        var image = new CommunityImageDto
        {
            CommunityId = Guid.NewGuid(),
            FileName = "test.png",
            ContentType = "image/png",
            ImageData = [1, 2, 3]
        };

        // Act
        var act = async () => await service.UploadImageAsync((StorageType)999, image);

        // Assert
        await act.Should().ThrowAsync<NotSupportedException>();
    }

    [Fact]
    public void Constructor_RequiresAllProviderConfigurations()
    {
        // PROBLEMA EDUCATIVO: Este test demuestra el acoplamiento
        // Incluso si solo uso Local, necesito pasar configuración de Azure

        // Arrange & Act
        var service = new ImageUploadService(
            localBasePath: "C:\\images",
            azureBlobConnectionString: "DefaultEndpointsProtocol=https;...",
            azureContainerName: "community-images");

        // Assert
        service.Should().NotBeNull();

        // El constructor obliga a conocer TODOS los proveedores
        // Agregar AWS S3 = agregar 2 parámetros más al constructor
        // Violación de OCP: el constructor crece con cada proveedor
    }

    [Fact]
    public async Task UploadImage_DemonstratesSwitchStatement()
    {
        // Este test demuestra que el switch en UploadImageAsync es el problema

        // Arrange
        var service = new ImageUploadService("C:\\temp", "conn", "container");
        var image = new CommunityImageDto
        {
            CommunityId = Guid.NewGuid(),
            FileName = "test.png",
            ContentType = "image/png",
            ImageData = [1, 2, 3]
        };

        // Act - Llamar con diferentes tipos requiere switch interno
        var localUrl = await service.UploadImageAsync(StorageType.Local, image);
        var azureUrl = await service.UploadImageAsync(StorageType.AzureBlob, image);

        // Assert
        localUrl.Should().StartWith("file://");
        azureUrl.Should().Contain("blob.core.windows.net");

        // PROBLEMA: Para agregar AWS S3, necesitaríamos:
        // 1. Agregar StorageType.AmazonS3
        // 2. Modificar el switch en UploadImageAsync
        // 3. Agregar método SaveToAmazonS3Async
        // 4. Modificar este constructor
        //
        // = Modificación de código existente 
    }
}

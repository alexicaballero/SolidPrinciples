using FluentAssertions;
using NSubstitute;
using SolidPrinciples.OCP.Solution.ImageStorage;

namespace SolidPrinciples.OCP.Tests.Solution.ImageStorage;

/// <summary>
/// Tests que demuestran los beneficios de seguir OCP en almacenamiento de imágenes.
/// </summary>
/// <remarks>
/// Comparado con Problem/ImageStorageTests.cs, estos tests son:
/// 1. Más fáciles de escribir (mock simple de IImageStore)
/// 2. Más rápidos (sin I/O real)
/// 3. Más enfocados (testean una cosa a la vez)
/// 4. Independientes (cada proveedor se testea en su propia clase)
///
/// Demuestran que el código que sigue OCP es más testeable.
/// </remarks>
public sealed class ImageUploadServiceTests
{
    [Fact]
    public async Task UploadImage_DelegatesToConfiguredStore()
    {
        // Arrange
        // MEJORA: Mock simple de IImageStore
        var mockStore = Substitute.For<IImageStore>();
        mockStore.SaveAsync(Arg.Any<CommunityImageDto>())
            .Returns("https://test.url/image.png");

        var service = new ImageUploadService(mockStore);
        var image = new CommunityImageDto
        {
            CommunityId = Guid.NewGuid(),
            FileName = "logo.png",
            ContentType = "image/png",
            ImageData = [1, 2, 3, 4, 5]
        };

        // Act
        var url = await service.UploadImageAsync(image);

        // Assert
        url.Should().Be("https://test.url/image.png");
        await mockStore.Received(1).SaveAsync(image);

        // Test rápido, sin I/O
        // Verifica comportamiento sin conocer la implementación
    }

    [Fact]
    public async Task UploadImage_PassesImageDataToStore()
    {
        // Arrange
        var capturedImage = default(CommunityImageDto);
        var mockStore = Substitute.For<IImageStore>();
        mockStore.SaveAsync(Arg.Do<CommunityImageDto>(img => capturedImage = img))
            .Returns("https://stored.url");

        var service = new ImageUploadService(mockStore);
        var expectedImage = new CommunityImageDto
        {
            CommunityId = Guid.NewGuid(),
            FileName = "avatar.jpg",
            ContentType = "image/jpeg",
            ImageData = [10, 20, 30]
        };

        // Act
        await service.UploadImageAsync(expectedImage);

        // Assert
        capturedImage.Should().Be(expectedImage);
        capturedImage!.CommunityId.Should().Be(expectedImage.CommunityId);
        capturedImage.FileName.Should().Be(expectedImage.FileName);
        capturedImage.ImageData.Should().BeEquivalentTo(expectedImage.ImageData);
    }

    [Fact]
    public void Constructor_OnlyNeedsStoreAbstraction()
    {
        // Arrange & Act
        var mockStore = Substitute.For<IImageStore>();
        var service = new ImageUploadService(mockStore);

        // Assert
        service.Should().NotBeNull();

        // MEJORA: Solo necesita IImageStore, no configuración de todos los proveedores
        // Agregar AWS S3 NO requiere cambiar este constructor
        // Cumple OCP: cerrado para modificación
    }
}

/// <summary>
/// Tests para LocalFileSystemImageStore.
/// </summary>
/// <remarks>
/// MEJORA: Cada proveedor se testea independientemente.
/// No necesitamos instanciar ImageUploadService para testear filesystem.
/// </remarks>
public sealed class LocalFileSystemImageStoreTests
{
    [Fact]
    public async Task SaveAsync_CreatesDirectoryAndSavesFile()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var store = new LocalFileSystemImageStore(tempPath);

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
            var url = await store.SaveAsync(image);

            // Assert
            url.Should().StartWith("file://");
            url.Should().Contain(communityId.ToString());
            url.Should().EndWith("logo.png");

            // Verificar que el archivo existe
            var filePath = url.Replace("file://", "");
            File.Exists(filePath).Should().BeTrue();

            // Verificar contenido
            var savedData = await File.ReadAllBytesAsync(filePath);
            savedData.Should().BeEquivalentTo(image.ImageData);
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
    public async Task SaveAsync_CreatesSubdirectoryPerCommunity()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var store = new LocalFileSystemImageStore(tempPath);

        var communityId = Guid.NewGuid();
        var image = new CommunityImageDto
        {
            CommunityId = communityId,
            FileName = "banner.jpg",
            ContentType = "image/jpeg",
            ImageData = [10, 20, 30]
        };

        try
        {
            // Act
            await store.SaveAsync(image);

            // Assert
            var expectedDirectory = Path.Combine(tempPath, communityId.ToString());
            Directory.Exists(expectedDirectory).Should().BeTrue();
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, recursive: true);
            }
        }
    }
}

/// <summary>
/// Tests para AzureBlobImageStore.
/// </summary>
/// <remarks>
/// MEJORA: Tests aislados para Azure Blob, sin conocer filesystem.
/// </remarks>
public sealed class AzureBlobImageStoreTests
{
    [Fact]
    public async Task SaveAsync_ReturnsBlobUrl()
    {
        // Arrange
        var store = new AzureBlobImageStore(
            connectionString: "DefaultEndpointsProtocol=https;...",
            containerName: "community-images");

        var communityId = Guid.NewGuid();
        var image = new CommunityImageDto
        {
            CommunityId = communityId,
            FileName = "avatar.png",
            ContentType = "image/png",
            ImageData = [1, 2, 3]
        };

        // Act
        var url = await store.SaveAsync(image);

        // Assert
        url.Should().Contain("blob.core.windows.net");
        url.Should().Contain("community-images");
        url.Should().Contain(communityId.ToString());
        url.Should().EndWith("avatar.png");
    }

    [Fact]
    public async Task SaveAsync_IncludesCommunityIdInBlobPath()
    {
        // Arrange
        var store = new AzureBlobImageStore("conn", "images");
        var communityId = Guid.NewGuid();
        var image = new CommunityImageDto
        {
            CommunityId = communityId,
            FileName = "logo.jpg",
            ContentType = "image/jpeg",
            ImageData = [5, 10, 15]
        };

        // Act
        var url = await store.SaveAsync(image);

        // Assert
        // URL debe incluir: {community-id}/{filename}
        url.Should().Contain($"{communityId}/logo.jpg");
    }
}

/// <summary>
/// Tests de integración que demuestran intercambio de estrategias.
/// </summary>
/// <remarks>
/// MEJORA: Demuestra que ImageUploadService funciona con cualquier IImageStore.
/// </remarks>
public sealed class ImageUploadServiceIntegrationTests
{
    [Fact]
    public async Task UploadImage_WorksWithLocalFileSystem()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        IImageStore store = new LocalFileSystemImageStore(tempPath);
        var service = new ImageUploadService(store);

        var image = new CommunityImageDto
        {
            CommunityId = Guid.NewGuid(),
            FileName = "test.png",
            ContentType = "image/png",
            ImageData = [1, 2, 3]
        };

        try
        {
            // Act
            var url = await service.UploadImageAsync(image);

            // Assert
            url.Should().StartWith("file://");

            // CORRECTO: Mismo servicio, estrategia diferente
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, recursive: true);
            }
        }
    }

    [Fact]
    public async Task UploadImage_WorksWithAzureBlob()
    {
        // Arrange
        IImageStore store = new AzureBlobImageStore("conn", "images");
        var service = new ImageUploadService(store);

        var image = new CommunityImageDto
        {
            CommunityId = Guid.NewGuid(),
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            ImageData = [4, 5, 6]
        };

        // Act
        var url = await service.UploadImageAsync(image);

        // Assert
        url.Should().Contain("blob.core.windows.net");

        // CORRECTO: Mismo servicio, estrategia diferente
        // ImageUploadService NO cambió para soportar Azure
    }

    [Fact]
    public async Task Strategy_CanBeSwappedAtRuntime()
    {
        // Demuestra que el patrón Strategy permite intercambiar implementaciones

        // Arrange
        var image = new CommunityImageDto
        {
            CommunityId = Guid.NewGuid(),
            FileName = "logo.png",
            ContentType = "image/png",
            ImageData = [1, 2, 3]
        };

        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        try
        {
            // Act - Usar Local en desarrollo
            IImageStore devStore = new LocalFileSystemImageStore(tempPath);
            var devService = new ImageUploadService(devStore);
            var devUrl = await devService.UploadImageAsync(image);

            // Act - Usar Azure en producción
            IImageStore prodStore = new AzureBlobImageStore("conn", "images");
            var prodService = new ImageUploadService(prodStore);
            var prodUrl = await prodService.UploadImageAsync(image);

            // Assert
            devUrl.Should().StartWith("file://");
            prodUrl.Should().Contain("blob.core.windows.net");

            // CORRECTO: ImageUploadService funciona con cualquier estrategia
            // Agregar AWS S3 = crear AmazonS3ImageStore, sin tocar este código
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, recursive: true);
            }
        }
    }
}

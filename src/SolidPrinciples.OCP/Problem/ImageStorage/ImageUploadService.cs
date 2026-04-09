namespace SolidPrinciples.OCP.Problem.ImageStorage;

/// <summary>
/// VIOLACIÓN: Esta clase debe cambiar cada vez que se agrega un nuevo proveedor de almacenamiento.
/// </summary>
/// <remarks>
/// Problema educativo:
/// Esta clase viola el principio Abierto/Cerrado porque:
///
/// 1. Contiene un switch basado en StorageType
/// 2. Cada nuevo proveedor (AWS S3, Google Cloud Storage) requiere:
///    - Agregar un case al switch
///    - Agregar un método privado nuevo
///    - Modificar código existente y probado
///
/// 3. No está "cerrada para modificación":
///    - Agregar AWS S3 = editar esta clase
///    - Agregar Google Cloud = editar esta clase
///    - Agregar cualquier proveedor = editar esta clase
///
/// 4. Dificulta testing:
///    - No se puede testear un proveedor sin instanciar la clase completa
///    - Mocks complicados para filesystem/Azure SDK
///    - Tests lentos (I/O real)
///
/// Consecuencias en el mundo real:
/// - Merge conflicts constantes (todos tocan esta clase)
/// - Riesgo de romper código existente al agregar proveedores
/// - Imposible que equipos trabajen en paralelo en diferentes proveedores
/// - Testing costoso y lento
///
/// Ver Solution/ImageStorage para la versión correcta con patrón Strategy.
/// </remarks>
public sealed class ImageUploadService
{
    private readonly string _localBasePath;
    private readonly string _azureBlobConnectionString;
    private readonly string _azureContainerName;

    /// <summary>
    /// Constructor que requiere configuración para TODOS los proveedores.
    /// </summary>
    /// <remarks>
    /// VIOLACIÓN: Incluso si solo uso Local, necesito pasar configuración de Azure.
    /// Acoplamiento innecesario con todos los proveedores posibles.
    /// </remarks>
    public ImageUploadService(
        string localBasePath,
        string azureBlobConnectionString,
        string azureContainerName)
    {
        _localBasePath = localBasePath;
        _azureBlobConnectionString = azureBlobConnectionString;
        _azureContainerName = azureContainerName;
    }

    /// <summary>
    /// Sube una imagen al almacenamiento configurado.
    /// </summary>
    /// <param name="storageType">Tipo de almacenamiento a usar.</param>
    /// <param name="image">Datos de la imagen.</param>
    /// <returns>URL pública de la imagen subida.</returns>
    /// <remarks>
    /// VIOLACIÓN: Este switch crece con cada proveedor nuevo.
    /// Agregar AWS S3 requiere agregar un case aquí = modificar código existente.
    /// </remarks>
    public async Task<string> UploadImageAsync(StorageType storageType, CommunityImageDto image)
    {
        // VIOLACIÓN: Switch basado en tipo
        // Cada nuevo proveedor = modificar este switch
        return storageType switch
        {
            StorageType.Local =>
                await SaveToLocalFileSystemAsync(image),

            StorageType.AzureBlob =>
                await SaveToAzureBlobStorageAsync(image),

            // TODO: Agregar AWS S3 requiere modificar esta clase:
            // StorageType.AmazonS3 =>
            //     await SaveToAmazonS3Async(image),
            //
            // TODO: Agregar Google Cloud requiere modificar esta clase:
            // StorageType.GoogleCloud =>
            //     await SaveToGoogleCloudStorageAsync(image),

            _ => throw new NotSupportedException($"Storage type {storageType} is not supported")
        };
    }

    /// <summary>
    /// Guarda imagen en el sistema de archivos local.
    /// </summary>
    private async Task<string> SaveToLocalFileSystemAsync(CommunityImageDto image)
    {
        var directoryPath = Path.Combine(_localBasePath, image.CommunityId.ToString());
        Directory.CreateDirectory(directoryPath);

        var filePath = Path.Combine(directoryPath, image.FileName);
        await File.WriteAllBytesAsync(filePath, image.ImageData);

        return $"file://{filePath}";
    }

    /// <summary>
    /// Guarda imagen en Azure Blob Storage.
    /// </summary>
    /// <remarks>
    /// En un código real, usaríamos Azure.Storage.Blobs SDK:
    /// var blobClient = new BlobContainerClient(_azureBlobConnectionString, _azureContainerName);
    /// var blob = blobClient.GetBlobClient($"{image.CommunityId}/{image.FileName}");
    /// await blob.UploadAsync(new BinaryData(image.ImageData));
    /// </remarks>
    private async Task<string> SaveToAzureBlobStorageAsync(CommunityImageDto image)
    {
        // Simulación de subida a Azure Blob
        await Task.Delay(50); // Simula latencia de red

        var blobName = $"{image.CommunityId}/{image.FileName}";
        var blobUrl = $"https://{_azureContainerName}.blob.core.windows.net/{blobName}";

        return blobUrl;
    }

    // PROBLEMA: Agregar cualquier proveedor nuevo requiere:
    // 1. Agregar método privado aquí
    // 2. Agregar case al switch
    // 3. Modificar constructor (agregar parámetros de configuración)
    // 4. Re-testear toda la clase
    //
    // NO está "cerrado para modificación" 
}

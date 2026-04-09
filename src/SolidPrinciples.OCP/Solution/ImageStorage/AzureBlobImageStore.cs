namespace SolidPrinciples.OCP.Solution.ImageStorage;

/// <summary>
/// CORRECTO: Implementación de almacenamiento en Azure Blob Storage.
/// </summary>
/// <remarks>
/// Implementa IImageStore para Azure Blob Storage.
///
/// Cumple OCP:
/// 1. Extensión sin modificación: nueva clase, sin tocar código existente
/// 2. No modifica IImageStore
/// 3. No modifica ImageUploadService
/// 4. No modifica LocalFileSystemImageStore
///
/// En producción real:
/// - Usaríamos Azure.Storage.Blobs SDK
/// - BlobContainerClient para subir blobs
/// - Manejo de errores y reintentos
/// - Configuración de permisos y access tiers
///
/// Ejemplo con SDK real:
/// <code>
/// var blobClient = new BlobContainerClient(_connectionString, _containerName);
/// var blob = blobClient.GetBlobClient($"{image.CommunityId}/{image.FileName}");
/// await blob.UploadAsync(new BinaryData(image.ImageData), overwrite: true);
/// return blob.Uri.ToString();
/// </code>
///
/// Registrado en DI:
/// services.AddSingleton&lt;IImageStore, AzureBlobImageStore&gt;();
/// </remarks>
public sealed class AzureBlobImageStore : IImageStore
{
    private readonly string _connectionString;
    private readonly string _containerName;

    /// <summary>
    /// Crea una nueva instancia de AzureBlobImageStore.
    /// </summary>
    /// <param name="connectionString">Connection string de Azure Storage.</param>
    /// <param name="containerName">Nombre del container de blobs.</param>
    public AzureBlobImageStore(string connectionString, string containerName)
    {
        _connectionString = connectionString;
        _containerName = containerName;
    }

    /// <summary>
    /// Guarda la imagen en Azure Blob Storage.
    /// </summary>
    /// <param name="image">Datos de la imagen.</param>
    /// <returns>URL HTTPS del blob guardado.</returns>
    public async Task<string> SaveAsync(CommunityImageDto image)
    {
        // En producción real, usaríamos Azure.Storage.Blobs SDK
        // Para este ejemplo educativo, simulamos la subida

        // Simular latencia de red
        await Task.Delay(50);

        // Estructura del blob: {community-id}/{filename}
        var blobName = $"{image.CommunityId}/{image.FileName}";

        // URL del blob en Azure
        var blobUrl = $"https://{_containerName}.blob.core.windows.net/{blobName}";

        return blobUrl;
    }
}

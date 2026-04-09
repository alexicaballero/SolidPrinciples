namespace SolidPrinciples.OCP.Solution.ImageStorage;

/// <summary>
/// CORRECTO: Servicio de subida de imágenes que sigue OCP.
/// </summary>
/// <remarks>
/// Este servicio está:
/// 1. CERRADO para modificación: No cambia al agregar nuevos proveedores
/// 2. ABIERTO para extensión: Acepta cualquier implementación de IImageStore
///
/// Comparación con la versión del problema:
///
/// PROBLEMA (violación OCP):
/// - Switch basado en StorageType
/// - Métodos para cada proveedor en la misma clase
/// - Agregar proveedor = modificar esta clase
///
/// SOLUCIÓN (cumple OCP):
/// - Depende de abstracción IImageStore
/// - Inyección de dependencias
/// - Agregar proveedor = crear nueva clase, sin tocar esta
///
/// Ejemplo de uso:
///
/// Desarrollo (filesystem local):
/// <code>
/// var store = new LocalFileSystemImageStore("C:\\images");
/// var service = new ImageUploadService(store);
/// var url = await service.UploadImageAsync(image);
/// </code>
///
/// Producción (Azure Blob):
/// <code>
/// var store = new AzureBlobImageStore(connectionString, "images");
/// var service = new ImageUploadService(store);
/// var url = await service.UploadImageAsync(image);
/// </code>
///
/// Testing (mock):
/// <code>
/// var mockStore = Substitute.For&lt;IImageStore&gt;();
/// mockStore.SaveAsync(Arg.Any&lt;CommunityImageDto&gt;()).Returns("https://test.url");
/// var service = new ImageUploadService(mockStore);
/// </code>
///
/// Extensión futura sin modificar código:
/// - AWS S3: crear AmazonS3ImageStore : IImageStore
/// - Google Cloud: crear GoogleCloudImageStore : IImageStore
/// - FTP: crear FtpImageStore : IImageStore
///
/// ImageUploadService NO necesita cambiar para soportar estos. 
/// </remarks>
public sealed class ImageUploadService
{
    private readonly IImageStore _store;

    /// <summary>
    /// Crea una nueva instancia de ImageUploadService.
    /// </summary>
    /// <param name="store">Estrategia de almacenamiento a usar.</param>
    /// <remarks>
    /// CORRECTO: Solo necesita la abstracción, no configuración de todos los proveedores.
    /// El proveedor concreto se inyecta según el ambiente (dev/prod).
    /// </remarks>
    public ImageUploadService(IImageStore store)
    {
        _store = store;
    }

    /// <summary>
    /// Sube una imagen usando la estrategia configurada.
    /// </summary>
    /// <param name="image">Datos de la imagen a subir.</param>
    /// <returns>URL pública de la imagen subida.</returns>
    /// <remarks>
    /// CORRECTO: Este método NO cambia al agregar proveedores.
    /// Delega a la estrategia inyectada (_store).
    /// No necesita switch, if-else ni conocer los tipos concretos.
    /// </remarks>
    public Task<string> UploadImageAsync(CommunityImageDto image)
    {
        // Delega a la estrategia inyectada
        // No switch, no if-else, no modificación al agregar proveedores
        return _store.SaveAsync(image);
    }
}

namespace SolidPrinciples.OCP.Solution.ImageStorage;

/// <summary>
/// CORRECTO: Implementación de almacenamiento en sistema de archivos local.
/// </summary>
/// <remarks>
/// Implementa IImageStore para filesystem local.
///
/// Cumple OCP:
/// 1. No modifica IImageStore
/// 2. No modifica ImageUploadService
/// 3. Encapsula toda la lógica de filesystem en una clase
/// 4. Testeable independientemente
///
/// Responsabilidad única:
/// - Sabe CÓMO guardar en filesystem
/// - No sabe de otros proveedores
/// - No contiene lógica de negocio
///
/// Registrado en DI:
/// services.AddSingleton&lt;IImageStore, LocalFileSystemImageStore&gt;();
/// </remarks>
public sealed class LocalFileSystemImageStore : IImageStore
{
    private readonly string _basePath;

    /// <summary>
    /// Crea una nueva instancia de LocalFileSystemImageStore.
    /// </summary>
    /// <param name="basePath">Ruta base donde se guardarán las imágenes.</param>
    public LocalFileSystemImageStore(string basePath)
    {
        _basePath = basePath;
    }

    /// <summary>
    /// Guarda la imagen en el sistema de archivos local.
    /// </summary>
    /// <param name="image">Datos de la imagen.</param>
    /// <returns>URL file:// de la imagen guardada.</returns>
    public async Task<string> SaveAsync(CommunityImageDto image)
    {
        // Crear directorio por comunidad: /base/community-guid/
        var directoryPath = Path.Combine(_basePath, image.CommunityId.ToString());
        Directory.CreateDirectory(directoryPath);

        // Guardar archivo: /base/community-guid/filename.png
        var filePath = Path.Combine(directoryPath, image.FileName);
        await File.WriteAllBytesAsync(filePath, image.ImageData);

        // Retornar URL local
        return $"file://{filePath}";
    }
}

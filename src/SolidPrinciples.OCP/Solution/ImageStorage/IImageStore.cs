namespace SolidPrinciples.OCP.Solution.ImageStorage;

/// <summary>
/// CORRECTO: Abstracción para almacenar imágenes.
/// </summary>
/// <remarks>
/// Patrón Strategy aplicado:
///
/// Esta interfaz define el contrato para guardar imágenes.
/// Cada proveedor (Local, Azure, AWS, Google) implementa esta interfaz.
///
/// Beneficios de OCP:
/// 1. Cerrada para modificación: Esta interfaz NO cambia al agregar proveedores
/// 2. Abierta para extensión: Crear nueva clase que implementa IImageStore
/// 3. Testabilidad: Fácil hacer mock de IImageStore
/// 4. Equipos paralelos: Cada equipo implementa su proveedor independientemente
///
/// Ejemplos de extensión sin modificar código existente:
/// - Agregar AWS S3 = crear AmazonS3ImageStore : IImageStore
/// - Agregar Google Cloud = crear GoogleCloudImageStore : IImageStore
/// - Agregar FTP = crear FtpImageStore : IImageStore
///
/// El código que usa IImageStore (ImageUploadService) no necesita cambiar.
/// </remarks>
public interface IImageStore
{
    /// <summary>
    /// Guarda una imagen en el almacenamiento.
    /// </summary>
    /// <param name="image">Datos de la imagen a guardar.</param>
    /// <returns>URL pública de la imagen guardada.</returns>
    /// <remarks>
    /// Contrato estable:
    /// - Acepta una imagen
    /// - Retorna una URL
    /// - Las implementaciones deciden CÓMO guardar (filesystem, blob, S3, etc.)
    /// </remarks>
    Task<string> SaveAsync(CommunityImageDto image);
}

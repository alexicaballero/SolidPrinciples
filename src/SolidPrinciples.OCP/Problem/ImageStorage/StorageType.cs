namespace SolidPrinciples.OCP.Problem.ImageStorage;

/// <summary>
/// VIOLACIÓN: Este enum obliga a modificar ImageUploadService cada vez que se agrega un proveedor.
/// </summary>
/// <remarks>
/// Problema educativo:
/// - Agregar AWS S3 requiere modificar ImageUploadService.UploadImageAsync
/// - Viola el principio Abierto/Cerrado: no está "cerrado para modificación"
/// - Crea acoplamiento entre la lógica de negocio y los proveedores de almacenamiento
///
/// Señales de alerta:
/// - enum que representa "tipos" de comportamiento
/// - switch/if-else basado en este enum
/// - Cada nuevo valor del enum requiere cambios en múltiples lugares
/// </remarks>
public enum StorageType
{
    /// <summary>
    /// Almacenamiento en sistema de archivos local.
    /// </summary>
    Local,

    /// <summary>
    /// Almacenamiento en Azure Blob Storage.
    /// </summary>
    AzureBlob,

    // TODO: Agregar AWS S3 requiere:
    // 1. Agregar AmazonS3 aquí
    // 2. Modificar ImageUploadService.UploadImageAsync (agregar case)
    // 3. Modificar todos los switch que usan StorageType
    // Viola OCP: el código existente debe cambiar para nuevas funcionalidades
}

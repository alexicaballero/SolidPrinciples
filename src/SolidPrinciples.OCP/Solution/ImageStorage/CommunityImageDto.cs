namespace SolidPrinciples.OCP.Solution.ImageStorage;

/// <summary>
/// DTO para transferir datos de imagen de comunidad.
/// </summary>
/// <remarks>
/// Idéntico al DTO del problema - los DTOs son neutrales respecto a OCP.
/// La diferencia está en cómo ImageUploadService los procesa.
/// </remarks>
public sealed class CommunityImageDto
{
    /// <summary>
    /// ID de la comunidad a la que pertenece la imagen.
    /// </summary>
    public required Guid CommunityId { get; init; }

    /// <summary>
    /// Nombre del archivo (ej: "community-logo.png").
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Tipo de contenido MIME (ej: "image/png", "image/jpeg").
    /// </summary>
    public required string ContentType { get; init; }

    /// <summary>
    /// Bytes de la imagen.
    /// </summary>
    public required byte[] ImageData { get; init; }

    /// <summary>
    /// Tamaño del archivo en bytes.
    /// </summary>
    public int SizeInBytes => ImageData.Length;
}

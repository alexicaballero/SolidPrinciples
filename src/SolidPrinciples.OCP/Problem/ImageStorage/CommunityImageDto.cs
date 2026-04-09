namespace SolidPrinciples.OCP.Problem.ImageStorage;

/// <summary>
/// DTO para transferir datos de imagen de comunidad.
/// </summary>
/// <remarks>
/// Representa una imagen (logo de comunidad, avatar de miembro) en el dominio Gathering.
/// Este DTO es neutral - no tiene violaciones de OCP.
/// La violación está en ImageUploadService que lo consume.
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

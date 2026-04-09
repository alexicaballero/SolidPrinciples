namespace SolidPrinciples.Common;

/// <summary>
/// Entidad de dominio que representa una comunidad de práctica.
/// </summary>
/// <remarks>
/// Una comunidad es un grupo de profesionales que comparten conocimiento
/// a través de sesiones, recursos y colaboración.
/// </remarks>
public sealed class Community : AuditableEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid AdminId { get; private set; }

    // Constructor privado - usar factory method
    private Community() { }

    /// <summary>
    /// Crea una nueva comunidad.
    /// </summary>
    public static Result<Community> Create(string name, string description, Guid adminId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Community>(new Error("Community.Name", "El nombre es requerido"));

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<Community>(new Error("Community.Description", "La descripción es requerida"));

        if (adminId == Guid.Empty)
            return Result.Failure<Community>(new Error("Community.AdminId", "El ID del administrador es inválido"));

        var community = new Community
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            AdminId = adminId
        };

        return Result.Success(community);
    }

    /// <summary>
    /// Actualiza la descripción de la comunidad.
    /// </summary>
    public Result UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
            return Result.Failure(new Error("Community.Description", "La descripción no puede estar vacía"));

        Description = newDescription;
        ModifiedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }
}

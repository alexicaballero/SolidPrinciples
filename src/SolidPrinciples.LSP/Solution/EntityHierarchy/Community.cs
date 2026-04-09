using SolidPrinciples.Common;

namespace SolidPrinciples.LSP.Solution.EntityHierarchy;

/// <summary>
/// SOLUCIÓN: Community es otra entidad concreta sellada.
/// </summary>
/// <remarks>
/// Community demuestra que múltiples tipos concretos pueden compartir la misma base
/// jerarquía mientras mantienen el cumplimiento de LSP. Como Session:
/// - Usa el sistema de eventos de Entity sin modificación
/// - Se beneficia del seguimiento de timestamps de AuditableEntity
/// - Está sellada para prevenir violaciones del contrato
/// </remarks>
public sealed class Community : AuditableEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private Community() { }

    /// <summary>
    /// CORRECTO: Método factory que crea una Community válida y levanta evento de dominio.
    /// </summary>
    public static Result<Community> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Community>(new Error(
                "Community.InvalidName",
                "Community name cannot be empty"));

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<Community>(new Error(
                "Community.InvalidDescription",
                "Community description cannot be empty"));

        var community = new Community
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description
        };

        // CORRECTO: Usa Raise() heredado - funciona idénticamente a Session
        community.Raise(new CommunityCreatedDomainEvent(community.Id));

        return Result.Success(community);
    }
}

namespace SolidPrinciples.Common;

/// <summary>
/// Entidad de dominio que representa un miembro de una comunidad.
/// </summary>
/// <remarks>
/// Un miembro puede participar en sesiones, proponer contenido y colaborar
/// dentro de una o más comunidades.
/// </remarks>
public sealed class Member : AuditableEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public MemberRole Role { get; private set; }

    // Constructor privado - usar factory method
    private Member() { }

    /// <summary>
    /// Crea un nuevo miembro.
    /// </summary>
    public static Result<Member> Create(string name, string email, MemberRole role = MemberRole.Member)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Member>(new Error("Member.Name", "El nombre es requerido"));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return Result.Failure<Member>(new Error("Member.Email", "El email es inválido"));

        var member = new Member
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Role = role
        };

        return Result.Success(member);
    }

    /// <summary>
    /// Promueve al miembro a un rol superior.
    /// </summary>
    public Result Promote()
    {
        if (Role == MemberRole.Admin)
            return Result.Failure(new Error("Member.Promote", "El miembro ya es administrador"));

        Role = Role == MemberRole.Member ? MemberRole.Moderator : MemberRole.Admin;
        ModifiedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }
}

/// <summary>
/// Roles disponibles para miembros de la comunidad.
/// </summary>
public enum MemberRole
{
    Member,
    Moderator,
    Admin
}

namespace SolidPrinciples.DIP.Problem;

/// <summary>
/// PROBLEMA: Clase auxiliar simplificada para el ejemplo de violación.
/// En producción, usar Session de SolidPrinciples.Common con factory method y validaciones.
/// </summary>
public sealed class Session
{
    public Guid Id { get; }
    public string Title { get; }
    public string Description { get; }
    public Guid CommunityId { get; }

    // VIOLACIÓN ADICIONAL: Constructor público permite crear instancias inválidas
    public Session(Guid id, string title, string description, Guid communityId)
    {
        Id = id;
        Title = title;
        Description = description;
        CommunityId = communityId;
    }
}

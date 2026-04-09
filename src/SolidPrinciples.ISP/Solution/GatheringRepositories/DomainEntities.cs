using SolidPrinciples.Common;

namespace SolidPrinciples.ISP.Solution.GatheringRepositories;

/// <summary>
/// Clase base para entidades de dominio.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; }
}

/// <summary>
/// Entidad de sesión para el dominio Gathering.
/// </summary>
public sealed class Session : Entity
{
    public Guid CommunityId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Speaker { get; private set; } = string.Empty;
    public DateTimeOffset ScheduledAt { get; private set; }

    private Session() { }

    public static Result<Session> Create(Guid communityId, string title, string description, string speaker, DateTimeOffset scheduledAt)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Session>(new Error("Session.InvalidTitle", "Title cannot be empty"));

        var session = new Session
        {
            Id = Guid.NewGuid(),
            CommunityId = communityId,
            Title = title,
            Description = description,
            Speaker = speaker,
            ScheduledAt = scheduledAt
        };

        return Result.Success(session);
    }
}

/// <summary>
/// Entidad de comunidad para el dominio Gathering.
/// </summary>
public sealed class Community : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private Community() { }

    public static Result<Community> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Community>(new Error("Community.InvalidName", "Name cannot be empty"));

        var community = new Community
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description
        };

        return Result.Success(community);
    }
}

/// <summary>
/// Recurso de sesión (materiales, enlaces, archivos).
/// </summary>
public sealed class SessionResource : Entity
{
    public Guid SessionId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;

    public SessionResource(Guid sessionId, string title, string url)
    {
        Id = Guid.NewGuid();
        SessionId = sessionId;
        Title = title;
        Url = url;
    }
}

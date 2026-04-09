using FluentAssertions;
using SolidPrinciples.Common;
using SolidPrinciples.LSP.Problem.EntityHierarchy;
using Xunit;

namespace SolidPrinciples.LSP.Tests.Problem.EntityHierarchy;

/// <summary>
/// Tests que demuestran violaciones de LSP en la implementación del Problema.
/// </summary>
public sealed class EntityHierarchyProblemTests
{
    [Fact]
    public void RestrictedEntity_RaisingNonAuditEvent_ViolatesLSPByThrowingException()
    {
        // Arrange: Crear una sesión usando entidad restringida
        var session = RestrictedSession.Create("SOLID Principles Workshop");

        // Act & Assert: Intentar lanzar un evento normal viola el contrato
        // Esto demuestra FORTALECIMIENTO DE PRECONDICIÓN - la base acepta cualquier evento,
        // el subtipo requiere un tipo de evento específico
        var act = () => session.RaiseEvent(new SessionCreatedEvent(Guid.NewGuid()));

        act.Should().Throw<System.Reflection.TargetInvocationException>()
            .WithInnerException<ArgumentException>()
            .WithMessage("*only accepts AuditEvent*");
    }

    [Fact]
    public void BrokenAuditableEntity_NullCreatedAt_ViolatesInvariant()
    {
        // Arrange & Act: Crear sesión rota
        var session = BrokenSession.Create("LSP Fundamentals");

        // Assert: Invariante violada - CreatedAt siempre debe tener un valor
        // pero BrokenAuditableEntity permite null
        session.CreatedAt.Should().BeNull(); // ¡Este es el problema!

        // Esto rompe las consultas del repositorio que esperan timestamps
        var repository = new AuditRepository();
        var entities = new List<BrokenAuditableEntity> { session };

        // El repositorio debe verificar defensivamente si es null
        var recent = repository.GetRecentEntities(entities, DateTimeOffset.UtcNow.AddDays(-1));
        recent.Should().BeEmpty(); // La sesión se excluye porque CreatedAt es null
    }

    [Fact]
    public void EventDispatcher_WorksWithEntity_ButNotWithRestrictedEntity()
    {
        // Arrange: Crear una entidad normal
        var brokenSession = BrokenSession.Create("Domain Events Deep Dive");
        var dispatcher = new EventDispatcher();

        // Act: El despacho funciona con entidades normales
        var act = () => dispatcher.DispatchEvents(brokenSession);

        // Assert: Funciona bien
        act.Should().NotThrow();
        brokenSession.DomainEvents.Should().BeEmpty(); // Eventos borrados
    }

    [Fact]
    public void AuditRepository_RequiresDefensiveNullChecks_DueToViolation()
    {
        // Arrange: Crear entidades con y sin CreatedAt
        var validSession = BrokenSession.Create("Session 1");
        validSession.CreatedAt = DateTimeOffset.UtcNow.AddHours(-2);

        var invalidSession = BrokenSession.Create("Session 2");
        // invalidSession.CreatedAt permanece null

        var repository = new AuditRepository();
        var entities = new List<BrokenAuditableEntity> { validSession, invalidSession };

        // Act: Consultar entidades recientes
        var recent = repository.GetRecentEntities(
            entities,
            DateTimeOffset.UtcNow.AddDays(-1));

        // Assert: Solo la sesión válida devuelta - demuestra invariante rota
        recent.Should().ContainSingle();
        recent.Should().Contain(validSession);
        recent.Should().NotContain(invalidSession); // ¡Excluida debido a null!
    }

    [Fact]
    public void BrokenHierarchy_ForcesTypeCheckingAndDefensiveCoding()
    {
        // Este test demuestra el olor de código: se requiere programación defensiva

        // Arrange
        var session1 = BrokenSession.Create("Test 1");
        session1.CreatedAt = DateTimeOffset.UtcNow;

        var session2 = BrokenSession.Create("Test 2");
        // session2.CreatedAt is null

        var entities = new List<BrokenAuditableEntity> { session1, session2 };

        // Act: Code must constantly check for null
        var validEntities = entities
            .Where(e => e.CreatedAt.HasValue) // Defensive check needed!
            .ToList();

        // Assert: This is a code smell - we can't trust the type
        validEntities.Should().ContainSingle();
    }
}

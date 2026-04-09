using FluentAssertions;
using SolidPrinciples.LSP.Solution.EntityHierarchy;
using Xunit;

namespace SolidPrinciples.LSP.Tests.Solution.EntityHierarchy;

/// <summary>
/// Tests demonstrating LSP compliance in the Solution implementation.
/// </summary>
public sealed class EntityHierarchySolutionTests
{
    [Fact]
    public void Session_Create_RaisesDomainEventCorrectly()
    {
        // Arrange & Act: Create session using factory method
        var result = Session.Create(
            "SOLID Principles Masterclass",
            "Martin Fowler",
            DateTimeOffset.UtcNow.AddDays(7));

        // Assert: Domain event raised using inherited Entity.Raise()
        result.IsSuccess.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle();
        result.Value.DomainEvents.Should().ContainItemsAssignableTo<SessionCreatedDomainEvent>();
    }

    [Fact]
    public void Community_Create_RaisesDomainEventCorrectly()
    {
        // Arrange & Act: Create community using factory method
        var result = Community.Create(
            "Clean Architecture Community",
            "A community focused on software architecture best practices");

        // Assert: Domain event raised using inherited Entity.Raise()
        result.IsSuccess.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle();
        result.Value.DomainEvents.Should().ContainItemsAssignableTo<CommunityCreatedDomainEvent>();
    }

    [Fact]
    public void AuditableEntity_GuaranteesCreatedAtIsAlwaysSet()
    {
        // Arrange & Act: Create session
        var sessionResult = Session.Create(
            "LSP Deep Dive",
            "Barbara Liskov",
            DateTimeOffset.UtcNow.AddDays(14));

        var communityResult = Community.Create(
            "SOLID Practitioners",
            "Engineers practicing SOLID principles");

        // Assert: CreatedAt is ALWAYS set (non-nullable, constructor-initialized)
        sessionResult.Value.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        communityResult.Value.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

        // UpdatedAt starts as null (not yet updated)
        sessionResult.Value.UpdatedAt.Should().BeNull();
        communityResult.Value.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Session_Update_SetsUpdatedAtTimestamp()
    {
        // Arrange: Create and update session
        var sessionResult = Session.Create(
            "Original Title",
            "Original Speaker",
            DateTimeOffset.UtcNow.AddDays(7));

        var session = sessionResult.Value;
        var originalCreatedAt = session.CreatedAt;

        // Act: Update session
        var updateResult = session.Update("Updated Title", "Updated Speaker");

        // Assert: UpdatedAt is now set, CreatedAt unchanged
        updateResult.IsSuccess.Should().BeTrue();
        session.UpdatedAt.Should().NotBeNull();
        session.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        session.CreatedAt.Should().Be(originalCreatedAt); // Unchanged

        // Update raises new domain event
        session.DomainEvents.Should().HaveCount(2); // Created + Updated
        session.DomainEvents.Last().Should().BeOfType<SessionUpdatedDomainEvent>();
    }

    [Fact]
    public void EventDispatcher_WorksPolymorphically_WithAllEntityTypes()
    {
        // Arrange: Create different entity types
        var sessionResult = Session.Create(
            "Event-Driven Architecture",
            "Martin Fowler",
            DateTimeOffset.UtcNow.AddDays(10));

        var communityResult = Community.Create(
            "DDD Practitioners",
            "Domain-Driven Design community");

        var dispatcher = new EventDispatcher();

        // Act: Dispatch events from both entity types
        // ✓ LSP: Can substitute Session or Community for Entity
        dispatcher.DispatchEvents(sessionResult.Value);
        dispatcher.DispatchEvents(communityResult.Value);

        // Assert: Events cleared for both - polymorphism works!
        sessionResult.Value.DomainEvents.Should().BeEmpty();
        communityResult.Value.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void AuditableRepository_WorksWithSessionAndCommunity_WithoutDefensiveChecks()
    {
        // Arrange: Create repository and entities
        var sessionRepository = new AuditableRepository<Session>();
        var communityRepository = new AuditableRepository<Community>();

        var oldSessionResult = Session.Create(
            "Old Session",
            "Speaker",
            DateTimeOffset.UtcNow.AddDays(1));

        // Simulate old creation time
        var oldSession = oldSessionResult.Value;
        typeof(Session).BaseType!
            .GetProperty("CreatedAt")!
            .SetValue(oldSession, DateTimeOffset.UtcNow.AddDays(-10));

        var newSessionResult = Session.Create(
            "New Session",
            "Speaker",
            DateTimeOffset.UtcNow.AddDays(1));

        var communityResult = Community.Create("Community", "Description");

        sessionRepository.Add(oldSession);
        sessionRepository.Add(newSessionResult.Value);
        communityRepository.Add(communityResult.Value);

        // Act: Query recent entities - NO NULL CHECKS NEEDED!
        var recentSessions = sessionRepository.GetRecentEntities(
            DateTimeOffset.UtcNow.AddDays(-1));

        var recentCommunities = communityRepository.GetRecentEntities(
            DateTimeOffset.UtcNow.AddDays(-1));

        // Assert: ✓ CreatedAt está garantizado como no-null - seguro para consultar directamente
        recentSessions.Should().ContainSingle();
        recentSessions.Should().Contain(newSessionResult.Value);

        recentCommunities.Should().ContainSingle();
        recentCommunities.Should().Contain(communityResult.Value);
    }

    [Fact]
    public void GenericCode_WorksWithAnyAuditableEntity_DemonstratesLSP()
    {
        // Este test prueba LSP: el código genérico funciona con CUALQUIER subtipo de AuditableEntity

        // Arrange: Crear diferentes tipos de entidad
        var sessionResult = Session.Create(
            "Generic Code Test",
            "Test Speaker",
            DateTimeOffset.UtcNow.AddDays(5));

        var communityResult = Community.Create(
            "Generic Test Community",
            "Test Description");

        // Act: Usar repositorio genérico con diferentes tipos
        var repository1 = new AuditableRepository<Session>();
        repository1.Add(sessionResult.Value);

        var repository2 = new AuditableRepository<Community>();
        repository2.Add(communityResult.Value);

        var dispatcher = new EventDispatcher();
        repository1.PublishEvents(dispatcher);
        repository2.PublishEvents(dispatcher);

        // Assert: ✓ Funciona sin problemas - sin verificación de tipo, sin casos especiales
        sessionResult.Value.DomainEvents.Should().BeEmpty();
        communityResult.Value.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void SubtypesCanBeSubstitutedForBaseType_DemonstratesLSP()
    {
        // Este test demuestra el principio central de LSP:
        // "Los subtipos deben ser sustituibles por sus tipos base"

        // Arrange: Crear diferentes tipos concretos
        var sessionResult = Session.Create(
            "LSP Test Session",
            "Test Speaker",
            DateTimeOffset.UtcNow.AddDays(1));

        var communityResult = Community.Create("LSP Test Community", "Test");

        // Act: Treat all as Entity (base type)
        Entity entity1 = sessionResult.Value;
        Entity entity2 = communityResult.Value;

        // Assert: ✓ All Entity operations work correctly
        entity1.DomainEvents.Should().NotBeEmpty();
        entity2.DomainEvents.Should().NotBeEmpty();

        entity1.ClearDomainEvents();
        entity2.ClearDomainEvents();

        entity1.DomainEvents.Should().BeEmpty();
        entity2.DomainEvents.Should().BeEmpty();

        // Act: Treat all as AuditableEntity
        AuditableEntity auditable1 = sessionResult.Value;
        AuditableEntity auditable2 = communityResult.Value;

        // Assert: ✓ All AuditableEntity operations work correctly
        auditable1.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        auditable2.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Session_ValidationFailures_ReturnFailureResult()
    {
        // Arrange & Act: Attempt to create invalid sessions
        var emptyTitleResult = Session.Create(
            "",
            "Speaker",
            DateTimeOffset.UtcNow.AddDays(1));

        var emptySpeakerResult = Session.Create(
            "Title",
            "",
            DateTimeOffset.UtcNow.AddDays(1));

        var pastDateResult = Session.Create(
            "Title",
            "Speaker",
            DateTimeOffset.UtcNow.AddDays(-1));

        // Assert: All return Failure results (no exceptions thrown)
        emptyTitleResult.IsFailure.Should().BeTrue();
        emptyTitleResult.Error.Code.Should().Be("Session.InvalidTitle");

        emptySpeakerResult.IsFailure.Should().BeTrue();
        emptySpeakerResult.Error.Code.Should().Be("Session.InvalidSpeaker");

        pastDateResult.IsFailure.Should().BeTrue();
        pastDateResult.Error.Code.Should().Be("Session.InvalidSchedule");
    }
}

using FluentAssertions;
using SolidPrinciples.SRP.Solution;

namespace SolidPrinciples.SRP.Tests.Solution;

/// <summary>
/// SOLUCIÓN: Tests de entidad Session — CERO infraestructura necesaria.
/// Probamos la única responsabilidad de la entidad (estado del dominio + validación) en aislamiento puro.
/// </summary>
public sealed class SessionTests
{
  [Fact]
  public void Create_WithValidData_ReturnsSuccess()
  {
    // Arrange — Sin cadenas de conexión, sin SMTP, sin rutas de archivos. Solo datos del dominio.
    var communityId = Guid.NewGuid();
    var title = "SOLID Workshop";
    var speaker = "Jane Smith";
    var scheduledAt = DateTimeOffset.UtcNow.AddDays(7);

    // Act
    var result = Session.Create(communityId, title, speaker, scheduledAt);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Title.Should().Be(title);
    result.Value.Speaker.Should().Be(speaker);
    result.Value.CommunityId.Should().Be(communityId);
    result.Value.Status.Should().Be(SessionStatus.Scheduled);
  }

  [Fact]
  public void Create_WithEmptyTitle_ReturnsFailure()
  {
    // Act
    var result = Session.Create(Guid.NewGuid(), "", "Speaker", DateTimeOffset.UtcNow.AddDays(1));

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionErrors.TitleRequired);
  }

  [Fact]
  public void Create_WithEmptySpeaker_ReturnsFailure()
  {
    // Act
    var result = Session.Create(Guid.NewGuid(), "Title", "", DateTimeOffset.UtcNow.AddDays(1));

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionErrors.SpeakerRequired);
  }

  [Fact]
  public void Create_WithPastDate_ReturnsFailure()
  {
    // Act
    var result = Session.Create(Guid.NewGuid(), "Title", "Speaker", DateTimeOffset.UtcNow.AddDays(-1));

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionErrors.DateMustBeFuture);
  }

  [Fact]
  public void Create_WithValidData_RaisesSessionCreatedDomainEvent()
  {
    // Act
    var result = Session.Create(Guid.NewGuid(), "Title", "Speaker", DateTimeOffset.UtcNow.AddDays(7));

    // Assert — Los eventos de dominio prueban que la entidad comunica intención sin hacer trabajo de infraestructura
    result.Value.DomainEvents.Should().ContainSingle()
        .Which.Should().BeOfType<SessionCreatedDomainEvent>();
  }
}

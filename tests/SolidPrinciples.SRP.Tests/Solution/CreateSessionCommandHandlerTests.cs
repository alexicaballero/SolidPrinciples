using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SolidPrinciples.SRP.Solution;

namespace SolidPrinciples.SRP.Tests.Solution;

/// <summary>
/// SOLUCIÓN: Tests del handler con dependencias mockeadas.
/// Cada colaborador es un mock — probamos SOLO la lógica de orquestación.
///
/// Comparar con Problem/SessionManagerTests.cs donde no podíamos mockear nada.
/// </summary>
public sealed class CreateSessionCommandHandlerTests
{
  private readonly ISessionRepository _sessionRepository = Substitute.For<ISessionRepository>();
  private readonly INotificationService _notificationService = Substitute.For<INotificationService>();
  private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
  private readonly ILogger<CreateSessionCommandHandler> _logger = Substitute.For<ILogger<CreateSessionCommandHandler>>();
  private readonly CreateSessionCommandHandler _sut;

  public CreateSessionCommandHandlerTests()
  {
    _sut = new CreateSessionCommandHandler(
        _sessionRepository,
        _notificationService,
        _unitOfWork,
        _logger);
  }

  [Fact]
  public async Task HandleAsync_WithValidCommand_ReturnsSuccessWithSessionId()
  {
    // Arrange
    var command = new CreateSessionCommand(
        CommunityId: Guid.NewGuid(),
        Title: "SOLID Workshop",
        Speaker: "Jane Smith",
        ScheduledAt: DateTimeOffset.UtcNow.AddDays(7));

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeEmpty();
  }

  [Fact]
  public async Task HandleAsync_WithValidCommand_PersistsSession()
  {
    // Arrange
    var command = new CreateSessionCommand(
        CommunityId: Guid.NewGuid(),
        Title: "SOLID Workshop",
        Speaker: "Jane Smith",
        ScheduledAt: DateTimeOffset.UtcNow.AddDays(7));

    // Act
    await _sut.HandleAsync(command);

    // Assert — Verificar que se llamó al repositorio (responsabilidad de persistencia)
    _sessionRepository.Received(1).Add(Arg.Any<Session>());
    await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WithValidCommand_SendsNotification()
  {
    // Arrange
    var command = new CreateSessionCommand(
        CommunityId: Guid.NewGuid(),
        Title: "SOLID Workshop",
        Speaker: "Jane Smith",
        ScheduledAt: DateTimeOffset.UtcNow.AddDays(7));

    // Act
    await _sut.HandleAsync(command);

    // Assert — Verificar que se envió la notificación (responsabilidad de notificación)
    await _notificationService.Received(1).SendSessionCreatedAsync(
        Arg.Any<Guid>(),
        Arg.Is("SOLID Workshop"),
        Arg.Is("Jane Smith"),
        Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WithInvalidTitle_ReturnsFailure()
  {
    // Arrange
    var command = new CreateSessionCommand(
        CommunityId: Guid.NewGuid(),
        Title: "",
        Speaker: "Jane Smith",
        ScheduledAt: DateTimeOffset.UtcNow.AddDays(7));

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionErrors.TitleRequired);
  }

  [Fact]
  public async Task HandleAsync_WithInvalidTitle_DoesNotPersistOrNotify()
  {
    // Arrange
    var command = new CreateSessionCommand(
        CommunityId: Guid.NewGuid(),
        Title: "",
        Speaker: "Jane Smith",
        ScheduledAt: DateTimeOffset.UtcNow.AddDays(7));

    // Act
    await _sut.HandleAsync(command);

    // Assert — On failure, no side effects should occur
    _sessionRepository.DidNotReceive().Add(Arg.Any<Session>());
    await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    await _notificationService.DidNotReceive().SendSessionCreatedAsync(
        Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
  }
}

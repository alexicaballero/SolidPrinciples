using FluentAssertions;
using SolidPrinciples.Common;
using SolidPrinciples.DIP.Solution;

namespace SolidPrinciples.DIP.Tests.Solution;

/// <summary>
/// Tests que demuestran los beneficios de la inversión de dependencias.
/// </summary>
public sealed class InvertedDependenciesTests
{
  [Fact]
  public void CreateSessionHandler_CanBeTestedWithInMemoryDependencies()
  {
    // SOLUCIÓN: El handler acepta dependencias por el constructor
    // Podemos inyectar dobles de prueba (repositorio en memoria, emisor de consola)

    // Arrange
    var repository = new InMemorySessionRepository();
    var notificationSender = new ConsoleNotificationSender();
    var handler = new CreateSessionHandler(repository, notificationSender);

    // Act
    var result = handler.HandleAsync("Test Session", "Description", Guid.NewGuid(), CancellationToken.None).Result;

    // Assert
    result.IsSuccess.Should().BeTrue();

    // SOLUCIÓN: Este test se ejecuta en <1ms
    // Sin base de datos, sin servidor SMTP requerido
    // Prueba unitaria pura
  }

  [Fact]
  public void CreateSessionHandler_CanSwapImplementationsAtRuntime()
  {
    // SOLUCIÓN: El handler depende de abstracciones (ISessionRepository, INotificationSender)
    // Podemos inyectar cualquier implementación

    // Arrange - Configuración de producción
    ISessionRepository sqlRepository = new SqlSessionRepository();
    INotificationSender emailSender = new EmailNotificationSender();
    var productionHandler = new CreateSessionHandler(sqlRepository, emailSender);

    // Arrange - Configuración de prueba
    ISessionRepository inMemoryRepository = new InMemorySessionRepository();
    INotificationSender consoleSender = new ConsoleNotificationSender();
    var testHandler = new CreateSessionHandler(inMemoryRepository, consoleSender);

    // Act & Assert
    productionHandler.Should().NotBeNull();
    testHandler.Should().NotBeNull();

    // SOLUCIÓN: Misma clase handler, diferentes dependencias
    // Sin cambios de código en CreateSessionHandler para intercambiar implementaciones
  }

  [Fact]
  public async Task InMemoryRepository_SavesSessionSuccessfully()
  {
    // SOLUCIÓN: Repositorio en memoria para pruebas rápidas

    // Arrange
    var repository = new InMemorySessionRepository();
    var session = Session.Create("SOLID Principles", "Learn SOLID", Guid.NewGuid()).Value;

    // Act
    await repository.SaveAsync(session, CancellationToken.None);
    var retrieved = await repository.GetByIdAsync(session.Id, CancellationToken.None);

    // Assert
    retrieved.Should().NotBeNull();
    retrieved!.Title.Should().Be("SOLID Principles");

    // SOLUCIÓN: Sin base de datos, se ejecuta instantáneamente
  }

  [Fact]
  public void DependencyInjection_EnablesTestability()
  {
    // SOLUCIÓN: La inyección de constructor hace que el código sea testeable

    // Arrange - Dependencias rápidas en memoria
    var repository = new InMemorySessionRepository();
    var sender = new ConsoleNotificationSender();
    var handler = new CreateSessionHandler(repository, sender);

    // Act
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    var result = handler.HandleAsync("Session", "Description", Guid.NewGuid(), CancellationToken.None).Result;
    stopwatch.Stop();

    // Assert
    result.IsSuccess.Should().BeTrue();

    // SOLUCIÓN: El test se completa en <10ms (vs 100+ms con base de datos)
    stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
  }

  [Fact]
  public void CommunityService_CanBeTestedWithInMemoryRepository()
  {
    // SOLUCIÓN: El servicio acepta dependencias por el constructor

    // Arrange
    var repository = new InMemoryCommunityRepository();
    var logger = new ConsoleLogger();
    var service = new CommunityService(repository, logger);

    // Act
    var result = service.CreateCommunityAsync("Test Community", "Description", CancellationToken.None).Result;

    // Assert
    result.IsSuccess.Should().BeTrue();

    // SOLUCIÓN: Sin EF Core, sin base de datos, sin sistema de archivos
    // Prueba unitaria pura
  }

  [Fact]
  public async Task InMemoryCommunityRepository_StoresCommunities()
  {
    // SOLUCIÓN: Repositorio en memoria para pruebas

    // Arrange
    var repository = new InMemoryCommunityRepository();
    var community = Community.Create("SOLID Community", "Learn SOLID principles", Guid.NewGuid()).Value;

    // Act
    await repository.AddAsync(community, CancellationToken.None);
    var retrieved = await repository.GetByIdAsync(community.Id, CancellationToken.None);

    // Assert
    retrieved.Should().NotBeNull();
    retrieved!.Name.Should().Be("SOLID Community");
  }

  [Fact]
  public void CommunityService_CanSwapLoggerImplementations()
  {
    // SOLUTION: Service depends on ILogger abstraction
    // We can inject console, file, or Azure logger

    // Arrange
    var repository = new InMemoryCommunityRepository();

    // Console logger for tests
    ILogger consoleLogger = new ConsoleLogger();
    var testService = new CommunityService(repository, consoleLogger);

    // File logger for local development
    ILogger fileLogger = new FileSystemLogger();
    var devService = new CommunityService(repository, fileLogger);

    // Azure Application Insights for production
    ILogger azureLogger = new AzureAppInsightsLogger();
    var prodService = new CommunityService(repository, azureLogger);

    // Act & Assert
    testService.Should().NotBeNull();
    devService.Should().NotBeNull();
    prodService.Should().NotBeNull();

    // SOLUTION: Same service class, different loggers
    // No code changes in CommunityService to swap logger
  }

  [Fact]
  public void DependencyInversion_FollowsCleanArchitecture()
  {
    // SOLUTION: High-level modules depend on abstractions
    // Low-level modules depend on abstractions
    // Dependency arrow: Infrastructure → Application (correct)

    // Arrange
    ISessionRepository repository = new SqlSessionRepository(); // Low-level implements abstraction
    INotificationSender sender = new EmailNotificationSender(); // Low-level implements abstraction
    var handler = new CreateSessionHandler(repository, sender); // High-level depends on abstraction

    // Act & Assert
    handler.Should().NotBeNull();

    // SOLUTION: Dependency flow is inverted
    // High-level (CreateSessionHandler) → Abstraction (ISessionRepository)
    // Low-level (SqlSessionRepository) → Abstraction (implements ISessionRepository)
    // Both depend on abstraction, not on each other
  }

  [Fact]
  public void AbstractionsDefined_InHighLevelModule()
  {
    // SOLUTION: Interfaces (ISessionRepository, INotificationSender) are defined
    // in the Application layer (high-level), not Infrastructure layer (low-level)

    // This ensures:
    // - Application layer owns the contract
    // - Infrastructure layer depends on Application (correct direction)
    // - High-level policy (Application) doesn't change when low-level details (Infrastructure) change

    // Act & Assert
    typeof(ISessionRepository).Namespace.Should().Contain("Solution");
    typeof(INotificationSender).Namespace.Should().Contain("Solution");

    // Concrete implementations also in Solution namespace (for this example)
    // In a real project:
    // - ISessionRepository → Application/Abstractions/ISessionRepository.cs
    // - SqlSessionRepository → Infrastructure/Persistence/SqlSessionRepository.cs
  }
}

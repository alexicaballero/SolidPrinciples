using FluentAssertions;
using SolidPrinciples.Common;
using SolidPrinciples.DIP.Problem;

namespace SolidPrinciples.DIP.Tests.Problem;

/// <summary>
/// Tests demonstrating the pain of direct dependencies (DIP violation).
/// </summary>
public sealed class DirectDependenciesTests
{
  [Fact]
  public void CreateSessionHandler_CannotBeTested_WithoutRealDependencies()
  {
    // PROBLEM: Handler instantiates concrete dependencies internally
    // We cannot inject test doubles

    // Arrange
    var handler = new CreateSessionHandler();

    // Act
    var result = handler.HandleAsync("Test Session", "Description", Guid.NewGuid(), CancellationToken.None).Result;

    // Assert
    result.IsSuccess.Should().BeTrue();

    // PROBLEM: This test actually calls:
    // - SqlSessionRepository (requires database)
    // - EmailNotificationSender (requires SMTP server)
    // We're not unit testing — we're integration testing
  }

  [Fact]
  public void CreateSessionHandler_IsHardcodedToSpecificImplementations()
  {
    // PROBLEM: Handler is tightly coupled to SqlSessionRepository and EmailNotificationSender
    // We cannot swap implementations for testing or production

    // Arrange
    var handler = new CreateSessionHandler();

    // Act & Assert
    // Handler constructor calls:
    // new SqlSessionRepository()
    // new EmailNotificationSender()

    // No way to inject InMemoryRepository or ConsoleNotificationSender
    // No way to test without database and email server
  }

  [Fact]
  public void DirectDependencies_MakeTestingSlow()
  {
    // PROBLEM: Tests that require database/SMTP are slow (seconds vs milliseconds)

    // Arrange
    var handler = new CreateSessionHandler();

    // Act
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    var result = handler.HandleAsync("Session", "Description", Guid.NewGuid(), CancellationToken.None).Result;
    stopwatch.Stop();

    // Assert
    result.IsSuccess.Should().BeTrue();

    // PROBLEM: This test is slower than it needs to be
    // With DIP + in-memory dependencies, this would run in <1ms
    // With direct dependencies (database/email), this could take 100ms+
    stopwatch.ElapsedMilliseconds.Should().BeGreaterThan(0);
  }

  [Fact]
  public void CommunityService_CannotBeTested_WithoutDatabase()
  {
    // PROBLEM: Service instantiates EFCoreCommunityRepository internally

    // Arrange
    var service = new CommunityService();

    // Act
    var result = service.CreateCommunityAsync("Test Community", "Description", CancellationToken.None).Result;

    // Assert
    result.IsSuccess.Should().BeTrue();

    // PROBLEM: This test requires EF Core context and database
    // Cannot inject InMemoryCommunityRepository for fast unit testing
  }

  [Fact]
  public void CommunityService_IsHardcodedToEFCore()
  {
    // PROBLEM: Service constructor calls:
    // new EFCoreCommunityRepository()
    // new FileSystemLogger()

    // Arrange
    var service = new CommunityService();

    // Act & Assert
    // Service is tightly coupled to EF Core and file system
    // Cannot swap to Dapper, MongoDB, or in-memory for testing
    // Cannot swap to console logger or Azure Application Insights
  }

  [Fact]
  public void DirectDependencies_ViolateLayeredArchitecture()
  {
    // PROBLEM: High-level business logic (handler/service) depends on
    // low-level infrastructure (SqlSessionRepository, EFCoreCommunityRepository)

    // This violates Clean Architecture:
    // Application layer should NOT depend on Infrastructure layer
    // Both should depend on abstractions (interfaces)

    // Arrange & Act
    var handler = new CreateSessionHandler();
    var service = new CommunityService();

    // Assert
    // Handler depends on SqlSessionRepository (low-level)
    // Service depends on EFCoreCommunityRepository (low-level)
    // Dependency arrow points the WRONG way: High → Low
  }
}

# Testing Patterns Reference

## Test Project Setup

Each test project references its source project and these packages:

```xml
<ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
    <PackageReference Include="xunit" Version="2.*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
    <PackageReference Include="FluentAssertions" Version="7.*" />
    <PackageReference Include="NSubstitute" Version="5.*" />
</ItemGroup>
```

## Test Naming Convention

```
MethodName_Condition_ExpectedResult
```

Examples:
- `Create_WithValidData_ReturnsSuccess`
- `Create_WithEmptyTitle_ReturnsFailure`
- `UpdateStatus_FromCanceledToScheduled_ReturnsInvalidTransition`

## Test Structure

### Domain Entity Tests (No mocks needed)

```csharp
namespace SolidPrinciples.SRP.Tests.Solution;

public sealed class SessionTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        // Arrange
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
        result.Value.Status.Should().Be(SessionStatus.Scheduled);
    }

    [Fact]
    public void Create_WithEmptyTitle_ReturnsFailure()
    {
        // Act
        var result = Session.Create(Guid.NewGuid(), "", "Speaker", DateTimeOffset.UtcNow.AddDays(1));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SessionError.TitleRequired);
    }
}
```

### Command Handler Tests (With mocks)

```csharp
namespace SolidPrinciples.SRP.Tests.Solution;

public sealed class CreateSessionCommandHandlerTests
{
    private readonly ISessionRepository _sessionRepository = Substitute.For<ISessionRepository>();
    private readonly ICommunityRepository _communityRepository = Substitute.For<ICommunityRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateSessionCommandHandler _sut;

    public CreateSessionCommandHandlerTests()
    {
        _sut = new CreateSessionCommandHandler(
            _sessionRepository,
            _communityRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesSession()
    {
        // Arrange
        var communityId = Guid.NewGuid();
        var community = new Community { Id = communityId, Name = "Architects" };
        _communityRepository.GetByIdAsync(communityId, Arg.Any<CancellationToken>())
            .Returns(community);

        var command = new CreateSessionCommand
        {
            CommunityId = communityId,
            Title = "Design Patterns",
            Speaker = "Gang of Four",
            ScheduledAt = DateTimeOffset.UtcNow.AddDays(7)
        };

        // Act
        var result = await _sut.HandleAsync(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _sessionRepository.Received(1).Add(Arg.Any<Session>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentCommunity_ReturnsFailure()
    {
        // Arrange
        _communityRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Community?)null);

        var command = new CreateSessionCommand { CommunityId = Guid.NewGuid() };

        // Act
        var result = await _sut.HandleAsync(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CommunityError.NotFound);
    }
}
```

## Problem vs Solution Test Organization

Each principle's test project mirrors the source:

```
SolidPrinciples.SRP.Tests/
├── Problem/
│   └── ReportServiceTests.cs       # Tests that show the problem still "works" 
│                                    # but is hard to test/maintain
└── Solution/
    ├── SessionTests.cs              # Clean, focused tests
    └── CreateSessionCommandHandlerTests.cs
```

### Violation Tests Pattern

Violation tests demonstrate WHY the code is problematic:

```csharp
/// <summary>
/// These tests demonstrate the testing difficulties caused by SRP problems.
/// Notice how we need complex setup and can't test responsibilities in isolation.
/// </summary>
public sealed class ReportServiceViolationTests
{
    [Fact]
    public void GenerateAndSendReport_CannotTestFormattingWithoutDatabase()
    {
        // This test CANNOT run without a real database connection
        // because data access and formatting are mixed in one class.
        // This is a direct consequence of violating SRP.
        var sut = new ReportService(); // Requires full infrastructure
        // ...
    }
}
```

### Solution Tests Pattern

Solution tests show clean, isolated testing:

```csharp
/// <summary>
/// After applying SRP, each responsibility can be tested independently.
/// </summary>
public sealed class HtmlReportFormatterTests
{
    [Fact]
    public void Format_WithSalesData_ReturnsValidHtml()
    {
        // No database, no email — just formatting logic
        var formatter = new HtmlReportFormatter();
        var data = CreateTestData();

        var result = formatter.Format(data);

        result.Should().Contain("<table>");
        result.Should().Contain("Product A");
    }
}
```

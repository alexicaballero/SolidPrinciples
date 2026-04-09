using FluentAssertions;
using NSubstitute;
using SolidPrinciples.SRP.Solution;

namespace SolidPrinciples.SRP.Tests.Solution;

/// <summary>
/// SOLUCIÓN: Tests del generador de reportes — cada colaborador está mockeado.
/// Verificamos el flujo de orquestación sin ninguna infraestructura real.
/// </summary>
public sealed class CommunityReportGeneratorTests
{
    private readonly IReportDataProvider _dataProvider = Substitute.For<IReportDataProvider>();
    private readonly IReportFormatter _formatter = Substitute.For<IReportFormatter>();
    private readonly IReportSender _sender = Substitute.For<IReportSender>();
    private readonly CommunityReportGenerator _sut;

    public CommunityReportGeneratorTests()
    {
        _sut = new CommunityReportGenerator(_dataProvider, _formatter, _sender);
    }

    [Fact]
    public async Task GenerateAndSendAsync_CallsDataProviderWithCommunityId()
    {
        // Arrange
        var communityId = Guid.NewGuid();
        _dataProvider.GetSessionsByCommunityAsync(communityId, Arg.Any<CancellationToken>())
            .Returns(new List<SessionReportItem>());
        _formatter.Format(Arg.Any<IReadOnlyList<SessionReportItem>>())
            .Returns("formatted report");

        // Act
        await _sut.GenerateAndSendAsync(communityId, "admin@test.com");

        // Assert
        await _dataProvider.Received(1).GetSessionsByCommunityAsync(communityId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GenerateAndSendAsync_FormatsDataAndSendsToRecipient()
    {
        // Arrange
        var communityId = Guid.NewGuid();
        var sessions = new List<SessionReportItem>
        {
            new("Workshop", "Speaker", DateTimeOffset.Now, "Scheduled")
        };
        _dataProvider.GetSessionsByCommunityAsync(communityId, Arg.Any<CancellationToken>())
            .Returns(sessions);
        _formatter.Format(sessions).Returns("<html>report</html>");

        // Act
        await _sut.GenerateAndSendAsync(communityId, "admin@test.com");

        // Assert
        _formatter.Received(1).Format(sessions);
        await _sender.Received(1).SendAsync(
            "admin@test.com",
            "Community Session Report",
            "<html>report</html>",
            Arg.Any<CancellationToken>());
    }
}

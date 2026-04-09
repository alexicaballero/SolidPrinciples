using FluentAssertions;
using SolidPrinciples.LSP.Solution;
using Xunit;

namespace SolidPrinciples.LSP.Tests.Solution;

/// <summary>
/// SOLUCIÓN: Estos tests demuestran cumplimiento de LSP con diseño de Member basado en capacidades.
/// Todos los Members son sustituibles — sin verificación de tipo, sin excepciones, comportamiento consistente.
/// </summary>
public sealed class MemberTests
{
  [Fact]
  public void CreateFull_Vote_Succeeds()
  {
    var member = Member.CreateFull(Guid.NewGuid(), "Jane Doe", "jane@example.com");

    var result = member.Vote(Guid.NewGuid(), true);

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void CreateGuest_Vote_ReturnsFailureWithClearReason()
  {
    // Guest es un Member válido — sin excepción, solo falla claramente
    var member = Member.CreateGuest(Guid.NewGuid(), "Guest User", "guest@example.com");

    var result = member.Vote(Guid.NewGuid(), true);

    // Sin excepción — devuelve Result con error
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(MemberErrors.VotingNotAllowed);
  }

  [Fact]
  public void CreateInactive_Vote_ReturnsFailureNotSilentFailure()
  {
    // Miembro inactivo devuelve falla explícitamente — no un no-op silencioso
    var member = Member.CreateInactive(Guid.NewGuid(), "Inactive User", "inactive@example.com");

    var result = member.Vote(Guid.NewGuid(), true);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(MemberErrors.VotingNotAllowed);
  }

  [Fact]
  public void CreateFull_CreateSessionProposal_Succeeds()
  {
    var member = Member.CreateFull(Guid.NewGuid(), "Jane Doe", "jane@example.com");

    var result = member.CreateSessionProposal("SOLID Workshop", "Learn SOLID");

    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeEmpty();
  }

  [Fact]
  public void CreateGuest_CreateSessionProposal_ReturnsFailure()
  {
    // Guest cannot propose — but returns Result, no exception
    var member = Member.CreateGuest(Guid.NewGuid(), "Guest User", "guest@example.com");

    var result = member.CreateSessionProposal("SOLID Workshop", "Learn SOLID");

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(MemberErrors.ProposalNotAllowed);
  }

  [Fact]
  public void CreateFull_SendMessage_Succeeds()
  {
    var member = Member.CreateFull(Guid.NewGuid(), "Jane Doe", "jane@example.com");

    var result = member.SendMessage("System", "Welcome!");

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void CreateGuest_SendMessage_Succeeds()
  {
    // Guests can receive messages
    var member = Member.CreateGuest(Guid.NewGuid(), "Guest User", "guest@example.com");

    var result = member.SendMessage("System", "Welcome!");

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void CreateInactive_SendMessage_ReturnsFailure()
  {
    // Inactive cannot receive messages — explicit failure
    var member = Member.CreateInactive(Guid.NewGuid(), "Inactive User", "inactive@example.com");

    var result = member.SendMessage("System", "Welcome!");

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(MemberErrors.MessagingNotAllowed);
  }

  /// <summary>
  /// BENEFIT: Can query capabilities before calling operations.
  /// No type-checking needed — capabilities are explicit.
  /// </summary>
  [Fact]
  public void Capabilities_AreQueryable()
  {
    var fullMember = Member.CreateFull(Guid.NewGuid(), "Full", "full@example.com");
    var guestMember = Member.CreateGuest(Guid.NewGuid(), "Guest", "guest@example.com");
    var inactiveMember = Member.CreateInactive(Guid.NewGuid(), "Inactive", "inactive@example.com");

    fullMember.Capabilities.CanVote.Should().BeTrue();
    fullMember.Capabilities.CanPropose.Should().BeTrue();

    guestMember.Capabilities.CanVote.Should().BeFalse();
    guestMember.Capabilities.CanPropose.Should().BeFalse();
    guestMember.Capabilities.CanReceiveMessages.Should().BeTrue();

    inactiveMember.Capabilities.CanVote.Should().BeFalse();
    inactiveMember.Capabilities.CanReceiveMessages.Should().BeFalse();
  }

  /// <summary>
  /// BENEFIT: Polymorphic code works correctly with ALL members.
  /// No type-checking, no exceptions, consistent behavior.
  /// </summary>
  [Fact]
  public void Members_CanBeUsedPolymorphically()
  {
    var members = new[]
    {
            Member.CreateFull(Guid.NewGuid(), "Full", "full@example.com"),
            Member.CreateGuest(Guid.NewGuid(), "Guest", "guest@example.com"),
            Member.CreateInactive(Guid.NewGuid(), "Inactive", "inactive@example.com")
    };

    var proposalId = Guid.NewGuid();
    var results = new List<bool>();

    // No type-checking needed — all members can be called the same way
    foreach (var member in members)
    {
      var result = member.Vote(proposalId, true);
      results.Add(result.IsSuccess);
    }

    // Results are predictable: Full succeeds, Guest and Inactive fail
    results.Should().ContainInOrder(true, false, false);
  }

  /// <summary>
  /// BENEFIT: Can write defensive code that checks capabilities first.
  /// No try-catch needed.
  /// </summary>
  [Fact]
  public void ProcessVote_ChecksCapabilities()
  {
    var members = new[]
    {
            Member.CreateFull(Guid.NewGuid(), "Full", "full@example.com"),
            Member.CreateGuest(Guid.NewGuid(), "Guest", "guest@example.com"),
            Member.CreateInactive(Guid.NewGuid(), "Inactive", "inactive@example.com")
    };

    var proposalId = Guid.NewGuid();
    var votedCount = 0;

    foreach (var member in members)
    {
      // Check capability before calling — explicit and safe
      if (member.Capabilities.CanVote)
      {
        var result = member.Vote(proposalId, true);
        if (result.IsSuccess)
          votedCount++;
      }
    }

    votedCount.Should().Be(1); // Only Full member voted
  }
}

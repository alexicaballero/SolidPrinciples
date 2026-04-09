using FluentAssertions;
using SolidPrinciples.LSP.Problem;
using Xunit;

namespace SolidPrinciples.LSP.Tests.Problem;

/// <summary>
/// PROBLEMA: Estos tests demuestran el dolor de las violaciones de LSP en MemberHierarchy.
/// No se pueden escribir tests polimórficos — debe manejar cada subtipo de forma diferente.
/// </summary>
public sealed class MemberHierarchyTests
{
  [Fact]
  public void FullMember_Vote_WorksCorrectly()
  {
    // FullMember funciona como se espera
    Member member = new FullMember(Guid.NewGuid(), "Jane Doe", "jane@example.com");

    var act = () => member.Vote(Guid.NewGuid(), true);

    act.Should().NotThrow();
  }

  [Fact]
  public void GuestMember_Vote_ThrowsNotImplementedException()
  {
    // VIOLACIÓN: GuestMember es un Member pero no puede ser sustituido
    Member member = new GuestMember(Guid.NewGuid(), "Guest User", "guest@example.com");

    var act = () => member.Vote(Guid.NewGuid(), true);

    // El test debe esperar excepción — no puede usar Guest donde se espera Member
    act.Should().Throw<NotImplementedException>()
        .WithMessage("*cannot vote*");
  }

  [Fact]
  public void InactiveMember_Vote_SilentlyDoesNothing()
  {
    // VIOLACIÓN: InactiveMember falla silenciosamente — el llamador piensa que el voto fue registrado
    Member member = new InactiveMember(Guid.NewGuid(), "Inactive User", "inactive@example.com");

    var act = () => member.Vote(Guid.NewGuid(), true);

    // Sin excepción — pero el voto NO fue registrado. Falla silenciosa.
    act.Should().NotThrow();
  }

  [Fact]
  public void GuestMember_CreateSessionProposal_ThrowsNotImplementedException()
  {
    // VIOLATION: Cannot substitute Guest for Member
    Member member = new GuestMember(Guid.NewGuid(), "Guest User", "guest@example.com");

    var act = () => member.CreateSessionProposal("SOLID Workshop", "Learn SOLID principles");

    act.Should().Throw<NotImplementedException>()
        .WithMessage("*cannot create proposals*");
  }

  /// <summary>
  /// PROBLEM: Cannot write a single test that works for ALL Member subtypes.
  /// Each subtype requires different assertions.
  /// This is the pain of broken substitutability.
  /// </summary>
  [Theory]
  [InlineData(typeof(FullMember), false)] // Should NOT throw
  [InlineData(typeof(GuestMember), true)]  // WILL throw
  [InlineData(typeof(InactiveMember), false)] // Will NOT throw (but also won't work)
  public void Members_Vote_InconsistentBehavior(Type memberType, bool shouldThrow)
  {
    // Create member of specified type
    Member member = memberType.Name switch
    {
      nameof(FullMember) => new FullMember(Guid.NewGuid(), "Full", "full@example.com"),
      nameof(GuestMember) => new GuestMember(Guid.NewGuid(), "Guest", "guest@example.com"),
      nameof(InactiveMember) => new InactiveMember(Guid.NewGuid(), "Inactive", "inactive@example.com"),
      _ => throw new InvalidOperationException()
    };

    var act = () => member.Vote(Guid.NewGuid(), true);

    // Must have different assertions for different subtypes
    if (shouldThrow)
      act.Should().Throw<NotImplementedException>();
    else
      act.Should().NotThrow();
  }

  /// <summary>
  /// PROBLEM: Demonstrates type-checking code smell.
  /// When LSP is violated, callers must check types.
  /// </summary>
  [Fact]
  public void ProcessVote_RequiresTypeChecking()
  {
    var members = new Member[]
    {
            new FullMember(Guid.NewGuid(), "Full", "full@example.com"),
            new GuestMember(Guid.NewGuid(), "Guest", "guest@example.com"),
            new InactiveMember(Guid.NewGuid(), "Inactive", "inactive@example.com")
    };

    var proposalId = Guid.NewGuid();
    var successCount = 0;

    foreach (var member in members)
    {
      // CODE SMELL: Type-checking is needed because subtypes are not substitutable
      if (member is not GuestMember)
      {
        member.Vote(proposalId, true);
        successCount++;
      }
    }

    // Only 2 out of 3 could vote (Full and Inactive, though Inactive did nothing)
    successCount.Should().Be(2);
  }
}

using FluentAssertions;
using SolidPrinciples.LSP.Solution;
using Xunit;

namespace SolidPrinciples.LSP.Tests.Solution;

/// <summary>
/// SOLUCIÓN: Estos tests demuestran cumplimiento de LSP con diseño de Session basado en políticas.
/// Todas las Sessions honran el mismo contrato — las políticas varían el comportamiento sin romper la sustituibilidad.
/// </summary>
public sealed class SessionTests
{
  [Fact]
  public void CreateStandard_Confirm_AlwaysSucceeds()
  {
    var session = Session.CreateStandard(Guid.NewGuid(), "SOLID Workshop", 50);

    var result = session.Confirm();

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void CreateWorkshop_Confirm_ReturnsFailureWithoutMinimumAttendees()
  {
    // La política impone asistentes mínimos — pero devuelve Result, no excepción
    var session = Session.CreateWorkshop(Guid.NewGuid(), "Advanced DDD", 50, minimumAttendees: 5);

    var result = session.Confirm();

    result.IsFailure.Should().BeTrue();
    result.Error.Code.Should().Contain("InsufficientAttendees");
  }

  [Fact]
  public void CreateWorkshop_Confirm_SucceedsWithEnoughAttendees()
  {
    var session = Session.CreateWorkshop(Guid.NewGuid(), "Advanced DDD", 50, minimumAttendees: 5);

    // Registrar 5 asistentes
    for (int i = 0; i < 5; i++)
      session.RegisterAttendee(Guid.NewGuid());

    var result = session.Confirm();

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void CreateStandard_RegisterAttendee_AcceptsAnyMember()
  {
    var session = Session.CreateStandard(Guid.NewGuid(), "SOLID Workshop", 50);

    var result = session.RegisterAttendee(Guid.NewGuid());

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void CreatePremium_RegisterAttendee_RequiresVerifiedMember()
  {
    // Policy enforces verification — but returns Result, not exception
    var verifiedMembers = new HashSet<Guid> { Guid.Parse("aaaa1111-bbbb-cccc-dddd-eeee2222ffff") };
    var session = Session.CreatePremium(Guid.NewGuid(), "Premium Talk", 20, verifiedMembers);

    var unverifiedMember = Guid.NewGuid();
    var result = session.RegisterAttendee(unverifiedMember);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionErrors.NotVerifiedMember);
  }

  [Fact]
  public void CreatePremium_RegisterAttendee_SucceedsForVerifiedMember()
  {
    var verifiedMemberId = Guid.Parse("aaaa1111-bbbb-cccc-dddd-eeee2222ffff");
    var verifiedMembers = new HashSet<Guid> { verifiedMemberId };
    var session = Session.CreatePremium(Guid.NewGuid(), "Premium Talk", 20, verifiedMembers);

    var result = session.RegisterAttendee(verifiedMemberId);

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void CreateStandard_Cancel_AlwaysSucceeds()
  {
    var session = Session.CreateStandard(Guid.NewGuid(), "SOLID Workshop", 50);
    session.RegisterAttendee(Guid.NewGuid()); // Add attendee

    var result = session.Cancel();

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void CreateFree_Cancel_ReturnsFailureWithAttendees()
  {
    // Policy enforces no cancelation with attendees — but returns Result
    var session = Session.CreateFree(Guid.NewGuid(), "Free Meetup", 30);
    session.RegisterAttendee(Guid.NewGuid()); // Add attendee

    var result = session.Cancel();

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionErrors.CannotCancelWithAttendees);
  }

  [Fact]
  public void CreateFree_Cancel_SucceedsWithoutAttendees()
  {
    var session = Session.CreateFree(Guid.NewGuid(), "Free Meetup", 30);

    var result = session.Cancel();

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void Session_RegisterAttendee_RespectsCapacity()
  {
    // All sessions respect capacity — consistent contract
    var session = Session.CreateStandard(Guid.NewGuid(), "Small Session", capacity: 2);

    session.RegisterAttendee(Guid.NewGuid()).IsSuccess.Should().BeTrue();
    session.RegisterAttendee(Guid.NewGuid()).IsSuccess.Should().BeTrue();

    var result = session.RegisterAttendee(Guid.NewGuid());

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionErrors.SessionFull);
  }

  /// <summary>
  /// BENEFIT: Polymorphic usage works correctly — all Sessions honor the same contract.
  /// Operations return Result with clear success/failure — no exceptions, no type-checking.
  /// </summary>
  [Fact]
  public void Sessions_CanBeUsedPolymorphicallyWithPredictableResults()
  {
    var sessions = new[]
    {
            Session.CreateStandard(Guid.NewGuid(), "Standard", 50),
            Session.CreateWorkshop(Guid.NewGuid(), "Workshop", 50, minimumAttendees: 5),
            Session.CreatePremium(Guid.NewGuid(), "Premium", 50, new HashSet<Guid>()),
            Session.CreateFree(Guid.NewGuid(), "Free", 50)
    };

    var memberId = Guid.NewGuid();

    // Can register to all sessions consistently
    // Some may fail due to policy, but all return Result (no exceptions)
    foreach (var session in sessions)
    {
      var result = session.RegisterAttendee(memberId);
      // Result is predictable: Standard/Workshop/Free succeed, Premium fails
      result.Should().NotBeNull(); // All return Result, none throw
    }

    // Can attempt to confirm all sessions
    var confirmResults = sessions.Select(s => s.Confirm()).ToList();
    confirmResults.Should().AllSatisfy(r => r.Should().NotBeNull());

    // Can attempt to cancel all sessions
    var cancelResults = sessions.Select(s => s.Cancel()).ToList();
    cancelResults.Should().AllSatisfy(r => r.Should().NotBeNull());
  }

  /// <summary>
  /// BENEFIT: Policies are testable in isolation by injecting them.
  /// </summary>
  [Fact]
  public void WorkshopSessionPolicy_EnforcesMinimumAttendees()
  {
    // Test the policy directly
    var policy = new WorkshopSessionPolicy(minimumAttendees: 3);

    var resultWithZero = policy.CanConfirm(attendeeCount: 0, capacity: 50);
    resultWithZero.IsFailure.Should().BeTrue();

    var resultWithTwo = policy.CanConfirm(attendeeCount: 2, capacity: 50);
    resultWithTwo.IsFailure.Should().BeTrue();

    var resultWithThree = policy.CanConfirm(attendeeCount: 3, capacity: 50);
    resultWithThree.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void PremiumSessionPolicy_EnforcesVerification()
  {
    var verifiedMemberId = Guid.Parse("aaaa1111-bbbb-cccc-dddd-eeee2222ffff");
    var verifiedMembers = new HashSet<Guid> { verifiedMemberId };
    var policy = new PremiumSessionPolicy(verifiedMembers);

    var resultForUnverified = policy.CanRegister(Guid.NewGuid());
    resultForUnverified.IsFailure.Should().BeTrue();

    var resultForVerified = policy.CanRegister(verifiedMemberId);
    resultForVerified.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void FreeSessionPolicy_PreventsCancelationWithAttendees()
  {
    var policy = new FreeSessionPolicy();

    var resultWithAttendees = policy.CanCancel(attendeeCount: 1);
    resultWithAttendees.IsFailure.Should().BeTrue();

    var resultWithoutAttendees = policy.CanCancel(attendeeCount: 0);
    resultWithoutAttendees.IsSuccess.Should().BeTrue();
  }
}

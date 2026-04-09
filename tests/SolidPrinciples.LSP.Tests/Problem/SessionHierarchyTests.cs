using FluentAssertions;
using SolidPrinciples.LSP.Problem;
using Xunit;

namespace SolidPrinciples.LSP.Tests.Problem;

/// <summary>
/// PROBLEMA: Estos tests demuestran violaciones de LSP en SessionHierarchy.
/// Los subtipos fortalecen las precondiciones o debilitan las postcondiciones — rompiendo la sustituibilidad.
/// </summary>
public sealed class SessionHierarchyTests
{
  [Fact]
  public void StandardSession_Confirm_AlwaysSucceeds()
  {
    // Contrato base: puede confirmar en cualquier momento
    SessionBase session = new StandardSession(Guid.NewGuid(), "SOLID Workshop", 50);

    var result = session.Confirm();

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void WorkshopSession_Confirm_RequiresMinimumAttendees()
  {
    // VIOLACIÓN: WorkshopSession FORTALECE la precondición
    // El contrato base dice "puede confirmar en cualquier momento" pero WorkshopSession necesita asistentes mínimos
    SessionBase session = new WorkshopSession(Guid.NewGuid(), "Advanced DDD", 50);

    var result = session.Confirm();

    // Falla porque no hay asistentes — rompe el contrato base
    result.IsFailure.Should().BeTrue();
    result.Error.Code.Should().Contain("InsufficientAttendees");
  }

  [Fact]
  public void WorkshopSession_Confirm_SucceedsWithEnoughAttendees()
  {
    var session = new WorkshopSession(Guid.NewGuid(), "Advanced DDD", 50);

    // Registrar 5 asistentes (mínimo)
    for (int i = 0; i < 5; i++)
      session.RegisterAttendee(Guid.NewGuid());

    var result = session.Confirm();

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void PremiumSession_RegisterAttendee_RequiresVerifiedMember()
  {
    // VIOLATION: PremiumSession STRENGTHENS the precondition
    // Base contract says "any member until capacity" but PremiumSession needs verification
    var verifiedMembers = new HashSet<Guid> { Guid.Parse("aaaa1111-bbbb-cccc-dddd-eeee2222ffff") };
    SessionBase session = new PremiumSession(Guid.NewGuid(), "Premium Talk", 20, verifiedMembers);

    var unverifiedMember = Guid.NewGuid();
    var result = session.RegisterAttendee(unverifiedMember);

    // Fails because member not verified — breaks base contract
    result.IsFailure.Should().BeTrue();
    result.Error.Code.Should().Contain("NotVerified");
  }

  [Fact]
  public void PremiumSession_RegisterAttendee_SucceedsForVerifiedMember()
  {
    var verifiedMemberId = Guid.Parse("aaaa1111-bbbb-cccc-dddd-eeee2222ffff");
    var verifiedMembers = new HashSet<Guid> { verifiedMemberId };
    var session = new PremiumSession(Guid.NewGuid(), "Premium Talk", 20, verifiedMembers);

    var result = session.RegisterAttendee(verifiedMemberId);

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void StandardSession_Cancel_AlwaysSucceeds()
  {
    // Base contract: can always cancel
    SessionBase session = new StandardSession(Guid.NewGuid(), "SOLID Workshop", 50);
    session.RegisterAttendee(Guid.NewGuid()); // Add attendee

    var result = session.Cancel();

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void FreeSession_Cancel_FailsWithAttendees()
  {
    // VIOLATION: FreeSession WEAKENS the postcondition
    // Base contract says "can always cancel" but FreeSession cannot cancel with attendees
    SessionBase session = new FreeSession(Guid.NewGuid(), "Free Meetup", 30);
    session.RegisterAttendee(Guid.NewGuid()); // Add attendee

    var result = session.Cancel();

    // Fails because has attendees — breaks base contract
    result.IsFailure.Should().BeTrue();
    result.Error.Code.Should().Contain("HasAttendees");
  }

  [Fact]
  public void FreeSession_Cancel_SucceedsWithoutAttendees()
  {
    var session = new FreeSession(Guid.NewGuid(), "Free Meetup", 30);

    var result = session.Cancel();

    result.IsSuccess.Should().BeTrue();
  }

  /// <summary>
  /// PROBLEM: Cannot write polymorphic code that works for all SessionBase subtypes.
  /// Each subtype has different rules that violate the base contract.
  /// </summary>
  [Fact]
  public void SessionBase_PolymorphicUsage_InconsistentBehavior()
  {
    SessionBase[] sessions =
    [
            new StandardSession(Guid.NewGuid(), "Standard", 50),
            new WorkshopSession(Guid.NewGuid(), "Workshop", 50),
            new PremiumSession(Guid.NewGuid(), "Premium", 50, new HashSet<Guid>()),
            new FreeSession(Guid.NewGuid(), "Free", 50)
    ];

    // Cannot confirm all sessions consistently (Workshop needs attendees)
    var confirmResults = sessions.Select(s => s.Confirm()).ToList();
    var allConfirmed = confirmResults.All(r => r.IsSuccess);
    allConfirmed.Should().BeFalse(); // Workshop will fail

    // Cannot register same member to all sessions (Premium needs verification)
    var memberId = Guid.NewGuid();
    var registerResults = sessions.Select(s => s.RegisterAttendee(memberId)).ToList();
    var allRegistered = registerResults.All(r => r.IsSuccess);
    allRegistered.Should().BeFalse(); // Premium will fail

    // Cannot cancel all sessions consistently after registration (Free will fail)
    var cancelResults = sessions.Select(s => s.Cancel()).ToList();
    var allCanceled = cancelResults.All(r => r.IsSuccess);
    allCanceled.Should().BeFalse(); // Free will fail
  }
}

using SolidPrinciples.Common;

namespace SolidPrinciples.LSP.Solution;

/// <summary>
/// SOLUCIÓN: En lugar de una jerarchía de herencia con contratos violados,
/// usamos una única clase Session con políticas componibles.
///
/// Las políticas pueden imponer diferentes reglas (asistentes mínimos, miembros verificados, reglas de cancelación)
/// sin violar LSP porque Session misma no tiene subtipos.
/// </summary>
/// <remarks>
/// Patrón Strategy: Inyectar políticas que validen operaciones.
/// El contrato de Session es consistente — las políticas proporcionan variación sin romper la substituibilidad.
/// </remarks>
public sealed class Session
{
  public Guid Id { get; private set; }
  public string Title { get; private set; } = string.Empty;
  public int Capacity { get; private set; }
  private readonly List<Guid> _attendees = [];
  private readonly ISessionPolicy _policy;

  private Session(Guid id, string title, int capacity, ISessionPolicy policy)
  {
    Id = id;
    Title = title;
    Capacity = capacity;
    _policy = policy;
  }

  public static Session CreateStandard(Guid id, string title, int capacity) =>
      new(id, title, capacity, new StandardSessionPolicy());

  public static Session CreateWorkshop(Guid id, string title, int capacity, int minimumAttendees) =>
      new(id, title, capacity, new WorkshopSessionPolicy(minimumAttendees));

  public static Session CreatePremium(Guid id, string title, int capacity, HashSet<Guid> verifiedMembers) =>
      new(id, title, capacity, new PremiumSessionPolicy(verifiedMembers));

  public static Session CreateFree(Guid id, string title, int capacity) =>
      new(id, title, capacity, new FreeSessionPolicy());

  public IReadOnlyList<Guid> Attendees => _attendees.AsReadOnly();

  /// <summary>
  /// CORRECTO: Contrato consistente — la política determina si se permite la confirmación.
  /// </summary>
  public Result Confirm()
  {
    return _policy.CanConfirm(_attendees.Count, Capacity);
  }

  /// <summary>
  /// CORRECTO: Contrato consistente — la política determina si se permite el registro.
  /// </summary>
  public Result RegisterAttendee(Guid memberId)
  {
    if (_attendees.Count >= Capacity)
      return Result.Failure(SessionErrors.SessionFull);

    var policyResult = _policy.CanRegister(memberId);
    if (policyResult.IsFailure)
      return policyResult;

    _attendees.Add(memberId);
    return Result.Success();
  }

  /// <summary>
  /// CORRECTO: Contrato consistente — la política determina si se permite la cancelación.
  /// </summary>
  public Result Cancel()
  {
    return _policy.CanCancel(_attendees.Count);
  }
}

/// <summary>
/// Abstracción de política — define las reglas para operaciones de sesión.
/// </summary>
public interface ISessionPolicy
{
  Result CanConfirm(int attendeeCount, int capacity);
  Result CanRegister(Guid memberId);
  Result CanCancel(int attendeeCount);
}

/// <summary>
/// Política estándar — sin restricciones especiales.
/// </summary>
public sealed class StandardSessionPolicy : ISessionPolicy
{
  public Result CanConfirm(int attendeeCount, int capacity) => Result.Success();

  public Result CanRegister(Guid memberId) => Result.Success();

  public Result CanCancel(int attendeeCount) => Result.Success();
}

/// <summary>
/// Workshop policy — requires minimum attendees to confirm.
/// </summary>
public sealed class WorkshopSessionPolicy(int minimumAttendees) : ISessionPolicy
{
  public Result CanConfirm(int attendeeCount, int capacity)
  {
    if (attendeeCount < minimumAttendees)
      return Result.Failure(SessionErrors.InsufficientAttendees(minimumAttendees));

    return Result.Success();
  }

  public Result CanRegister(Guid memberId) => Result.Success();

  public Result CanCancel(int attendeeCount) => Result.Success();
}

/// <summary>
/// Premium policy — only verified members can register.
/// </summary>
public sealed class PremiumSessionPolicy(HashSet<Guid> verifiedMembers) : ISessionPolicy
{
  public Result CanConfirm(int attendeeCount, int capacity) => Result.Success();

  public Result CanRegister(Guid memberId)
  {
    if (!verifiedMembers.Contains(memberId))
      return Result.Failure(SessionErrors.NotVerifiedMember);

    return Result.Success();
  }

  public Result CanCancel(int attendeeCount) => Result.Success();
}

/// <summary>
/// Free session policy — cannot cancel if attendees are registered.
/// </summary>
public sealed class FreeSessionPolicy : ISessionPolicy
{
  public Result CanConfirm(int attendeeCount, int capacity) => Result.Success();

  public Result CanRegister(Guid memberId) => Result.Success();

  public Result CanCancel(int attendeeCount)
  {
    if (attendeeCount > 0)
      return Result.Failure(SessionErrors.CannotCancelWithAttendees);

    return Result.Success();
  }
}

/// <summary>
/// Domain errors for session operations.
/// </summary>
public static class SessionErrors
{
  public static readonly Error SessionFull = new(
      "Session.Full",
      "Session has reached maximum capacity.");

  public static Error InsufficientAttendees(int minimum) => new(
      "Session.InsufficientAttendees",
      $"Session requires at least {minimum} attendees to confirm.");

  public static readonly Error NotVerifiedMember = new(
      "Session.NotVerifiedMember",
      "Only verified members can register for this session.");

  public static readonly Error CannotCancelWithAttendees = new(
      "Session.CannotCancelWithAttendees",
      "Cannot cancel session with registered attendees.");
}

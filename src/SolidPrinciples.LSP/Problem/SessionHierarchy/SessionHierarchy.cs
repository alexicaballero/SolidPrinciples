using SolidPrinciples.Common;

namespace SolidPrinciples.LSP.Problem;

/// <summary>
/// PROBLEMA: Esta jerarchía de sesiones viola LSP porque las clases derivadas
/// fortalecen las precondiciones (requieren más de lo que promete la clase base) y
/// debilitan las postcondiciones (entregan menos de lo que garantiza la clase base).
/// </summary>
/// <remarks>
/// Reglas del Contrato LSP:
/// - Las precondiciones NO pueden fortalecerse en un subtipo
/// - Las postcondiciones NO pueden debilitarse en un subtipo
/// - Los invariantes de la superclase DEBEN preservarse
///
/// Estos tipos de sesión violan estas reglas.
/// </remarks>
public abstract class SessionBase
{
  public Guid Id { get; protected set; }
  public string Title { get; protected set; } = string.Empty;
  public int Capacity { get; protected set; }

  /// <summary>
  /// Confirma la sesión. Contrato base: puede confirmarse si está programada.
  /// </summary>
  public abstract Result Confirm();

  /// <summary>
  /// Registra un asistente. Contrato base: acepta cualquier miembro hasta alcanzar capacidad.
  /// </summary>
  public abstract Result RegisterAttendee(Guid memberId);

  /// <summary>
  /// Cancela la sesión. Contrato base: siempre puede cancelarse antes de que comience.
  /// </summary>
  public abstract Result Cancel();
}

/// <summary>
/// Sesión estándar — implementa el contrato base correctamente.
/// </summary>
public sealed class StandardSession : SessionBase
{
  private readonly List<Guid> _attendees = [];

  public StandardSession(Guid id, string title, int capacity)
  {
    Id = id;
    Title = title;
    Capacity = capacity;
  }

  public override Result Confirm()
  {
    // Puede confirmarse en cualquier momento
    return Result.Success();
  }

  public override Result RegisterAttendee(Guid memberId)
  {
    if (_attendees.Count >= Capacity)
      return Result.Failure(new Error("Session.Full", "Session has reached capacity."));

    _attendees.Add(memberId);
    return Result.Success();
  }

  public override Result Cancel()
  {
    // Siempre puede cancelarse
    return Result.Success();
  }
}

/// <summary>
/// VIOLATION: WorkshopSession FORTALECE las precondiciones.
/// La clase base dice "puede confirmarse si está programada" pero WorkshopSession requiere asistentes mínimos.
/// </summary>
/// <remarks>
/// Violación de LSP: Una función que espera SessionBase fallará cuando se le da una WorkshopSession
/// porque Confirm() tiene requisitos más estrictos que lo que prometió el contrato base.
///
/// Impacto:
/// - `sessionBase.Confirm()` podría fallar inesperadamente si sessionBase es en realidad una WorkshopSession
/// - Rompe la substituibilidad
/// </remarks>
public sealed class WorkshopSession : SessionBase
{
  private readonly List<Guid> _attendees = [];
  private const int MinimumAttendees = 5;

  public WorkshopSession(Guid id, string title, int capacity)
  {
    Id = id;
    Title = title;
    Capacity = capacity;
  }

  // VIOLATION: Fortalece la precondición — requiere asistentes mínimos
  public override Result Confirm()
  {
    if (_attendees.Count < MinimumAttendees)
      return Result.Failure(new Error(
          "Workshop.InsufficientAttendees",
          $"Workshop requires at least {MinimumAttendees} attendees to confirm."));

    return Result.Success();
  }

  public override Result RegisterAttendee(Guid memberId)
  {
    if (_attendees.Count >= Capacity)
      return Result.Failure(new Error("Session.Full", "Session has reached capacity."));

    _attendees.Add(memberId);
    return Result.Success();
  }

  public override Result Cancel()
  {
    return Result.Success();
  }
}

/// <summary>
/// VIOLATION: PremiumSession FORTALECE las precondiciones para el registro.
/// La clase base dice "acepta cualquier miembro" pero PremiumSession solo acepta miembros verificados.
/// </summary>
/// <remarks>
/// Violación de LSP: RegisterAttendee requiere más que el contrato base (el miembro debe ser verificado).
/// La persona que llama usando una referencia SessionBase fallará cuando se le dé una PremiumSession con un miembro no verificado.
/// </remarks>
public sealed class PremiumSession : SessionBase
{
  private readonly List<Guid> _attendees = [];
  private readonly HashSet<Guid> _verifiedMembers;

  public PremiumSession(Guid id, string title, int capacity, HashSet<Guid> verifiedMembers)
  {
    Id = id;
    Title = title;
    Capacity = capacity;
    _verifiedMembers = verifiedMembers;
  }

  public override Result Confirm()
  {
    return Result.Success();
  }

  // VIOLATION: Fortalece la precondición — requiere membresía verificada
  public override Result RegisterAttendee(Guid memberId)
  {
    if (!_verifiedMembers.Contains(memberId))
      return Result.Failure(new Error(
          "PremiumSession.NotVerified",
          "Only verified members can register for premium sessions."));

    if (_attendees.Count >= Capacity)
      return Result.Failure(new Error("Session.Full", "Session has reached capacity."));

    _attendees.Add(memberId);
    return Result.Success();
  }

  public override Result Cancel()
  {
    return Result.Success();
  }
}

/// <summary>
/// VIOLATION: FreeSession DEBILITA las postcondiciones para Cancel.
/// La clase base promete "siempre puede cancelarse" pero FreeSession solo permite cancelación si no hay asistentes.
/// </summary>
/// <remarks>
/// Violación de LSP: Cancel() tiene condiciones de fallo más estrictas que el contrato base.
/// El código que espera SessionBase fallará cuando llame a Cancel() en una FreeSession con asistentes.
/// </remarks>
public sealed class FreeSession : SessionBase
{
  private readonly List<Guid> _attendees = [];

  public FreeSession(Guid id, string title, int capacity)
  {
    Id = id;
    Title = title;
    Capacity = capacity;
  }

  public override Result Confirm()
  {
    return Result.Success();
  }

  public override Result RegisterAttendee(Guid memberId)
  {
    if (_attendees.Count >= Capacity)
      return Result.Failure(new Error("Session.Full", "Session has reached capacity."));

    _attendees.Add(memberId);
    return Result.Success();
  }

  // VIOLATION: Debilita la postcondición — no siempre puede cancelarse como promete el contrato base
  public override Result Cancel()
  {
    if (_attendees.Count > 0)
      return Result.Failure(new Error(
          "FreeSession.HasAttendees",
          "Cannot cancel free sessions with registered attendees."));

    return Result.Success();
  }
}

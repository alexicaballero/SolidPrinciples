namespace SolidPrinciples.SRP.Solution;

/// <summary>
/// Definiciones de error de dominio para operaciones de sesión.
/// </summary>
public static class SessionErrors
{
  public static readonly Common.Error TitleRequired = new("Session.TitleRequired", "Se requiere el título de la sesión.");
  public static readonly Common.Error SpeakerRequired = new("Session.SpeakerRequired", "Se requiere el nombre del orador.");
  public static readonly Common.Error DateMustBeFuture = new("Session.DateMustBeFuture", "La fecha de la sesión debe estar en el futuro.");
  public static readonly Common.Error NotFound = new("Session.NotFound", "No se encontró la sesión.");
}

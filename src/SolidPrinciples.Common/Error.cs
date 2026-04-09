namespace SolidPrinciples.Common;

/// <summary>
/// Representa un error de dominio con un código y descripción legible por humanos.
/// </summary>
public sealed record Error(string Code, string Description)
{
  public static readonly Error None = new(string.Empty, string.Empty);
}

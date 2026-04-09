using SolidPrinciples.Common;

namespace SolidPrinciples.LSP.Problem;

/// <summary>
/// PROBLEMA: Esta clase base define un contrato para miembros de comunidad,
/// pero las clases derivadas violan el Principio de Sustitución de Liskov por:
/// 1. Lanzar NotImplementedException para métodos heredados
/// 2. Debilitar precondiciones o fortalecer postcondiciones
/// 3. Cambiar el comportamiento esperado de formas que rompen la substituibilidad
/// </summary>
/// <remarks>
/// LSP establece: "Los objetos de una superclase deben poderse reemplazar con objetos
/// de sus subclases sin romper la aplicación."
///
/// La jerarquía a continuación viola esto porque NO PUEDES substituir un Member
/// con un GuestMember o InactiveMember sin fallas en tiempo de ejecución.
/// </remarks>
public abstract class Member
{
  public Guid Id { get; protected set; }
  public string Name { get; protected set; } = string.Empty;
  public string Email { get; protected set; } = string.Empty;

  /// <summary>
  /// Permite que un miembro vote en propuestas comunitarias.
  /// </summary>
  public abstract void Vote(Guid proposalId, bool approve);

  /// <summary>
  /// Permite que un miembro cree una nueva propuesta de sesión.
  /// </summary>
  public abstract Result<Guid> CreateSessionProposal(string title, string description);

  /// <summary>
  /// Envía un mensaje directo a este miembro.
  /// </summary>
  public abstract void SendMessage(string from, string content);
}

/// <summary>
/// Miembro completo — implementa todas las operaciones como se espera.
/// </summary>
public sealed class FullMember : Member
{
  public FullMember(Guid id, string name, string email)
  {
    Id = id;
    Name = name;
    Email = email;
  }

  public override void Vote(Guid proposalId, bool approve)
  {
    // Implementación: registrar el voto en la base de datos
  }

  public override Result<Guid> CreateSessionProposal(string title, string description)
  {
    // Implementación: crear propuesta y devolver su ID
    return Result.Success(Guid.NewGuid());
  }

  public override void SendMessage(string from, string content)
  {
    // Implementación: enviar notificación por correo electrónico
  }
}

/// <summary>
/// VIOLATION: GuestMember es un subtipo pero NO PUEDE ser sustituido por Member.
/// Lanza NotImplementedException para operaciones que los miembros invitados no pueden realizar.
/// </summary>
/// <remarks>
/// Señales de advertencia:
/// - NotImplementedException en métodos heredados
/// - Código que verifica el tipo en tiempo de ejecución antes de llamar a métodos
/// - Código del cliente que captura NotImplementedException
///
/// Impacto:
/// - Una función que espera un Member fallará en tiempo de ejecución si se le da un GuestMember
/// - Debes verificar `member is GuestMember` antes de llamar a Vote() — la verificación de tipos es una mala señal
/// </remarks>
public sealed class GuestMember : Member
{
  public GuestMember(Guid id, string name, string email)
  {
    Id = id;
    Name = name;
    Email = email;
  }

  // VIOLATION: No se puede sustituir — lanza excepción
  public override void Vote(Guid proposalId, bool approve)
  {
    throw new NotImplementedException("Guest members cannot vote.");
  }

  // VIOLATION: No se puede sustituir — lanza excepción
  public override Result<Guid> CreateSessionProposal(string title, string description)
  {
    throw new NotImplementedException("Guest members cannot create proposals.");
  }

  // Este funciona
  public override void SendMessage(string from, string content)
  {
    // Los invitados pueden recibir mensajes
  }
}

/// <summary>
/// VIOLATION: InactiveMember es otro subtipo que viola LSP.
/// Cambia el contrato: TODAS las operaciones no hacen nada silenciosamente.
/// </summary>
/// <remarks>
/// Esto viola LSP porque la persona que llama espera que Vote() registre un voto
/// y CreateSessionProposal() cree una propuesta — pero no lo hacen.
/// El comportamiento es silenciosamente diferente.
/// </remarks>
public sealed class InactiveMember : Member
{
  public InactiveMember(Guid id, string name, string email)
  {
    Id = id;
    Name = name;
    Email = email;
  }

  // VIOLATION: No hace nada silenciosamente — la persona que llama espera que se registre el voto
  public override void Vote(Guid proposalId, bool approve)
  {
    // Sin operación — ¡pero la persona que llama cree que el voto fue registrado!
  }

  // VIOLATION: Devuelve éxito pero en realidad no crea una propuesta
  public override Result<Guid> CreateSessionProposal(string title, string description)
  {
    // Devuelve una ID falsa — la propuesta nunca se crea
    return Result.Success(Guid.Empty);
  }

  // VIOLATION: Ignora silenciosamente los mensajes
  public override void SendMessage(string from, string content)
  {
    // Sin operación — el mensaje no se envía
  }
}

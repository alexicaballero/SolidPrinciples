using SolidPrinciples.Common;

namespace SolidPrinciples.LSP.Solution;

/// <summary>
/// SOLUCIÓN: En lugar de una jerarchía de herencia con substituibilidad rota,
/// usamos composición con capacidades basadas en roles.
///
/// La entidad Member tiene capacidades que pueden ser otorgadas o revocadas.
/// Sin NotImplementedExceptions, sin fallos silenciosos, sin verificación de tipos.
/// </summary>
/// <remarks>
/// Principio de diseño: Preferir composición sobre herencia cuando el comportamiento varía según el subtipo.
///
/// Beneficios:
/// - Todos los Members son substituibles — no se necesita verificación de tipos en tiempo de ejecución
/// - Las capacidades son explícitas y consultables
/// - Puedes otorgar/revocar capacidades sin cambiar tipos
/// </remarks>
public sealed class Member
{
  public Guid Id { get; private set; }
  public string Name { get; private set; } = string.Empty;
  public string Email { get; private set; } = string.Empty;
  public MemberCapabilities Capabilities { get; private set; } = null!;

  private Member() { }

  public static Member CreateFull(Guid id, string name, string email) =>
      new()
      {
        Id = id,
        Name = name,
        Email = email,
        Capabilities = new MemberCapabilities(CanVote: true, CanPropose: true, CanReceiveMessages: true)
      };

  public static Member CreateGuest(Guid id, string name, string email) =>
      new()
      {
        Id = id,
        Name = name,
        Email = email,
        Capabilities = new MemberCapabilities(CanVote: false, CanPropose: false, CanReceiveMessages: true)
      };

  public static Member CreateInactive(Guid id, string name, string email) =>
      new()
      {
        Id = id,
        Name = name,
        Email = email,
        Capabilities = new MemberCapabilities(CanVote: false, CanPropose: false, CanReceiveMessages: false)
      };

  /// <summary>
  /// CORRECTO: Devuelve Result con razón clara del fallo en lugar de lanzar excepción.
  /// La persona que llama puede verificar capacidades ANTES de llamar.
  /// </summary>
  public Result Vote(Guid proposalId, bool approve)
  {
    if (!Capabilities.CanVote)
      return Result.Failure(MemberErrors.VotingNotAllowed);

    // Record vote
    return Result.Success();
  }

  /// <summary>
  /// CORRECTO: Devuelve Result — sin excepciones, sin fallos silenciosos.
  /// </summary>
  public Result<Guid> CreateSessionProposal(string title, string description)
  {
    if (!Capabilities.CanPropose)
      return Result.Failure<Guid>(MemberErrors.ProposalNotAllowed);

    // Create proposal
    return Result.Success(Guid.NewGuid());
  }

  /// <summary>
  /// CORRECTO: Devuelve Result — la persona que llama sabe si el mensaje fue enviado o no.
  /// </summary>
  public Result SendMessage(string from, string content)
  {
    if (!Capabilities.CanReceiveMessages)
      return Result.Failure(MemberErrors.MessagingNotAllowed);

    // Send message
    return Result.Success();
  }
}

/// <summary>
/// Define lo que un miembro puede hacer. Explícito y consultable.
/// </summary>
public sealed record MemberCapabilities(
    bool CanVote,
    bool CanPropose,
    bool CanReceiveMessages);

/// <summary>
/// Domain errors for member operations.
/// </summary>
public static class MemberErrors
{
  public static readonly Error VotingNotAllowed = new(
      "Member.VotingNotAllowed",
      "This member does not have permission to vote.");

  public static readonly Error ProposalNotAllowed = new(
      "Member.ProposalNotAllowed",
      "This member does not have permission to create proposals.");

  public static readonly Error MessagingNotAllowed = new(
      "Member.MessagingNotAllowed",
      "This member cannot receive messages.");
}

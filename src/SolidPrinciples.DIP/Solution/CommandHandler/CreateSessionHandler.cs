using SolidPrinciples.Common;

namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// SOLUCIÓN: La lógica empresarial de alto nivel depende de abstracciones (interfaces),
/// no de implementaciones concretas.
///
/// Esto sigue el Principio de Inversión de Dependencias:
/// - Los módulos de alto nivel dependen de abstracciones
/// - Los módulos de bajo nivel dependen de abstracciones
/// - Las dependencias se inyectan vía constructor
/// </summary>
/// <remarks>
/// Beneficios:
/// - El controlador puede ser probado con implementaciones en memoria/simuladas
/// - Sin acoplamiento a SQL, Correo, o ninguna infraestructura específica
/// - Fácil intercambiar implementaciones en tiempo de ejecución (contenedor DI)
/// - El controlador se enfoca en lógica empresarial, no en detalles de infraestructura
/// </remarks>
public sealed class CreateSessionHandler
{
    private readonly ISessionRepository _repository;
    private readonly INotificationSender _notificationSender;

    /// <summary>
    /// CORRECTO: Las dependencias se inyectan vía constructor.
    /// El controlador depende de abstracciones (ISessionRepository, INotificationSender).
    /// </summary>
    public CreateSessionHandler(ISessionRepository repository, INotificationSender notificationSender)
    {
        _repository = repository;
        _notificationSender = notificationSender;
    }

    /// <summary>
    /// Maneja la creación de una nueva sesión.
    /// Se puede hacer pruebas unitarias con dependencias en memoria/simuladas.
    /// </summary>
    public async Task<Result> HandleAsync(string title, string description, Guid communityId, CancellationToken cancellationToken)
    {
        // CORRECTO: Usar factory method de la entidad de dominio (de SolidPrinciples.Common)
        var sessionResult = Session.Create(title, description, communityId);
        if (sessionResult.IsFailure)
            return Result.Failure(sessionResult.Error);

        // CORRECTO: Llamar métodos en abstracciones
        await _repository.SaveAsync(sessionResult.Value, cancellationToken);
        await _notificationSender.SendAsync("admin@example.com", $"New session proposed: {title}", cancellationToken);

        return Result.Success();
    }
}

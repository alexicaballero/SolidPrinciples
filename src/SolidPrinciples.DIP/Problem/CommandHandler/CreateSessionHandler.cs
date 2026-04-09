using SolidPrinciples.Common;

namespace SolidPrinciples.DIP.Problem;

/// <summary>
/// PROBLEMA: La lógica empresarial de alto nivel (controlador de comando) depende directamente de
/// clases concretas de infraestructura de bajo nivel (SqlSessionRepository, EmailNotificationSender).
///
/// Esto viola el Principio de Inversión de Dependencias:
/// - Los módulos de alto nivel NO deben depender de módulos de bajo nivel
/// - Ambos deben depender de abstracciones
/// </summary>
/// <remarks>
/// Señales de advertencia:
/// - El controlador de comando usa `new` para instanciar dependencias
/// - El controlador importa System.Data.SqlClient (preocupación de infraestructura de bajo nivel)
/// - El controlador no se puede probar sin una base de datos real y servidor de correo
/// - Cambiar persistencia (SQL → MongoDB) requiere modificar el controlador
///
/// Impacto:
/// - Imposible hacer pruebas unitarias (requiere prueba de integración con base de datos/correo)
/// - Acoplamiento estrecho entre lógica empresarial e infraestructura
/// - Difícil intercambiar implementaciones (por ejemplo, en-memoria para pruebas)
/// </remarks>
public sealed class CreateSessionHandler
{
    /// <summary>
    /// VIOLACIÓN: El controlador instancia directamente dependencias concretas.
    /// La lógica empresarial está acoplada a detalles de infraestructura.
    /// </summary>
    public async Task<Result> HandleAsync(string title, string description, Guid communityId, CancellationToken cancellationToken)
    {
        // VIOLACIÓN: El código de alto nivel depende del repositorio SQL de bajo nivel
        var repository = new SqlSessionRepository();

        // VIOLACIÓN: El código de alto nivel depende del remitente de correo de bajo nivel
        var notificationSender = new EmailNotificationSender();

        // VIOLACIÓN: Creación manual en lugar de usar factory method
        // (además, no valida entradas - doble problema)
        var session = new Session(Guid.NewGuid(), title, description, communityId);

        // VIOLACIÓN: Llamar métodos en clases concretas
        await repository.SaveAsync(session, cancellationToken);
        await notificationSender.SendAsync("admin@example.com", $"New session proposed: {title}", cancellationToken);

        return Result.Success();
    }
}

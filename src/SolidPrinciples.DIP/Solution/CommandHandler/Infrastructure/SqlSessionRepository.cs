using SolidPrinciples.Common;

namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// CORRECTO: Implementación concreta de repositorio SQL.
/// Esta clase depende de la abstracción ISessionRepository (la implementa).
/// El código de alto nivel NO depende de esta clase directamente.
/// </summary>
/// <remarks>
/// Detalle de infraestructura de bajo nivel. Esta clase se registra en el contenedor DI:
/// services.AddScoped&lt;ISessionRepository, SqlSessionRepository&gt;();
///
/// Flujo de dependencia:
/// - CreateSessionHandler → ISessionRepository (abstracción)
/// - SqlSessionRepository → ISessionRepository (implementa abstracción)
/// La flecha de dependencia apunta HACIA ADENTRO (hacia abstracciones), no hacia afuera.
/// </remarks>
public sealed class SqlSessionRepository : ISessionRepository
{
    public async Task SaveAsync(Session session, CancellationToken cancellationToken)
    {
        // Lógica de persistencia específica de SQL
        await Task.Delay(10, cancellationToken);
        Console.WriteLine($"[SQL] Saved session: {session.Title}");
    }

    public async Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Lógica de consulta específica de SQL
        await Task.Delay(10, cancellationToken);
        return null;
    }
}

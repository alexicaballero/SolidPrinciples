namespace SolidPrinciples.DIP.Problem;

/// <summary>
/// PROBLEMA: Implementación concreta de repositorio con lógica específica de SQL.
/// El código de alto nivel depende directamente de esto, haciendo imposible intercambiar implementaciones.
/// </summary>
/// <remarks>
/// Esta clase es infraestructura de bajo nivel. La lógica empresarial NO debería depender de esto directamente.
/// En lugar de eso, la lógica empresarial debería depender de una abstracción (interfaz).
/// </remarks>
public sealed class SqlSessionRepository
{
    /// <summary>
    /// Guarda una sesión en SQL Server.
    /// El código de alto nivel depende de este método concreto.
    /// </summary>
    public async Task SaveAsync(Session session, CancellationToken cancellationToken)
    {
        // Simulated SQL database save
        // In a real system: using System.Data.SqlClient to execute INSERT
        await Task.Delay(10, cancellationToken); // Simulate I/O
        Console.WriteLine($"[SQL] Saved session: {session.Title}");
    }

    /// <summary>
    /// Recupera una sesión de SQL Server.
    /// </summary>
    public async Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Simulated SQL database query
        await Task.Delay(10, cancellationToken);
        return null; // Simplified
    }
}

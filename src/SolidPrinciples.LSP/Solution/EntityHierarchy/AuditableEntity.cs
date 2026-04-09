namespace SolidPrinciples.LSP.Solution.EntityHierarchy;

/// <summary>
/// SOLUCIÓN: AuditableEntity EXTIENDE sin cambiar el contrato base.
/// </summary>
/// <remarks>
/// AuditableEntity respeta LSP porque:
/// 1. NO sobrescribe ni oculta Raise() - hereda el comportamiento tal cual
/// 2. AGREGA nuevas propiedades (CreatedAt, UpdatedAt) sin cambiar el comportamiento de Entity
/// 3. Garantiza que CreatedAt esté SIEMPRE establecido (no anulable, inicializado en constructor)
/// 4. Puede sustituirse por Entity en cualquier lugar sin romper el código
///
/// Patrón: Extensión por adición, no por modificación
/// </remarks>
public abstract class AuditableEntity : Entity
{
    /// <summary>
    /// CORRECTO: Timestamp no anulable asegura que el invariante siempre se mantenga.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    protected AuditableEntity()
    {
        // CORRECTO: Invariante garantizado en tiempo de construcción
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Marca la entidad como actualizada con el timestamp actual.
    /// </summary>
    protected void MarkAsUpdated()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

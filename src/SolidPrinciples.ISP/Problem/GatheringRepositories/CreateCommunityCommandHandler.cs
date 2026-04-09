using SolidPrinciples.Common;

namespace SolidPrinciples.ISP.Problem.GatheringRepositories;

/// <summary>
/// PROBLEMA: CreateCommunityCommandHandler solo necesita operaciones de community,
/// pero depende de la interfaz COMPLETA con 16+ métodos.
/// </summary>
/// <remarks>
/// Acoplamiento excesivo en acción:
/// - Depende de IDataRepository (16+ métodos)
/// - Usa solo 2 métodos: AddCommunity() y SaveChangesAsync()
/// - Los otros 14+ métodos son acoplamiento innecesario
///
/// Impacto:
/// Difícil de probar: Mock requiere 16+ métodos para configurar
/// Cambios en métodos de Session afectan este handler de Community
/// No está claro qué métodos usa realmente el handler
/// Violación de ISP: forzado a depender de métodos que no usa
///
/// Del artículo:
/// "Este handler depende de 17+ métodos pero usa solo 2:
/// AddCommunity() y SaveChangesAsync().
/// ¿Los otros 15 métodos? Acoplamiento innecesario."
/// </remarks>
public sealed class CreateCommunityCommandHandler
{
    private readonly IDataRepository _repository;

    public CreateCommunityCommandHandler(IDataRepository repository)
    {
        _repository = repository;
        // Este handler depende de 16+ métodos pero usa solo 2:
        // AddCommunity() y SaveChangesAsync()
    }

    public async Task<Result<Guid>> HandleAsync(
        string name,
        string description,
        CancellationToken ct = default)
    {
        var result = Community.Create(name, description);
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        _repository.AddCommunity(result.Value);       // Usa 1 método
        await _repository.SaveChangesAsync(ct);        // Usa 1 método

        return Result.Success(result.Value.Id);
        // ¿Los otros 14 métodos? Acoplamiento innecesario.
    }
}

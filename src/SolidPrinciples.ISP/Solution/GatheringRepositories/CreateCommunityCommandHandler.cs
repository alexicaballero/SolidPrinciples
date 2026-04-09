using SolidPrinciples.Common;

namespace SolidPrinciples.ISP.Solution.GatheringRepositories;

/// <summary>
/// SOLUCIÓN ISP - Paso 4: Handler que depende solo de lo que necesita.
/// </summary>
/// <remarks>
/// ISP en acción: este handler demuestra dependencias precisas y enfocadas.
///
/// Dependencias:
/// ✓ ICommunityRepository: Solo CRUD de community. Sin métodos de session contaminando.
/// ✓ IUnitOfWork: Solo SaveChangesAsync. No mezclado con consultas.
/// ✓ IImageStorageService: Solo Upload y Delete. No un IFileService gordo con 20 métodos.
///
/// Del artículo (Gathering.Application/Communities/Create/CreateCommunityCommandHandler.cs):
/// "ICommunityRepository: Solo CRUD de community. Sin métodos de session contaminando la dependencia.
/// IUnitOfWork: Solo SaveChangesAsync. No mezclado con consultas de repositorio.
/// IImageStorageService: Solo Upload y Delete. No un IFileService gordo con 20 métodos."
///
/// Comparación con el problema:
/// Antes: dependía de IDataRepository (16+ métodos), usaba 2
/// ✓ Ahora: depende de ICommunityRepository + IUnitOfWork (2 interfaces enfocadas)
///
/// Beneficios:
/// ✓ Intención clara: el constructor dice exactamente qué hace el handler
/// ✓ Fácil de probar: mock 2 interfaces pequeñas vs 1 interface gorda
/// ✓ Acoplamiento mínimo: solo lo necesario, nada más
/// ✓ Cambios aislados: modificar Session no afecta este handler
/// </remarks>
public sealed class CreateCommunityCommandHandler
{
    private readonly ICommunityRepository _communityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageStorageService _imageStorageService;

    public CreateCommunityCommandHandler(
        ICommunityRepository communityRepository,
        IUnitOfWork unitOfWork,
        IImageStorageService imageStorageService)
    {
        _communityRepository = communityRepository;
        _unitOfWork = unitOfWork;
        _imageStorageService = imageStorageService;

        // ✓ Dependencias precisas: solo lo que necesitamos
        // ✓ Sin acoplamiento a operaciones de Session
        // ✓ Sin acoplamiento a métodos de almacenamiento de documentos/video
    }

    public async Task<Result<Guid>> HandleAsync(
        string name,
        string description,
        Stream? imageStream = null,
        string? imageFileName = null,
        string? imageContentType = null,
        CancellationToken cancellationToken = default)
    {
        // Validación y creación de entidad
        var result = Community.Create(name, description);
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        // Subir imagen si se proporciona
        if (imageStream != null && imageFileName != null && imageContentType != null)
        {
            var uploadResult = await _imageStorageService.UploadImageAsync(
                imageStream,
                imageFileName,
                imageContentType,
                "communities",
                cancellationToken);

            if (uploadResult.IsFailure)
                return Result.Failure<Guid>(uploadResult.Error);
        }

        // Persistir
        _communityRepository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}

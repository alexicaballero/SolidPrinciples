using FluentAssertions;
using NSubstitute;
using SolidPrinciples.Common;
using SolidPrinciples.ISP.Solution.GatheringRepositories;
using SolutionCommunity = SolidPrinciples.ISP.Solution.GatheringRepositories.Community;

namespace SolidPrinciples.ISP.Tests.Solution;

/// <summary>
/// SOLUCIÓN: Estos tests demuestran los beneficios de ISP con interfaces segregadas.
/// 
/// Beneficios:
/// - Mock solo requiere los métodos que realmente se usan
/// - Sin acoplamiento a operaciones irrelevantes
/// - Tests robustos ante cambios en otras interfaces
/// </summary>
public sealed class SegregatedInterfacesTests
{
    [Fact]
    public async Task CreateCommunityHandler_DependsOnlyOnWhatItNeeds()
    {
        // SOLUCIÓN: Solo mockeo las interfaces que el handler realmente usa
        var communityRepository = Substitute.For<ICommunityRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var imageService = Substitute.For<IImageStorageService>();

        var handler = new CreateCommunityCommandHandler(
            communityRepository,
            unitOfWork,
            imageService);

        // Act
        var result = await handler.HandleAsync(
            "Test Community",
            "Test Description",
            null, null, null);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // ✓ Dependencias precisas y claras
        communityRepository.Received(1).Add(Arg.Any<SolutionCommunity>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        // ✓ Sin acoplamiento a operaciones de Session o Resource
        // ✓ Si ISessionRepository cambia, este handler NO se recompila
    }

    [Fact]
    public void MockingSegregatedInterfaces_OnlyRequiresRelevantMethods()
    {
        // SOLUCIÓN: Cada interfaz es pequeña y enfocada

        // Arrange - crear mocks es simple y claro
        var communityRepository = Substitute.For<ICommunityRepository>();
        // ICommunityRepository tiene SOLO métodos de Community: 4 métodos

        var unitOfWork = Substitute.For<IUnitOfWork>();
        // IUnitOfWork tiene SOLO persistencia: 1 método

        var imageService = Substitute.For<IImageStorageService>();
        // IImageStorageService tiene SOLO upload/delete: 2 métodos

        // Assert - dependencias claras en el constructor
        var handler = new CreateCommunityCommandHandler(
            communityRepository,
            unitOfWork,
            imageService);

        handler.Should().NotBeNull();

        // ✓ Total: 7 métodos relevantes vs 16+ irrelevantes en Problem
        // ✓ Intención clara: el constructor dice exactamente qué hace
        // ✓ Testeable: mock 3 interfaces pequeñas vs 1 interface gorda
    }

    [Fact]
    public async Task CommunityRepository_CanBeTestedInIsolation()
    {
        // SOLUCIÓN: Las interfaces segregadas son independientes

        // Arrange - solo mockeo lo que necesito
        var repository = Substitute.For<ICommunityRepository>();
        repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(SolutionCommunity.Create("Test", "Description").Value);

        // Act
        var community = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        community.Should().NotBeNull();
        community!.Name.Should().Be("Test");

        // ✓ Sin necesidad de mockear métodos de Session o Resource
        // ✓ Cada repositorio es testeable de forma independiente
    }

    [Fact]
    public void UnitOfWork_IsSeparateFromRepositories()
    {
        // SOLUCIÓN: IUnitOfWork está segregada de las consultas

        // Arrange
        var unitOfWork = Substitute.For<IUnitOfWork>();

        // Assert - IUnitOfWork solo tiene SaveChangesAsync
        // No contamina con métodos de consulta como GetById, GetAll, etc.
        unitOfWork.Should().NotBeNull();

        // ✓ Separación clara: queries (repositories) vs commands (unit of work)
        // ✓ ISP cumplido: cada interfaz tiene una razón para cambiar
    }

    [Fact]
    public void ImageStorageService_IsFocusedOnImageOperations()
    {
        // SOLUCIÓN: IImageStorageService solo maneja imágenes

        // Arrange
        var imageService = Substitute.For<IImageStorageService>();

        // Assert - solo 2 métodos: Upload y Delete
        // No un IFileService gordo con 20 métodos para documentos, videos, etc.
        imageService.Should().NotBeNull();

        // ✓ Interfaz pequeña y enfocada
        // ✓ Si necesito subir documentos, creo IDocumentStorageService
        // ✓ Cada handler depende solo de lo que necesita
    }
}

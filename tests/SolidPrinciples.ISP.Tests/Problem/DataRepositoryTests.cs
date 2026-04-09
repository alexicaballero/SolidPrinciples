using FluentAssertions;
using NSubstitute;
using SolidPrinciples.Common;
using SolidPrinciples.ISP.Problem.GatheringRepositories;
using ProblemCommunity = SolidPrinciples.ISP.Problem.GatheringRepositories.Community;

namespace SolidPrinciples.ISP.Tests.Problem;

/// <summary>
/// PROBLEMA: Estos tests demuestran por qué las interfaces gordas violan ISP.
/// 
/// Puntos de dolor:
/// - Mock requiere implementar 16+ métodos aunque solo uses 2
/// - Acoplamiento innecesario a operaciones que no se necesitan
/// - Tests frágiles cuando la interfaz cambia
/// </summary>
public sealed class DataRepositoryTests
{
    [Fact]
    public async Task CreateCommunityHandler_UsesOnly2Methods_ButDependsOn16Plus()
    {
        // PROBLEMA: Necesito mockear una interfaz con 16+ métodos
        // aunque solo voy a usar 2 (AddCommunity y SaveChangesAsync)
        var repository = Substitute.For<IDataRepository>();

        var handler = new CreateCommunityCommandHandler(repository);

        // Act
        var result = await handler.HandleAsync(
            "Test Community",
            "Test Description");

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Solo se usan 2 métodos de 16+
        repository.Received(1).AddCommunity(Arg.Any<ProblemCommunity>());
        await repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        // PROBLEMA: El handler está acoplado a 14 métodos que nunca usa:
        // - GetSessionByIdAsync, GetAllSessionsAsync, GetSessionsByCommunityIdAsync, etc.
        // - GetResourcesBySessionIdAsync, AddResource, RemoveResource
        // Este es el dolor de ISP violado
    }

    [Fact]
    public void MockingFatInterface_RequiresImplementing16Methods_PainPoint()
    {
        // PROBLEMA: Para probar CreateCommunityCommandHandler, NSubstitute
        // debe generar implementaciones para TODOS los métodos de IDataRepository
        // aunque solo 2 se usen.

        // Arrange - crear el mock es el problema en sí
        var repository = Substitute.For<IDataRepository>();

        // Este mock tiene que "conocer" sobre:
        // ✗ 7 métodos de Session (que nunca se usarán)
        // ✗ 3 métodos de Resource (que nunca se usarán)
        // ✓ 5 métodos de Community (solo 1 se usará: AddCommunity)
        // ✓ 1 método de persistencia (SaveChangesAsync)

        // Assert - el acoplamiento es evidente
        var handler = new CreateCommunityCommandHandler(repository);
        handler.Should().NotBeNull();

        // PROBLEMA: Si IDataRepository agrega un nuevo método de Session,
        // esta prueba necesita recompilarse aunque no use sessions
    }
}

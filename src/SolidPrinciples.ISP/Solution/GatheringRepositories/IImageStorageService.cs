using SolidPrinciples.Common;

namespace SolidPrinciples.ISP.Solution.GatheringRepositories;

/// <summary>
/// SOLUCIÓN ISP: Interfaz de almacenamiento de imágenes enfocada.
/// </summary>
/// <remarks>
/// En lugar de un IFileService gordo que maneje todas las operaciones de archivo imaginables,
/// definimos una interfaz enfocada para exactamente lo que la aplicación necesita.
///
/// Dos métodos. Esa es la interfaz completa.
/// Cada handler que necesita operaciones de imagen depende de exactamente lo que necesita:
/// subir y eliminar, nada más.
///
/// Del artículo (Gathering.Application/Abstractions/IImageStorageService.cs):
/// "Dos métodos. Esa es la interfaz completa. Cada handler que necesita operaciones
/// de imagen depende de exactamente lo que necesita: subir y eliminar, nada más."
///
/// Beneficios de ISP en la práctica:
/// ✓ Fácil de implementar: 2 métodos, no 12
/// ✓ Fácil de mockear: Las pruebas solo configuran 2 métodos
/// ✓ Fácil de intercambiar: Azure → AWS = implementar 2 métodos, no 12
/// ✓ Interfaz estable: Agregar video no afecta IImageStorageService
/// </remarks>
public interface IImageStorageService
{
    Task<Result<string>> UploadImageAsync(
        Stream imageStream,
        string fileName,
        string contentType,
        string entityType,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteImageAsync(
        string imageUrl,
        CancellationToken cancellationToken = default);
}

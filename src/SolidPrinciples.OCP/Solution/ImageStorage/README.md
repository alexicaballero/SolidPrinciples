# Solución: Almacenamiento de Imágenes con OCP

## La Solución: Patrón Strategy

El código de almacenamiento de imágenes ahora sigue OCP usando el **patrón Strategy**: cada proveedor es una clase separada que implementa una interfaz común.

### 📁 Archivos de la Solución

| Archivo                        | Rol                                                         |
| ------------------------------ | ----------------------------------------------------------- |
| `IImageStore.cs`               | Abstracción que define el contrato                          |
| `LocalFileSystemImageStore.cs` | Estrategia concreta para filesystem                         |
| `AzureBlobImageStore.cs`       | Estrategia concreta para Azure Blob                         |
| `ImageUploadService.cs`        | Orquestador que usa IImageStore (cerrado para modificación) |
| `CommunityImageDto.cs`         | DTO neutral (igual que en Problem)                          |

## 🎯 La Transformación

### Antes (Violación OCP)

```csharp
// Switch que cambia con cada proveedor
public async Task<string> UploadImageAsync(StorageType storageType, CommunityImageDto image)
{
    return storageType switch
    {
        StorageType.Local => await SaveToLocalFileSystemAsync(image),
        StorageType.AzureBlob => await SaveToAzureBlobStorageAsync(image),
        // Agregar AWS S3 = modificar esta clase
        _ => throw new NotSupportedException()
    };
}
```

### Después (Cumple OCP)

```csharp
// Delega a estrategia inyectada
public Task<string> UploadImageAsync(CommunityImageDto image)
{
    return _store.SaveAsync(image);
}
```

**Diferencia clave**: Ya no hay switch. La lógica de cada proveedor está en clases separadas.

## 🏗️ Arquitectura de la Solución

### 1️⃣ `IImageStore.cs` - Abstracción

**Responsabilidad**: Definir el contrato para almacenar imágenes.

```csharp
// Esta interfaz NO cambia al agregar proveedores
public interface IImageStore
{
    Task<string> SaveAsync(CommunityImageDto image);
}
```

**Cerrada para modificación**: Agregar AWS S3, Google Cloud o FTP no requiere cambiar esta interfaz.

### 2️⃣ Estrategias Concretas - Implementaciones

Cada proveedor vive en su propia clase:

```csharp
// LocalFileSystemImageStore: Solo cambia si cambia lógica de filesystem
public sealed class LocalFileSystemImageStore : IImageStore
{
    public async Task<string> SaveAsync(CommunityImageDto image)
    {
        // Lógica de filesystem local
        var directoryPath = Path.Combine(_basePath, image.CommunityId.ToString());
        Directory.CreateDirectory(directoryPath);
        var filePath = Path.Combine(directoryPath, image.FileName);
        await File.WriteAllBytesAsync(filePath, image.ImageData);
        return $"file://{filePath}";
    }
}

// AzureBlobImageStore: Solo cambia si cambia lógica de Azure
public sealed class AzureBlobImageStore : IImageStore
{
    public async Task<string> SaveAsync(CommunityImageDto image)
    {
        // Lógica de Azure Blob Storage
        var blobName = $"{image.CommunityId}/{image.FileName}";
        // En prod: await blobClient.UploadAsync(...)
        return $"https://{_containerName}.blob.core.windows.net/{blobName}";
    }
}
```

**Abierto para extensión**: Agregar un nuevo proveedor es crear una nueva clase.

### 3️⃣ `ImageUploadService.cs` - Orquestador

**Responsabilidad**: Coordinar la subida de imágenes usando la estrategia inyectada.

```csharp
// Depende de abstracción, no de implementaciones concretas
public sealed class ImageUploadService
{
    private readonly IImageStore _store;

    public ImageUploadService(IImageStore store)
    {
        _store = store;
    }

    // Este método NO cambia al agregar proveedores
    public Task<string> UploadImageAsync(CommunityImageDto image)
    {
        return _store.SaveAsync(image);
    }
}
```

**Cerrado para modificación**: El servicio no cambia al agregar AWS S3 o Google Cloud.

## 🚀 Extensibilidad sin Modificación

### Agregar AWS S3

Para soportar AWS S3, simplemente creamos una nueva clase:

```csharp
// Nueva clase, SIN modificar código existente
public sealed class AmazonS3ImageStore : IImageStore
{
    private readonly string _bucketName;
    private readonly string _region;

    public AmazonS3ImageStore(string bucketName, string region)
    {
        _bucketName = bucketName;
        _region = region;
    }

    public async Task<string> SaveAsync(CommunityImageDto image)
    {
        // Lógica de AWS S3 SDK
        // var s3Client = new AmazonS3Client(region);
        // await s3Client.PutObjectAsync(new PutObjectRequest { ... });

        var key = $"{image.CommunityId}/{image.FileName}";
        return $"https://{_bucketName}.s3.{_region}.amazonaws.com/{key}";
    }
}
```

**Cambios necesarios**:

- Crear `AmazonS3ImageStore.cs` (nueva clase)
- Registrar en DI: `services.AddSingleton<IImageStore, AmazonS3ImageStore>()`

**NO se modifica**:

- `IImageStore.cs` (sin cambios)
- `ImageUploadService.cs` (sin cambios)
- `LocalFileSystemImageStore.cs` (sin cambios)
- `AzureBlobImageStore.cs` (sin cambios)

### Agregar Google Cloud Storage

```csharp
// Nueva clase independiente
public sealed class GoogleCloudImageStore : IImageStore
{
    private readonly string _bucketName;

    public async Task<string> SaveAsync(CommunityImageDto image)
    {
        // Lógica de Google Cloud Storage SDK
        var objectName = $"{image.CommunityId}/{image.FileName}";
        return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
    }
}
```

**Mismo proceso**: Crear clase, registrar en DI. Cero modificaciones en código existente.

## 🧪 Testabilidad Mejorada

### Tests Unitarios

```csharp
[Fact]
public async Task UploadImage_WithLocalStorage_ReturnsFileUrl()
{
    // Arrange
    var store = new LocalFileSystemImageStore("C:\\temp");
    var service = new ImageUploadService(store);
    var image = new CommunityImageDto { ... };

    // Act
    var url = await service.UploadImageAsync(image);

    // Assert
    url.Should().StartWith("file://");
}

[Fact]
public async Task UploadImage_WithAzureBlob_ReturnsBlobUrl()
{
    // Arrange
    var store = new AzureBlobImageStore("conn", "images");
    var service = new ImageUploadService(store);
    var image = new CommunityImageDto { ... };

    // Act
    var url = await service.UploadImageAsync(image);

    // Assert
    url.Should().Contain("blob.core.windows.net");
}
```

### Tests con Mocks

```csharp
[Fact]
public async Task UploadImage_DelegatesToStore()
{
    // Arrange
    var mockStore = Substitute.For<IImageStore>();
    mockStore.SaveAsync(Arg.Any<CommunityImageDto>())
        .Returns("https://mock.url/image.png");

    var service = new ImageUploadService(mockStore);
    var image = new CommunityImageDto { ... };

    // Act
    var url = await service.UploadImageAsync(image);

    // Assert
    url.Should().Be("https://mock.url/image.png");
    await mockStore.Received(1).SaveAsync(image);
}
```

**Beneficios**:

- Cada proveedor se testea independientemente
- Mocks fáciles con `IImageStore`
- No necesito configuración de proveedores que no uso

## 🎨 Patrones Aplicados

| Patrón                    | Aplicación                                                           |
| ------------------------- | -------------------------------------------------------------------- |
| **Strategy**              | `IImageStore` con múltiples implementaciones                         |
| **Dependency Injection**  | `ImageUploadService` recibe `IImageStore` en constructor             |
| **Open/Closed Principle** | Cerrado: `ImageUploadService` / Abierto: nuevas clases `IImageStore` |

## 📊 Comparación Final

| Aspecto               | Violación (Problem)                       | Solución (Solution)                         |
| --------------------- | ----------------------------------------- | ------------------------------------------- |
| **Agregar proveedor** | Modificar clase existente                 | Crear nueva clase                           |
| **Switch/if-else**    | Sí, crece con cada proveedor              | No, usa polimorfismo                        |
| **Testabilidad**      | Difícil (mocks complejos)                 | Fácil (mock IImageStore)                    |
| **Equipos paralelos** | Merge conflicts (todos tocan misma clase) | Trabajo independiente                       |
| **Riesgo al cambiar** | Alto (tocar código existente)             | Bajo (solo nueva clase)                     |
| **Complejidad**       | Crece linealmente con proveedores         | Constante (cada proveedor es independiente) |

## 💡 Lecciones Clave

1. **Abstracción estable**: `IImageStore` define un contrato que no cambia
2. **Estrategias concretas**: Cada proveedor en su propia clase
3. **Inyección de dependencias**: El servicio recibe la estrategia desde fuera
4. **Extensión sin modificación**: Nuevas funcionalidades = nuevas clases

## 🔗 Referencias

- [Open/Closed Principle - Gathering Article](https://calm-field-0d87ced10.6.azurestaticapps.net/post/es/solid-principles/open-closed)
- [Gathering Backend - IImageStore Implementation](https://github.com/alexicaballero/gathering/tree/main/backend)
- [Problem/ImageStorage](../../Problem/ImageStorage/README.md) - Ver la violación original

## ▶️ Próximo Paso

Ver los tests unitarios en `tests/SolidPrinciples.OCP.Tests/Solution/ImageStorageTests.cs` para ejemplos ejecutables.

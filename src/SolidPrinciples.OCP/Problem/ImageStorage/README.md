# Problema: Almacenamiento de Imágenes (Violación de OCP)

## 🚨 La Violación: Switch de Proveedores de Almacenamiento

El servicio de subida de imágenes viola OCP porque **debe ser modificado** cada vez que queremos agregar un nuevo proveedor de almacenamiento.

### 📁 Archivos del Problema

| Archivo                 | Rol                                                                 |
| ----------------------- | ------------------------------------------------------------------- |
| `StorageType.cs`        | Enum que representa tipos de almacenamiento (Local, Azure)          |
| `CommunityImageDto.cs`  | DTO con datos de la imagen a subir                                  |
| `ImageUploadService.cs` | **VIOLACIÓN**: Switch en `storageType` que crece con cada proveedor |

## ¿Qué está mal?

### 1. Switch Basado en Tipo

```csharp
public async Task<string> UploadImageAsync(StorageType storageType, CommunityImageDto image)
{
    return storageType switch
    {
        StorageType.Local => await SaveToLocalFileSystemAsync(image),
        StorageType.AzureBlob => await SaveToAzureBlobStorageAsync(image),
        // Agregar AWS S3 requiere editar esta clase
        _ => throw new NotSupportedException()
    };
}
```

**Problema**: El switch obliga a modificar `ImageUploadService` cada vez que se agrega un proveedor.

### 2. No está "Cerrado para Modificación"

Para agregar **AWS S3**, necesitamos:

1. ✏️ Modificar `StorageType` (agregar `AmazonS3`)
2. ✏️ Modificar `ImageUploadService.UploadImageAsync` (agregar case)
3. ✏️ Agregar método `SaveToAmazonS3Async`
4. ✏️ Modificar constructor (agregar parámetros de configuración S3)

**Cuatro modificaciones** en código existente = **OCP violado**.

### 3. Constructor Acoplado a Todos los Proveedores

```csharp
public ImageUploadService(
    string localBasePath,
    string azureBlobConnectionString,  // Necesito esto incluso si solo uso Local
    string azureContainerName)         // Acoplamiento innecesario
```

**Problema**: Incluso si solo uso almacenamiento local, debo pasar configuración de Azure.

## 🔎 Señales de Alerta (Warning Signs)

Cuando ves estos patrones, probablemente estás violando OCP:

| Patrón                                          | Indica                                             |
| ----------------------------------------------- | -------------------------------------------------- |
| `enum` que representa "tipos" de comportamiento | Probablemente habrá un switch sobre este enum      |
| `switch (tipo)` con casos de comportamiento     | Cada nuevo tipo requiere modificar el switch       |
| Métodos privados por cada tipo                  | Cada proveedor = un método nuevo en la misma clase |
| Constructor con N configuraciones               | Acoplado a todos los proveedores posibles          |

## 💥 ¿Qué pasa cuando cambian los requisitos?

### Escenario 1: Agregar AWS S3

**Requisito**: "Necesitamos almacenar imágenes en AWS S3 para clientes enterprise."

**Cambios necesarios**:

- ✏️ Modificar `StorageType.cs`
- ✏️ Modificar `ImageUploadService.UploadImageAsync`
- ✏️ Agregar `SaveToAmazonS3Async`
- ✏️ Modificar constructor
- ✏️ Re-compilar y re-testear **todo**

**Impacto**: Alto riesgo de romper código existente (Local y Azure).

### Escenario 2: Agregar Google Cloud Storage

**Requisito**: "Queremos soporte para Google Cloud Storage."

**Cambios necesarios**: Los mismos 5 pasos anteriores.

**Problema**: Cada desarrollador que agrega un proveedor **toca la misma clase** = merge conflicts constantes.

### Escenario 3: Testing

**Problema con tests**:

```csharp
[Fact]
public async Task UploadImage_WithLocalStorage_SavesFile()
{
    // Necesito pasar configuración de Azure aunque no la uso
    var service = new ImageUploadService(
        localBasePath: "C:\\temp",
        azureBlobConnectionString: "irrelevant",  // Ruido
        azureContainerName: "irrelevant");        // Ruido

    // Difícil hacer mock del filesystem
    var result = await service.UploadImageAsync(StorageType.Local, image);
}
```

**Problemas**:

- No puedo testear un proveedor independientemente
- Necesito configuración de proveedores que no uso
- Difícil hacer mocks (filesystem, Azure SDK)

## 🎯 ¿Qué debería pasar?

**Ideal**: Agregar un nuevo proveedor sin modificar código existente.

```csharp
// Código ideal (ver Solution/)
// Agregar AWS S3 = crear nueva clase que implementa IImageStore
public sealed class AmazonS3ImageStore : IImageStore
{
    // Nueva funcionalidad SIN tocar código viejo
}
```

## 🔗 Siguiente Paso

Ver [`../Solution/ImageStorage/README.md`](../../Solution/ImageStorage/README.md) para la implementación correcta usando el **patrón Strategy**.

## 📖 Referencia

- [Open/Closed Principle - Gathering Article](https://calm-field-0d87ced10.6.azurestaticapps.net/post/es/solid-principles/open-closed)
- Implementación de referencia: [Gathering Backend - IImageStore](https://github.com/alexicaballero/gathering/tree/main/backend)

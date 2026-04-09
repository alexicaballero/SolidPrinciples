# Principio de Segregación de Interfaces (ISP)

## Definición

> **Ningún cliente debería verse forzado a depender de métodos que no utiliza.**

En términos prácticos: preferir varias interfaces pequeñas y enfocadas sobre una sola interfaz grande y de propósito general.

## Ejemplos Educativos

Este proyecto contiene dos casos de uso principales del artículo ISP, progresando de simple a complejo.

### 1. Impresora Multifunción (Ejemplo Simple)

**Ubicación**: `PrinterExample/`

Ejemplo clásico y universal de ISP que demuestra el concepto básico.

#### Problem

- [IMultiFunctionDevice.cs](Problem/PrinterExample/IMultiFunctionDevice.cs) - Interfaz gorda (5 métodos)
- [BasicInkjetPrinter.cs](Problem/PrinterExample/BasicInkjetPrinter.cs) - Implementación que lanza `NotSupportedException`

**Problema**: Una impresora básica que solo puede imprimir es forzada a implementar 5 métodos (scan, fax, staple, photocopy).

#### Solution

- [IPrinter.cs](Solution/PrinterExample/IPrinter.cs) - Interfaz enfocada en impresión
- [IScanner.cs](Solution/PrinterExample/IScanner.cs) - Interfaz enfocada en escaneo
- [IFax.cs](Solution/PrinterExample/IFax.cs) - Interfaz enfocada en fax
- [BasicInkjetPrinter.cs](Solution/PrinterExample/BasicInkjetPrinter.cs) - Solo implementa `IPrinter`
- [OfficePrinter.cs](Solution/PrinterExample/OfficePrinter.cs) - Implementa múltiples interfaces
- [DocumentService.cs](Solution/PrinterExample/DocumentService.cs) - Cliente que depende solo de lo necesario

**Solución**: Cada capacidad es su propia interfaz. Las clases implementan solo lo que realmente soportan.

📖 **Ver**: [Problem/PrinterExample/README.md](Problem/PrinterExample/README.md) y [Solution/PrinterExample/README.md](Solution/PrinterExample/README.md)

---

### 2. Interfaces de Repositorio en Gathering (Caso de Estudio Real)

**Ubicación**: `GatheringRepositories/`

Ejemplo del mundo real basado en el [backend de Gathering](https://github.com/alexicaballero/gathering/tree/main/backend/src).

#### Problem

- [IDataRepository.cs](Problem/GatheringRepositories/IDataRepository.cs) - Interfaz gorda (16+ métodos)
- [CreateCommunityCommandHandler.cs](Problem/GatheringRepositories/CreateCommunityCommandHandler.cs) - Depende de 16+ métodos, usa solo 2

**Problema**: Un handler que crea comunidades está acoplado a operaciones de Session, Community, Resource y Persistencia.

#### Solution

- [IRepository<T>.cs](Solution/GatheringRepositories/IRepository.cs) - Repositorio genérico CRUD
- [ISessionRepository.cs](Solution/GatheringRepositories/ISessionRepository.cs) - Extiende con consultas específicas
- [ICommunityRepository.cs](Solution/GatheringRepositories/ICommunityRepository.cs) - Solo hereda `IRepository<Community>`
- [IUnitOfWork.cs](Solution/GatheringRepositories/IUnitOfWork.cs) - Solo persistencia (1 método)
- [IImageStorageService.cs](Solution/GatheringRepositories/IImageStorageService.cs) - Solo Upload y Delete
- [CreateCommunityCommandHandler.cs](Solution/GatheringRepositories/CreateCommunityCommandHandler.cs) - Depende solo de lo necesario

**Solución**: Interfaces segregadas que permiten dependencias precisas y enfocadas.

📖 **Ver**: [Problem/GatheringRepositories/README.md](Problem/GatheringRepositories/README.md) y [Solution/GatheringRepositories/README.md](Solution/GatheringRepositories/README.md)

---

## Progresión Didáctica

```
Ejemplo Simple (Impresora)
    ↓
Concepto básico de ISP
    ↓
Caso de Estudio (Gathering)
    ↓
ISP en aplicación real (Clean Architecture + CQRS)
```

## Señales de Violación de ISP

✋ **Si una clase implementa una interfaz y la mitad de los métodos lanzan `NotImplementedException`**, la interfaz viola ISP.

✋ **Si un cliente depende de una interfaz con 16+ métodos pero usa solo 2**, ISP está violado.

## Beneficios de Aplicar ISP

| Beneficio                   | Descripción                                                      |
| --------------------------- | ---------------------------------------------------------------- |
| **Acoplamiento reducido**   | Los cambios en una interfaz no afectan a clientes que no la usan |
| **Pruebas más simples**     | Mockear interfaces pequeñas vs interfaces gordas                 |
| **Intención más clara**     | El constructor dice exactamente qué hace la clase                |
| **Evolución independiente** | Cada interfaz evoluciona sin afectar a otras                     |
| **DI preciso**              | Inyección de dependencias enfocada                               |

## Comparación: Antes vs Ahora (Gathering Example)

| Aspecto                 | Problema         | Solución               |
| ----------------------- | ---------------- | ---------------------- |
| **Dependencias**        | 1 interfaz gorda | 3 interfaces enfocadas |
| **Métodos disponibles** | 16+              | 5                      |
| **Métodos usados**      | 2 de 16 (12.5%)  | 5 de 5 (100%)          |
| **Acoplamiento**        | Alto             | Mínimo                 |
| **Mock en tests**       | 16+ métodos      | 5 métodos              |

## Referencias

- **Artículo completo**: [ISP - Interface Segregation Principle](https://calm-field-0d87ced10.6.azurestaticapps.net/post/es/solid-principles/interface-segregation)
- **Backend de referencia**: [Gathering](https://github.com/alexicaballero/gathering/tree/main/backend/src)
- **Artículo en inglés**: [ISP - English](https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/interface-segregation)

## Estructura del Proyecto

```
SolidPrinciples.ISP/
├── Problem/
│   ├── PrinterExample/          (Interfaz gorda + violación)
│   │   ├── IMultiFunctionDevice.cs
│   │   ├── BasicInkjetPrinter.cs
│   │   ├── Document.cs
│   │   └── README.md
│   └── GatheringRepositories/   (Repositorio gordo + acoplamiento)
│       ├── IDataRepository.cs
│       ├── CreateCommunityCommandHandler.cs
│       ├── DomainEntities.cs
│       └── README.md
└── Solution/
    ├── PrinterExample/          (Interfaces segregadas)
    │   ├── IPrinter.cs
    │   ├── IScanner.cs
    │   ├── IFax.cs
    │   ├── BasicInkjetPrinter.cs
    │   ├── OfficePrinter.cs
    │   ├── DocumentService.cs
    │   ├── Document.cs
    │   └── README.md
    └── GatheringRepositories/   (Interfaces enfocadas + DI preciso)
        ├── IRepository.cs
        ├── ISessionRepository.cs
        ├── ICommunityRepository.cs
        ├── IUnitOfWork.cs
        ├── IImageStorageService.cs
        ├── CreateCommunityCommandHandler.cs
        ├── DomainEntities.cs
        └── README.md
```

## Patrón Clave

**Segregación + Composición**: Interfaces pequeñas y enfocadas que se componen según las necesidades del cliente.

```csharp
// Cliente simple
class SimpleHandler(ICommunityRepository repo, IUnitOfWork uow) { }

// Cliente complejo
class ComplexHandler(
    ISessionRepository sessionRepo,
    ICommunityRepository communityRepo,
    IImageStorageService imageStorage,
    IUnitOfWork uow) { }
```

Cada cliente depende **solo de lo que necesita**.

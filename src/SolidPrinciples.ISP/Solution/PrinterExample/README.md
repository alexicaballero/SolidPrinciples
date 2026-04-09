# Solución: Interfaces Segregadas - Ejemplo de Impresora

## La Solución

En lugar de una interfaz gorda, creamos interfaces enfocadas:

- [IPrinter.cs](IPrinter.cs) - Solo impresión
- [IScanner.cs](IScanner.cs) - Solo escaneo
- [IFax.cs](IFax.cs) - Solo fax

## Implementaciones Correctas

### BasicInkjetPrinter

[BasicInkjetPrinter.cs](BasicInkjetPrinter.cs) implementa **solo IPrinter**:

- ✅ 1 método implementado (el que funciona)
- ✅ Sin NotSupportedException
- ✅ Signature honesta sobre sus capacidades

### OfficePrinter

[OfficePrinter.cs](OfficePrinter.cs) implementa **IPrinter, IScanner, IFax**:

- ✅ Composición de interfaces
- ✅ Cada método funciona realmente
- ✅ Flexible para agregar más interfaces

### DocumentService

[DocumentService.cs](DocumentService.cs) depende **solo de IPrinter**:

- ✅ Inyección de dependencia precisa
- ✅ Funciona con cualquier implementación de IPrinter
- ✅ Sin riesgo de excepciones

## Ventajas de ISP

| Aspecto          | Antes (Problema)             | Ahora (Solución)        |
| ---------------- | ---------------------------- | ----------------------- |
| **Métodos**      | 5 (solo 1 funciona)          | 1 (100% funcional)      |
| **Excepciones**  | 4 NotSupportedException      | 0                       |
| **Acoplamiento** | Alto (depende de 5 métodos)  | Bajo (depende de 1)     |
| **Mockeo tests** | 5 métodos                    | 1 método                |
| **Claridad**     | Confusa (¿cuáles funcionan?) | Clara (todos funcionan) |

## Patrón Clave

**Composición de interfaces**: Cada capacidad es su propia interfaz.
Las clases implementan solo las interfaces que realmente soportan.

```csharp
// Impresora simple
class BasicInkjetPrinter : IPrinter { }

// Multifunción
class OfficePrinter : IPrinter, IScanner, IFax { }

// Cliente enfocado
class DocumentService(IPrinter printer) { }
```

## Referencia

Ver [artículo ISP](https://calm-field-0d87ced10.6.azurestaticapps.net/post/es/solid-principles/interface-segregation) -
Sección: "Con ISP: Interfaces Enfocadas"

# Caso de Uso: Jerarquía de Figuras (Rectángulo/Cuadrado) 📐

> **Objetivo de aprendizaje**: Entender la clásica violación del Principio de Sustitución de Liskov (LSP) mediante el problema Rectángulo/Cuadrado — cuando las relaciones matemáticas de "es-un" no se traducen correctamente a la herencia en código.

## 📚 Artículo de referencia

Este caso de estudio implementa el ejemplo canónico de LSP extraído de:
**[Principio de Sustitución de Liskov - El Problema: Subtipos que Rompen Expectativas](https://calm-field-0d87ced10.6.azurestaticapps.net/post/es/solid-principles/liskov-substitution#el-problema-subtipos-que-rompen-expectativas)**

Este es **el ejemplo más conocido de violación de LSP** en ingeniería de software.

## 🎯 El escenario

Estamos construyendo una librería de geometría. Tenemos una clase `Rectangle` con propiedades independientes `Width` y `Height`.

**La tentación**: "Un cuadrado ES-UN rectángulo en matemáticas, por lo que `Square` debería heredar de `Rectangle`, ¿verdad?"

**La trampa**: Las relaciones matemáticas ≠ la herencia en código.

## Problema: Cuadrado heredando de Rectángulo

### La violación

```csharp
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }

    public int CalculateArea() => Width * Height;
}

// VIOLACIÓN: Square fuerza que Width y Height sean iguales
public class Square : Rectangle
{
    public override int Width
    {
        get => base.Width;
        set
        {
            base.Width = value;
            base.Height = value; // Efecto secundario oculto
        }
    }

    public override int Height
    {
        get => base.Height;
        set
        {
            base.Height = value;
            base.Width = value; // Efecto secundario oculto
        }
    }
}
```

### Por qué esto viola LSP

Contrato implícito de `Rectangle`:

- `Width` y `Height` son propiedades **independientes**
- Al asignar `Width` NO se debe ver afectado `Height`
- Al asignar `Height` NO se debe ver afectado `Width`

Violación de `Square`:

- Asignar `Width` TAMBIÉN asigna `Height` (postcondición oculta)
- Asignar `Height` TAMBIÉN asigna `Width` (postcondición oculta)
- Las propiedades quedan **acopladas**, rompiendo el contrato de independencia

### Escenario que rompe el contrato

```csharp
public void ResizeAndVerify(Rectangle rect)
{
    rect.Width = 5;
    rect.Height = 10;

    int expectedArea = 50; // 5 * 10
    int actualArea = rect.CalculateArea();

    // Se esperaba: 50
}

// Con Rectangle:
ResizeAndVerify(new Rectangle()); // Area = 50 ✓ Correcto

// Con Square (violación LSP):
ResizeAndVerify(new Square());    // Area = 100 ✗ INCORRECTO
                                  // ¡Width fue sobrescrito a 10!
```

Qué ocurrió:

1. `rect.Width = 5` → Square asigna Width=5 y Height=5
2. `rect.Height = 10` → Square asigna Width=10 y Height=10
3. El área es 10 × 10 = 100 (no los 50 esperados)

### Olores de código introducidos

1. Comprobación de tipo por todas partes

```csharp
public int CalculateAreaSafely(Rectangle rect)
{
    // OLOR: Necesito comprobar el tipo porque Square rompe el contrato
    if (rect is Square square)
    {
        return square.Width * square.Width; // Tratamiento especial
    }

    rect.Width = 5;
    rect.Height = 10;
    return rect.CalculateArea();
}
```

2. Programación defensiva

```csharp
public void ProcessRectangles(List<Rectangle> rectangles)
{
    foreach (var rect in rectangles)
    {
        // No se puede confiar en que Width y Height sean independientes
        // Hay que tratar Square como un caso especial
    }
}
```

## ✅ Solución: Tipos separados con interfaz común

### El diseño correcto

```
IShape (interfaz)
  ├─ Rectangle (Width y Height independientes)
  └─ Square (solo Side)
```

No hay herencia entre `Rectangle` y `Square`.

### Implementación

```csharp
public interface IShape
{
    int CalculateArea();
}

public sealed class Rectangle : IShape
{
    public int Width { get; }
    public int Height { get; }

    public Rectangle(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public int CalculateArea() => Width * Height;
}

public sealed class Square : IShape
{
    public int Side { get; } // Solo una dimensión

    public Square(int side)
    {
        Side = side;
    }

    public int CalculateArea() => Side * Side;
}
```

### Por qué esto respeta LSP

✅ **Sin efectos secundarios ocultos**: cada figura controla sus propiedades de forma independiente

✅ **Contratos claros**:

- `Rectangle` tiene `Width` y `Height` (independientes)
- `Square` tiene `Side` (una sola dimensión)

✅ **Sustitución segura**: cualquier `IShape` puede usarse de forma intercambiable

✅ **Sin comprobaciones de tipo**: el código polimórfico funciona correctamente

✅ **Inmutabilidad**: propiedades de solo lectura para evitar usos indebidos

### Uso de la solución

```csharp
public int CalculateArea(IShape shape)
{
    // Funciona con Rectangle, Square o cualquier IShape futuro
    return shape.CalculateArea();
}

// ✓ Ambos funcionan correctamente
CalculateArea(new Rectangle(5, 10)); // 50
CalculateArea(new Square(7));        // 49
```

## 🔑 Ideas clave

### Relación matemática vs herencia en código

| Matemática                     | Herencia en código           | Realidad       |
| ------------------------------ | ---------------------------- | -------------- |
| "Un cuadrado ES-UN rectángulo" | `Square : Rectangle`         | Rompe LSP      |
| "Ambos son figuras"            | `Rectangle, Square : IShape` | ✅ Respeta LSP |

Lección: en POO, "ES-UN" significa "es sustituible por", no solo una relación matemática.

### El principio

La herencia modela la sustituibilidad del comportamiento, no relaciones puramente conceptuales.

Un cuadrado puede ser un rectángulo en matemáticas, pero en código:

- **Comportamiento de Rectangle**: ancho y alto independientes
- **Comportamiento de Square**: una sola dimensión uniforme

Estos comportamientos son incompatibles → no conviene usar herencia.

### Cuándo usar herencia vs interfaz

| Usar herencia cuando...                  | Usar interfaz cuando...                         |
| ---------------------------------------- | ----------------------------------------------- |
| El subtipo EXTIENDE el comportamiento    | Los tipos comparten contrato, no implementación |
| No se cambia el comportamiento existente | Los comportamientos difieren significativamente |
| El subtipo es realmente SUSTITUIBLE      | Se desea polimorfismo sin acoplamiento          |

## 🎓 Valor educativo

Este ejemplo enseña:

1. Las postcondiciones ocultas rompen LSP (asignar Width cambia Height)
2. El acoplamiento de propiedades viola contratos de independencia
3. La inmutabilidad previene usos indebidos (propiedades de solo lectura)
4. Preferir composición sobre herencia cuando los comportamientos varían
5. El razonamiento del mundo real a veces contradice modelos matemáticos

## 🧪 Cómo verificar

Ejecuta las pruebas:

```bash
dotnet test --filter "FullyQualifiedName~ShapeHierarchy"
```

Las pruebas demuestran:

- ✗ **Problema**: `Square` rompe el código que espera un `Rectangle`
- ✓ **Solución**: `IShape` permite polimorfismo seguro

## 💡 Aplicaciones en el mundo real

Este patrón aparece en situaciones como:

- **Componentes UI**: Un botón NO es una etiqueta (aunque muestre texto)
- **Colecciones**: `ImmutableList` NO es un `List` (no se puede agregar/quitar)
- **Autenticación**: `Guest` NO es un `User` (capacidades distintas)
- **Tipos de archivo**: `CompressedFile` NO es un `File` (operaciones distintas)

Regla general: si el subtipo debe **deshabilitar, restringir o agregar efectos secundarios** al comportamiento heredado, usa composición en lugar de herencia.

## 📖 Lecturas adicionales

- [Wikipedia: Circle-Ellipse Problem](https://en.wikipedia.org/wiki/Circle%E2%80%93ellipse_problem) (similar al problema Rectángulo/Cuadrado)
- Artículo de referencia: [Liskov Substitution Principle](https://calm-field-0d87ced10.6.azurestaticapps.net/post/es/solid-principles/liskov-substitution)
- Artículo original de Barbara Liskov (1987): "Data Abstraction and Hierarchy"

# Principio de Sustitución de Liskov (LSP)

> **\"Los objetos de una superclase deben ser reemplazables por objetos de sus subclases sin afectar la corrección del programa.\"**  
> \u2014 Barbara Liskov\n\nEl Principio de Sustitución de Liskov establece que los subtipos deben ser sustituibles por sus tipos base. Si tienes código que funciona con una clase base, deber\u00eda funcionar correctamente con cualquier clase derivada sin modificación, verificació de tipo o fallas inesperadas.

LSP asegura que las jerarquías de herencia sean **lógicamente sólidas** y **coherentes en el comportamiento**.

## Por qué importa

En proyectos del mundo real, violar LSP conduce a:

- **Excepciones en tiempo de ejecución** de clases derivadas que lanzan `NotImplementedException`
- **Olor de código de verificación de tipos**: `if (x is SubType)` antes de llamar métodos
- **Fallas silenciosas**: Clases derivadas que ignoran operaciones, rompen contratos o devuelven resultados falsos
- **Polimorfismo roto**: No puedes escribir código genérico porque cada subtipo se comporta de manera diferente

Cuando LSP se viola, la promesa de la programación orientada a objetos — reutilización segura a través de herencia y polimorfismo — se rompe.

## Objetivos de aprendizaje

Después de estudiar este módulo, deberías poder:

- Identificar violaciones de LSP: `NotImplementedException`, precondiciones fortalecidas, postcondiciones debilitadas
- Reconocer el olor de código: verificación de tipo (`is`, `as`, `typeof`) antes de llamadas a métodos
- Aplicar **composición sobre herencia** cuando el comportamiento varía significativamente por subtipo
- Usar el **patrón Estrategia** para variar el comportamiento sin violar contratos
- Diseñar clases donde cualquier instancia sea **seguramente substituible** para el tipo base
- Escribir código polimórfico que funcione correctamente con todos los subtipos

## Reglas de contrato (Diseño por contrato)

LSP se formaliza a través de tres reglas:

| Rule                                     | Meaning                                              | Violation Example                                                  |
| ---------------------------------------- | ---------------------------------------------------- | ------------------------------------------------------------------ |
| **Preconditions cannot be strengthened** | Subtype cannot require more than base type           | Base: any member can register; Subtype: only verified members      |
| **Postconditions cannot be weakened**    | Subtype must guarantee at least what base guarantees | Base: cancel always succeeds; Subtype: cancel fails with attendees |
| **Invariants must be preserved**         | Subtype must maintain all constraints of base type   | Base: capacity > 0; Subtype: allows capacity = 0                   |

## 📁 Casos de Uso Incluidos

Este módulo contiene **cuatro casos de uso** que demuestran violaciones y correcciones de LSP:

### 📁 Caso de Uso 1: Jerarquía de Entidades ⭐ **FUNDAMENTAL**

**El ejemplo más simple y fundamental de LSP** - basado en el caso de estudio de [Gathering](https://github.com/alexicaballero/gathering/tree/main/backend).

Demuestra cómo construir jerarquías de herencia correctas: `Entity` → `AuditableEntity` → `Session`/`Community`.

| Carpeta                                                  | Descripción                                                  |
| -------------------------------------------------------- | ------------------------------------------------------------ |
| [`Problem/EntityHierarchy/`](Problem/EntityHierarchy/)   | Precondiciones fortalecidas e invariantes rotos              |
| [`Solution/EntityHierarchy/`](Solution/EntityHierarchy/) | Jerarquía correcta que extiende sin modificar comportamiento |

**Ver detalles**: [Problem/EntityHierarchy/README.md](Problem/EntityHierarchy/README.md)

**📖 Artículo de referencia**: [Caso de Estudio: Jerarquía de Entidades en Gathering](https://calm-field-0d87ced10.6.azurestaticapps.net/post/es/solid-principles/liskov-substitution#caso-de-estudio-jerarqu%C3%ADa-de-entidades-en-gathering)

---

### 📁 Caso de Uso 2: Rectángulo/Cuadrado 📐 **CLÁSICO**

**El ejemplo más famoso de violación LSP** - demuestra que las relaciones matemáticas "IS-A" no se traducen a herencia en código.

El problema clásico: `Square` hereda de `Rectangle` pero rompe el contrato de independencia de propiedades.

| Carpeta                                                | Descripción                                              |
| ------------------------------------------------------ | -------------------------------------------------------- |
| [`Problem/ShapeHierarchy/`](Problem/ShapeHierarchy/)   | Square hereda de Rectangle (efectos secundarios ocultos) |
| [`Solution/ShapeHierarchy/`](Solution/ShapeHierarchy/) | IShape con Rectangle y Square como tipos independientes  |

**Ver detalles**: [Problem/ShapeHierarchy/README.md](Problem/ShapeHierarchy/README.md)

**📖 Artículo de referencia**: [El Problema: Subtipos que Rompen Expectativas](https://calm-field-0d87ced10.6.azurestaticapps.net/post/es/solid-principles/liskov-substitution#el-problema-subtipos-que-rompen-expectativas)

---

### 📁 Caso de Uso 3: Jerarquía de Miembros

Demuestra cómo subtipos que lanzan `NotImplementedException` rompen la substitutibilidad.

| Carpeta                                                  | Descripción                                        |
| -------------------------------------------------------- | -------------------------------------------------- |
| [`Problem/MemberHierarchy/`](Problem/MemberHierarchy/)   | Jerarquía Member → GuestMember (lanza excepciones) |
| [`Solution/MemberHierarchy/`](Solution/MemberHierarchy/) | Diseño basado en capacidades con composición       |

**Ver detalles**: [Problem/MemberHierarchy/README.md](Problem/MemberHierarchy/README.md) • [Solution/MemberHierarchy/README.md](Solution/MemberHierarchy/README.md)

---

### 📁 Caso de Uso 4: Jerarquía de Sesiones

Demuestra precondiciones fortalecidas y postcondiciones debilitadas en subtipos.

| Carpeta                                                    | Descripción                                               |
| ---------------------------------------------------------- | --------------------------------------------------------- |
| [`Problem/SessionHierarchy/`](Problem/SessionHierarchy/)   | Session → PrivateSession/PublicSession (rompen contratos) |
| [`Solution/SessionHierarchy/`](Solution/SessionHierarchy/) | Diseño basado en políticas (Strategy pattern)             |

**Ver detalles**: [Problem/SessionHierarchy/README.md](Problem/SessionHierarchy/README.md) • [Solution/SessionHierarchy/README.md](Solution/SessionHierarchy/README.md)

## Article Reference

📖 [Liskov Substitution Principle](https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/liskov-substitution)

## How to Run Tests

```bash
dotnet test --filter "FullyQualifiedName~LSP"
```

## Quick LSP Checklist

**Your code follows LSP if:**

- Subtypes can be used anywhere the base type is expected
- No type-checking (`is`, `as`) needed before calling methods
- No `NotImplementedException` in inherited methods
- Derived classes honor the same contracts as the base class
- Polymorphic code works correctly with all subtypes

**Your code violates LSP if:**

- You check types before calling methods: `if (x is GuestMember)`
- Derived classes throw exceptions for inherited methods
- Operations silently do nothing in some subtypes
- Subtypes require more (strengthened preconditions) than base type
- Subtypes guarantee less (weakened postconditions) than base type
- Comments say "don't use this method in SubTypeX"

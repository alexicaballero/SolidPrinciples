---
description: "Use when creating or editing C# source files. Enforces code style conventions for the SolidPrinciples project: sealed classes, file-scoped namespaces, Result pattern, nullable reference types, XML doc comments."
applyTo: "**/*.cs"
---

# C# Code Conventions

## Required Patterns

- **Idioma español**: Toda la documentación XML, comentarios y README deben estar en español
- **File-scoped namespaces**: `namespace SolidPrinciples.SRP.Problem;` (no braces)
- **Sealed by default**: All classes are `sealed` unless designed for inheritance
- **Nullable reference types**: Always enabled, handle nullability explicitly
- **Primary constructors**: Use where they simplify code (e.g., DI injection)
- **Result pattern**: Return `Result<T>` or `Result` for operations that can fail — never throw exceptions for flow control

## Entity Pattern

```csharp
public sealed class EntityName : AuditableEntity
{
    public Guid Id { get; private set; }
    private EntityName() { }
    public static Result<EntityName> Create(/* params */) { /* factory */ }
}
```

## Test Pattern

```csharp
public sealed class EntityNameTests
{
    [Fact]
    public void MethodName_Condition_ExpectedResult() { /* Arrange/Act/Assert */ }
}
```

## Educational Annotations

- **Idioma**: Todos los comentarios y documentación XML en español
- Problem files: Use `// VIOLACIÓN:` comments and XML doc summary starting with `PROBLEMA:`
- Solution files: Use `// CORRECTO:` comments and XML doc summary starting with `SOLUCIÓN:`

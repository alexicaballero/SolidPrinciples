# SolidPrinciples - Project Guidelines

## Purpose

Educational project for teaching SOLID principles through practical ASP.NET Core examples.
Each principle is demonstrated with a dedicated class library project containing:

- **Problem examples** (code that violates the principle)
- **Solution implementations** (correctly applied principle)
- **Unit tests** validating both approaches

## Architecture

This project follows **Clean Architecture** patterns inspired by the [Gathering](https://github.com/alexicaballero/gathering/tree/main/backend) reference project.

### Solution Structure

```
SolidPrinciples.slnx
├── src/
│   ├── SolidPrinciples.Common/          # Shared kernel: Result, Error, base classes
│   ├── SolidPrinciples.SRP/             # Single Responsibility Principle
│   ├── SolidPrinciples.OCP/             # Open/Closed Principle
│   ├── SolidPrinciples.LSP/             # Liskov Substitution Principle
│   ├── SolidPrinciples.ISP/             # Interface Segregation Principle
│   └── SolidPrinciples.DIP/             # Dependency Inversion Principle
├── tests/
│   ├── SolidPrinciples.SRP.Tests/
│   ├── SolidPrinciples.OCP.Tests/
│   ├── SolidPrinciples.LSP.Tests/
│   ├── SolidPrinciples.ISP.Tests/
│   └── SolidPrinciples.DIP.Tests/
└── .github/
    ├── copilot-instructions.md
    ├── instructions/
    ├── agents/
    ├── prompts/
    └── skills/
```

### Per-Principle Project Structure

Each principle project (`SolidPrinciples.{Principle}/`) follows this layout:

```
SolidPrinciples.SRP/
├── Problem/             # Code that violates the principle (before)
│   └── README.md        # Explanation of what's wrong
├── Solution/            # Correct implementation (after)
│   └── README.md        # Explanation of the fix
└── README.md            # Principle overview, learning objectives
```

## Code Style

- C# 12, .NET 9, nullable reference types enabled
- File-scoped namespaces
- Primary constructors where appropriate
- `sealed` classes by default unless designed for inheritance
- Interfaces prefixed with `I` (e.g., `ISessionRepository`)
- Use the Result pattern for operations that can fail (no exceptions for flow control)
- **XML doc comments on public APIs in Spanish**
- **All code comments and documentation in Spanish**

## Domain Context

Examples use a **Community of Practice management system** (Gathering) as the domain:

- **Community**: A group with name, description, and admin
- **Session**: A scheduled talk with title, speaker, date, status
- **Member**: A person who belongs to communities
- Domain entities use factory methods (`Create`) with Result return types
- Domain events for side effects (`SessionCreatedDomainEvent`)

## Testing

- xUnit as test framework
- FluentAssertions for assertions
- NSubstitute for mocking
- Test naming: `MethodName_Condition_ExpectedResult`
- Each principle has a test project mirroring its source structure

## Build & Test Commands

```bash
dotnet build
dotnet test
dotnet test --filter "FullyQualifiedName~SRP"   # Test single principle
```

## Reference Content

The educational articles for this project are published at:
https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles

The reference backend codebase (Gathering) is at:
https://github.com/alexicaballero/gathering/tree/main/backend

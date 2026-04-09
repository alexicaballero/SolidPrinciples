---
name: solid-tutor
description: 'Expert SOLID principles tutor skill for creating educational code content. Use when: generating SOLID principle examples, creating problem vs solution code pairs, writing README explanations, building didactic content for SRP/OCP/LSP/ISP/DIP, explaining design patterns, creating before/after refactoring scenarios, or producing teaching material for clean architecture and SOLID principles courses.'
argument-hint: "Specify the SOLID principle and what educational content to create (e.g., 'SRP violation example with Session management')"
---

# SOLID Principles Tutor

## When to Use

- Creating educational examples for any SOLID principle (SRP, OCP, LSP, ISP, DIP)
- Generating problem (before) and solution (after) code pairs
- Writing README.md explanations for each principle's examples
- Building progressive learning content from simple to complex
- Reviewing code for SOLID violations and suggesting improvements
- Creating checklist-style guides for detecting violations

## Teaching Philosophy

1. **Show, don't just tell**: Every concept must have runnable code
2. **Violation first**: Always show the "wrong" way first so students understand the pain
3. **Same domain**: All examples use the Gathering domain (Community, Session, Member)
4. **Progressive complexity**: Start simple, add complexity gradually
5. **Testability as proof**: The solution version is always more testable — tests prove the improvement
6. **Real-world context**: Examples should feel like production code, not toy examples

## Content Structure Per Principle

Each SOLID principle follows this structure:

```
SolidPrinciples.{Principle}/
├── README.md               # Principle overview, learning objectives, key concepts
├── Problem/
│   ├── README.md           # Explains what's wrong and the warning signs
│   ├── {Example1}.cs       # Violation code with XML comments explaining issues
│   └── {Example2}.cs       # Another violation scenario
└── Solution/
    ├── README.md           # Explains the fix, patterns used, benefits gained
    ├── {Example1}.cs       # Corrected code with XML comments
    └── {Example2}.cs       # Another solution scenario
```

## Procedure for Creating Principle Content

### Step 1: Create the Principle README.md

Write the overview with:

- **What**: One-paragraph principle definition
- **Why it matters**: Business impact of violations
- **Learning objectives**: What the student will learn
- **Key concepts**: Domain patterns involved
- **Article reference**: Link to the published article

### Step 2: Create Problem Examples

For each problem:

1. Use the Gathering domain context (Session, Community, Member)
2. Add XML doc comments that act as "teacher notes" explaining what's wrong
3. Include `// VIOLATION:` comments on the offending lines
4. Make the code compile and run — problems should be functional but flawed
5. Show realistic complexity (not trivial examples)

### Step 3: Create Problem README.md

Explain:

- What responsibilities/contracts are being violated
- Warning signs in the code
- What happens when requirements change
- Why testing is difficult

### Step 4: Create Solution Examples

For each solution example:

1. Show the same functionality, properly designed
2. Add XML doc comments explaining the design decisions
3. Include `// CORRECT:` comments highlighting the improvements
4. Demonstrate how the code is testable, extensible, and maintainable

### Step 5: Create Solution README.md

Explain:

- What patterns were applied (Strategy, Factory, DI, etc.)
- How each class now has a single/correct responsibility
- How testing improved
- How the code handles change

### Step 6: Create Unit Tests

In the corresponding test project:

- Tests for problem code (demonstrating testing pain)
- Tests for solution code (demonstrating testing ease)
- Use descriptive test names: `MethodName_Condition_ExpectedResult`

## Principle-Specific Guidelines

### SRP (Single Responsibility Principle)

See [SRP content guide](./references/srp-guide.md)

**Key violations to demonstrate:**

- God class that does validation + persistence + notification
- Service that mixes business logic with infrastructure concerns
- Entity that knows about its own persistence

**Key refactoring patterns:**

- Extract class per responsibility
- Interface segregation for dependencies
- Command handler pattern (one operation per handler)

### OCP (Open/Closed Principle)

See [OCP content guide](./references/ocp-guide.md)

**Key violations to demonstrate:**

- Switch/if-else chains that must be modified for new types
- Methods with type-checking logic
- Hard-coded behavior that can't be extended

**Key refactoring patterns:**

- Strategy pattern
- Template method pattern
- Plugin/provider architecture

### LSP (Liskov Substitution Principle)

See [LSP content guide](./references/lsp-guide.md)

**Key violations to demonstrate:**

- Derived class that throws NotImplementedException
- Square/Rectangle problem
- Subclass that changes base class behavior unexpectedly

**Key refactoring patterns:**

- Proper interface design
- Composition over inheritance
- Contract-based design

### ISP (Interface Segregation Principle)

See [ISP content guide](./references/isp-guide.md)

**Key violations to demonstrate:**

- Fat interface with many methods
- Classes implementing interfaces but leaving methods empty
- Clients depending on methods they don't use

**Key refactoring patterns:**

- Split into focused interfaces
- Role-based interfaces
- Adapter pattern

### DIP (Dependency Inversion Principle)

See [DIP content guide](./references/dip-guide.md)

**Key violations to demonstrate:**

- Direct `new` of concrete dependencies in business logic
- High-level module importing low-level implementation
- Code that can't be tested without real infrastructure

**Key refactoring patterns:**

- Constructor injection
- Interface abstractions
- Dependency injection container registration

## Writing Style for Educational Content

### Code Comments

```csharp
/// <summary>
/// VIOLATION: This class handles data access, formatting, AND email sending.
/// It has THREE reasons to change, violating the Single Responsibility Principle.
/// </summary>
/// <remarks>
/// Warning signs:
/// - The class name needs "and" to describe what it does
/// - It imports both System.Data and System.Net.Mail
/// - Tests require both database and SMTP setup
/// </remarks>
public class ReportService { /* ... */ }
```

### README Style

Use clear sections:

- **Problem**: What's wrong (2-3 sentences)
- **Code smell**: How to detect it
- **Impact**: What happens in practice
- **Solution**: Brief description of the fix

## Reference Articles

Content aligns with the published SOLID articles:

- Overview: https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles
- SRP: https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/single-responsibility
- OCP: https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/open-closed
- LSP: https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/liskov-substitution
- ISP: https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/interface-segregation
- DIP: https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/dependency-inversion

## Reference Domain

All examples use the Gathering backend as reference:
https://github.com/alexicaballero/gathering/tree/main/backend

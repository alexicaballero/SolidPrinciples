---
description: 'Use when creating or editing README files inside principle project folders. Enforces educational content structure with problem/solution explanations, learning objectives, and article references.'
applyTo:
  [
    'src/SolidPrinciples.*/README.md',
    'src/SolidPrinciples.*/Problem/README.md',
    'src/SolidPrinciples.*/Solution/README.md',
  ]
---

# Educational README Structure

## Principle Overview README (src/SolidPrinciples.{Principle}/README.md)

Required sections:

1. **Principle name and definition** (one paragraph)
2. **Why it matters** (business impact)
3. **Learning objectives** (bullet list of what students will learn)
4. **Examples included** (table of problem and solution files)
5. **Article reference** (link to published article)
6. **How to run tests**: `dotnet test --filter "FullyQualifiedName~{Principle}"`

## Problem README (Problem/README.md)

Required sections:

1. **What's wrong** (2-3 sentence summary)
2. **Warning signs** (bullet list of code smells)
3. **Impact** (what happens when requirements change)
4. **Files** (table listing each problem file with description)

## Solution README (Solution/README.md)

Required sections:

1. **What was fixed** (2-3 sentence summary)
2. **Patterns applied** (Strategy, Factory, DI, etc.)
3. **Benefits gained** (testability, extensibility, etc.)
4. **Files** (table listing each solution file with description)

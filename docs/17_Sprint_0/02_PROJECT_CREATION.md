# PROJECT CREATION SPECIFICATION

> Version: 1.0
> Module: Sprint 0

---

# 1. Purpose

This document defines the complete project structure of the AikoOS solution.

It specifies every project that belongs to the solution, the purpose of each project, dependency rules, project type, and creation order.

This specification is the authoritative source for the physical solution architecture.

---

# 2. Objectives

The project structure should:

* Reflect the software architecture.
* Minimize coupling.
* Maximize cohesion.
* Support future expansion.
* Enable independent testing.
* Keep responsibilities isolated.

---

# 3. Target Framework

All projects should target the same framework unless explicitly documented otherwise.

| Setting                  | Value                    |
| ------------------------ | ------------------------ |
| Framework                | .NET 10 (Current Stable) |
| Nullable                 | Enabled                  |
| Implicit Usings          | Enabled                  |
| LangVersion              | Latest                   |
| Treat Warnings As Errors | Enabled (Release)        |

Framework versions should remain consistent across the solution.

---

# 4. Solution Projects

## Production Projects

| Project               | Type          | Responsibility                     |
| --------------------- | ------------- | ---------------------------------- |
| AikoOS.App            | WPF           | Application Entry Point            |
| AikoOS.Core           | Class Library | Shared abstractions and primitives |
| AikoOS.Runtime        | Class Library | Runtime orchestration              |
| AikoOS.Infrastructure | Class Library | Infrastructure implementations     |
| AikoOS.Configuration  | Class Library | Configuration loading              |
| AikoOS.Logging        | Class Library | Logging implementation             |
| AikoOS.Database       | Class Library | Persistence layer                  |
| AikoOS.AI             | Class Library | AI runtime and providers           |
| AikoOS.Memory         | Class Library | Memory engine                      |

Future modules:

* AikoOS.Voice
* AikoOS.Vision
* AikoOS.Automation
* AikoOS.Plugins
* AikoOS.Telemetry

These projects should only be added when active development begins.

---

# 5. Test Projects

Each major module should have its own test project.

| Project                  |
| ------------------------ |
| AikoOS.Core.Tests        |
| AikoOS.Runtime.Tests     |
| AikoOS.AI.Tests          |
| AikoOS.Memory.Tests      |
| AikoOS.Integration.Tests |

Unit tests and integration tests should remain separate.

---

# 6. Dependency Matrix

Allowed dependency flow:

```text
                App
                 │
         ┌───────┴────────┐
         │                │
     Runtime      Infrastructure
         │                │
         ├──────┬─────────┤
         ▼      ▼         ▼
      AI     Memory   Database
         │      │
         └──┬───┘
            ▼
          Core
```

Rules:

* Dependencies always point downward.
* Core depends on nothing.
* Runtime never depends on App.
* Infrastructure never references UI.
* Test projects may reference production projects only.

Circular references are prohibited.

---

# 7. Assembly Naming

Assembly names should match project names exactly.

Examples:

```text
AikoOS.Core
AikoOS.Runtime
AikoOS.Memory
AikoOS.AI
```

Assembly names should remain stable throughout the project lifecycle.

---

# 8. Root Namespace

Each project should use:

```text
AikoOS.<Module>
```

Examples:

```text
AikoOS.Core

AikoOS.Runtime

AikoOS.Memory

AikoOS.AI
```

Namespaces should mirror folder structure whenever practical.

---

# 9. Project Creation Order

Projects should be created in the following sequence:

1. AikoOS.App
2. AikoOS.Core
3. AikoOS.Runtime
4. AikoOS.Infrastructure
5. AikoOS.Configuration
6. AikoOS.Logging
7. AikoOS.Database
8. AikoOS.AI
9. AikoOS.Memory
10. Test Projects

Creating projects in this order minimizes dependency adjustments.

---

# 10. Project Templates

| Project    | Template           |
| ---------- | ------------------ |
| AikoOS.App | WPF Application    |
| Others     | Class Library      |
| Tests      | xUnit Test Project |

No console applications should exist inside the production solution.

---

# 11. Project Standards

Every project should:

* Enable nullable reference types.
* Enable implicit usings.
* Contain an AssemblyInfo generated automatically.
* Use SDK-style project files.
* Follow identical formatting conventions.

Consistency is preferred over project-specific customization.

---

# 12. Validation Checklist

Before Sprint 1:

* ☐ All projects created.
* ☐ Solution builds.
* ☐ Project references verified.
* ☐ No circular dependencies.
* ☐ Tests compile.
* ☐ Root namespaces verified.

---

# 13. Why This Structure?

### Why?

A modular project structure keeps responsibilities isolated, simplifies maintenance, and allows individual modules to evolve independently.

### Why not?

Combining all functionality into a small number of large projects quickly increases coupling, reduces clarity, and complicates testing.

### Trade-offs

* More projects to manage.
* Cleaner architecture.
* Easier scalability.
* Better testing boundaries.

---

# 14. Future Expansion

The solution may later include additional projects such as:

* AikoOS.SDK
* AikoOS.Cloud
* AikoOS.Mobile
* AikoOS.Live2D
* AikoOS.Telemetry

Each new project should follow the dependency and naming rules defined in this specification.

---

# 15. Summary

The Project Creation Specification defines the complete physical structure of the AikoOS solution.

By standardizing project types, dependencies, naming conventions, and creation order, the solution remains organized, scalable, and aligned with the overall software architecture.

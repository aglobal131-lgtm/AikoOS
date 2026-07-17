# MODULE DEPENDENCY

> Version: 1.0
> Status: Draft

---

# 1. Purpose

This document defines the dependency rules between modules in AikoOS.

Its goals are to:

* Prevent circular dependencies.
* Keep modules independent.
* Make future maintenance easier.
* Support replacing modules without affecting the whole system.

---

# 2. Dependency Principles

Every module should:

* Have a single responsibility.
* Expose only public interfaces.
* Hide implementation details.
* Never access another module's internal classes.

Allowed:

```text
Conversation
      │
      ▼
IMemoryService
```

Not allowed:

```text
Conversation
      │
      ▼
MemoryRepository
```

---

# 3. Dependency Direction

All dependencies move inward.

```text
Client
    │
    ▼
Application
    │
    ▼
Domain
    │
    ▼
Infrastructure
```

Infrastructure must never call Domain directly.

---

# 4. Core Modules

```text
Conversation
AI Gateway
Memory
Emotion
Personality
Plugins
Automation
Scheduler
Notification
Permission
Configuration
```

Each module owns its own business rules.

---

# 5. Allowed Dependencies

## Conversation

May depend on:

* AI Gateway
* Memory
* Emotion
* Permission

Must not depend on:

* Database
* Redis
* Plugin implementation

---

## AI Gateway

May depend on:

* Provider Adapters
* Configuration

Must not depend on:

* Conversation
* Emotion
* Memory rules

---

## Memory

May depend on:

* Vector Search
* Database

Must not depend on:

* Voice
* Animation
* UI

---

## Emotion

May depend on:

* Conversation
* Memory

Must not depend on:

* AI Provider
* Database implementation

---

## Plugins

May depend on:

* Plugin SDK
* Permission

Must not depend on:

* Internal repositories
* Internal services

---

# 6. Client Dependencies

```text
View
    │
ViewModel
    │
Client Service
    │
Backend API
```

Views never communicate directly with the backend.

---

# 7. Backend Dependencies

```text
Controller
      │
Application
      │
Domain
      │
Infrastructure
```

Controllers must remain thin.

---

# 8. Forbidden Dependencies

The following are prohibited:

* View → Database
* ViewModel → Entity Framework
* Domain → ASP.NET Core
* Domain → Redis
* Domain → HTTP Client
* Plugin → Internal Database
* Plugin → Internal Memory Engine
* Client → AI Provider SDK

---

# 9. Cross-Module Communication

Modules communicate using:

* Interfaces
* Domain Events
* Application Events
* Integration Events

Never through direct database access.

---

# 10. Dependency Injection

Every module exposes interfaces.

Example:

```csharp
public interface IMemoryService
{
    Task<IReadOnlyList<MemoryResult>> SearchAsync(
        MemoryQuery query,
        CancellationToken cancellationToken = default);
}
```

Consumers reference the interface only.

---

# 11. Future Modules

New modules should follow the same rules:

* Own their data.
* Expose interfaces.
* Avoid leaking implementation.
* Avoid circular references.

---

# 12. Summary

The dependency graph of AikoOS is intentionally simple:

```text
Client
    │
Application
    │
Domain
    │
Infrastructure
```

Modules communicate through contracts rather than implementation.

This keeps the project modular, testable, and scalable while allowing individual systems to evolve independently.

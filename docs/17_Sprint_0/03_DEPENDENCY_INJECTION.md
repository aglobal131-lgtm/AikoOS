# DEPENDENCY INJECTION SPECIFICATION

> Version: 1.0
> Module: Sprint 0

---

# 1. Purpose

This document defines the Dependency Injection (DI) architecture for AikoOS.

Dependency Injection is the only approved mechanism for constructing application services. It ensures loose coupling, improves testability, and allows implementations to evolve independently of consumers.

No application module should manually construct another service unless explicitly documented.

---

# 2. Objectives

The DI architecture aims to:

* Eliminate manual service construction.
* Centralize object composition.
* Improve testability.
* Enforce dependency inversion.
* Simplify module replacement.
* Support future expansion.

---

# 3. Design Principles

Dependency Injection follows these principles:

* Constructor Injection First.
* Composition Root Only.
* Program to Interfaces.
* No Service Locator.
* No Hidden Dependencies.
* Explicit Registration.

Every dependency should be visible in the constructor signature.

---

# 4. Composition Root

The application shall contain exactly one Composition Root.

Recommended location:

```text
AikoOS.App
└── Bootstrap/
    └── ServiceCollectionExtensions.cs
```

Responsibilities:

* Register all services.
* Register configuration.
* Register logging.
* Register infrastructure.
* Build the ServiceProvider.
* Resolve only the application root.

No other project should build its own `ServiceProvider`.

---

# 5. Constructor Injection

Constructor Injection is the preferred injection method.

Approved:

```text
Class
 └── Constructor
      ├── ILogger
      ├── IConfiguration
      └── IAIProvider
```

Not approved:

* Property Injection.
* Method Injection (except framework requirements).
* Static service access.

Dependencies should never be optional unless explicitly justified.

---

# 6. Service Lifetime Matrix

The following lifetimes are recommended.

| Service Type        | Lifetime  |
| ------------------- | --------- |
| Configuration       | Singleton |
| Logging             | Singleton |
| AI Provider         | Singleton |
| Memory Repository   | Singleton |
| Database Factory    | Singleton |
| Runtime Coordinator | Singleton |
| Stateless Utility   | Singleton |
| Factory Services    | Transient |
| ViewModels          | Transient |

Lifetime changes should be documented before implementation.

---

# 7. Module Registration Pattern

Every module should expose a registration method.

Example:

```text
services.AddCore();

services.AddInfrastructure();

services.AddConfiguration();

services.AddLogging();

services.AddDatabase();

services.AddAI();

services.AddMemory();
```

Each module is responsible for registering only its own services.

---

# 8. Dependency Rules

The following rules apply:

* Services depend on abstractions.
* Interfaces belong to the owning module.
* Infrastructure implements interfaces.
* UI depends only on abstractions.
* Core has no external dependencies.

Dependency direction must always follow the architecture.

---

# 9. Forbidden Practices

The following are prohibited:

* Calling `new` for registered services.
* Static singleton implementations.
* Global service locators.
* Calling `BuildServiceProvider()` outside the Composition Root.
* Circular service dependencies.
* Hidden runtime resolution.

Violations should be treated as architectural defects.

---

# 10. Registration Order

Recommended registration order:

```text
Configuration
      │
      ▼
Logging
      │
      ▼
Core
      │
      ▼
Infrastructure
      │
      ▼
Database
      │
      ▼
AI
      │
      ▼
Memory
      │
      ▼
Runtime
      │
      ▼
Application
```

This order minimizes dependency resolution issues.

---

# 11. Validation Rules

Before implementation proceeds:

* ☐ Every service has an interface where appropriate.
* ☐ Constructors contain all required dependencies.
* ☐ No manual service construction.
* ☐ No circular dependencies.
* ☐ Service lifetimes verified.
* ☐ Registration methods organized by module.

---

# 12. Why This Architecture?

### Why?

A centralized Dependency Injection architecture provides clear dependency management, improves maintainability, and simplifies testing.

### Why not?

Allowing arbitrary object creation throughout the application increases coupling, hides dependencies, and makes future refactoring significantly more difficult.

### Trade-offs

* Slightly more setup.
* Better modularity.
* Improved testability.
* Easier long-term maintenance.

---

# 13. Future Expansion

Future enhancements may include:

* Automatic assembly scanning.
* Conditional registrations.
* Plugin-based service registration.
* Environment-specific registrations.
* Decorator pattern support.

These extensions should integrate without changing the core DI principles.

---

# 14. Governance

All new services introduced into AikoOS must comply with this specification.

Architectural reviews should verify:

* Lifetime selection.
* Registration location.
* Dependency direction.
* Constructor clarity.
* Module ownership.

Non-compliant registrations should be corrected before merge.

---

# 15. Summary

The Dependency Injection Specification establishes a single, consistent approach to service composition throughout AikoOS.

By enforcing constructor injection, a single Composition Root, explicit registrations, and clear dependency rules, the project remains modular, testable, and aligned with its architectural goals.

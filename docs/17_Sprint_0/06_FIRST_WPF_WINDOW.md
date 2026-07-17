# FIRST WPF WINDOW SPECIFICATION

> Version: 1.0
> Module: Sprint 0

---

# 1. Purpose

This document defines the initial desktop user interface architecture of AikoOS.

The first WPF window is not merely a startup screen. It establishes the application's MVVM foundation, dependency injection integration, navigation strategy, and lifecycle.

Every future screen should follow the architectural rules introduced here.

---

# 2. Objectives

The first WPF application should:

* Start successfully.
* Resolve dependencies through Dependency Injection.
* Display the Main Window.
* Bind to a ViewModel.
* Demonstrate MVVM architecture.
* Be ready for future navigation.

Sprint 0 is complete only when these objectives are satisfied.

---

# 3. UI Architecture

AikoOS adopts the Model–View–ViewModel (MVVM) pattern.

```text
User
 │
 ▼
View (XAML)
 │
 ▼
ViewModel
 │
 ▼
Application Services
 │
 ▼
Core / Runtime
```

Views should remain as thin as possible.

Business logic belongs in services.

Presentation logic belongs in ViewModels.

---

# 4. Startup Flow

Application startup should follow this sequence:

```text
Application Starts
        │
        ▼
Load Configuration
        │
        ▼
Initialize Logging
        │
        ▼
Register Services
        │
        ▼
Build Service Provider
        │
        ▼
Resolve MainWindow
        │
        ▼
Display MainWindow
```

The application should fail fast if any critical startup step cannot be completed.

---

# 5. Initial UI Components

The initial application should contain:

```text
App.xaml

MainWindow.xaml

MainWindowViewModel

INavigationService

Theme Resources
```

Additional windows should not be introduced during Sprint 0.

---

# 6. Dependency Injection

Views should not instantiate ViewModels directly.

Approved flow:

```text
ServiceProvider
      │
      ▼
MainWindow
      │
      ▼
MainWindowViewModel
```

ViewModels should receive all dependencies through constructor injection.

---

# 7. Navigation Strategy

Navigation should be abstracted from the beginning.

Recommended interface:

```text
INavigationService
```

Responsibilities:

* Navigate between Views.
* Manage navigation history.
* Support future dialogs.
* Support future multi-window workflows.

Views should never navigate directly to other Views.

---

# 8. Data Binding

All UI state should be exposed through ViewModels.

Rules:

* Two-way binding only when user input is expected.
* One-way binding for display-only information.
* Commands instead of Click event handlers.
* No business logic inside code-behind.

XAML should remain declarative.

---

# 9. Window Responsibilities

MainWindow should only:

* Host the root UI.
* Receive user interaction.
* Display bound data.
* Delegate work to the ViewModel.

MainWindow should not contain business logic.

---

# 10. Folder Structure

Recommended layout:

```text
App/

Views/

ViewModels/

Navigation/

Themes/

Resources/

Converters/
```

Each folder should have a single, well-defined responsibility.

---

# 11. Validation Checklist

Sprint 0 UI is complete when:

* ☐ Application launches.
* ☐ MainWindow appears.
* ☐ MainWindow uses a ViewModel.
* ☐ ViewModel resolved through DI.
* ☐ Commands execute successfully.
* ☐ No business logic exists in code-behind.

---

# 12. Why This Architecture?

### Why?

Starting with MVVM and dependency injection ensures that future UI features can be added consistently without large-scale refactoring.

### Why not?

Placing logic directly in Views or relying on code-behind creates tightly coupled interfaces that become increasingly difficult to test and maintain.

### Trade-offs

* Slightly more setup.
* Better separation of concerns.
* Easier testing.
* Scalable desktop architecture.

---

# 13. Future Expansion

Future UI capabilities may include:

* Navigation Shell.
* Chat workspace.
* Settings window.
* Notification center.
* Theme switching.
* Multi-window support.
* Live2D assistant host.
* Dockable panels.

These features should build upon the architecture defined in this document.

---

# 14. Governance

All future Views should comply with the following rules:

* Use MVVM.
* Receive ViewModels through DI.
* Avoid business logic in code-behind.
* Use Commands instead of event handlers where practical.
* Keep Views focused on presentation.

Architectural consistency should take precedence over convenience.

---

# 15. Summary

The First WPF Window Specification establishes the desktop architecture for AikoOS.

By combining MVVM, dependency injection, navigation abstraction, and clean presentation boundaries from the first window onward, the project creates a scalable UI foundation that supports future features without compromising maintainability.

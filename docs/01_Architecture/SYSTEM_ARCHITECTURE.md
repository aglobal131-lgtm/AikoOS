# SYSTEM ARCHITECTURE

> Version: 1.0

---

# Purpose

This document defines the overall architecture of AikoOS.

It explains how every major subsystem communicates and why the project adopts a modular client-server architecture.

This document is the architectural reference for the entire project.

---

# High-Level Architecture

```text
                    User
                      │
              Voice / Mouse / Keyboard
                      │
              ┌────────────────────┐
              │   Desktop Client   │
              │      (WPF)         │
              └────────────────────┘
                      │
          REST API / WebSocket
                      │
              ┌────────────────────┐
              │ Backend API Server │
              └────────────────────┘
                      │
      ┌───────────────┼────────────────┐
      │               │                │
 AI Gateway      Memory Engine    Plugin Engine
      │               │                │
      │         PostgreSQL        Plugin Host
      │               │
      ├────── Redis Cache
      │
      └────── AI Providers
             ├── Gemini
             ├── Groq
             ├── Ollama
             └── Future Providers
```

---

# Architectural Goals

The architecture is designed to achieve:

* Long-term maintainability
* High modularity
* Replaceable AI providers
* Independent client and server
* Easy feature expansion
* Low coupling
* High cohesion

---

# Layer Overview

The system consists of five primary layers.

## Presentation Layer

Responsible for user interaction.

Components:

* WPF
* Live2D
* Windows
* Settings UI

Responsibilities:

* Rendering
* Animation
* Input
* Notifications

Must NOT contain business logic.

---

## Application Layer

Responsible for coordinating requests.

Components:

* Controllers
* Services
* Validation
* DTO Mapping

Responsibilities:

* Process requests
* Route data
* Coordinate modules

---

## Domain Layer

The heart of AikoOS.

Contains:

* Memory Engine
* Emotion Engine
* Personality Engine
* Scheduler
* Plugin Manager

Rules:

* No UI code
* No database code
* No framework-specific code

---

## Infrastructure Layer

Responsible for external systems.

Examples:

* PostgreSQL
* Redis
* AI Providers
* File System
* Logging
* Cloud Storage

Infrastructure can be replaced without changing business rules.

---

## External Services

External dependencies include:

* Gemini
* Groq
* Ollama
* Whisper
* Piper

These services are accessed only through dedicated abstraction interfaces.

---

# Client Responsibilities

The desktop application is intentionally lightweight.

Responsibilities:

* Character rendering
* Animation
* User interaction
* Audio playback
* Desktop integration
* Window management

Not responsible for:

* AI reasoning
* Memory management
* Database operations
* Business rules

---

# Backend Responsibilities

The backend acts as the system brain.

Responsibilities:

* AI communication
* Prompt building
* Context assembly
* Memory management
* Plugin execution
* Automation
* Scheduling
* Data persistence

---

# AI Gateway

The AI Gateway isolates the rest of the system from AI providers.

Responsibilities:

* Provider selection
* Request routing
* Token usage
* Retry policy
* Response normalization

The rest of the system must never directly call external AI APIs.

---

# Memory Engine

The Memory Engine is responsible for:

* storing memories
* retrieving memories
* summarizing conversations
* semantic search
* memory scoring
* forgetting strategy

It does not communicate directly with the desktop client.

---

# Plugin System

Plugins extend AikoOS without modifying core code.

Plugins communicate through:

* Events
* Service Interfaces
* Plugin API

Plugins must never directly access internal database tables.

---

# Event-Driven Communication

Modules communicate primarily through events instead of direct dependencies.

Example:

```text
User Speaks
        │
        ▼
Voice Engine
        │
        ▼
Conversation Started
        │
        ▼
Memory Engine

Emotion Engine

Animation Engine
```

This reduces coupling.

---

# Dependency Direction

Dependencies always point inward.

```text
Presentation

↓

Application

↓

Domain

↓

Infrastructure
```

Infrastructure must never control business logic.

---

# Communication Protocol

Desktop ↔ Backend

Primary

* REST API

Realtime

* WebSocket

Future

* gRPC (optional)

---

# Error Handling

Every module should return structured errors.

Example

```text
Success

Error

ValidationError

PermissionDenied

AIUnavailable

DatabaseFailure
```

Errors must be logged before reaching the client.

---

# Logging Strategy

Every critical operation should generate logs.

Examples:

* AI Request
* Plugin Execution
* Memory Save
* Authentication
* Database Failure

Logging must support future monitoring systems.

---

# Security Principles

The desktop client is not trusted with sensitive operations.

Sensitive operations include:

* API keys
* Memory persistence
* AI credentials
* Database access

These remain exclusively on the backend.

---

# Scalability

The architecture should support future additions without redesign.

Future modules may include:

* Mobile Client
* Web Dashboard
* Cloud Synchronization
* Marketplace
* Multiple Characters
* Team Collaboration

Existing modules should require minimal changes when new modules are introduced.

---

# Architecture Principles Summary

The following principles govern every technical decision.

* Separation of Concerns
* Clean Architecture
* Dependency Injection
* Event-Driven Design
* API First
* Modular Development
* Documentation First
* Replaceable Infrastructure
* AI Provider Independence
* Long-Term Maintainability

These principles should not be violated unless a documented architectural decision explicitly approves the change.

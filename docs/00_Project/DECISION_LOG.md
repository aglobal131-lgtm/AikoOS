# DECISION LOG

> Version: 1.0

---

# Purpose

This document records all major architectural and technical decisions made during the development of AikoOS.

Its purpose is to:

* Preserve project history.
* Explain why decisions were made.
* Prevent repeated discussions.
* Help future contributors understand the project.

Every major decision should include:

* Status
* Context
* Decision
* Alternatives
* Consequences

---

# Decision Format

Every new decision must follow this template.

---

## Decision ID

ADR-XXXX

### Status

Accepted

### Date

YYYY-MM-DD

### Context

Why this decision is needed.

### Decision

What was chosen.

### Alternatives

Other options considered.

### Consequences

Advantages and disadvantages.

---

# ADR-0001

## Project Type

### Status

Accepted

### Context

The project needs to evolve over many years.

### Decision

AikoOS will be developed as a platform instead of a single application.

### Alternatives

* Standalone desktop app

### Consequences

Pros

* Easier expansion
* Plugin support
* Better architecture

Cons

* Higher initial complexity

---

# ADR-0002

## Desktop Framework

### Status

Accepted

### Decision

Use WPF.

### Alternatives

* WinForms
* Avalonia
* Electron

### Reason

* Mature ecosystem
* Excellent Windows integration
* Strong MVVM support
* Suitable for desktop companion

---

# ADR-0003

## Backend

### Status

Accepted

### Decision

ASP.NET Core Web API

### Alternatives

* Node.js
* Python FastAPI
* Go

### Reason

* Same language ecosystem as client
* High performance
* Excellent Dependency Injection
* Long-term maintainability

---

# ADR-0004

## Database

### Status

Accepted

### Decision

PostgreSQL

### Alternatives

* SQLite
* MongoDB
* MySQL

### Reason

* Enterprise ready
* Strong relational model
* Supports pgvector
* Excellent scalability

---

# ADR-0005

## Cache

### Status

Accepted

### Decision

Redis

### Reason

Improve performance for:

* Sessions
* Frequently accessed data
* AI cache
* Rate limiting

---

# ADR-0006

## Vector Search

### Status

Accepted

### Decision

pgvector

### Future

Can migrate to Qdrant if needed.

---

# ADR-0007

## AI Provider

### Status

Accepted

### Decision

The project must support multiple AI providers.

Supported providers may include:

* Gemini
* Groq
* Ollama
* OpenAI
* Claude

No provider-specific logic should leak into business modules.

---

# ADR-0008

## Voice System

### Status

Accepted

### Decision

Speech Recognition

* Whisper

Speech Synthesis

* Piper

The voice pipeline must allow replacement of both components.

---

# ADR-0009

## Character Animation

### Status

Accepted

### Decision

Live2D

### Alternatives

* Spine
* Sprite Animation
* Unity

Reason

Provides expressive 2D character animation while remaining lightweight.

---

# ADR-0010

## Repository Structure

### Status

Accepted

### Decision

Separate documentation from source code.

Repository structure:

* docs/
* client/
* server/
* shared/
* plugins/
* docker/
* assets/

---

# ADR-0011

## Documentation First

### Status

Accepted

### Decision

Every major feature must be documented before implementation.

Documentation becomes the project's source of truth.

---

# ADR-0012

## API First

### Status

Accepted

### Decision

Business logic belongs in the backend.

Desktop client should focus on:

* UI
* Animation
* User interaction

---

# ADR-0013

## Modular Architecture

### Status

Accepted

### Decision

Each major subsystem must be independently replaceable.

Examples:

* Memory
* Voice
* Vision
* Emotion
* Plugin

Modules communicate through well-defined interfaces.

---

# ADR-0014

## Free-First Strategy

### Status

Accepted

### Decision

Prefer free or open-source technologies whenever practical.

Commercial services should only be introduced when they provide significant long-term value.

---

# ADR-0015

## Long-Term Goal

### Status

Accepted

### Decision

AikoOS is intended to become a reusable AI Companion framework capable of supporting multiple characters, AI providers, and future client applications without fundamental architectural changes.

---

# Future Decisions

New ADRs should continue numbering sequentially.

Examples:

ADR-0016 Authentication

ADR-0017 Plugin Marketplace

ADR-0018 Cloud Sync

ADR-0019 Mobile Companion

ADR-0020 Multi-user Support

---

# Change Policy

Accepted ADRs should not be modified lightly.

If a previous decision becomes unsuitable:

1. Create a new ADR.
2. Reference the previous ADR.
3. Explain why the change is necessary.
4. Document migration impacts.

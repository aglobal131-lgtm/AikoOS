# AikoOS

> A modular AI Companion platform for Windows.

## Vision

AikoOS is an AI Companion designed to feel like a real friend instead of a traditional chatbot.

The project aims to create a desktop character that can communicate naturally through voice, remember long-term conversations, understand what happens on the user's computer, and gradually build a unique personality over time.

Unlike most AI desktop assistants, AikoOS is designed as a long-term platform rather than a single application.

---

# Core Goals

* Desktop AI Companion
* Anime/Chibi character
* Natural voice conversation
* Long-term memory
* Emotion simulation
* Vision capability
* Plugin ecosystem
* Multi-model AI support
* Production-ready architecture
* Free-first development

---

# Design Philosophy

AikoOS follows several fundamental principles.

## Modular

Every feature is an independent module.

Examples:

* Voice
* Vision
* Memory
* Emotion
* Plugin
* Desktop
* AI Gateway

Each module can evolve independently.

---

## AI Provider Independent

The system must never depend on a single AI provider.

Supported providers may include:

* Gemini
* Groq
* Ollama
* OpenAI
* Claude
* Future providers

Changing AI providers must require minimal code changes.

---

## API First

The Desktop application communicates only with the Backend API.

Business logic should never be implemented directly inside the desktop application.

---

## Production Ready

The architecture should be scalable from the beginning.

Avoid temporary solutions that require major rewrites later.

---

## Free First

Whenever possible:

* Use open-source software.
* Use free AI models.
* Self-host services when practical.
* Minimize operational costs.

---

# Technology Stack

## Desktop

* C#
* .NET 8+
* WPF

## Backend

* ASP.NET Core Web API

## Database

* PostgreSQL

## Cache

* Redis

## Vector Search

* pgvector

Future migration to dedicated vector databases such as Qdrant should remain possible.

## AI

* Gemini
* Groq
* Ollama

## Speech Recognition

* Whisper

## Speech Synthesis

* Piper

## Animation

* Live2D

---

# Repository Structure

client/
server/
shared/
plugins/
docker/
docs/
assets/
scripts/

---

# Documentation

All documentation is stored inside the `docs/` directory.

Documentation is considered the single source of truth for the project.

Code should follow the documentation whenever possible.

---

# Development Strategy

The project is developed in phases.

1. Planning
2. Backend
3. Desktop
4. Animation
5. Voice
6. Memory
7. Vision
8. Emotion
9. Plugins
10. Optimization
11. Release

---

# License

This project will determine its license before the first public release.

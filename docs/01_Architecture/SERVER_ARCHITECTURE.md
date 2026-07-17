# SERVER ARCHITECTURE

> Version: 1.0
> Status: Draft
> Framework: ASP.NET Core
> Primary Database: PostgreSQL
> Cache and Distributed Coordination: Redis
> Vector Search: pgvector
> Architecture Style: Modular Clean Architecture

---

# 1. Purpose

This document defines the architecture of the AikoOS backend server.

The server is the central intelligence and coordination layer of AikoOS. It processes conversations, manages persistent memory, routes requests to AI providers, executes plugins, schedules tasks, stores data, applies permissions, and synchronizes state with desktop clients.

The desktop client represents Aiko's visible body. The server represents Aiko's persistent brain and operational core.

---

# 2. Server Responsibilities

The backend is responsible for:

* User and device authentication.
* Conversation orchestration.
* AI model selection and routing.
* Prompt and context construction.
* Long-term memory management.
* Semantic memory retrieval.
* Emotion and personality state.
* Plugin registration and execution.
* Permission enforcement.
* Task scheduling.
* Automation workflows.
* Notification delivery.
* Database persistence.
* Cache management.
* Realtime event delivery.
* Configuration management.
* Logging and auditing.
* Health monitoring.
* Rate limiting.
* Background processing.

The server must not be responsible for:

* Rendering Live2D characters.
* Managing WPF windows.
* Directly controlling local audio devices.
* Performing unrestricted desktop actions.
* Assuming a specific client implementation.
* Storing UI-only temporary state unless synchronization requires it.

---

# 3. Architectural Goals

The server architecture must support:

* Long-term maintainability.
* Clear module boundaries.
* Replaceable infrastructure.
* Multiple AI providers.
* Multiple desktop devices.
* Future mobile and web clients.
* Reliable background processing.
* Horizontal scaling when needed.
* Secure plugin execution.
* Realtime communication.
* Testable domain logic.
* Low-cost self-hosted deployment.
* Gradual migration from a single server to distributed services.

---

# 4. Architecture Style

AikoOS uses a modular monolith based on Clean Architecture during its early and intermediate stages.

```text
API Layer
    │
    ▼
Application Layer
    │
    ▼
Domain Modules
    │
    ▼
Infrastructure Adapters
    │
    ▼
PostgreSQL / Redis / AI Providers / File Storage
```

A modular monolith is chosen because it provides:

* Simpler deployment.
* Easier debugging.
* Lower infrastructure cost.
* Shared transactions.
* Clear code ownership.
* Future migration paths to separate services.

The project must not begin as a large microservice system.

Modules should still be designed so that selected components can be extracted later if scaling requires it.

---

# 5. Recommended Solution Structure

```text
server/
├── AikoOS.Server.Api/
├── AikoOS.Server.Application/
├── AikoOS.Server.Domain/
├── AikoOS.Server.Infrastructure/
├── AikoOS.Server.Contracts/
├── AikoOS.Server.Workers/
├── AikoOS.Server.Plugins/
├── AikoOS.Server.Migrations/
├── AikoOS.Server.IntegrationTests/
└── AikoOS.Server.UnitTests/
```

As the project grows, major domain modules may become separate projects.

Example:

```text
server/
├── Modules/
│   ├── AikoOS.Modules.Conversation/
│   ├── AikoOS.Modules.Memory/
│   ├── AikoOS.Modules.AI/
│   ├── AikoOS.Modules.Emotion/
│   ├── AikoOS.Modules.Plugins/
│   ├── AikoOS.Modules.Automation/
│   └── AikoOS.Modules.Identity/
```

---

# 6. Project Responsibilities

## 6.1 AikoOS.Server.Api

Contains the ASP.NET Core host.

Responsibilities:

* Application startup.
* Dependency injection composition.
* HTTP endpoints.
* WebSocket endpoints.
* Authentication middleware.
* Authorization middleware.
* Exception handling middleware.
* Rate limiting.
* Request correlation.
* API versioning.
* Health endpoints.
* OpenAPI configuration.
* Request logging.

This project should contain minimal business logic.

---

## 6.2 AikoOS.Server.Application

Coordinates use cases.

Responsibilities:

* Commands.
* Queries.
* Application services.
* Input validation.
* Transaction boundaries.
* Authorization checks.
* DTO mapping.
* Workflow orchestration.
* Domain event dispatch.
* Integration event publishing.

Examples of application use cases:

* Send a conversation message.
* Save a memory.
* Search memories.
* Execute a plugin skill.
* Create a scheduled reminder.
* Update user settings.
* Approve a permission request.

---

## 6.3 AikoOS.Server.Domain

Contains core business rules.

Responsibilities:

* Entities.
* Value objects.
* Aggregates.
* Domain services.
* Domain events.
* Repository interfaces.
* Business invariants.
* Permission rules.
* Memory scoring logic.
* Emotion transition rules.
* Plugin capability definitions.

The Domain project must not depend on:

* ASP.NET Core.
* Entity Framework Core.
* Redis clients.
* AI provider SDKs.
* File storage SDKs.
* HTTP clients.

---

## 6.4 AikoOS.Server.Infrastructure

Implements external integrations.

Responsibilities:

* PostgreSQL persistence.
* Entity Framework Core configuration.
* Redis integration.
* AI provider clients.
* Vector search implementation.
* File storage.
* Email or notification gateways.
* Encryption.
* Distributed locks.
* External API clients.
* Logging exporters.

Infrastructure implements interfaces defined by inner layers.

---

## 6.5 AikoOS.Server.Contracts

Contains stable contracts shared with clients.

Examples:

* Request DTOs.
* Response DTOs.
* Realtime event contracts.
* Error contracts.
* Enumerations.
* Pagination models.
* Capability descriptors.
* API version identifiers.

Contracts must not expose internal database entities.

---

## 6.6 AikoOS.Server.Workers

Contains background processing hosts or worker registrations.

Responsibilities:

* Conversation summarization.
* Memory extraction.
* Embedding generation.
* Scheduled task execution.
* Cleanup jobs.
* Expired token cleanup.
* Notification delivery.
* Plugin maintenance jobs.
* Data retention tasks.
* Retry queues.

Workers may run inside the API process initially.

They should remain separable so they can later run as independent processes.

---

## 6.7 AikoOS.Server.Plugins

Contains the plugin runtime and built-in plugins.

Responsibilities:

* Plugin discovery.
* Manifest validation.
* Plugin loading.
* Permission validation.
* Skill registration.
* Plugin lifecycle.
* Plugin isolation.
* Plugin health checks.
* Built-in skill implementations.

---

## 6.8 AikoOS.Server.Migrations

Contains database migration tooling and deployment migrations.

Responsibilities:

* Schema migrations.
* Seed data.
* Migration verification.
* Rollback guidance.
* pgvector extension setup.
* Development database initialization.

---

# 7. Core Server Modules

The backend consists of several major modules.

```text
Identity
Devices
Conversation
AI Gateway
Memory
Emotion
Personality
Plugins
Permissions
Automation
Scheduler
Notifications
Configuration
Audit
Files
Realtime
```

Each module owns its rules and data access boundaries.

---

# 8. Module Ownership

A module should own:

* Its domain entities.
* Its application services.
* Its database mappings.
* Its commands and queries.
* Its events.
* Its public interfaces.
* Its validation rules.

A module must not directly modify another module's tables.

Cross-module interaction should use:

* Public application interfaces.
* Domain events.
* Integration events.
* Read-only projections where approved.

---

# 9. Request Processing Pipeline

A typical HTTP request follows this flow:

```text
Client Request
      │
      ▼
Correlation Middleware
      │
      ▼
Authentication
      │
      ▼
Authorization
      │
      ▼
Rate Limiting
      │
      ▼
Endpoint
      │
      ▼
Validation
      │
      ▼
Application Use Case
      │
      ▼
Domain Logic
      │
      ▼
Repository / Provider
      │
      ▼
Response Mapping
      │
      ▼
Client Response
```

Every request must have a correlation identifier.

---

# 10. Conversation Processing Flow

```text
User Message
     │
     ▼
Conversation Module
     │
     ├── Validate Request
     ├── Save User Message
     ├── Load Conversation State
     └── Create Processing Request
             │
             ▼
       Context Builder
             │
             ├── User Profile
             ├── Recent Messages
             ├── Relevant Memories
             ├── Personality State
             ├── Emotion State
             ├── Tool Availability
             └── Permission Context
             │
             ▼
        AI Gateway
             │
             ├── Select Provider
             ├── Select Model
             ├── Execute Tools
             └── Normalize Response
             │
             ▼
       Response Processor
             │
             ├── Save Assistant Message
             ├── Extract Memory Candidates
             ├── Update Emotion
             ├── Generate TTS Metadata
             └── Publish Realtime Events
```

Conversation processing should support streaming.

---

# 11. API Design

The server exposes:

* REST APIs for standard commands and queries.
* WebSocket for realtime events and streaming.
* Health endpoints for diagnostics.
* Administrative endpoints where required.

Potential endpoint groups:

```text
/api/v1/auth
/api/v1/devices
/api/v1/conversations
/api/v1/messages
/api/v1/memories
/api/v1/plugins
/api/v1/permissions
/api/v1/tasks
/api/v1/schedules
/api/v1/settings
/api/v1/files
/api/v1/system
```

Detailed endpoint contracts belong in the API documentation.

---

# 12. Realtime Communication

Realtime communication is required for:

* Streaming assistant text.
* Animation state updates.
* Emotion changes.
* TTS status.
* Task progress.
* Notifications.
* Permission requests.
* Plugin events.
* Server availability.
* Conversation cancellation.

The realtime subsystem should support:

* Client reconnect.
* Authentication.
* Event sequencing.
* Event correlation.
* Message acknowledgement where necessary.
* Duplicate detection.
* Missed-state recovery.

---

# 13. Commands and Queries

The Application Layer should separate operations conceptually into commands and queries.

## Commands

Commands change state.

Examples:

```text
SendMessage
CreateMemory
DeleteMemory
UpdateSettings
InstallPlugin
ApprovePermission
CreateSchedule
CancelTask
```

## Queries

Queries return data without changing core state.

Examples:

```text
GetConversation
SearchMemories
GetPluginList
GetSettings
GetTaskStatus
GetSystemHealth
```

A full CQRS infrastructure is optional.

The conceptual separation should still be preserved.

---

# 14. Domain Events

Domain events represent facts that occurred inside the system.

Examples:

```text
ConversationStarted
UserMessageReceived
AssistantResponseGenerated
MemoryCandidateDetected
MemoryStored
EmotionChanged
PluginInstalled
PluginExecutionRequested
PermissionRequested
TaskScheduled
TaskCompleted
```

Domain events should be immutable.

They should use past-tense names.

---

# 15. Integration Events

Integration events communicate changes across module or process boundaries.

Examples:

```text
conversation.response.streamed
memory.created
emotion.updated
plugin.execution.completed
notification.created
task.progress.updated
```

Integration events should be versioned.

Event handlers must be idempotent where duplicate delivery is possible.

---

# 16. Event Delivery Strategy

During early development, events may use an in-process event dispatcher.

```text
Module
  │
  ▼
In-Process Event Bus
  │
  ├── Memory Handler
  ├── Emotion Handler
  └── Notification Handler
```

Later, selected integration events may use external messaging infrastructure.

The domain must not depend directly on a specific message broker.

---

# 17. Database Architecture

PostgreSQL is the system of record.

The server uses Entity Framework Core for relational persistence.

Database responsibilities include:

* Identity data.
* Device registrations.
* Conversations.
* Messages.
* Memories.
* Embeddings.
* Personality settings.
* Emotion history.
* Plugin metadata.
* Permissions.
* Scheduled tasks.
* Audit records.

The database schema is documented separately.

---

# 18. Transaction Boundaries

Each application use case should define a clear transaction boundary.

Examples:

* Saving a user message and conversation metadata.
* Creating a memory and embedding status record.
* Updating a permission decision.
* Registering a scheduled task.

External API calls should not remain inside long-running database transactions.

Recommended sequence:

1. Save durable request state.
2. Commit transaction.
3. Call external provider.
4. Save normalized result.
5. Publish events.

Compensating actions should be used where full atomicity is impossible.

---

# 19. Repository Pattern

Repositories may be used for domain aggregates where they improve abstraction.

Example:

```csharp
public interface IConversationRepository
{
    Task<Conversation?> GetAsync(
        ConversationId conversationId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Conversation conversation,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
```

Avoid unnecessary generic repositories that merely duplicate Entity Framework Core.

---

# 20. Unit of Work

A unit-of-work abstraction may coordinate changes across repositories inside one transaction.

It should not hide important transaction behavior.

Example:

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
```

---

# 21. Redis Responsibilities

Redis may be used for:

* Distributed cache.
* Short-lived session state.
* Rate limiting.
* Conversation processing locks.
* Realtime connection metadata.
* Temporary AI results.
* Task coordination.
* Distributed leases.
* Idempotency keys.

Redis must not become the only storage for critical long-term data.

---

# 22. Cache Rules

Every cache entry must define:

* Cache key format.
* Time-to-live.
* Invalidation strategy.
* Source of truth.
* Sensitivity classification.
* Maximum size.

Example key format:

```text
aikoos:{environment}:{module}:{entity}:{identifier}
```

Example:

```text
aikoos:production:user-profile:8a30f8e2
```

---

# 23. AI Gateway Boundary

All model providers are accessed through the AI Gateway.

The AI Gateway handles:

* Provider abstraction.
* Model abstraction.
* Request normalization.
* Response normalization.
* Streaming.
* Retry policy.
* Timeout policy.
* Rate limits.
* Provider health.
* Token estimation.
* Cost estimation.
* Fallback routing.
* Structured output validation.
* Tool-call normalization.

No other module may directly depend on a provider SDK.

---

# 24. AI Provider Interface

Example abstraction:

```csharp
public interface IAIProvider
{
    string ProviderId { get; }

    Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<AIStreamChunk> StreamAsync(
        AIRequest request,
        CancellationToken cancellationToken = default);
}
```

Provider-specific request formats must remain inside provider adapters.

---

# 25. Model Router

The Model Router selects the most appropriate provider and model.

Selection factors may include:

* Requested capability.
* User preference.
* Provider availability.
* Context size.
* Latency.
* Cost.
* Privacy level.
* Tool support.
* Vision support.
* Structured output support.
* Local versus cloud preference.

The routing strategy must be configurable.

---

# 26. Memory Engine Integration

The server owns all persistent memory operations.

The Memory Engine provides:

* Candidate extraction.
* Importance scoring.
* Duplicate detection.
* Semantic embedding.
* Memory storage.
* Memory retrieval.
* Memory consolidation.
* Archiving.
* Forgetting.
* User-controlled deletion.

Conversation modules should request memory services through interfaces.

---

# 27. Vector Search

pgvector is used initially for semantic search.

The vector subsystem must remain abstracted.

Example:

```csharp
public interface IVectorSearchService
{
    Task<IReadOnlyList<VectorSearchResult>> SearchAsync(
        VectorSearchQuery query,
        CancellationToken cancellationToken = default);
}
```

This allows later migration to:

* Qdrant.
* Weaviate.
* Milvus.
* Another vector engine.

---

# 28. Plugin Runtime

The server plugin runtime manages plugin capabilities.

Responsibilities:

* Discover plugins.
* Validate manifests.
* Check compatibility.
* Register skills.
* Enforce permissions.
* Execute plugin methods.
* Apply timeouts.
* Capture logs.
* Disable unhealthy plugins.
* Handle plugin updates.

Plugins must not gain unrestricted access to internal services.

---

# 29. Plugin Execution Flow

```text
AI Tool Request
      │
      ▼
Plugin Skill Resolver
      │
      ▼
Permission Check
      │
      ▼
Input Validation
      │
      ▼
Execution Policy
      │
      ▼
Plugin Execution
      │
      ▼
Output Validation
      │
      ▼
Audit Record
      │
      ▼
Normalized Tool Result
```

---

# 30. Permission System

Every sensitive capability must be represented by a permission.

Examples:

```text
screen.capture
camera.read
microphone.record
filesystem.read
filesystem.write
process.launch
command.execute
browser.control
calendar.read
notification.send
```

Permission decisions may be:

* Denied.
* Allowed once.
* Allowed for the current session.
* Allowed permanently.
* Allowed with scope restrictions.

The server stores permission policy.

The desktop client displays user confirmation.

---

# 31. Automation Engine

The Automation Engine coordinates actions that occur without an immediate conversational request.

Examples:

* Reminders.
* Scheduled summaries.
* Periodic checks.
* Device notifications.
* Plugin-triggered workflows.
* Maintenance tasks.

Every automation must include:

* Owner.
* Trigger.
* Action.
* Permissions.
* Schedule or condition.
* Retry policy.
* Cancellation policy.
* Audit history.

---

# 32. Scheduler

The scheduler manages future and recurring work.

Requirements:

* Persistent schedules.
* Time zone support.
* Misfire handling.
* Retry policy.
* Cancellation.
* Concurrency control.
* Execution history.
* Idempotency.
* User-visible status.

Scheduled tasks must survive server restarts.

---

# 33. Background Jobs

Background jobs are used for work that should not block API requests.

Examples:

* Generate embeddings.
* Summarize long conversations.
* Clean expired data.
* Reprocess failed AI requests.
* Generate reports.
* Deliver notifications.
* Validate plugin health.

Jobs should have:

* Unique identifier.
* Status.
* Attempt count.
* Created time.
* Started time.
* Completed time.
* Failure reason.
* Correlation identifier.

---

# 34. Job Queue Strategy

Initial versions may use:

* PostgreSQL-backed job records.
* Hosted background services.
* Redis-based coordination.

The job abstraction should permit migration to a dedicated queue later.

The project must not introduce a complex queue system before requirements justify it.

---

# 35. Identity

The server must distinguish:

* User.
* Device.
* Client session.
* Character profile.
* Plugin identity.
* Service identity.

A single user may have multiple devices.

Each device should have its own registration and revocable credentials.

---

# 36. Authentication

Authentication options may evolve over time.

Initial self-hosted deployment may support:

* Local account.
* Device registration.
* Secure access token.
* Refresh token.

Future options may include:

* External identity providers.
* Passkeys.
* OAuth.
* Mobile pairing.

Authentication implementation must remain separate from domain business rules.

---

# 37. Authorization

Authorization should combine:

* User identity.
* Device identity.
* Role.
* Permission policy.
* Resource ownership.
* Requested capability.
* Plugin scope.

A successful authentication does not imply unrestricted authorization.

---

# 38. Secrets Management

Secrets include:

* AI API keys.
* Database passwords.
* Encryption keys.
* OAuth secrets.
* Storage credentials.
* Plugin secrets.

Secrets must not be:

* Committed to Git.
* Stored in ordinary settings files.
* Returned to clients.
* Written to logs.
* Embedded in compiled client code.

Development and production environments should use separate secrets.

---

# 39. Configuration System

Configuration sources may include:

1. Default application configuration.
2. Environment-specific configuration.
3. Environment variables.
4. Secret provider.
5. Database-backed runtime settings.
6. User settings.

Higher-priority sources override lower-priority sources.

Configuration must be validated during startup.

Critical invalid configuration should prevent unsafe startup.

---

# 40. Error Handling

The server uses centralized exception handling.

Errors should be converted into stable API error contracts.

Example:

```json
{
  "error": {
    "code": "AI_PROVIDER_UNAVAILABLE",
    "message": "The selected AI service is temporarily unavailable.",
    "correlationId": "b5c6478f-3922-43a4-9d12-78ad96010904",
    "retryable": true
  }
}
```

Internal stack traces must never be returned in production responses.

---

# 41. Error Categories

Recommended categories:

```text
VALIDATION_ERROR
AUTHENTICATION_REQUIRED
ACCESS_DENIED
RESOURCE_NOT_FOUND
CONFLICT
RATE_LIMITED
AI_PROVIDER_UNAVAILABLE
AI_RESPONSE_INVALID
PLUGIN_FAILURE
PERMISSION_REQUIRED
DATABASE_FAILURE
REALTIME_FAILURE
BACKGROUND_JOB_FAILURE
INTERNAL_ERROR
```

Error codes should remain stable across versions.

---

# 42. Logging

Server logging should use structured logs.

Important fields include:

* Timestamp.
* Log level.
* Service.
* Module.
* Event name.
* User identifier where permitted.
* Device identifier.
* Correlation identifier.
* Request identifier.
* Duration.
* Result.
* Error code.

Logs must avoid sensitive conversation content by default.

---

# 43. Audit Logging

Security-sensitive and user-impacting actions require audit records.

Examples:

* Permission granted.
* Permission denied.
* Plugin installed.
* Plugin disabled.
* Memory deleted.
* Account setting changed.
* Automation created.
* Process launch requested.
* Screen capture requested.
* Administrative action executed.

Audit records should be append-only from the application perspective.

---

# 44. Health Checks

The server should expose health checks for:

* API process.
* PostgreSQL.
* Redis.
* Vector extension.
* AI providers.
* File storage.
* Background workers.
* Realtime subsystem.
* Plugin runtime.

Health checks should distinguish:

* Healthy.
* Degraded.
* Unhealthy.

Sensitive details must be protected.

---

# 45. Observability

The backend should support:

* Structured logging.
* Metrics.
* Distributed tracing.
* Health checks.
* Correlation identifiers.
* Performance timings.
* Dependency diagnostics.

Potential metrics:

```text
HTTP request duration
AI request duration
AI provider failure rate
Active realtime connections
Memory retrieval duration
Embedding queue depth
Plugin execution duration
Background job failure rate
Database query duration
Cache hit ratio
```

---

# 46. Rate Limiting

Rate limiting protects:

* Authentication endpoints.
* Conversation endpoints.
* AI requests.
* File uploads.
* Plugin execution.
* Permission submissions.
* Administrative endpoints.

Limits may depend on:

* User.
* Device.
* IP address.
* Provider quota.
* Endpoint type.

Rate limits should return a stable retry indication.

---

# 47. Resilience Policies

External dependencies require resilience policies.

Recommended mechanisms:

* Timeout.
* Retry with backoff.
* Circuit breaker.
* Bulkhead isolation.
* Fallback.
* Cancellation.
* Idempotency.
* Health-based provider routing.

Retries must not be applied blindly to non-idempotent operations.

---

# 48. Idempotency

Operations that may be submitted more than once should support idempotency keys.

Examples:

* Send message.
* Create scheduled task.
* Execute plugin action.
* Upload file.
* Approve permission.
* Register device.

The server should return the original result for repeated valid requests where appropriate.

---

# 49. Concurrency Control

Concurrency issues may occur in:

* Conversation processing.
* Memory updates.
* Task execution.
* Plugin execution.
* Permission decisions.
* User settings updates.

Possible controls include:

* Optimistic concurrency.
* Database row versions.
* Distributed locks.
* Conversation-scoped processing locks.
* Unique constraints.
* Idempotency keys.

Avoid global locks.

---

# 50. File Storage

Files may include:

* Character packages.
* Generated speech.
* User attachments.
* Plugin packages.
* Screenshots.
* Export archives.
* Diagnostic bundles.

File metadata belongs in PostgreSQL.

File content may be stored using:

* Local filesystem initially.
* S3-compatible storage later.
* Cloud object storage when required.

The application should use a file-storage abstraction.

---

# 51. Data Classification

Data should be classified into categories.

## Public

Examples:

* Public plugin descriptions.
* Public character metadata.

## Internal

Examples:

* System configuration.
* Operational metrics.

## Personal

Examples:

* User settings.
* Conversation history.
* Memories.

## Sensitive

Examples:

* Access tokens.
* Screen captures.
* Microphone recordings.
* Private files.
* Provider credentials.

Storage, logging, and retention rules depend on classification.

---

# 52. Data Retention

Every stored data type should define:

* Retention duration.
* Deletion policy.
* User controls.
* Export support.
* Backup behavior.
* Audit requirements.

Raw temporary data should not be stored indefinitely.

Examples:

* Temporary TTS audio may expire quickly.
* Diagnostic logs may have limited retention.
* User memories remain until deletion or retention rules apply.
* Raw microphone recordings are not persisted by default.

---

# 53. Privacy Principles

The server follows these principles:

* Collect only necessary data.
* Obtain permission for sensitive access.
* Make stored memories visible to the user.
* Support memory deletion.
* Support data export.
* Avoid hidden recording.
* Avoid hidden screen capture.
* Do not train external systems on user data without explicit consent.
* Separate operational logs from personal content.

---

# 54. API Versioning

Public APIs should be versioned.

Example:

```text
/api/v1/conversations
```

Breaking changes require a new version.

Realtime events should include schema version information.

Internal module interfaces may evolve more frequently but must remain controlled.

---

# 55. Contract Compatibility

Contracts should follow additive evolution where possible.

Safe changes include:

* Adding optional fields.
* Adding new event types.
* Adding new endpoints.

Risky changes include:

* Renaming fields.
* Changing field meanings.
* Removing enum values.
* Changing required behavior.

Clients should ignore unknown optional fields safely.

---

# 56. Database Migrations

Database changes must use migrations.

Migration requirements:

* Unique identifier.
* Clear name.
* Forward migration.
* Rollback guidance where possible.
* Data migration plan.
* Index impact review.
* Deployment notes.
* Backup requirement for destructive changes.

Production schema changes must not be performed manually without documentation.

---

# 57. Startup Sequence

```text
Process Start
     │
     ▼
Load Configuration
     │
     ▼
Validate Secrets
     │
     ▼
Initialize Logging
     │
     ▼
Register Services
     │
     ▼
Check Database
     │
     ▼
Check Migrations
     │
     ▼
Initialize Redis
     │
     ▼
Register AI Providers
     │
     ▼
Load Plugins
     │
     ▼
Start Background Workers
     │
     ▼
Open API and Realtime Endpoints
     │
     ▼
Ready
```

Optional subsystem failure may place the server in degraded mode.

Critical persistence or security failure should prevent startup.

---

# 58. Shutdown Sequence

During graceful shutdown:

1. Stop accepting new long-running work.
2. Notify realtime clients where practical.
3. Stop schedule dispatch.
4. Wait for safe background operations.
5. Cancel remaining operations after timeout.
6. Flush logs and telemetry.
7. Close provider clients.
8. Close Redis connections.
9. Close database connections.
10. Exit.

---

# 59. Deployment Model

Initial deployment uses Docker Compose.

Potential components:

```text
AikoOS API
PostgreSQL
Redis
Optional reverse proxy
Optional object storage
Optional monitoring
```

The backend should run in:

* Local development.
* Local self-hosted mode.
* Private home server.
* Cloud virtual machine.
* Container platform.

Deployment-specific logic must not leak into domain modules.

---

# 60. Horizontal Scaling

The initial server may run as one instance.

Future horizontal scaling requires:

* Stateless API nodes.
* Shared PostgreSQL.
* Shared Redis.
* Distributed task coordination.
* Realtime connection strategy.
* Shared file storage.
* Distributed locking where required.
* Idempotent background jobs.

In-memory state must not be required for correctness.

---

# 61. Multi-Device Support

A user may connect multiple clients.

The server should support:

* Device registration.
* Device-specific settings.
* Device presence.
* Notification routing.
* Device capability reporting.
* Per-device permission context.
* Session revocation.
* Realtime synchronization.

Example capabilities:

```text
microphone
speaker
screen_capture
camera
desktop_actions
notifications
live2d_rendering
```

---

# 62. Server-Side Capability Registry

The server should know which capabilities are available.

Capabilities may come from:

* AI providers.
* Plugins.
* Connected devices.
* Local models.
* Server configuration.

The capability registry helps the AI Gateway and Automation Engine select valid actions.

---

# 63. Administrative Interface

A future administrative interface may support:

* System health.
* Plugin management.
* User management.
* Provider configuration.
* Task inspection.
* Audit review.
* Migration status.
* Feature flags.
* Log access.

Administrative endpoints must have stronger authorization and auditing.

---

# 64. Feature Flags

Feature flags may control:

* Experimental providers.
* New memory strategies.
* Plugin features.
* Beta UI events.
* Alternative model routing.
* New automation capabilities.

Feature flags should not permanently replace clean configuration or proper release management.

---

# 65. Testing Architecture

## Unit Tests

Cover:

* Domain rules.
* Application handlers.
* Permission checks.
* Memory scoring.
* Model routing.
* Emotion transitions.
* Scheduling logic.
* Validation.

## Integration Tests

Cover:

* PostgreSQL persistence.
* Redis behavior.
* Provider adapters.
* WebSocket delivery.
* Plugin execution.
* Authentication.
* File storage.
* Background jobs.

## Contract Tests

Verify:

* Client-server DTO compatibility.
* Realtime event schemas.
* AI provider normalization.
* Plugin contracts.

## End-to-End Tests

Cover complete flows such as:

```text
User sends message
AI responds
Message persists
Memory candidate is created
Realtime events reach client
```

---

# 66. Development Environment

The local development environment should support one-command infrastructure startup.

Example components:

```text
PostgreSQL
Redis
API
Worker
Optional local AI service
```

Development configuration must use safe test credentials and isolated data.

---

# 67. Prohibited Server Practices

The following are prohibited unless approved through an architectural decision:

* Direct provider calls outside the AI Gateway.
* Business rules inside controllers.
* Cross-module table modification.
* Plain-text secret storage.
* Unbounded background tasks.
* Silent permission bypass.
* Returning internal entities directly through APIs.
* Logging raw secrets.
* Permanent reliance on in-memory state.
* Unversioned public contracts.
* Executing plugin code without limits.
* Long external calls inside database transactions.
* Using Redis as the sole source of truth.
* Hard-coding environment-specific configuration.
* Automatic destructive migrations without safeguards.

---

# 68. Definition of Done

The backend foundation is considered complete when:

* The API starts reliably.
* Configuration is validated.
* PostgreSQL is connected.
* Redis is connected.
* Migrations work.
* Authentication foundation exists.
* REST endpoints work.
* Realtime communication works.
* Central error handling works.
* Structured logging works.
* Health checks work.
* Background jobs can execute.
* AI provider interfaces exist.
* Plugin runtime interfaces exist.
* Permission checks exist.
* Unit and integration test foundations exist.
* Docker-based local deployment works.
* Documentation matches implementation.

---

# 69. Evolution Strategy

AikoOS should evolve through the following stages.

## Stage 1: Modular Monolith

* One deployable backend.
* Clear modules.
* Shared PostgreSQL.
* Shared Redis.
* In-process event bus.

## Stage 2: Separate Workers

* API process.
* Background worker process.
* Shared database and queue coordination.

## Stage 3: Selective Service Extraction

Extract only modules with real scaling or isolation needs.

Potential candidates:

* AI Gateway.
* Plugin Execution.
* Voice Processing.
* Vision Processing.
* Notification Delivery.

## Stage 4: Distributed Platform

Only considered if actual usage requires it.

The project must not adopt distributed complexity merely for appearance.

---

# 70. Summary

The AikoOS backend is the persistent brain and coordination center of the platform.

It must remain:

* Modular.
* Secure.
* Observable.
* Provider-independent.
* Client-independent.
* Testable.
* Cost-conscious.
* Scalable.
* Privacy-focused.
* Suitable for long-term development.

The initial implementation should remain operationally simple while preserving clear boundaries that allow future expansion without rebuilding the entire system.

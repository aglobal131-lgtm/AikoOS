# EVENT SYSTEM

> Version: 1.0
> Status: Draft
> Scope: Client Events, Domain Events, Integration Events, Realtime Events
> Architecture Style: Event-Driven Modular Architecture

---

# 1. Purpose

This document defines the event architecture of AikoOS.

The Event System allows modules to communicate without creating unnecessary direct dependencies.

Instead of one module calling many other modules directly, it publishes an event describing something that has already happened. Interested modules may then react independently.

Example:

```text
Assistant Response Generated
             │
             ├── Memory Engine extracts memories
             ├── Emotion Engine updates emotional state
             ├── Voice Engine prepares speech
             ├── Realtime System notifies the client
             └── Audit System records the event
```

This approach helps AikoOS remain modular, extensible, and easier to maintain.

---

# 2. Event System Goals

The Event System must support:

* Loose coupling between modules.
* Strongly typed event contracts.
* Reliable event handling.
* Realtime client updates.
* Background processing.
* Retry and failure handling.
* Event versioning.
* Event tracing.
* Plugin subscriptions.
* Future distributed deployment.
* Idempotent handlers.
* Clear separation between event types.

---

# 3. Event Categories

AikoOS uses four main event categories.

```text
Domain Events
Application Events
Integration Events
Client Events
```

Each category has a different purpose and lifecycle.

---

# 4. Domain Events

Domain events represent meaningful facts that occurred inside a domain model.

Examples:

```text
MemoryCreated
MemoryDeleted
EmotionChanged
PermissionGranted
PluginInstalled
TaskScheduled
ConversationStarted
```

Domain events should describe something that has already happened.

Correct:

```text
MemoryCreated
```

Incorrect:

```text
CreateMemory
```

Commands request actions.

Events describe completed facts.

---

## 4.1 Domain Event Characteristics

Domain events are:

* Immutable.
* Past tense.
* Produced by domain entities or domain services.
* Scoped to internal business logic.
* Independent of transport technology.
* Independent of databases.
* Independent of WebSocket or HTTP.

Domain events must not contain infrastructure-specific objects.

---

## 4.2 Domain Event Example

```csharp
public sealed record MemoryCreatedDomainEvent(
    Guid MemoryId,
    Guid UserId,
    string MemoryType,
    DateTimeOffset CreatedAt
) : IDomainEvent;
```

---

## 4.3 Domain Event Interface

```csharp
public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
```

A base record may provide common metadata.

```csharp
public abstract record DomainEventBase : IDomainEvent
{
    public DateTimeOffset OccurredAt { get; init; }
        = DateTimeOffset.UtcNow;
}
```

---

# 5. Application Events

Application events describe events relevant to application workflows.

They are not always pure domain events.

Examples:

```text
ConversationProcessingStarted
AIResponseStreamingStarted
EmbeddingGenerationRequested
PluginExecutionQueued
NotificationDeliveryRequested
```

Application events may coordinate:

* Use cases.
* Background jobs.
* Workflow steps.
* Infrastructure work.
* Cross-module application behavior.

---

## 5.1 Application Event Example

```csharp
public sealed record EmbeddingGenerationRequestedEvent(
    Guid MemoryId,
    string Text,
    string EmbeddingModel
) : IApplicationEvent;
```

---

# 6. Integration Events

Integration events cross module or process boundaries.

They may be:

* Stored in an outbox.
* Published to a message broker.
* Delivered to another service.
* Replayed.
* Retried.
* Versioned independently.

Examples:

```text
memory.created.v1
conversation.completed.v1
plugin.execution.completed.v1
task.failed.v1
notification.created.v1
```

Integration events are more stable than internal application events.

---

## 6.1 Integration Event Characteristics

Integration events must:

* Use stable schemas.
* Include version information.
* Be serializable.
* Avoid internal entity types.
* Include correlation metadata.
* Support duplicate delivery.
* Be safe for asynchronous processing.
* Avoid exposing sensitive data unnecessarily.

---

## 6.2 Integration Event Example

```json
{
  "eventId": "46b0fb2d-97a8-4344-9a89-74b818adcc10",
  "eventType": "memory.created",
  "eventVersion": 1,
  "occurredAt": "2026-07-17T10:30:00Z",
  "correlationId": "7da7a4bd-72d7-4c23-8325-6c0363d964e2",
  "causationId": "19c264e6-dc0d-4cbe-9180-3de50a03ca80",
  "payload": {
    "memoryId": "2dde9f8f-a3b1-43f3-a02f-02f6c2213bd1",
    "userId": "edcf4da5-c675-48be-a960-dbcff72ec062",
    "memoryType": "preference"
  }
}
```

---

# 7. Client Events

Client events are sent from the backend to connected clients.

Examples:

```text
assistant.response.chunk
assistant.response.completed
character.state.changed
emotion.state.changed
permission.requested
task.progress.updated
notification.created
server.degraded
```

Client events are transport contracts and must be versioned.

---

## 7.1 Client Event Example

```json
{
  "eventId": "b21fa0ab-c947-41bd-bf49-76a331b78cef",
  "eventType": "character.state.changed",
  "eventVersion": 1,
  "sequence": 214,
  "occurredAt": "2026-07-17T10:32:11Z",
  "payload": {
    "state": "thinking",
    "expression": "curious",
    "priority": 60,
    "durationMs": 0
  }
}
```

---

# 8. Event Naming Convention

Event names should be:

* Clear.
* Stable.
* Past tense where representing completed facts.
* Namespaced by module where useful.
* Versioned for external contracts.

Recommended internal C# names:

```text
ConversationStartedDomainEvent
MemoryCreatedDomainEvent
EmotionChangedDomainEvent
PluginExecutionRequestedEvent
NotificationDeliveryRequestedEvent
```

Recommended external names:

```text
conversation.started
memory.created
emotion.changed
plugin.execution.requested
notification.delivery.requested
```

Versioned external name:

```text
memory.created.v1
```

---

# 9. Event Metadata

Every important event should support common metadata.

Recommended fields:

```text
EventId
EventType
EventVersion
OccurredAt
CorrelationId
CausationId
UserId
DeviceId
ConversationId
SourceModule
TraceId
```

Not every event requires every field.

Sensitive metadata should be minimized.

---

## 9.1 Event Envelope

```csharp
public sealed record EventEnvelope<TPayload>(
    Guid EventId,
    string EventType,
    int EventVersion,
    DateTimeOffset OccurredAt,
    Guid? CorrelationId,
    Guid? CausationId,
    string SourceModule,
    TPayload Payload
);
```

---

# 10. Correlation and Causation

Correlation identifiers connect all events belonging to one workflow.

Example:

```text
User sends message
      │
      ├── UserMessageReceived
      ├── MemoryRetrieved
      ├── AIRequestStarted
      ├── AIResponseGenerated
      ├── EmotionChanged
      └── SpeechRequested
```

All events may share the same `CorrelationId`.

The `CausationId` identifies the direct event or request that caused the current event.

Example:

```text
UserMessageReceived
        │
        └── causes AIRequestStarted
```

`AIRequestStarted.CausationId` should reference `UserMessageReceived.EventId`.

---

# 11. Event Bus Types

AikoOS may use several event buses.

```text
Domain Event Bus
Application Event Bus
Integration Event Bus
Client Realtime Bus
Client Local Event Bus
```

Each serves a different boundary.

---

# 12. Domain Event Bus

The Domain Event Bus operates inside the backend process.

Responsibilities:

* Dispatch domain events.
* Invoke registered handlers.
* Preserve module separation.
* Support transactional coordination.

Initial implementation may be in-process.

Example:

```csharp
public interface IDomainEventDispatcher
{
    Task DispatchAsync(
        IReadOnlyCollection<IDomainEvent> events,
        CancellationToken cancellationToken = default);
}
```

---

# 13. Application Event Bus

The Application Event Bus coordinates use cases and background work.

Example:

```csharp
public interface IApplicationEventPublisher
{
    Task PublishAsync<TEvent>(
        TEvent applicationEvent,
        CancellationToken cancellationToken = default)
        where TEvent : IApplicationEvent;
}
```

Application event handlers may:

* Schedule a job.
* Trigger a notification.
* Request embedding generation.
* Publish a realtime event.
* Request a plugin execution.

---

# 14. Integration Event Bus

The Integration Event Bus communicates across durable boundaries.

Initial implementation may use:

* PostgreSQL Outbox.
* Background dispatcher.
* In-process delivery to registered integration handlers.

Future implementation may use:

* RabbitMQ.
* NATS.
* Azure Service Bus.
* Redis Streams.
* Kafka.

The domain and application layers must not depend on any specific broker.

---

# 15. Client Realtime Bus

The Client Realtime Bus delivers server events to connected clients.

Possible transport:

* ASP.NET Core SignalR.
* Native WebSocket abstraction.

SignalR may be considered because it provides:

* Connection management.
* Automatic reconnect support.
* Groups.
* User targeting.
* Streaming.
* Multiple transport fallbacks.

The architecture must still expose an internal abstraction.

---

# 16. Client Local Event Bus

The desktop client may use a local strongly typed event bus.

Responsibilities:

* Reduce direct service dependencies.
* Notify ViewModels.
* Coordinate audio, animation, and connection state.
* Distribute backend events.

Example:

```csharp
public interface IClientEventBus
{
    IDisposable Subscribe<TEvent>(
        Func<TEvent, Task> handler);

    Task PublishAsync<TEvent>(
        TEvent clientEvent);
}
```

---

# 17. Event Dispatch Flow

Typical backend flow:

```text
Domain Entity Changes State
          │
          ▼
Domain Event Recorded
          │
          ▼
Application Transaction Commits
          │
          ▼
Domain Event Dispatcher
          │
          ├── Internal Handler
          ├── Integration Event Creation
          ├── Background Job Creation
          └── Realtime Event Creation
```

---

# 18. Transactional Consistency

A common problem occurs when:

1. Database transaction succeeds.
2. Event publishing fails.

This can leave other modules unaware of the change.

AikoOS should use the Transactional Outbox pattern for durable integration events.

---

# 19. Transactional Outbox

The Outbox pattern stores an event in the same database transaction as the business change.

```text
Business Change
      │
      ├── Update Domain Table
      └── Insert Outbox Event
              │
              ▼
        Commit Transaction
              │
              ▼
        Outbox Dispatcher
              │
              ▼
        Publish Event
```

This ensures that business data and event intent remain consistent.

---

## 19.1 Outbox Table

Suggested fields:

```text
Id
EventType
EventVersion
Payload
OccurredAt
CorrelationId
CausationId
ProcessingStatus
AttemptCount
NextAttemptAt
ProcessedAt
LastError
CreatedAt
```

---

## 19.2 Outbox Processing States

```text
Pending
Processing
Completed
Failed
DeadLettered
```

---

# 20. Inbox Pattern

When events may be delivered more than once, consumers should use an Inbox pattern.

The Inbox records processed event identifiers.

Suggested fields:

```text
EventId
ConsumerName
ProcessedAt
Result
```

Before handling an event:

1. Check whether the event was already processed.
2. Skip duplicate events.
3. Process new event.
4. Store completion record.

---

# 21. Idempotency

Event handlers must be idempotent whenever duplicate delivery is possible.

Example unsafe handler:

```text
Increment friendship score by 5
```

If delivered twice, the result becomes incorrect.

Safer approaches:

* Use event identifiers.
* Store a processed-event record.
* Use deterministic state calculation.
* Apply unique constraints.
* Use version checking.

---

# 22. Event Ordering

Some workflows require ordered events.

Examples:

```text
assistant.response.started
assistant.response.chunk
assistant.response.completed
```

These events should include:

* Stream identifier.
* Sequence number.
* Timestamp.
* Conversation identifier.

Example:

```json
{
  "streamId": "bd731e15-baf2-4aa3-9705-899570d421b5",
  "sequence": 4,
  "eventType": "assistant.response.chunk",
  "payload": {
    "text": "I'm still here."
  }
}
```

---

# 23. Ordering Guarantees

AikoOS should not assume global event ordering.

Ordering may only be guaranteed within a defined scope.

Possible scopes:

* One conversation.
* One response stream.
* One task.
* One device session.
* One aggregate.

Handlers must tolerate unrelated events arriving in different orders.

---

# 24. Event Priority

Some client and application events require priority.

Suggested priority levels:

```text
0   Background
20  Low
40  Normal
60  High
80  Urgent
100 Critical
```

Examples:

```text
Idle animation: 10
Emotion reaction: 30
Thinking animation: 50
Speaking animation: 70
Permission request: 90
Critical security warning: 100
```

Priority should not replace correct domain rules.

---

# 25. Event Expiration

Some events become irrelevant after a short time.

Examples:

* Animation hints.
* Temporary subtitles.
* Typing indicators.
* Presence events.
* Task progress updates.

These events may include:

```text
ExpiresAt
TimeToLive
```

Expired ephemeral events should not be replayed.

---

# 26. Durable and Ephemeral Events

## Durable Events

Must survive restarts.

Examples:

* Memory created.
* Permission changed.
* Task scheduled.
* Plugin installed.
* Conversation completed.

## Ephemeral Events

Relevant only for current realtime behavior.

Examples:

* Lip-sync amplitude.
* Cursor-look direction.
* Temporary animation hint.
* Streaming token chunk.
* Live microphone level.

Ephemeral events should not be stored permanently unless diagnostic mode explicitly requires it.

---

# 27. Synchronous Event Handling

Synchronous handlers are appropriate when:

* The operation must complete within the current use case.
* Failure should fail the transaction.
* The handler is fast.
* The operation is deterministic.
* No external network call is needed.

Example:

```text
Validate emotion transition
Update aggregate projection
Enforce domain invariant
```

---

# 28. Asynchronous Event Handling

Asynchronous handlers are appropriate when:

* Work may take time.
* External services are involved.
* Retry is required.
* User response should not wait.
* Work can be processed later.

Examples:

* Generate embeddings.
* Summarize a conversation.
* Deliver notification.
* Run plugin maintenance.
* Export user data.

---

# 29. Handler Isolation

One event handler failure should not automatically prevent unrelated handlers from running unless the workflow requires atomic behavior.

Example:

```text
AssistantResponseGenerated
    ├── Save message — critical
    ├── Update emotion — important
    ├── Generate memory candidates — retryable
    └── Send analytics — optional
```

Each handler should define its failure impact.

---

# 30. Handler Failure Policies

Possible policies:

```text
FailTransaction
Retry
IgnoreAfterLogging
MoveToDeadLetter
DisableConsumer
NotifyAdministrator
NotifyUser
```

The selected policy depends on event criticality.

---

# 31. Retry Strategy

Retryable event failures should use exponential backoff.

Example schedule:

```text
Attempt 1: immediate
Attempt 2: 5 seconds
Attempt 3: 30 seconds
Attempt 4: 2 minutes
Attempt 5: 10 minutes
```

Retries should include random jitter to avoid synchronized retry storms.

---

# 32. Non-Retryable Failures

Examples:

* Invalid schema.
* Missing required permission.
* Unsupported event version.
* Deleted resource.
* Invalid plugin manifest.
* Permanent authorization failure.

These should not be retried automatically.

---

# 33. Dead-Letter Events

Events that repeatedly fail should enter a dead-letter state.

Dead-letter data should include:

```text
Original Event
Consumer
Failure Reason
Attempt Count
First Failure Time
Last Failure Time
Stack Trace Reference
Correlation ID
```

Dead-letter events should be inspectable and replayable through administrative tooling.

---

# 34. Event Versioning

External events must have explicit versions.

Example:

```text
memory.created.v1
memory.created.v2
```

A new version is required when:

* A required field changes.
* Field semantics change.
* A field is removed.
* Payload structure becomes incompatible.
* Behavior changes materially.

Adding optional fields may remain backward compatible.

---

# 35. Event Schema Compatibility

Consumers should:

* Ignore unknown optional fields.
* Validate required fields.
* Reject unsupported major versions.
* Avoid depending on property order.
* Avoid using database entity serialization.

Schemas should be documented.

---

# 36. Event Serialization

Recommended serialization format:

```text
JSON
```

Future high-volume internal transports may use another format.

Requirements:

* UTF-8.
* Stable field naming.
* ISO 8601 timestamps.
* Explicit enum representation.
* No polymorphic unsafe deserialization.
* No sensitive data unless necessary.
* Deterministic schema where practical.

---

# 37. Event Security

Events may contain sensitive information.

Security rules:

* Minimize payload content.
* Avoid provider API keys.
* Avoid raw authentication tokens.
* Avoid unnecessary full conversation text.
* Encrypt transport.
* Authenticate consumers.
* Apply authorization before client delivery.
* Redact sensitive logs.
* Scope client events by user and device.

---

# 38. Event Authorization

Not every connected client may receive every event.

Delivery may depend on:

* User identity.
* Device identity.
* Device capabilities.
* Conversation ownership.
* Permission state.
* Character identity.
* Session state.

Example:

A screen-capture permission request should only be delivered to the device requesting screen access.

---

# 39. Event Routing

Events may be routed by:

```text
User
Device
Conversation
Character
Plugin
Task
Module
Capability
Session
```

Example realtime groups:

```text
user:{userId}
device:{deviceId}
conversation:{conversationId}
character:{characterId}
task:{taskId}
```

---

# 40. Subscription Management

Clients and plugins may subscribe only to allowed event categories.

Subscription lifecycle:

```text
Register
Authenticate
Authorize
Subscribe
Receive
Acknowledge when required
Unsubscribe
Disconnect
```

Subscriptions should be cleaned up automatically after disconnection.

---

# 41. Plugin Events

Plugins may:

* Subscribe to approved events.
* Publish approved plugin events.
* Request system actions through APIs.

Plugins must not subscribe to every internal event by default.

Example plugin manifest:

```json
{
  "id": "aikoos.weather",
  "subscriptions": [
    "conversation.intent.detected",
    "schedule.triggered"
  ],
  "publishes": [
    "weather.forecast.received"
  ]
}
```

---

# 42. Plugin Event Restrictions

Plugins must not receive:

* Raw secrets.
* Unrelated private memories.
* Internal authentication events.
* Unfiltered system logs.
* Private files without permission.
* Events outside declared capabilities.

Plugin event access should be validated during installation and runtime.

---

# 43. Client Event Handling

The desktop client receives a server event and routes it internally.

```text
WebSocket Event
      │
      ▼
Realtime Client
      │
      ▼
Event Deserializer
      │
      ▼
Event Validator
      │
      ▼
Client Event Bus
      │
      ├── Character ViewModel
      ├── Animation Controller
      ├── Audio Service
      └── Notification Service
```

Unknown event types should be logged and ignored safely.

---

# 44. Reconnection and Missed Events

Realtime connections may disconnect.

After reconnection, the client may request:

* Current conversation state.
* Current task states.
* Current character state.
* Pending permission requests.
* Missed durable notifications.

Ephemeral events such as old lip-sync updates should not be replayed.

---

# 45. Realtime Event Acknowledgement

Not every event requires acknowledgement.

Acknowledgement may be required for:

* Permission dialog displayed.
* Critical notification received.
* Audio playback completed.
* Client action completed.
* File transfer completed.

Acknowledgements should include:

```text
EventId
DeviceId
Status
ReceivedAt
CompletedAt
ErrorCode
```

---

# 46. Event Replay

Durable events may support replay for:

* Projection rebuilding.
* Debugging.
* Recovery.
* Testing.
* Data migration.

Replay must be controlled carefully.

Handlers should know whether they are processing:

* A live event.
* A replayed event.
* A manually retried event.

External side effects should not occur accidentally during replay.

---

# 47. Event Store

AikoOS does not require full event sourcing initially.

The relational database remains the source of truth.

However, selected event history may be stored for:

* Audit.
* Debugging.
* Workflow history.
* Task history.
* Realtime recovery.
* Projection rebuilding.

A dedicated event store should only be introduced when actual requirements justify it.

---

# 48. Event Observability

Every important event should be traceable.

Monitoring should include:

* Events published.
* Events processed.
* Handler duration.
* Handler failures.
* Retry count.
* Dead-letter count.
* Outbox backlog.
* Client delivery failures.
* Duplicate events.
* Unsupported versions.

---

# 49. Event Metrics

Recommended metrics:

```text
event_published_total
event_processed_total
event_failed_total
event_retry_total
event_dead_letter_total
event_handler_duration
outbox_pending_count
outbox_oldest_age
realtime_event_delivery_total
realtime_delivery_failure_total
```

Metrics may be labeled by event type and consumer, but high-cardinality identifiers should be avoided.

---

# 50. Event Logging

Event logs should include:

```text
EventId
EventType
EventVersion
SourceModule
Consumer
CorrelationId
CausationId
ProcessingResult
Duration
Attempt
ErrorCode
```

Do not log full sensitive payloads by default.

---

# 51. Common Event Flows

## 51.1 Conversation Flow

```text
UserMessageReceived
        │
        ├── ConversationStateUpdated
        ├── MemoryRetrievalRequested
        └── AIResponseRequested
                │
                ▼
        AIResponseStreamingStarted
                │
                ├── AIResponseChunkGenerated
                └── AIResponseCompleted
                         │
                         ├── AssistantMessageStored
                         ├── MemoryExtractionRequested
                         ├── EmotionEvaluationRequested
                         └── SpeechGenerationRequested
```

---

## 51.2 Permission Flow

```text
SensitiveActionRequested
          │
          ▼
PermissionEvaluationStarted
          │
          ├── Already Allowed
          │       └── ActionAuthorized
          │
          ├── Denied
          │       └── ActionRejected
          │
          └── User Confirmation Required
                  │
                  ▼
           PermissionRequested
                  │
                  ▼
           Client Displays Dialog
                  │
                  ▼
           PermissionDecisionReceived
                  │
          ┌───────┴────────┐
          ▼                ▼
    PermissionGranted  PermissionDenied
```

---

## 51.3 Memory Flow

```text
ConversationCompleted
         │
         ▼
MemoryExtractionRequested
         │
         ▼
MemoryCandidatesGenerated
         │
         ├── DuplicateDetected
         ├── CandidateRejected
         └── MemoryApproved
                  │
                  ▼
            MemoryCreated
                  │
                  ├── EmbeddingRequested
                  ├── MemoryIndexUpdated
                  └── UserMemoryViewUpdated
```

---

## 51.4 Task Flow

```text
TaskCreated
    │
    ▼
TaskQueued
    │
    ▼
TaskStarted
    │
    ├── TaskProgressUpdated
    ├── TaskCompleted
    ├── TaskFailed
    └── TaskCancelled
```

---

## 51.5 Plugin Flow

```text
PluginExecutionRequested
          │
          ▼
PluginPermissionValidated
          │
          ▼
PluginExecutionStarted
          │
          ├── PluginExecutionCompleted
          ├── PluginExecutionFailed
          └── PluginExecutionTimedOut
```

---

# 52. Event Interface Examples

```csharp
public interface IEvent
{
    Guid EventId { get; }

    DateTimeOffset OccurredAt { get; }

    Guid? CorrelationId { get; }

    Guid? CausationId { get; }
}
```

Domain event:

```csharp
public interface IDomainEvent : IEvent
{
}
```

Application event:

```csharp
public interface IApplicationEvent : IEvent
{
}
```

Integration event:

```csharp
public interface IIntegrationEvent : IEvent
{
    int EventVersion { get; }
}
```

---

# 53. Event Handler Example

```csharp
public interface IEventHandler<in TEvent>
    where TEvent : IEvent
{
    Task HandleAsync(
        TEvent eventMessage,
        CancellationToken cancellationToken = default);
}
```

Example handler:

```csharp
public sealed class GenerateEmbeddingWhenMemoryCreatedHandler
    : IEventHandler<MemoryCreatedDomainEvent>
{
    private readonly IApplicationEventPublisher _publisher;

    public GenerateEmbeddingWhenMemoryCreatedHandler(
        IApplicationEventPublisher publisher)
    {
        _publisher = publisher;
    }

    public Task HandleAsync(
        MemoryCreatedDomainEvent eventMessage,
        CancellationToken cancellationToken = default)
    {
        return _publisher.PublishAsync(
            new EmbeddingGenerationRequestedEvent(
                eventMessage.MemoryId),
            cancellationToken);
    }
}
```

---

# 54. Handler Registration

Handlers should be registered automatically through controlled assembly scanning or explicit registration.

Rules:

* Duplicate handlers are allowed when intentional.
* Handler ordering must not be assumed unless configured.
* Critical synchronous handlers should be explicit.
* Plugin handlers require separate registration controls.
* Invalid handlers should fail startup or plugin activation clearly.

---

# 55. Event Handler Timeouts

Handlers that perform external work require timeouts.

Examples:

```text
Internal state update: short timeout
AI provider call: provider-specific timeout
Plugin execution: plugin policy timeout
Notification delivery: delivery timeout
```

Timeouts should generate structured failure events where appropriate.

---

# 56. Event Cancellation

Cancellation tokens should be supported for:

* Application shutdown.
* User cancellation.
* Request cancellation.
* Task cancellation.
* Plugin timeout.
* Conversation interruption.

Cancellation should not leave durable state ambiguous.

---

# 57. Event Testing

## Unit Tests

Test:

* Event creation.
* Event naming.
* Handler behavior.
* Idempotency.
* Validation.
* Failure policy.
* Priority resolution.

## Integration Tests

Test:

* Outbox persistence.
* Outbox dispatch.
* Inbox duplicate detection.
* Realtime event delivery.
* Retry.
* Dead-letter behavior.
* Client reconnection.
* Event schema compatibility.

## Contract Tests

Test:

* Serialization.
* Required fields.
* Version compatibility.
* Unknown optional fields.
* Enum compatibility.
* Client-server event contracts.

---

# 58. Event Documentation

Every external event should document:

```text
Event Name
Version
Purpose
Producer
Consumers
Delivery Type
Durability
Ordering Scope
Payload
Permissions
Retry Behavior
Expiration
Examples
```

Internal events may use lighter documentation but should still have clear ownership.

---

# 59. Prohibited Practices

The following practices are prohibited unless approved through an architectural decision:

* Publishing anonymous dictionary payloads.
* Using unversioned external events.
* Passing database entities as event payloads.
* Assuming exactly-once delivery.
* Depending on global event ordering.
* Logging full sensitive payloads.
* Giving plugins unrestricted subscriptions.
* Performing unlimited retries.
* Blocking the UI while waiting for noncritical events.
* Using events when a direct function call is clearer.
* Creating events with unclear ownership.
* Using future-tense event names for completed facts.
* Hiding critical failures silently.
* Replaying side-effecting events without safeguards.
* Treating ephemeral events as durable business records.

---

# 60. When Not to Use Events

Events should not replace every direct interaction.

Use a direct call when:

* A result is immediately required.
* One module explicitly depends on a defined service interface.
* The workflow must be synchronous.
* Only one consumer exists.
* Failure must be returned directly.
* Event indirection would make behavior harder to understand.

Example:

```text
MemoryService.SearchAsync()
```

This is clearer than publishing `MemorySearchRequested` when the caller needs immediate search results.

---

# 61. Decision Guide

Use a domain event when:

* A meaningful domain fact occurred.
* Multiple internal reactions may exist.
* The domain should remain independent.

Use an application event when:

* A workflow step should trigger additional work.
* Background processing is appropriate.

Use an integration event when:

* The event crosses a durable module or process boundary.
* Retry and replay matter.

Use a client event when:

* A connected device must react in realtime.

Use a direct service call when:

* A result is required immediately.
* The dependency is explicit and stable.

---

# 62. Initial Implementation Strategy

The first implementation should use:

```text
In-process domain event dispatcher
In-process application event bus
PostgreSQL outbox
Background outbox dispatcher
SignalR or WebSocket realtime delivery
Client-side local event bus
```

A dedicated external message broker is not required initially.

This keeps operational cost and complexity low while preserving future scalability.

---

# 63. Future Evolution

The Event System may evolve through these stages.

## Stage 1

* In-process internal events.
* PostgreSQL outbox.
* Realtime delivery.

## Stage 2

* Separate background worker.
* Shared outbox dispatcher.
* More durable event consumers.

## Stage 3

* External broker for selected integration events.
* Distributed consumers.
* Independent plugin execution service.

## Stage 4

* Multi-region or large-scale event infrastructure only when justified.

AikoOS must not adopt distributed messaging complexity before it is needed.

---

# 64. Definition of Done

The initial Event System is complete when:

* Domain events can be recorded and dispatched.
* Application events can be published.
* Event handlers are strongly typed.
* Correlation and causation metadata exist.
* PostgreSQL outbox is implemented.
* Outbox retry works.
* Duplicate event handling is supported.
* Dead-letter state is supported.
* Realtime client events are delivered.
* Reconnection behavior is documented.
* Event schemas are versioned.
* Sensitive payload rules are enforced.
* Unit and integration tests exist.
* Event documentation matches implementation.

---

# 65. Summary

The AikoOS Event System is the communication backbone connecting independent modules, background workflows, plugins, and clients.

It must remain:

* Strongly typed.
* Traceable.
* Versioned.
* Secure.
* Idempotent.
* Retry-aware.
* Modular.
* Operationally simple at first.
* Ready for future distribution.

Events should reduce coupling, not hide architecture.

Whenever event-driven communication makes a workflow harder to understand, a direct interface should be preferred.

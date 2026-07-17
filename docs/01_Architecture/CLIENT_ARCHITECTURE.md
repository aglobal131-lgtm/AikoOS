# CLIENT ARCHITECTURE

> Version: 1.0
> Status: Draft
> Target Platform: Windows 10 and Windows 11
> Framework: WPF on .NET

---

# 1. Purpose

This document defines the architecture of the AikoOS desktop client.

The desktop client is the visible body of AikoOS. It renders the character, receives user input, plays audio, interacts with Windows, and displays application interfaces.

The client must remain lightweight and must not contain the main intelligence, memory logic, AI routing, or persistent business rules of AikoOS.

---

# 2. Client Role

The client acts as a presentation and device-integration layer.

Its primary responsibilities are:

* Render the companion character.
* Display animation and expressions.
* Capture microphone input.
* Play synthesized voice.
* Receive mouse and keyboard interaction.
* Integrate with the Windows desktop.
* Communicate with the backend.
* Display notifications and settings.
* Maintain temporary UI state.
* Recover gracefully from backend disconnection.

The client must not directly:

* Call AI providers.
* Store long-term memories.
* Access PostgreSQL.
* Execute server-side plugins.
* Build final AI prompts.
* Make permanent personality decisions.
* Store provider API keys.
* Perform unrestricted operating-system actions.

---

# 3. Architectural Goals

The client architecture must support:

* Clear separation between UI and logic.
* Replaceable character rendering systems.
* Multiple animation states.
* Real-time communication with the backend.
* Stable behavior when the server is unavailable.
* Low CPU and memory usage.
* Windows desktop integration.
* Testable ViewModels and services.
* Future support for multiple characters.
* Future migration to another client framework if required.

---

# 4. Recommended Pattern

The desktop client uses the Model-View-ViewModel pattern.

```text
View
  │
  ▼
ViewModel
  │
  ▼
Client Services
  │
  ▼
Backend API / Windows APIs / Local Runtime
```

## 4.1 View

The View contains:

* XAML layout.
* Visual controls.
* Character surface.
* Settings interface.
* Notification interface.
* Debug interface.

The View must not contain application rules.

Code-behind should be limited to UI-specific behavior that cannot be expressed cleanly through bindings, commands, or behaviors.

---

## 4.2 ViewModel

The ViewModel contains presentation state.

Examples:

* Current character state.
* Connection status.
* Current subtitle.
* Current expression.
* Whether the microphone is active.
* Whether Aiko is speaking.
* Settings page state.
* Current notification.

The ViewModel communicates through abstractions and must not directly use HTTP clients, Windows APIs, or database code.

---

## 4.3 Model

Client-side models represent data exchanged with the backend or used temporarily by the UI.

Examples:

* Chat response.
* Character state.
* Animation command.
* Voice playback request.
* Backend status.
* Client settings.
* Plugin display metadata.

Client models must not be confused with server domain entities.

---

# 5. High-Level Client Architecture

```text
┌─────────────────────────────────────────────┐
│                 WPF Views                   │
│                                             │
│ Character Window │ Settings │ Notifications │
└───────────────────────┬─────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────┐
│                 ViewModels                  │
│                                             │
│ Character │ Voice │ Settings │ Connection   │
└───────────────────────┬─────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────┐
│              Client Services                │
│                                             │
│ API Client                                  │
│ Realtime Client                             │
│ Animation Controller                        │
│ Audio Service                               │
│ Input Service                               │
│ Window Service                              │
│ Tray Service                                │
│ Local Configuration Service                 │
└───────────────────────┬─────────────────────┘
                        │
          ┌─────────────┼─────────────┐
          ▼             ▼             ▼
      Backend API   Windows APIs   Local Files
```

---

# 6. Client Projects

The client solution should be divided into projects with clear responsibilities.

Recommended structure:

```text
client/
├── AikoOS.Client.App/
├── AikoOS.Client.Core/
├── AikoOS.Client.Infrastructure/
├── AikoOS.Client.Animation/
├── AikoOS.Client.Audio/
├── AikoOS.Client.Windows/
└── AikoOS.Client.Tests/
```

---

## 6.1 AikoOS.Client.App

Contains the executable WPF application.

Responsibilities:

* Application startup.
* Dependency injection setup.
* View registration.
* ViewModel registration.
* Resource dictionaries.
* Themes.
* Main windows.
* Application lifecycle.
* Global exception handling.

This project should reference the remaining client modules but should contain minimal logic.

---

## 6.2 AikoOS.Client.Core

Contains client-side abstractions and presentation models.

Responsibilities:

* Service interfaces.
* ViewModel base classes.
* Commands.
* Client events.
* DTOs used by presentation.
* Client state abstractions.

This project should not depend on WPF-specific infrastructure where avoidable.

---

## 6.3 AikoOS.Client.Infrastructure

Contains backend communication and local infrastructure.

Responsibilities:

* REST API client.
* WebSocket client.
* Serialization.
* Authentication token handling.
* Retry policies.
* Connection monitoring.
* Local cache.
* Local configuration persistence.

---

## 6.4 AikoOS.Client.Animation

Contains character rendering and animation coordination.

Responsibilities:

* Live2D integration.
* Animation state management.
* Expression changes.
* Lip synchronization input.
* Movement interpolation.
* Idle behavior playback.
* Character asset loading.

The rest of the client should access this module through interfaces.

---

## 6.5 AikoOS.Client.Audio

Contains audio input and playback.

Responsibilities:

* Microphone capture.
* Voice activity state.
* Audio playback.
* Playback queue.
* Volume control.
* Device selection.
* Audio interruption.
* Lip-sync amplitude generation.

Speech recognition and speech synthesis may run locally or remotely, but the audio module only manages device-level input and output.

---

## 6.6 AikoOS.Client.Windows

Contains Windows-specific integration.

Responsibilities:

* Transparent windows.
* Always-on-top behavior.
* Click-through mode.
* System tray.
* Global shortcuts.
* Monitor detection.
* Screen boundary detection.
* Startup registration.
* Window snapping.
* Desktop notifications.
* Safe process launching through approved commands.

---

## 6.7 AikoOS.Client.Tests

Contains:

* ViewModel tests.
* State transition tests.
* API client tests.
* Animation controller tests.
* Audio queue tests.
* Configuration tests.
* Connection recovery tests.

---

# 7. Main Windows

The client may contain several windows, but only the character window should remain visible during normal use.

---

## 7.1 Character Window

The Character Window renders Aiko on the desktop.

Requirements:

* Transparent background.
* Borderless.
* Optional always-on-top mode.
* Movable by dragging the character.
* Position persistence.
* Multi-monitor support.
* Dynamic sizing.
* Character hit testing.
* Optional click-through mode.
* Low resource usage.
* No taskbar entry unless configured.

The window must separate transparent background regions from interactive character regions.

---

## 7.2 Conversation Overlay

Displays temporary text related to conversation.

Possible content:

* User transcription.
* Aiko response.
* Listening state.
* Thinking state.
* Error message.
* Suggested actions.

The overlay should disappear automatically and must not obstruct normal desktop use.

---

## 7.3 Settings Window

Provides configuration for:

* Character.
* Voice.
* Microphone.
* Speaker.
* Backend address.
* Startup behavior.
* Privacy.
* Notifications.
* Performance.
* Shortcuts.
* Animation.
* Accessibility.

Settings should be grouped by module.

---

## 7.4 Permission Dialog

Displays explicit confirmation when an action requires user approval.

Examples:

* Capturing the screen.
* Accessing the webcam.
* Opening a program.
* Reading a directory.
* Running a command.
* Using a new plugin permission.

Permission dialogs must clearly state:

* What action will occur.
* Which component requested it.
* Why it is needed.
* Whether permission is temporary or persistent.

---

## 7.5 Debug Window

Available only in development or diagnostic mode.

Displays:

* Backend connection.
* Current animation state.
* Current emotion state.
* WebSocket events.
* Audio device state.
* Performance metrics.
* Recent errors.
* Active tasks.

The debug window must not expose secrets.

---

# 8. Application Lifecycle

The client lifecycle consists of the following stages.

```text
Application Start
      │
      ▼
Load Local Configuration
      │
      ▼
Initialize Dependency Injection
      │
      ▼
Initialize Logging
      │
      ▼
Initialize Character Runtime
      │
      ▼
Connect to Backend
      │
      ▼
Open Character Window
      │
      ▼
Start Realtime Event Listener
      │
      ▼
Ready
```

---

## 8.1 Startup

During startup, the client should:

1. Validate the runtime environment.
2. Load safe local configuration.
3. Initialize logging.
4. Restore window position.
5. Detect audio devices.
6. Load the selected character.
7. connect to the backend.
8. Start the realtime channel.
9. Enter the idle state.

A failure in one optional subsystem must not terminate the entire client.

For example, if the backend is unavailable, the character may still load in offline mode.

---

## 8.2 Shutdown

During shutdown, the client should:

1. Stop microphone capture.
2. Cancel pending client tasks.
3. Stop audio playback.
4. Save window position.
5. Close the realtime connection.
6. Dispose of rendering resources.
7. Flush logs.
8. Exit cleanly.

The client must avoid forcefully terminating background operations unless required.

---

## 8.3 Suspend and Resume

The client should detect:

* Windows sleep.
* User lock.
* Session resume.
* Display changes.
* Audio device changes.

On resume, it should:

* Reconnect to the backend.
* Restore audio devices.
* Revalidate the character window position.
* Restart realtime subscriptions.
* Return to an appropriate animation state.

---

# 9. Dependency Injection

All major client services should be registered through dependency injection.

Example abstractions:

```csharp
public interface IBackendApiClient
{
    Task<HealthStatus> GetHealthAsync(
        CancellationToken cancellationToken = default);
}

public interface IRealtimeClient
{
    Task ConnectAsync(
        CancellationToken cancellationToken = default);

    Task DisconnectAsync(
        CancellationToken cancellationToken = default);
}

public interface IAnimationController
{
    Task PlayStateAsync(
        CharacterState state,
        CancellationToken cancellationToken = default);
}

public interface IAudioPlaybackService
{
    Task PlayAsync(
        AudioPlaybackRequest request,
        CancellationToken cancellationToken = default);

    Task StopAsync();
}
```

Views and ViewModels must depend on interfaces rather than concrete infrastructure classes.

---

# 10. State Management

The client maintains temporary runtime state.

Examples:

* Backend connection state.
* Character position.
* Current animation.
* Current expression.
* Current subtitle.
* Microphone activity.
* Audio playback state.
* Current user interaction.
* Active notification.

Persistent personal data must remain on the backend unless explicitly classified as local configuration.

---

## 10.1 Application State

Recommended top-level states:

```text
Starting
Connecting
Ready
Listening
Processing
Speaking
Offline
Recovering
ShuttingDown
Error
```

These states describe the client runtime, not the character's emotional state.

---

## 10.2 Character State

Character states may include:

```text
Idle
Walking
Running
Sitting
Sleeping
Listening
Thinking
Speaking
Happy
Sad
Surprised
Concerned
Busy
```

The animation module maps logical character states to actual Live2D motions and expressions.

---

## 10.3 State Ownership

Each state must have one authoritative owner.

Examples:

* Backend connection state: Connection Service.
* Audio playback state: Audio Service.
* Character animation state: Animation Controller.
* Window position: Window Service.
* Conversation state: Conversation ViewModel.

Multiple services must not independently mutate the same state.

---

# 11. Backend Communication

The client communicates with the server through REST and WebSocket.

---

## 11.1 REST

REST is used for request-response operations.

Examples:

* Send a chat message.
* Retrieve settings.
* Update configuration.
* Request memory data.
* Submit permission decisions.
* Retrieve plugin metadata.
* Check system health.

---

## 11.2 WebSocket

WebSocket is used for realtime events.

Examples:

* Streaming AI response.
* Animation command.
* Emotion update.
* Task progress.
* Notification.
* TTS playback metadata.
* Server status.
* Plugin event.
* Permission request.

---

## 11.3 Connection Recovery

The client must handle temporary backend failures.

Recommended strategy:

1. Detect disconnection.
2. Enter offline or recovering state.
3. Retry with exponential backoff.
4. Show a non-intrusive status indicator.
5. Restore subscriptions after reconnection.
6. Request missed critical state when necessary.

The client must not create an infinite aggressive reconnect loop.

---

## 11.4 Request Correlation

Every important client-server operation should contain a correlation identifier.

Example:

```json
{
  "requestId": "3e3bbf9e-8f81-4dad-96ab-601cfb962a41",
  "type": "conversation.message",
  "payload": {
    "text": "Hello, Aiko."
  }
}
```

This identifier helps correlate:

* Client logs.
* Server logs.
* AI requests.
* Plugin execution.
* Error responses.

---

# 12. Local Configuration

Only device-specific and non-sensitive configuration should be stored locally.

Examples:

* Window position.
* Selected monitor.
* Local volume.
* Microphone device identifier.
* Speaker device identifier.
* Animation quality.
* Backend address.
* Start-with-Windows setting.
* Click-through preference.
* Debug mode.

Do not store:

* AI provider keys.
* Database credentials.
* Long-term memories.
* Sensitive user profile data.
* Raw authentication secrets without protection.

---

## 12.1 Recommended Storage

Local configuration may use:

* JSON files.
* Windows application data directory.
* Windows Credential Manager for protected tokens.

Recommended path:

```text
%LOCALAPPDATA%/AikoOS/
```

Example:

```text
AikoOS/
├── config/
│   └── client-settings.json
├── cache/
├── logs/
├── characters/
└── temp/
```

---

# 13. Character Rendering

The rendering system must remain isolated from the rest of the client.

The application communicates using logical commands.

Example:

```text
Play animation: Listening
Set expression: Curious
Set lip sync: 0.72
Move to: X=1200, Y=700
Look at cursor: Enabled
```

The application must not directly reference specific Live2D motion file names outside the animation module.

---

## 13.1 Character Manifest

Each character package should include a manifest.

Example:

```json
{
  "id": "aiko.default",
  "name": "Aiko",
  "version": "1.0.0",
  "renderer": "live2d",
  "model": "model/aiko.model3.json",
  "defaultState": "idle",
  "states": {
    "idle": ["motions/idle_01.motion3.json"],
    "listening": ["motions/listening.motion3.json"],
    "thinking": ["motions/thinking.motion3.json"],
    "speaking": ["motions/talk.motion3.json"]
  },
  "expressions": {
    "happy": "expressions/happy.exp3.json",
    "sad": "expressions/sad.exp3.json",
    "curious": "expressions/curious.exp3.json"
  }
}
```

This allows characters to be replaced without changing application logic.

---

# 14. Animation Coordination

The Animation Controller receives requests from multiple sources.

Possible sources:

* Conversation state.
* Emotion state.
* User interaction.
* Idle behavior.
* Plugin event.
* System notification.

The controller must resolve priority conflicts.

Example priority:

```text
Critical System State
Permission Request
Speaking
Listening
Thinking
Direct User Interaction
Emotion Reaction
Idle Behavior
```

A low-priority idle animation must not interrupt speaking.

---

# 15. Audio Architecture

The client handles microphone and speaker devices.

```text
Microphone
    │
    ▼
Audio Capture
    │
    ▼
Wake Word / Speech Pipeline
    │
    ▼
Backend or Local STT
```

Playback pipeline:

```text
TTS Audio
    │
    ▼
Playback Queue
    │
    ├── Lip Sync
    └── Speaker Output
```

---

## 15.1 Audio Queue

The playback queue should support:

* Sequential playback.
* Priority playback.
* Cancellation.
* User interruption.
* Volume normalization.
* Lip-sync metadata.
* Playback completion events.

---

## 15.2 Voice Interruption

When the user interrupts Aiko:

1. Stop or fade current speech.
2. Stop the speaking animation.
3. Enter the listening state.
4. Notify the backend that the previous response was interrupted.
5. Preserve conversation consistency.

---

# 16. Windows Integration

The Windows integration module provides safe operating-system access.

Features may include:

* Start with Windows.
* System tray.
* Global shortcut.
* Monitor information.
* Cursor tracking.
* Active-window metadata.
* Desktop bounds.
* Notifications.
* Approved application launching.
* Screen capture after permission.

All sensitive Windows operations must pass through the permission system.

---

# 17. Multi-Monitor Support

The client must support multiple monitors from the beginning.

Requirements:

* Remember the selected monitor.
* Detect monitor removal.
* Move the character back into visible bounds.
* Support different display scaling.
* Persist normalized position.
* Avoid placing the character outside the work area.
* Handle resolution changes.

Character location should not be stored only as absolute pixels when a normalized representation is more reliable.

---

# 18. DPI and Scaling

The client must support different Windows scaling settings.

Examples:

* 100%
* 125%
* 150%
* 200%

Requirements:

* Per-monitor DPI awareness.
* Correct character size.
* Correct hit testing.
* Correct window positioning.
* No blurred settings UI.
* No unexpected resize when moving between monitors.

---

# 19. Offline Behavior

The client should retain limited behavior without the backend.

Available offline features may include:

* Render the character.
* Play idle animations.
* Respond to clicks with local animations.
* Open settings.
* Display connection status.
* Retry backend connection.
* Use local wake word detection.
* Play predefined local responses.

Unavailable offline features may include:

* AI conversation.
* Long-term memory access.
* Server plugins.
* Cloud synchronization.
* Server-controlled automation.

The client must clearly distinguish offline behavior from full AI functionality.

---

# 20. Local Cache

The client may cache temporary data to improve startup and resilience.

Examples:

* Character assets.
* Last known non-sensitive settings.
* Recently used UI metadata.
* Server capability information.
* Temporary TTS audio.
* Plugin icons.

Cache rules:

* Cache must be disposable.
* Cache must not become the source of truth.
* Sensitive data must not be stored unencrypted.
* Temporary audio must have a cleanup policy.

---

# 21. Security Boundaries

The desktop client runs on the user's device, but it must still be treated as an untrusted boundary from the server's perspective.

The client must not receive:

* Database credentials.
* Provider secrets.
* Internal service credentials.
* Unrestricted plugin secrets.
* Server encryption keys.

Authentication tokens should:

* Have limited lifetime.
* Be revocable.
* Be securely stored.
* Use secure transport.
* Contain minimal permissions.

---

# 22. Error Handling

Client errors should be classified.

```text
NetworkError
AuthenticationError
ConfigurationError
AudioDeviceError
CharacterLoadError
AnimationError
PermissionError
BackendError
UnexpectedClientError
```

Every error should include:

* Error code.
* Human-readable message.
* Technical details for logs.
* Correlation ID when applicable.
* Whether retry is possible.
* Suggested recovery action.

Technical stack traces must not be shown directly to normal users.

---

# 23. Logging

Client logging should include:

* Startup.
* Shutdown.
* Backend connection.
* Reconnect attempts.
* API failures.
* WebSocket events.
* Audio device changes.
* Character loading.
* Animation failures.
* Permission requests.
* Unexpected exceptions.

Logs must exclude:

* API keys.
* Raw access tokens.
* Sensitive memory contents.
* Raw microphone recordings by default.
* Private conversation content unless diagnostic consent is enabled.

---

# 24. Performance Targets

Initial performance targets:

* Low idle CPU usage.
* Stable memory usage during long sessions.
* Smooth animation at the configured frame rate.
* Fast application startup.
* No UI blocking during network operations.
* No synchronous disk or network calls on the UI thread.
* Graceful quality reduction on weaker hardware.

The animation quality system may provide:

* Low.
* Balanced.
* High.

---

# 25. Threading Rules

The WPF UI thread must only handle UI work.

Background operations include:

* API requests.
* WebSocket receive loop.
* Audio processing.
* Asset loading.
* File access.
* Logging.
* Character package validation.

UI updates must return to the dispatcher safely.

Cancellation tokens should be used for long-running operations.

---

# 26. Event Handling

Client modules should communicate through a local event abstraction where direct references are unnecessary.

Example events:

```text
BackendConnected
BackendDisconnected
ListeningStarted
ListeningStopped
SpeechPlaybackStarted
SpeechPlaybackCompleted
CharacterStateChanged
PermissionRequested
ConfigurationChanged
AudioDeviceChanged
```

Events should use strongly typed payloads.

---

# 27. Client Service Interfaces

Recommended interfaces include:

```text
IBackendApiClient
IRealtimeClient
IConnectionMonitor
IAnimationController
ICharacterRenderer
IAudioCaptureService
IAudioPlaybackService
IWindowManager
ITrayService
IPermissionDialogService
ILocalSettingsService
IClientEventBus
IStartupService
IMonitorService
ICursorTrackingService
```

These interfaces define module boundaries and improve testability.

---

# 28. ViewModel Structure

Recommended ViewModels:

```text
CharacterViewModel
ConversationOverlayViewModel
SettingsViewModel
VoiceSettingsViewModel
CharacterSettingsViewModel
ConnectionViewModel
PermissionDialogViewModel
NotificationViewModel
DebugViewModel
```

A large universal ViewModel must be avoided.

---

# 29. Navigation

The normal companion experience does not require traditional page navigation.

The Settings Window may use module-based navigation:

```text
General
Character
Voice
Audio
Connection
Privacy
Permissions
Notifications
Performance
Advanced
About
```

Navigation state should remain independent from character runtime state.

---

# 30. Accessibility

The client should support:

* Keyboard navigation in settings.
* Configurable subtitles.
* Adjustable subtitle size.
* Reduced motion mode.
* Mute mode.
* Text-only conversation fallback.
* High-contrast-compatible settings UI.
* Configurable notification duration.
* Ability to disable autonomous movement.

Accessibility features should not be treated as optional polish.

---

# 31. Testing Strategy

## Unit Tests

Test:

* ViewModels.
* Commands.
* State transitions.
* Animation priority.
* Reconnect policies.
* Configuration validation.
* Audio queue logic.

## Integration Tests

Test:

* API client against a test server.
* WebSocket reconnect.
* Local configuration persistence.
* Character package loading.
* Permission response flow.

## Manual Tests

Required for:

* Multiple monitors.
* Different DPI settings.
* Audio hardware.
* Transparent windows.
* System tray.
* Windows startup.
* Live2D rendering.
* Sleep and resume.

---

# 32. Future Client Extensions

The architecture should allow future clients such as:

* Android companion application.
* Web control panel.
* Linux desktop client.
* Remote control application.
* Stream overlay client.
* Lightweight tray-only client.

These future clients should reuse server APIs and shared contracts without depending on WPF implementation details.

---

# 33. Prohibited Client Practices

The following practices are prohibited unless approved through an architectural decision:

* Direct database access.
* Hard-coded AI provider calls.
* Storing API keys in plain text.
* Large business rules inside ViewModels.
* Long-running operations on the UI thread.
* Direct plugin access to Windows APIs.
* Hard-coded animation file names throughout the application.
* Unrestricted process execution.
* Silent screen or webcam capture.
* Permanent storage of raw microphone audio by default.
* Using the desktop client as the system source of truth.

---

# 34. Definition of Done

The client foundation is considered complete when:

* The WPF application starts reliably.
* Dependency injection is configured.
* The character window is transparent and movable.
* Multi-monitor behavior is functional.
* Local configuration is persisted safely.
* REST communication works.
* WebSocket communication works.
* Reconnection is automatic.
* Basic character states can be triggered.
* Audio input and output devices can be selected.
* The system tray is functional.
* Global client errors are handled.
* Unit tests cover core state behavior.
* Documentation matches the implementation.

---

# 35. Summary

The AikoOS desktop client is the visible and interactive body of the companion.

It must provide a responsive, lightweight, safe, and expressive experience while delegating intelligence, persistence, and central business rules to the backend.

Its architecture must prioritize:

* Presentation separation.
* Replaceable rendering.
* Stable realtime communication.
* Safe Windows integration.
* Testability.
* Resource efficiency.
* Long-term extensibility.

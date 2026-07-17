# WAKE WORD

> Version: 1.0
> Module: Voice

---

# 1. Purpose

The Wake Word subsystem enables hands-free activation of AikoOS by continuously listening for a predefined activation phrase.

Once the wake word is detected, the subsystem notifies the Voice Runtime to begin an interactive voice session.

The subsystem operates independently from speech recognition and AI reasoning.

---

# 2. Responsibilities

The subsystem is responsible for:

* Monitoring microphone input.
* Detecting configured wake phrases.
* Minimizing false activations.
* Supporting multiple wake words.
* Triggering voice session startup.
* Remaining lightweight during idle operation.

It does not recognize general speech or perform command interpretation.

---

# 3. High-Level Architecture

```text
Microphone
     │
     ▼
Audio Stream
     │
     ▼
Wake Word Detector
     │
     ▼
Wake Word Event
     │
     ▼
Voice Runtime
```

The detector only analyzes enough audio to identify configured activation phrases.

---

# 4. Wake Word Flow

```text
Idle
 │
 ▼
Listening
 │
 ▼
Wake Phrase Detected
 │
 ▼
Generate Event
 │
 ▼
Start Voice Session
```

The subsystem immediately returns to listening after the session ends.

---

# 5. Wake Word Profiles

Supported configuration options include:

* Activation phrase.
* Language.
* Sensitivity.
* Confidence threshold.
* Cooldown period.
* Enabled/disabled state.

Profiles should be configurable without restarting the application.

---

# 6. Detection Pipeline

```text
Audio Frames
      │
      ▼
Feature Extraction
      │
      ▼
Wake Word Model
      │
      ▼
Confidence Score
      │
      ▼
Decision
```

Detection should be optimized for low CPU usage.

---

# 7. False Activation Handling

To reduce false positives:

* Apply confidence thresholds.
* Require complete phrase matching.
* Use configurable cooldown periods.
* Ignore repeated detections during an active session.

Thresholds should be configurable for different environments.

---

# 8. Session Coordination

When the wake phrase is detected:

1. Generate a WakeWordDetected event.
2. Stop idle monitoring.
3. Start the voice session.
4. Resume monitoring after the session completes.

The subsystem should not manage conversation logic.

---

# 9. Error Handling

Possible failures include:

* Microphone unavailable.
* Wake model unavailable.
* Audio stream interruption.
* Configuration errors.

The subsystem should recover automatically whenever possible.

---

# 10. Performance

Performance goals:

* Continuous listening.
* Very low idle CPU usage.
* Minimal memory footprint.
* Fast detection.
* Stable long-running operation.

Wake word detection should not noticeably affect overall system performance.

---

# 11. Security

The subsystem must:

* Respect microphone permissions.
* Avoid storing continuous audio by default.
* Keep processing local whenever supported.
* Clearly indicate active listening status.

User trust depends on transparent microphone usage.

---

# 12. Observability

Collect metrics including:

* Wake word detections.
* False activation rate.
* Detection latency.
* Average confidence score.
* Idle CPU utilization.
* Session activation count.

These metrics support ongoing tuning of detection quality.

---

# 13. Testing Checklist

Verify that:

* Wake phrases are detected reliably.
* False positives remain within acceptable limits.
* Cooldown periods function correctly.
* Detection resumes after sessions.
* Configuration changes apply correctly.

---

# 14. Why This Design?

### Why?

Separating wake word detection from speech recognition minimizes resource usage and allows the detection engine to be replaced independently.

### Why not?

Using a full speech recognizer continuously would consume significantly more CPU and increase latency for idle listening.

### Trade-offs

* Additional subsystem.
* Separate wake word model.
* Lower resource usage.
* Better responsiveness.

---

# 15. Future Expansion

Potential enhancements:

* Personalized wake phrases.
* Speaker verification before activation.
* Multi-language wake words.
* Hardware-accelerated detection.
* Context-aware activation.

---

# 16. Summary

The Wake Word subsystem provides an efficient, low-power mechanism for initiating voice interaction.

By separating activation detection from the rest of the voice processing pipeline, AikoOS delivers responsive hands-free interaction while keeping idle resource usage low.

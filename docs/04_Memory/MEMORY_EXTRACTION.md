# MEMORY EXTRACTION

> Version: 1.0
> Module: Memory

---

# 1. Purpose

The Memory Extraction subsystem identifies information from conversations that has potential long-term value.

Its purpose is to transform raw dialogue into structured memory candidates without immediately committing them to permanent storage.

Extraction is intentionally conservative. Not everything said should become a memory.

---

# 2. Responsibilities

The subsystem is responsible for:

* Detecting memory candidates.
* Classifying candidate types.
* Assigning initial confidence.
* Recording source references.
* Forwarding candidates for validation.

It does not decide whether a memory is ultimately stored.

---

# 3. High-Level Architecture

```text
Conversation Stream
        │
        ▼
Candidate Detector
        │
        ▼
Intent Classifier
        │
        ▼
Memory Classifier
        │
        ▼
Confidence Estimator
        │
        ▼
Candidate Builder
        │
        ▼
Memory Validator
```

---

# 4. Extraction Sources

Candidate memories may originate from:

* User messages.
* Voice transcripts.
* Tool results.
* Plugin outputs.
* Calendar events.
* Automation results.

System-generated metadata should only become memory when explicitly configured.

---

# 5. Candidate Structure

```text
MemoryCandidate

├── CandidateId
├── UserId
├── SourceType
├── SourceReference
├── Category
├── Content
├── Confidence
├── Timestamp
└── Metadata
```

Candidates are temporary objects and may never become permanent memories.

---

# 6. Memory Categories

Supported categories include:

| Category      | Example                     |
| ------------- | --------------------------- |
| Preference    | "I prefer dark mode."       |
| Personal Fact | "I live in Tokyo."          |
| Goal          | "I want to learn Rust."     |
| Habit         | "I study every morning."    |
| Relationship  | "Sarah is my sister."       |
| Project       | "I'm building AikoOS."      |
| Temporary     | "I'm travelling this week." |

Categories should remain extensible.

---

# 7. Extraction Rules

A candidate should generally satisfy one or more of the following:

* Likely to be useful in future conversations.
* Explicitly stated by the user.
* Represents a stable preference or fact.
* Describes an ongoing goal or project.
* Has meaningful long-term value.

Casual greetings, filler words, and transient conversation should not be extracted.

---

# 8. Confidence Estimation

Each candidate receives an initial confidence score.

Factors may include:

| Factor                  | Effect   |
| ----------------------- | -------- |
| Explicit statement      | Increase |
| Repeated mention        | Increase |
| Inference               | Decrease |
| Ambiguous wording       | Decrease |
| Conflicting information | Decrease |

Confidence reflects certainty, not importance.

---

# 9. Sequence Diagram

```text
Conversation
      │
      ▼
Candidate Detector
      │
      ▼
Category Detection
      │
      ▼
Confidence Estimation
      │
      ▼
Candidate Creation
      │
      ▼
Memory Validator
```

---

# 10. Error Handling

The subsystem should handle:

* Empty input.
* Invalid transcripts.
* Duplicate candidate generation.
* Unsupported message formats.
* Partial extraction failures.

Failures should be logged without interrupting the conversation.

---

# 11. Performance

Design considerations:

* Streaming-friendly extraction.
* Incremental processing.
* Low latency.
* Batch extraction for long conversations when appropriate.
* Avoid repeated analysis of unchanged content.

---

# 12. Security

The extraction subsystem must:

* Respect privacy settings.
* Avoid extracting restricted information automatically.
* Record source references for auditability.
* Never expose internal extraction logic to users.

---

# 13. Observability

Record metrics such as:

* Candidates generated.
* Candidate categories.
* Average confidence.
* Extraction latency.
* Candidate acceptance rate (after validation).

These metrics help improve extraction quality.

---

# 14. Testing Checklist

Verify that:

* Stable facts are detected.
* Preferences are extracted.
* Temporary information is classified correctly.
* Duplicate candidates are minimized.
* Confidence scores are assigned consistently.
* Empty conversations produce no candidates.

---

# 15. Future Expansion

Possible future enhancements:

* Multilingual extraction.
* Emotion-aware extraction.
* Continuous streaming extraction.
* User-confirmed memories.
* Domain-specific extractors.
* Learning-based extraction policies.

---

# 16. Summary

Memory Extraction is the entry point of the long-term memory pipeline.

By converting conversations into structured memory candidates while remaining conservative and explainable, it ensures that only potentially valuable information progresses to later validation and consolidation stages.

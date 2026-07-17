# CONVERSATION DATABASE

> Version: 1.0
> Module: Conversation

---

# 1. Purpose

The Conversation module stores all user-assistant conversations.

It acts as the permanent conversation history for AikoOS and serves as the primary source for context retrieval, memory extraction, and conversation replay.

The module does not determine which memories are important. That responsibility belongs to the Memory module.

---

# 2. Tables

```text
Conversations
Messages
Attachments
ConversationSummaries
```

---

# 3. Entity Relationship

```text
Users
   │
   ▼
Conversations
   │
   ├──────────────┐
   ▼              ▼
Messages     ConversationSummaries
   │
   ▼
Attachments
```

---

# 4. Conversations Table

## Purpose

Represents one conversation session.

Examples:

* Manual chat
* Voice conversation
* Future plugin conversation

---

### Columns

| Column        | Type         | Description        |
| ------------- | ------------ | ------------------ |
| Id            | UUID         | Primary key        |
| UserId        | UUID         | Owner              |
| Title         | VARCHAR(200) | Conversation title |
| Status        | SMALLINT     | Active / Archived  |
| StartedAt     | TIMESTAMP    | Start time         |
| LastMessageAt | TIMESTAMP    | Latest activity    |
| CreatedAt     | TIMESTAMP    | Creation time      |
| UpdatedAt     | TIMESTAMP    | Last update        |
| DeletedAt     | TIMESTAMP    | Soft delete        |

---

### Status Values

| Value | Meaning  |
| ----- | -------- |
| 0     | Active   |
| 1     | Archived |
| 2     | Deleted  |

---

# 5. Messages Table

## Purpose

Stores every message exchanged.

One row equals one message.

---

### Columns

| Column         | Type      |
| -------------- | --------- |
| Id             | UUID      |
| ConversationId | UUID      |
| Sender         | SMALLINT  |
| MessageType    | SMALLINT  |
| Content        | TEXT      |
| TokenCount     | INTEGER   |
| CreatedAt      | TIMESTAMP |

---

### Sender Values

| Value | Meaning |
| ----- | ------- |
| 0     | User    |
| 1     | Aiko    |
| 2     | System  |
| 3     | Plugin  |

---

### Message Types

| Value | Meaning     |
| ----- | ----------- |
| 0     | Text        |
| 1     | Voice       |
| 2     | Image       |
| 3     | System      |
| 4     | Tool Result |

---

# 6. Attachments Table

## Purpose

Stores metadata for files referenced by messages.

Files themselves are stored separately.

---

### Columns

| Column      | Type      |
| ----------- | --------- |
| Id          | UUID      |
| MessageId   | UUID      |
| FileName    | TEXT      |
| MimeType    | TEXT      |
| FileSize    | BIGINT    |
| StoragePath | TEXT      |
| CreatedAt   | TIMESTAMP |

---

# 7. ConversationSummaries Table

## Purpose

Stores AI-generated summaries of long conversations.

Summaries reduce prompt size while preserving important context.

---

### Columns

| Column         | Type      |
| -------------- | --------- |
| Id             | UUID      |
| ConversationId | UUID      |
| Summary        | TEXT      |
| MessageCount   | INTEGER   |
| GeneratedAt    | TIMESTAMP |

---

# 8. Typical Flow

```text
User sends message
        │
        ▼
Insert Messages
        │
        ▼
Update Conversations.LastMessageAt
        │
        ▼
AI generates response
        │
        ▼
Insert assistant message
        │
        ▼
Memory module evaluates memories
```

---

# 9. Ownership Rules

Only the Conversation module may:

* Create conversations.
* Save messages.
* Archive conversations.
* Generate summaries.

Other modules must access conversations through service interfaces.

---

# 10. Index Strategy

Recommended indexes:

```text
ConversationId
UserId
CreatedAt
LastMessageAt
Sender
```

Composite indexes:

```text
(UserId, LastMessageAt)

(ConversationId, CreatedAt)
```

---

# 11. Data Lifecycle

```text
Conversation Created
        │
        ▼
Messages Added
        │
        ▼
Summary Generated
        │
        ▼
Archived
        │
        ▼
Soft Deleted
        │
        ▼
Permanent Cleanup (optional)
```

---

# 12. Edge Cases

The module must handle:

* Empty conversations.
* Interrupted AI responses.
* Duplicate message submissions.
* Extremely long conversations.
* Deleted attachments.
* Concurrent messages from multiple devices.
* Conversation recovery after reconnect.

---

# 13. Performance Guidelines

* Never load every message at once.
* Always paginate conversation history.
* Generate summaries for very large conversations.
* Cache recent conversations when appropriate.
* Avoid repeated counting queries.

---

# 14. Related Modules

Reads from:

* Identity

Used by:

* AI Gateway
* Memory
* Emotion
* Plugin
* Automation

---

# 15. Testing Checklist

* Create conversation.
* Append messages.
* Archive conversation.
* Restore archived conversation.
* Paginate message history.
* Load latest conversations.
* Save attachment metadata.
* Generate conversation summary.
* Verify soft delete.
* Verify indexes with large datasets.

---

# 16. Future Expansion

Future versions may support:

* Branching conversations.
* Shared conversations.
* Conversation labels.
* Search by semantic meaning.
* Pinned conversations.
* Conversation export/import.
* AI-generated titles.
* Automatic archival rules.

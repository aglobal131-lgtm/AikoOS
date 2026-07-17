# MEMORY DATABASE

> Version: 1.0
> Module: Memory

---

# 1. Purpose

The Memory module is responsible for storing, organizing, retrieving, and maintaining Aiko's long-term memories.

Unlike conversation history, memories represent information that should persist because it is meaningful or useful in future interactions.

The Memory module is the foundation of Aiko's long-term personality and continuity.

---

# 2. Design Goals

The memory system must:

* Preserve important information.
* Ignore temporary noise.
* Support semantic search.
* Support future memory consolidation.
* Be scalable.
* Remain explainable to the user.

The module must never become a dump of every conversation.

---

# 3. Tables

```text
Memories
MemoryEmbeddings
MemoryRelations
MemoryTags
MemoryAccessHistory
```

---

# 4. Entity Relationship

```text
Messages
    │
    ▼
Memories
    │
    ├──────────────┐
    ▼              ▼
MemoryEmbeddings  MemoryTags
    │
    ▼
MemoryRelations
    │
    ▼
MemoryAccessHistory
```

---

# 5. Memories Table

## Purpose

Stores the canonical memory record.

One row represents one meaningful memory.

---

### Columns

| Column          | Type      | Description       |
| --------------- | --------- | ----------------- |
| Id              | UUID      | Primary key       |
| UserId          | UUID      | Memory owner      |
| SourceMessageId | UUID      | Original message  |
| MemoryType      | SMALLINT  | Memory category   |
| Importance      | SMALLINT  | 0-100             |
| Confidence      | SMALLINT  | 0-100             |
| Content         | TEXT      | Canonical memory  |
| CreatedAt       | TIMESTAMP | Creation          |
| LastAccessedAt  | TIMESTAMP | Last retrieval    |
| UpdatedAt       | TIMESTAMP | Last modification |
| ArchivedAt      | TIMESTAMP | Archive time      |
| DeletedAt       | TIMESTAMP | Soft delete       |

---

# 6. Memory Types

| Value | Meaning      |
| ----- | ------------ |
| 0     | Preference   |
| 1     | Fact         |
| 2     | Relationship |
| 3     | Goal         |
| 4     | Schedule     |
| 5     | Personality  |
| 6     | Skill        |
| 7     | Context      |
| 8     | Temporary    |

These values may expand over time.

---

# 7. Importance

Importance determines how valuable a memory is.

Suggested range:

| Score  | Meaning    |
| ------ | ---------- |
| 0-20   | Disposable |
| 21-40  | Low        |
| 41-60  | Normal     |
| 61-80  | Important  |
| 81-100 | Critical   |

Importance influences:

* Retrieval priority.
* Forgetting policy.
* Summarization.
* Future consolidation.

---

# 8. Confidence

Confidence represents how certain Aiko is that a memory is correct.

Examples:

* User explicitly says "My favorite food is ramen." → High confidence.
* AI infers "User may like ramen." → Lower confidence.

Confidence allows the system to distinguish facts from assumptions.

---

# 9. MemoryEmbeddings Table

## Purpose

Stores vector embeddings used for semantic search.

---

### Columns

| Column    | Type         |
| --------- | ------------ |
| Id        | UUID         |
| MemoryId  | UUID         |
| Model     | VARCHAR(100) |
| Embedding | VECTOR       |
| Dimension | INTEGER      |
| CreatedAt | TIMESTAMP    |

Embeddings are stored separately to keep the main memory table lightweight.

---

# 10. MemoryTags Table

Stores optional tags.

Example:

```text
food
anime
unity
work
health
music
travel
```

A memory may have multiple tags.

---

# 11. MemoryRelations Table

Stores links between related memories.

Example:

```text
Memory A
      │
      ▼
related_to
      ▼
Memory B
```

Relationship types may include:

* Related
* Parent
* Child
* Duplicate
* Contradiction
* Follow-up

---

# 12. MemoryAccessHistory Table

Tracks when memories are retrieved.

Columns:

* MemoryId
* AccessedAt
* ConversationId
* RetrievalReason

Purpose:

* Ranking.
* Analytics.
* Forgetting.
* Consolidation.

---

# 13. Memory Lifecycle

```text
Conversation
      │
      ▼
Candidate Memory
      │
      ▼
Validation
      │
      ▼
Memory Created
      │
      ▼
Embedding Generated
      │
      ▼
Retrieved
      │
      ▼
Updated
      │
      ▼
Archived
```

---

# 14. Retrieval Rules

The database stores memories.

Selection logic belongs to the Memory Engine.

Typical ranking factors include:

* Semantic similarity.
* Importance.
* Confidence.
* Recency.
* Access frequency.

The database should expose the data needed for ranking without embedding ranking logic into SQL unnecessarily.

---

# 15. Index Strategy

Recommended indexes:

```text
UserId
MemoryType
Importance
CreatedAt
LastAccessedAt
```

Vector index:

```text
Embedding
```

---

# 16. Edge Cases

The schema must support:

* Duplicate memories.
* Contradictory memories.
* Memory updates.
* Archived memories.
* Deleted conversations with surviving memories.
* Multiple embeddings after model upgrades.

---

# 17. Security

Only authorized services may modify memories.

Users should always be able to:

* View memories.
* Delete memories.
* Export memories.

Hidden system memories should be avoided unless explicitly documented.

---

# 18. Future Expansion

Possible future additions:

* Memory version history.
* Episodic memories.
* Emotional memories.
* Shared memories.
* Memory clusters.
* Memory graph visualization.
* Automatic contradiction detection.

---

# 19. Summary

The Memory database is designed to store durable, meaningful knowledge rather than raw conversation history.

Its structure separates canonical memories, semantic embeddings, relationships, tags, and access history, enabling scalable retrieval and future intelligence features without tightly coupling storage to retrieval algorithms.

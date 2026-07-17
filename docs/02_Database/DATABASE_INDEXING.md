# DATABASE INDEXING

> Version: 1.0
> Module: Database

---

# 1. Purpose

This document defines the indexing strategy for every database module in AikoOS.

A good indexing strategy ensures low query latency, efficient storage usage, and scalability as data volume grows.

Indexes should be created based on actual query patterns rather than assumptions.

---

# 2. Design Principles

The indexing strategy follows these principles:

* Optimize for read-heavy workloads.
* Keep write overhead reasonable.
* Avoid redundant indexes.
* Prefer composite indexes for common filtering patterns.
* Periodically review unused indexes.
* Monitor query execution plans before introducing new indexes.

Indexes are not a substitute for poor query design.

---

# 3. Index Types

The following index types are expected to be used.

| Type           | Use Case                            |
| -------------- | ----------------------------------- |
| B-Tree         | Equality, sorting, range queries    |
| Hash           | Exact lookups (rarely needed)       |
| GIN            | JSONB, full-text search             |
| GiST           | Specialized search                  |
| HNSW / IVFFlat | Vector similarity search (pgvector) |
| Unique         | Data integrity                      |
| Partial        | Frequently filtered subsets         |
| Composite      | Multi-column filtering              |

---

# 4. Module Index Overview

| Module       | Primary Focus                            |
| ------------ | ---------------------------------------- |
| Identity     | Authentication and session lookup        |
| Conversation | Recent conversations and message history |
| Memory       | Semantic retrieval                       |
| Automation   | Next scheduled execution                 |
| Plugin       | Plugin lookup and configuration          |

---

# 5. Identity Indexes

## Users

Unique

```text
Username
```

Optional

```text
Email
```

## Devices

Composite

```text
(UserId, LastSeenAt)
```

## Sessions

Composite

```text
(UserId, ExpiresAt)
```

Partial

```text
WHERE ExpiresAt > NOW()
```

---

# 6. Conversation Indexes

## Conversations

Composite

```text
(UserId, LastMessageAt DESC)
```

Supports:

* Recent chats
* Conversation list
* Dashboard

---

## Messages

Composite

```text
(ConversationId, CreatedAt)
```

Supports:

* Infinite scrolling
* Pagination
* History loading

---

## Attachments

```text
MessageId
```

---

# 7. Memory Indexes

## Memories

Composite

```text
(UserId, Importance DESC)
```

Composite

```text
(UserId, MemoryType)
```

---

## Embeddings

Vector index

```text
HNSW
```

or

```text
IVFFlat
```

depending on dataset size and latency requirements.

---

## Memory Tags

Composite

```text
(Tag, MemoryId)
```

---

# 8. Automation Indexes

## Schedules

Composite

```text
(IsEnabled, NextRunAt)
```

---

## Tasks

Composite

```text
(Status, Priority, CreatedAt)
```

---

## Task Executions

```text
TaskId
```

---

# 9. Plugin Indexes

## Plugins

Unique

```text
Identifier
```

Composite

```text
(Status, Version)
```

---

## Plugin Permissions

Composite

```text
(PluginId, Permission)
```

---

# 10. JSONB Indexing

The following tables store JSONB values:

* UserSettings
* Tasks.Payload
* PluginSettings

Recommended index type:

```sql
GIN (Value)
```

Only create JSONB indexes for keys that are frequently queried.

Avoid indexing entire documents unnecessarily.

---

# 11. Vector Index Strategy

Memory embeddings are expected to become one of the largest datasets in AikoOS.

Recommended approach:

Small datasets (<100k vectors)

```text
HNSW
```

Large datasets

```text
IVFFlat
```

Rebuild indexes after major embedding model migrations if required.

---

# 12. Query Optimization Guidelines

Before creating a new index:

1. Inspect slow query logs.
2. Analyze the execution plan.
3. Confirm repeated query patterns.
4. Measure improvement after indexing.

Indexes that provide no measurable benefit should be removed.

---

# 13. Maintenance Strategy

Indexes require regular maintenance.

Recommended tasks:

* Reindex fragmented indexes.
* Monitor index usage.
* Remove unused indexes.
* Vacuum and analyze regularly.
* Review query statistics after major releases.

---

# 14. Common Anti-Patterns

Avoid:

* Indexing every column.
* Duplicate indexes.
* Very wide composite indexes.
* Indexes that are never used.
* Using indexes to compensate for inefficient queries.

---

# 15. Performance Checklist

Before deployment, verify:

* User login queries remain under target latency.
* Conversation history pagination is index-backed.
* Memory semantic search uses vector indexes.
* Automation workers retrieve pending tasks efficiently.
* Plugin lookups avoid full table scans.

---

# 16. Future Expansion

Future versions may include:

* Partitioned indexes.
* Read-replica optimization.
* Time-series indexing for logs.
* Automatic index recommendations.
* Adaptive indexing based on workload.

---

# 17. Summary

AikoOS uses a modular indexing strategy tailored to each subsystem.

The goal is to balance read performance, write efficiency, and long-term scalability while avoiding unnecessary storage and maintenance costs.

Indexes should evolve alongside application query patterns rather than remaining static.

# DATABASE ARCHITECTURE

> Version: 1.0
> Status: Draft
> Primary Database: PostgreSQL
> ORM: Entity Framework Core

---

# 1. Purpose

This document defines the database architecture of AikoOS.

It explains how data is organized, who owns it, and how modules interact with the database.

Detailed table definitions are documented separately.

---

# 2. Goals

The database must provide:

* Reliable persistence.
* Strong data consistency.
* Clear ownership.
* Easy schema evolution.
* Efficient querying.
* Support for semantic memory.
* Future horizontal scalability.

---

# 3. Design Principles

The database follows these principles:

* PostgreSQL is the source of truth.
* Every table has a clear owner.
* No duplicated business data unless intentionally denormalized.
* Soft delete is preferred for user-generated content.
* Migrations are mandatory.
* Database access goes through the Application and Infrastructure layers.

---

# 4. Technology Stack

| Component     | Technology            |
| ------------- | --------------------- |
| Database      | PostgreSQL            |
| ORM           | Entity Framework Core |
| Cache         | Redis                 |
| Vector Search | pgvector              |
| Migration     | EF Core Migrations    |

---

# 5. Database Ownership

Each module owns its own tables.

Example:

```text
Identity
 ├── Users
 ├── Devices
 └── Sessions

Conversation
 ├── Conversations
 └── Messages

Memory
 ├── Memories
 ├── MemoryEmbeddings
 └── MemoryRelations

Plugin
 ├── Plugins
 └── PluginPermissions
```

Other modules may read through services, but must not modify tables they do not own.

---

# 6. Planned Schemas

The database may eventually be split into logical schemas.

Example:

```text
identity
conversation
memory
emotion
plugin
automation
system
audit
```

This improves organization without requiring multiple databases.

---

# 7. Core Tables

The initial database will include:

```text
Users
Devices
Sessions

Conversations
Messages

Memories
MemoryEmbeddings

Emotions
PersonalityProfiles

Plugins
PluginPermissions

Tasks
Schedules

Settings

AuditLogs
```

Additional tables should be introduced only when justified.

---

# 8. Primary Keys

All tables should use UUID as the primary key.

Example:

```sql
Id UUID PRIMARY KEY
```

Reasons:

* Globally unique.
* Safe for synchronization.
* Easier future distributed deployment.

---

# 9. Foreign Keys

Relationships should always be explicit.

Example:

```text
User
 │
 ├── Conversations
 │
 ├── Memories
 │
 └── Settings
```

Foreign key constraints should be enabled unless a documented exception exists.

---

# 10. Timestamps

Every persistent entity should include:

```text
CreatedAt
UpdatedAt
```

Where appropriate, also include:

```text
DeletedAt
LastAccessedAt
ArchivedAt
```

All timestamps must use UTC.

---

# 11. Soft Delete

Soft delete is recommended for:

* Memories.
* Conversations.
* Messages.
* User content.

Instead of removing rows immediately:

```text
DeletedAt != NULL
```

Permanent deletion should occur through controlled cleanup processes.

---

# 12. Indexing Strategy

Indexes should be created based on real query patterns.

Typical indexed fields:

* UserId
* ConversationId
* MemoryId
* CreatedAt
* UpdatedAt
* Status

Avoid creating indexes without a measurable benefit.

---

# 13. Vector Storage

Semantic memory embeddings are stored separately from the main memory record.

Example:

```text
Memories
     │
     ▼
MemoryEmbeddings
```

This keeps the primary table lightweight while allowing efficient vector search.

---

# 14. Transactions

A single business operation should execute within one transaction whenever possible.

Example:

```text
Save Message
    │
    ├── Insert Message
    ├── Update Conversation
    └── Commit
```

Long-running AI requests should occur outside database transactions.

---

# 15. Migrations

Every schema change must be applied through migrations.

Rules:

* Never modify production tables manually.
* Name migrations clearly.
* Review destructive operations.
* Keep migration history.

---

# 16. Backup Strategy

The database should support:

* Daily backups.
* Point-in-time recovery where available.
* Periodic restore testing.
* Version compatibility during upgrades.

Backup procedures will be documented separately.

---

# 17. Performance Guidelines

* Use pagination for large result sets.
* Prefer asynchronous queries.
* Avoid N+1 query problems.
* Select only required columns.
* Profile expensive queries before optimization.

---

# 18. Security

Sensitive data must never be stored in plain text.

Examples:

* Passwords → hashed.
* API keys → encrypted.
* Tokens → protected.
* Personal data → access controlled.

Database credentials must never be embedded in source code.

---

# 19. Future Expansion

The architecture should allow future support for:

* Read replicas.
* Partitioned tables.
* External vector databases.
* Object storage for large files.
* Multi-tenant deployments.

These features should not complicate the initial implementation.

---

# 20. Summary

The AikoOS database is designed as a modular relational system.

Key principles:

* PostgreSQL is the source of truth.
* Modules own their own data.
* UUID primary keys.
* Explicit relationships.
* Migrations for all schema changes.
* pgvector for semantic search.
* Clean separation between persistence and business logic.

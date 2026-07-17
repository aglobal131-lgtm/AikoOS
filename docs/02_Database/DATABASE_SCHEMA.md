# DATABASE SCHEMA

> Version: 1.0
> Status: Draft

---

# 1. Purpose

This document defines the logical schema organization of the AikoOS database.

A schema groups tables belonging to the same business domain. It improves maintainability, security, and future scalability without requiring multiple databases.

---

# 2. Database Layout

```text
PostgreSQL
│
├── identity
├── conversation
├── memory
├── emotion
├── personality
├── automation
├── plugin
├── system
└── audit
```

Each schema owns its own tables.

---

# 3. identity

Responsible for authentication and devices.

Tables:

```text
Users
Devices
Sessions
RefreshTokens
UserSettings
```

Owner:

Identity Module

---

# 4. conversation

Stores all conversations.

Tables:

```text
Conversations
Messages
Attachments
ConversationSummaries
```

Owner:

Conversation Module

---

# 5. memory

Stores long-term memory.

Tables:

```text
Memories
MemoryEmbeddings
MemoryRelations
MemoryTags
MemoryAccessHistory
```

Owner:

Memory Module

---

# 6. emotion

Stores emotional information.

Tables:

```text
EmotionStates
EmotionHistory
MoodHistory
```

Owner:

Emotion Module

---

# 7. personality

Stores persistent personality configuration.

Tables:

```text
PersonalityProfiles
Traits
BehaviorProfiles
```

Owner:

Personality Module

---

# 8. automation

Stores scheduled tasks.

Tables:

```text
Schedules
Tasks
TaskExecutions
ReminderHistory
```

Owner:

Automation Module

---

# 9. plugin

Stores plugin metadata.

Tables:

```text
Plugins
PluginPermissions
PluginSettings
PluginLogs
```

Owner:

Plugin Module

---

# 10. system

Stores internal configuration.

Tables:

```text
Configurations
FeatureFlags
ServerSettings
```

Owner:

System Module

---

# 11. audit

Stores security and audit logs.

Tables:

```text
AuditLogs
SecurityLogs
PermissionLogs
```

Owner:

Audit Module

---

# 12. Cross-Schema Rules

Modules may read another schema only through services.

Allowed:

```text
Conversation
      │
      ▼
IMemoryService
      │
      ▼
memory schema
```

Not allowed:

```text
Conversation
      │
      ▼
SELECT * FROM memory.Memories
```

Direct cross-module queries should be avoided.

---

# 13. Naming Convention

Schemas:

```text
lowercase
```

Tables:

```text
PascalCase
```

Columns:

```text
PascalCase
```

Examples:

```text
identity.Users

conversation.Messages

memory.MemoryEmbeddings
```

---

# 14. Shared Columns

Most tables should contain:

```text
Id
CreatedAt
UpdatedAt
```

Optional:

```text
DeletedAt
CreatedBy
UpdatedBy
Version
```

---

# 15. Entity Versioning

Frequently modified tables should contain:

```text
Version
```

Used for optimistic concurrency.

---

# 16. Foreign Keys

Foreign keys should always reference UUID primary keys.

Example:

```text
Conversations
     │
     ▼
Messages
```

```text
ConversationId
```

---

# 17. Relationships

Typical relationships:

```text
User
 │
 ├── Conversations
 │        │
 │        └── Messages
 │
 ├── Memories
 │
 ├── EmotionHistory
 │
 ├── Tasks
 │
 └── Plugins
```

---

# 18. Future Schemas

Possible future schemas:

```text
analytics
vision
voice
marketplace
knowledge
```

They should only be introduced when necessary.

---

# 19. Migration Policy

Changing a schema requires:

* Migration
* Review
* Documentation update

Schemas must never be modified directly in production.

---

# 20. Summary

The schema organization of AikoOS follows business boundaries rather than technical layers.

Each module owns its data, exposing access through services instead of direct table access. This approach supports long-term maintainability and future scalability.

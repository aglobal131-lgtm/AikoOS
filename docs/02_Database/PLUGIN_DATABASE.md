# PLUGIN DATABASE

> Version: 1.0
> Module: Plugin

---

# 1. Purpose

The Plugin module stores metadata, configuration, permissions, and lifecycle information for every plugin installed in AikoOS.

The database is responsible only for plugin management.

Plugin code, binaries, and assets are stored separately on the filesystem.

---

# 2. Goals

The Plugin database must:

* Track installed plugins.
* Support versioning.
* Manage permissions.
* Store plugin configuration.
* Record plugin health.
* Support future plugin marketplace.
* Support plugin updates.

---

# 3. Tables

```text
Plugins
PluginPermissions
PluginSettings
PluginExecutionLogs
```

---

# 4. Entity Relationship

```text
Plugins
    │
    ├──────────────┐
    ▼              ▼
PluginSettings  PluginPermissions
    │
    ▼
PluginExecutionLogs
```

---

# 5. Plugins Table

## Purpose

Represents one installed plugin.

One row equals one installed plugin instance.

---

### Columns

| Column      | Type         | Description                 |
| ----------- | ------------ | --------------------------- |
| Id          | UUID         | Primary key                 |
| Name        | VARCHAR(150) | Display name                |
| Identifier  | VARCHAR(150) | Unique plugin identifier    |
| Version     | VARCHAR(30)  | Installed version           |
| Author      | VARCHAR(150) | Plugin author               |
| Description | TEXT         | Description                 |
| EntryPoint  | TEXT         | Main executable or assembly |
| Status      | SMALLINT     | Current state               |
| InstalledAt | TIMESTAMP    | Installation time           |
| UpdatedAt   | TIMESTAMP    | Last update                 |

---

### Status Values

| Value | Meaning     |
| ----- | ----------- |
| 0     | Installed   |
| 1     | Enabled     |
| 2     | Disabled    |
| 3     | Error       |
| 4     | Uninstalled |

---

# 6. PluginPermissions Table

## Purpose

Stores permissions granted to each plugin.

---

### Columns

| Column     | Type         |
| ---------- | ------------ |
| Id         | UUID         |
| PluginId   | UUID         |
| Permission | VARCHAR(100) |
| Granted    | BOOLEAN      |
| GrantedAt  | TIMESTAMP    |

---

### Example Permissions

```text
filesystem.read
filesystem.write
screen.capture
camera.read
microphone.record
notification.send
browser.open
process.launch
```

Permissions are granted individually.

---

# 7. PluginSettings Table

## Purpose

Stores plugin-specific configuration.

---

### Columns

| Column    | Type         |
| --------- | ------------ |
| Id        | UUID         |
| PluginId  | UUID         |
| Key       | VARCHAR(100) |
| Value     | JSONB        |
| UpdatedAt | TIMESTAMP    |

---

JSONB allows plugins to evolve without frequent schema migrations.

---

# 8. PluginExecutionLogs Table

## Purpose

Stores execution history for diagnostics.

---

### Columns

| Column       | Type      |
| ------------ | --------- |
| Id           | UUID      |
| PluginId     | UUID      |
| StartedAt    | TIMESTAMP |
| FinishedAt   | TIMESTAMP |
| DurationMs   | INTEGER   |
| Result       | SMALLINT  |
| ErrorMessage | TEXT      |

---

Execution logs should have a configurable retention policy.

---

# 9. Ownership Rules

Only the Plugin module may:

* Install plugins.
* Enable or disable plugins.
* Update plugin metadata.
* Modify plugin permissions.
* Store plugin settings.

Other modules interact with plugins through the Plugin Runtime.

---

# 10. Installation Flow

```text
Plugin Package
      │
      ▼
Validate Manifest
      │
      ▼
Create Plugin Record
      │
      ▼
Register Permissions
      │
      ▼
Initialize Settings
      │
      ▼
Enable Plugin
```

---

# 11. Update Flow

```text
Download Update
      │
      ▼
Compatibility Check
      │
      ▼
Backup Configuration
      │
      ▼
Replace Files
      │
      ▼
Update Version
      │
      ▼
Restart Plugin
```

Plugin settings should survive updates whenever possible.

---

# 12. Performance

Recommended indexes:

* Identifier
* Status
* Version
* PluginId

Execution logs should be archived or removed after the configured retention period.

---

# 13. Edge Cases

The system must support:

* Failed installation.
* Interrupted update.
* Corrupted plugin package.
* Duplicate identifiers.
* Missing permissions.
* Plugin crash during execution.
* Version downgrade.

---

# 14. Security

Plugins must never receive unrestricted access.

Rules:

* Permissions are explicit.
* Default permission state is denied.
* Permission changes are audited.
* Plugin configuration is isolated.
* Plugin logs must not expose sensitive data.

---

# 15. Future Expansion

Future versions may support:

* Plugin marketplace.
* Automatic updates.
* Dependency management.
* Plugin signing.
* Sandbox execution.
* Usage analytics.
* Plugin compatibility matrix.

---

# 16. Summary

The Plugin database provides a secure and extensible foundation for managing plugins.

It separates plugin metadata, permissions, settings, and execution history, allowing plugins to evolve independently while maintaining security and operational visibility.

# IDENTITY DATABASE

> Version: 1.0
> Module: Identity

---

# 1. Purpose

The Identity module manages users, devices, authentication sessions, and user-specific settings.

It provides a secure identity foundation for every other module in AikoOS.

No other module should implement its own identity storage.

---

# 2. Tables

The Identity module owns the following tables.

```text
Users
Devices
Sessions
RefreshTokens
UserSettings
```

---

# 3. Relationships

```text
                Users
                  │
      ┌───────────┼────────────┐
      │           │            │
      ▼           ▼            ▼
 Devices      Sessions   UserSettings
      │
      ▼
RefreshTokens
```

---

# 4. Users Table

## Purpose

Stores one logical user.

One user may own multiple devices.

---

### Columns

| Column       | Type         | Nullable | Description      |
| ------------ | ------------ | -------- | ---------------- |
| Id           | UUID         | No       | Primary Key      |
| Username     | VARCHAR(50)  | No       | Unique username  |
| DisplayName  | VARCHAR(100) | No       | Name shown in UI |
| Email        | VARCHAR(255) | Yes      | Optional email   |
| PasswordHash | TEXT         | No       | Hashed password  |
| Avatar       | TEXT         | Yes      | Avatar URL       |
| CreatedAt    | TIMESTAMP    | No       | Creation time    |
| UpdatedAt    | TIMESTAMP    | No       | Last update      |
| DeletedAt    | TIMESTAMP    | Yes      | Soft delete      |

---

### Indexes

```text
PK(Id)

UNIQUE(Username)

INDEX(Email)
```

---

### Constraints

* Username must be unique.
* Passwords are never stored in plain text.
* Email may be null for local accounts.

---

# 5. Devices Table

## Purpose

Represents a registered client device.

Examples:

* Desktop
* Laptop
* Future Android app

---

### Columns

| Column           | Type         |
| ---------------- | ------------ |
| Id               | UUID         |
| UserId           | UUID         |
| DeviceName       | VARCHAR(100) |
| Platform         | VARCHAR(30)  |
| DeviceIdentifier | TEXT         |
| LastSeenAt       | TIMESTAMP    |
| CreatedAt        | TIMESTAMP    |

---

### Relationships

```text
User

↓

Devices
```

---

### Notes

A user may register multiple devices.

Each device has independent permissions and sessions.

---

# 6. Sessions Table

## Purpose

Tracks active login sessions.

---

### Columns

| Column        | Type      |
| ------------- | --------- |
| Id            | UUID      |
| UserId        | UUID      |
| DeviceId      | UUID      |
| AccessTokenId | UUID      |
| ExpiresAt     | TIMESTAMP |
| LastActivity  | TIMESTAMP |
| CreatedAt     | TIMESTAMP |

---

### Notes

Expired sessions should be cleaned automatically.

---

# 7. RefreshTokens Table

## Purpose

Stores refresh tokens for authentication.

---

### Columns

| Column    | Type      |
| --------- | --------- |
| Id        | UUID      |
| SessionId | UUID      |
| TokenHash | TEXT      |
| ExpiresAt | TIMESTAMP |
| RevokedAt | TIMESTAMP |

---

### Security Rules

* Store only hashed refresh tokens.
* Tokens are single-use when possible.
* Revoked tokens must never become valid again.

---

# 8. UserSettings Table

## Purpose

Stores persistent user preferences.

Examples:

* Language
* Theme
* Timezone
* Default AI provider
* Preferred voice
* Privacy preferences

---

### Columns

| Column    | Type         |
| --------- | ------------ |
| Id        | UUID         |
| UserId    | UUID         |
| Key       | VARCHAR(100) |
| Value     | JSONB        |
| UpdatedAt | TIMESTAMP    |

---

### Why JSONB?

Settings evolve over time.

Using JSONB allows adding new settings without frequent schema changes while keeping one logical table.

---

# 9. Ownership Rules

Only the Identity module may:

* Create users.
* Register devices.
* Create sessions.
* Revoke tokens.
* Modify user settings.

Other modules must use Identity services instead of direct table access.

---

# 10. Common Queries

Examples:

* Find user by username.
* Find device by identifier.
* Validate active session.
* Load user settings.
* Revoke all sessions for a device.
* Revoke all sessions for a user.

---

# 11. Performance Considerations

Recommended indexes:

* Username
* Email
* UserId
* DeviceId
* SessionId
* ExpiresAt

Expired sessions and refresh tokens should be removed periodically by a background cleanup job.

---

# 12. Future Expansion

The Identity module may later support:

* OAuth providers.
* Passkeys.
* Multi-factor authentication.
* Device trust levels.
* Role-based access control.
* Organization accounts.

These additions should extend the existing schema rather than replace it.

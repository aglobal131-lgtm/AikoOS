# CONTEXT ENGINE

> Version: 1.0
> Module: AI

---

# 1. Purpose

The Context Engine is responsible for selecting, ranking, and assembling all information required for a single AI request.

Its objective is to provide the LLM with the **most relevant context possible** while remaining within the model's token budget.

Unlike the Memory module, which stores information, the Context Engine decides **what should be retrieved** and **how much should be included**.

---

# 2. Responsibilities

The Context Engine is responsible for:

* Loading conversation history.
* Retrieving long-term memories.
* Loading assistant personality.
* Retrieving external knowledge.
* Ranking context relevance.
* Managing token budgets.
* Producing a unified context package.

The Context Engine never calls the LLM directly.

---

# 3. High-Level Architecture

```text
                User Request
                     │
                     ▼
            Intent Analyzer
                     │
                     ▼
           Context Orchestrator
                     │
      ┌──────────────┼──────────────┐
      ▼              ▼              ▼
Conversation     Memory         Knowledge
 Retriever      Retriever      Retriever
      │              │              │
      └──────────────┼──────────────┘
                     ▼
          Personality Provider
                     │
                     ▼
            Context Ranker
                     │
                     ▼
          Token Budget Manager
                     │
                     ▼
            Context Assembler
                     │
                     ▼
            Prompt Pipeline
```

---

# 4. Core Components

| Component              | Responsibility                                     |
| ---------------------- | -------------------------------------------------- |
| Intent Analyzer        | Determines what information is likely to be needed |
| Conversation Retriever | Loads recent conversation                          |
| Memory Retriever       | Performs semantic memory search                    |
| Knowledge Retriever    | Retrieves external information                     |
| Personality Provider   | Loads assistant and user personality               |
| Context Ranker         | Scores retrieved context                           |
| Token Budget Manager   | Allocates token usage                              |
| Context Assembler      | Produces the final context package                 |

---

# 5. Context Sources

The engine may retrieve information from:

```text
Current User Message
Recent Conversation
Conversation Summary
Long-Term Memory
Assistant Personality
User Preferences
Knowledge Base
Plugin Results
Automation Status
```

Every source is optional except the current user message.

---

# 6. Retrieval Pipeline

```text
User Request
      │
      ▼
Intent Detection
      │
      ▼
Determine Needed Sources
      │
      ▼
Parallel Retrieval
      │
      ▼
Merge Results
      │
      ▼
Relevance Ranking
      │
      ▼
Token Budget Allocation
      │
      ▼
Final Context Package
```

Whenever possible, retrieval operations should execute in parallel to reduce latency.

---

# 7. Context Ranking

Every retrieved item receives a relevance score.

Example factors:

| Factor              | Description                                    |
| ------------------- | ---------------------------------------------- |
| Semantic Similarity | Relation to current request                    |
| Importance          | Memory importance score                        |
| Confidence          | Reliability of the information                 |
| Recency             | More recent information is often more relevant |
| Access Frequency    | Frequently used memories may have higher value |
| User Intent Match   | Direct relevance to detected intent            |

Weights should be configurable rather than hard-coded.

---

# 8. Token Budget Manager

Each request starts with an available token budget.

Example:

```text
Model Context Window
          │
          ▼
Reserve Response Tokens
          │
          ▼
Available Context Budget
          │
          ▼
Distribute Across Sources
```

Suggested allocation:

| Source       | Share |
| ------------ | ----: |
| Conversation |   35% |
| Memory       |   25% |
| Knowledge    |   20% |
| Personality  |   10% |
| Tools        |    5% |
| Buffer       |    5% |

The allocation may vary depending on the detected task.

---

# 9. Context Compression

If retrieved context exceeds the budget, compression is applied in this order:

1. Remove duplicate information.
2. Replace older messages with conversation summaries.
3. Remove low-relevance memories.
4. Compress retrieved knowledge.
5. Remove optional metadata.

The user's latest message must never be compressed or omitted.

---

# 10. Context Package

The Context Engine outputs a provider-independent structure.

Example:

```text
ContextPackage
├── SystemInstructions
├── Personality
├── ConversationHistory
├── ConversationSummary
├── RelevantMemories
├── RetrievedKnowledge
├── AvailableTools
├── UserMessage
└── Metadata
```

The Prompt Pipeline consumes this package directly.

---

# 11. Sequence Diagram

```text
User Request
      │
      ▼
Intent Analyzer
      │
      ▼
Retrieve Conversation ─────────────┐
Retrieve Memories ─────────────────┤
Retrieve Knowledge ────────────────┤ (Parallel)
Load Personality ──────────────────┘
      │
      ▼
Merge Results
      │
      ▼
Rank Context
      │
      ▼
Apply Token Budget
      │
      ▼
Build Context Package
      │
      ▼
Prompt Pipeline
```

---

# 12. Edge Cases

The engine must correctly handle:

* Empty conversation history.
* No relevant memories found.
* Conflicting memories.
* Missing personality profile.
* External knowledge unavailable.
* Extremely large conversations.
* Token budget exhaustion.
* Simultaneous requests from multiple devices.

---

# 13. Performance Guidelines

Recommended targets:

* Retrieval should execute concurrently where possible.
* Memory search should avoid full table scans.
* Conversation history should always be paginated.
* Cache frequently accessed personality data.
* Cache conversation summaries when appropriate.

Latency introduced by the Context Engine should remain minimal compared to total AI response time.

---

# 14. Observability

Record the following metrics:

* Retrieval latency by source.
* Number of memories retrieved.
* Number of memories selected.
* Compression ratio.
* Token usage by source.
* Final context size.
* Ranking execution time.

These metrics help tune retrieval quality over time.

---

# 15. Testing Checklist

Verify that the engine:

* Retrieves relevant memories.
* Correctly ranks context.
* Honors token limits.
* Produces deterministic results for identical inputs.
* Handles missing sources gracefully.
* Compresses context without losing essential information.
* Builds a valid `ContextPackage`.

---

# 16. Future Expansion

Future versions may support:

* Hierarchical memory retrieval.
* Episodic vs semantic memory balancing.
* Multi-agent context sharing.
* Predictive prefetching.
* Adaptive ranking using user feedback.
* Personalized retrieval strategies.
* Context caching across consecutive requests.

---

# 17. Summary

The Context Engine is the intelligence layer that determines **what information the AI should know before answering**.

By combining conversation history, long-term memory, personality, external knowledge, and token-aware ranking into a single provider-independent context package, it enables AikoOS to generate responses that are relevant, efficient, and consistent while remaining scalable for future AI capabilities.

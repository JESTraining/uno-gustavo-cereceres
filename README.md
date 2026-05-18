## 🔥 Senior Exercise: Distributed Event-Driven Notification Engine

### Business Context
Build a **multi-tenant notification engine** that:
- Accepts events from various sources (webhook, API, message queue)
- Routes them to user-configured destinations (Email, Slack, Webhook, SMS via mock provider)
- Handles 1000+ events/minute
- Provides delivery guarantees (at-least-once)
- Includes dead-letter queue, retry policies, and monitoring

---

## 🧠 Core Requirements (Senior+ Level)

### System Architecture
- [ ] **Event-driven design** - no request/response for processing (all async)
- [ ] **Multiple entry points**:
  - HTTP API (REST)
  - Redis Streams consumer
  - RabbitMQ consumer (optional, choose one message broker)
- [ ] **Multi-tenancy** - tenants isolated by API key, each has own routing rules
- [ ] **Rule engine** - tenants configure: "if event.type = 'payment_failed' then send to Slack channel #alerts"

### Backend (.NET 8+)
- [ ] **Vertical slice architecture** (not traditional layered) - feature folders
- [ ] **Distributed locking** (Redlock or Postgres advisory locks) - prevent duplicate notifications
- [ ] **Outbox pattern** with idempotent consumer
- [ ] **Rate limiting per tenant** (100 events/min, 1000/hour)
- [ ] **Circuit breaker** for each destination type (Slack mock fails after 50% errors)
- [ ] **Retry with jitter** - exponential backoff: 1s, 2s, 4s, 8s, 16s (max 5 retries)
- [ ] **Dead Letter Queue (DLQ)** - failed events after all retries
- [ ] **Idempotency keys** - same event ID processed once across all retries

### Database Design
- [ ] **PostgreSQL** (primary) with:
  - Tenants table
  - Routing rules table (JSONB for conditions)
  - Events table (partitioned by month)
  - Notification attempts table
  - Outbox messages table
- [ ] **Redis** for:
  - Rate limiting counters (sliding window)
  - Distributed locks
  - Cache for tenant rules (expire 5 min)
  - Real-time metrics aggregation

### API Endpoints
- [ ] `POST /api/v1/events` - ingest event (returns `202 Accepted`, `X-Event-Id` header)
- [ ] `GET /api/v1/events/{id}` - check delivery status
- [ ] `GET /api/v1/tenants/{id}/metrics` - delivery rates, failure reasons, latency p95
- [ ] `POST /api/v1/tenants/{id}/rules` - configure routing rule
- [ ] `GET /api/v1/admin/dlq` - view dead-letter queue (admin only)
- [ ] `POST /api/v1/admin/dlq/replay` - replay DLQ events

### Frontend (Angular 18+)
- [ ] **Tenant dashboard** showing:
  - Real-time event throughput (SignalR + Chart.js)
  - Failure rate by destination type
  - Top 5 error reasons
- [ ] **Rule builder UI** - no-code condition editor (e.g., `event.amount > 1000 AND event.country = "US"`)
- [ ] **Event inspector** - search events by ID, see full delivery trace
- [ ] **Admin panel** - view DLQ, replay events, simulate destination failures
- [ ] **Service worker** for offline capability (cache last 100 events)

### Reliability & Observability
- [ ] **Health checks** with degradation detection:
  - `/health/ready` - fails if Redis disconnected
  - `/health/live` - always 200
  - `/health/degraded` - warns if DLQ > 1000 events
- [ ] **OpenTelemetry** with:
  - Traces across HTTP → Redis stream → worker → outbox → webhook call
  - Custom span attributes (tenant_id, event_type, retry_count)
  - Export to Jaeger
- [ ] **Structured logging** with:
  - Correlation ID propagated across all services
  - Logs in JSON format (for Seq/ELK)
  - Different levels per tenant (debug for tenant X only)
- [ ] **Prometheus metrics**:
  - `events_ingested_total` by tenant, event_type
  - `delivery_duration_seconds` histogram
  - `dlq_size` gauge
  - `circuit_breaker_state` (closed/open/half-open)

### Docker & Orchestration
- [ ] **Multi-container setup** with:
  - API server (3 replicas in compose)
  - Background worker (2 replicas)
  - PostgreSQL (with pg_partman extension for auto-partitioning)
  - Redis (with sentinel mode simulation)
  - RabbitMQ or Redis Streams
  - Jaeger + Prometheus + Grafana
  - Seq (structured logs)
- [ ] **Healthcheck dependencies** - worker waits for Redis/DB before processing
- [ ] **Resource limits** (CPU/memory) on all services
- [ ] **Init container** for DB migrations
- [ ] **Readiness probes** - API only accepts traffic after migrations
- [ ] **Graceful shutdown**:
  - API finishes in-flight requests (30s timeout)
  - Worker completes current event processing before exit

---

## 🧪 Testing Requirements
- [ ] **Integration test** with Testcontainers (spins up real PostgreSQL/Redis)
- [ ] **Chaos test** - kill Redis mid-request, verify system recovers
- [ ] **Load test** (k6 script) - 1000 events/second, verify:
  - No duplicate deliveries under partition
  - P95 latency < 500ms
  - Zero event loss
- [ ] **Contract tests** (Pact) between API and worker
- [ ] **Fault injection test** - mock Slack fails 80% of requests, verify circuit breaker opens

---

## 📅 1-Week Senior Schedule (Intense)

| Day | Focus | Deliverables |
|-----|-------|-------------|
| **Day 1** | Foundation + Infra | Vertical slice setup, Docker Compose with all dependencies, DB schema with partitioning, Redis connection |
| **Day 2** | Event Ingestion + Outbox | HTTP endpoint, Redis Streams publisher, outbox pattern, idempotency, rate limiting |
| **Day 3** | Processing Engine | Worker service, rule engine (dynamic expression trees), retry with jitter, circuit breaker, DLQ |
| **Day 4** | Observability + Frontend Core | OpenTelemetry, Prometheus metrics, Angular dashboard with real-time charts, SignalR hub |
| **Day 5** | Testing + Production Readiness | Integration tests, chaos test, k6 load test, graceful shutdown, health checks, Grafana dashboards |

---

## 🏆 Senior Differentiators (Make or break)

| Concept | Why it's hard | What separates seniors |
|---------|---------------|----------------------|
| **Outbox pattern** | Easy to implement wrong (dual-write problem) | Transactional outbox with polling publisher, idempotent consumer |
| **Partitioning** | Most devs never design for growth | Monthly partitioning, archive strategy, index maintenance |
| **Distributed locking** | Race conditions on retries | Redlock vs Postgres advisory locks, lock TTL, fencing tokens |
| **Circuit breaker** | Hard to test, state management | 3-state machine, half-open testing, metrics-based thresholds |
| **Rule engine** | Performance nightmare if naive | Compiled expression trees, cache by tenant, timeout per evaluation |
| **Multi-tenancy** | Data leakage risk | Row-level policies (Postgres RLS), tenant ID in all queries, middleware validation |

---

## 🔥 Stretch Goals (Exceptional candidates only)

- [ ] **Blue/Green deployment** - zero-downtime migrations using shadow table pattern
- [ ] **A/B test** two rule engine implementations (interpreted vs compiled) and compare performance
- [ ] **WebAssembly** filter rules (run user-defined filters in WASM sandbox)
- [ ] **Bloom filter** for idempotency (reduce Redis memory 90%)
- [ ] **Automatic scaling** - worker pod count based on Redis stream length
- [ ] **Disaster recovery** - replicate to secondary region with RTO < 5 min
- [ ] **Billing integration** - track events per tenant, generate usage report (CSV export)

---

## 📝 Deliverables

- [ ] GitHub repo with **detailed ADRs** (Architecture Decision Records) for:
  - Why Redis Streams over Kafka/RabbitMQ
  - Why outbox over 2PC
  - Why PostgreSQL partitioning strategy
  - Circuit breaker vs retry decision
- [ ] **Architecture diagram** (C4 model - Level 1-3)
- [ ] **Runbook** - how to handle:
  - Redis OOM (out of memory)
  - PostgreSQL connection pool exhaustion
  - DLQ growing indefinitely
  - Circuit breaker stuck open
- [ ] **SLA document** - RTO/RPO for each component
- [ ] **Security review** - API key rotation, rate limiting evasion, tenant isolation proof

---

## ✅ Evaluation Criteria (Senior+)

| Area | Weight | Senior must demonstrate |
|------|--------|------------------------|
| **Architecture decisions** | 25% | Clear trade-offs documented, not just "I used X because I know it" |
| **Failure handling** | 20% | No silent failures, all errors observable, system self-recovers |
| **Performance** | 20% | Handles 1000/s locally on modest hardware, no O(n) queries |
| **Production readiness** | 20% | Health checks, metrics, logs, graceful shutdown, resource limits |
| **Testing depth** | 10% | Integration + chaos + load tests, not just unit mocks |
| **Code quality** | 5% | Clean architecture, meaningful names, no copy-paste code |

---

## 💀 Common Failure Modes (Auto-fail)

- ❌ **In-memory queues** (lose events on restart)
- ❌ **No idempotency** (retries create duplicates)
- ❌ **Tight coupling** (API waits for notification delivery)
- ❌ **No circuit breaker** (cascading failures)
- ❌ **Single database with no partitioning** (won't scale past 10M events)
- ❌ **Hardcoded retry delays** (causes thundering herd)
- ❌ **No distributed lock** (duplicate notifications on retries)
- ❌ **Metrics only in logs** (no Prometheus/Grafana for real-time alerting)

---

## 📚 Reference Architecture (For evaluation)

The solution should resemble this flow:

```
HTTP/Webhook → API → Outbox DB → Poller → Redis Stream → Worker → Rule Engine → Destination
                     ↓                                              ↓
                  Idempotency                                    Retry + DLQ
                     ↓                                              ↓
                  Partitioned                                   Circuit Breaker
                     Events Table                                    ↓
                                                                 Metrics/Monitoring
```

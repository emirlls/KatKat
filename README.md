# KatKat Backend 🏢🚀

**KatKat** is a next-generation, cloud-native B2B2C SaaS backend ecosystem engineered to transform traditional, rigid property management platforms into a dynamic, gamified community experience. Moving beyond mere accounting tools, KatKat bridges official estate administration with real-time neighborhood solidarity and social gamification.

### 🤖 AI-Collaborative Engineering
This project is fully designed and co-architected in collaboration with **Claude AI**. By combining human domain expertise with AI-driven architectural design patterns, the codebase adheres to production-grade enterprise practices from day one.

The system is built on top of **ABP Framework** and **.NET 10 (C# 14)** using strict **Domain-Driven Design (DDD)** principles and robust multi-tenant data isolation.

---

## 🌟 Core Features & Modules

* **Official Property Management:** Zero-friction automated billing split engine based on dynamic property share factors (`ShareFactor`), digital public notices, and comprehensive maintenance/ticket logging.
* **"KatKat Score" Gamification Engine:** A fully deterministic nightly background worker that aggregates building-wide performance across Financial Health (payment speed), Social Solidarity (completed neighborly requests), and Management Operational Velocity (issue resolution time) into a localized public leaderboard.
* **Peer-to-Peer (P2P) Solidarity Hub:** A real-time, channel-based request network allowing verified residents to broadcast localized community needs (e.g., borrowing tools or immediate logistical help) with flexible user-configured notification controls (*Opt-In/Opt-Out*).
* **Real-Time Resource Reservation & SOS Hub:** Live multi-client dashboard powered by SignalR handling spot-on reservation logic for limited parking zones or community zones alongside high-priority disaster matrix visualization dashboards.

---

## 📐 Software Architecture & Structure

KatKat implements a clean, layered architectural pattern following DDD practices as a decoupled modular plugin framework.

## 👥 Development & Collaboration

This repository is a product of human-AI pair programming:
* **Lead Software Engineer & Product Owner:** Emir Leylek
* **Co-Architect & AI Collaborator:** Claude AI (Anthropic)

# KatKat

## Docker ile çalıştırma

```
docker compose up -d --build
```

Bu tek komut PostgreSQL, Redis ve API'yi ayağa kaldırır; migration'lar ve seed verileri (81 il, varsayılan admin kullanıcısı, OpenIddict client'ları) otomatik uygulanır - ekstra bir adım gerekmez.

- API: http://localhost:8080 (Swagger: http://localhost:8080/swagger)
- Varsayılan giriş: `admin` / `1q2w3E*`

Postgres ve Redis verileri Docker volume'larında (`katkat_postgres_data`, `katkat_redis_data`) saklanır; normal yeniden başlatma/yeniden derleme (`docker compose up -d --build`, `docker compose restart`, `docker compose down` + `up`) veriyi SİLMEZ.

**Dikkat:** `docker compose down -v` (veya `docker compose down --volumes`) bu volume'ları da siler ve tüm veriyi (siteler, kullanıcılar, daireler...) kalıcı olarak kaybettirir. Sadece bilerek sıfırdan başlamak istediğinizde kullanın.
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

Bu mod düz HTTP üzerinden çalışır (yerel geliştirme/demo amaçlı) ve `KatKat-UI` reposundaki frontend'in `docker compose up` çıktısıyla birlikte kullanılmak üzere tasarlanmıştır. Docker olmadan `dotnet run` ile çalıştırmak için mevcut `appsettings.json`/HTTPS dev-cert akışı değişmeden duruyor.
# KatKat

## Docker ile çalıştırma

```
docker compose up -d --build
```

Bu tek komut PostgreSQL, Redis ve API'yi ayağa kaldırır; migration'lar ve seed verileri (81 il, varsayılan admin kullanıcısı, OpenIddict client'ları) otomatik uygulanır - ekstra bir adım gerekmez.

- API: http://localhost:8080 (Swagger: http://localhost:8080/swagger)
- Varsayılan giriş: `admin` / `1q2w3E*`
- Veriyi sıfırlamak için: `docker compose down -v`

Bu mod düz HTTP üzerinden çalışır (yerel geliştirme/demo amaçlı) ve `KatKat-UI` reposundaki frontend'in `docker compose up` çıktısıyla birlikte kullanılmak üzere tasarlanmıştır. Docker olmadan `dotnet run` ile çalıştırmak için mevcut `appsettings.json`/HTTPS dev-cert akışı değişmeden duruyor.
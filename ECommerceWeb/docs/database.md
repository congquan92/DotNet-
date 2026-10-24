# Database & EF Core Migration Guide

Xem tài liệu đầy đủ tại [README.md](file:///c:/Users/Wuanz/Desktop/ECommerceWeb/README.md).

## Tóm tắt các lệnh nhanh

### 1. Khởi chạy Docker PostgreSQL
```powershell
docker compose up -d
```

### 2. Chạy Migration tạo bảng
```powershell
# Tạo file migration
dotnet ef migrations add InitialCreate --project ECommerceWeb

# Áp dụng migration vào database
dotnet ef database update --project ECommerceWeb
```

### 3. Tắt Docker PostgreSQL
```powershell
docker compose down
```

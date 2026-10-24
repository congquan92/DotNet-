# ECommerceWeb - Hướng dẫn Docker & Entity Framework Core Migration

Tài liệu này tổng hợp các lệnh cần thiết để khởi chạy PostgreSQL bằng Docker và thực hiện EF Core Migration cho dự án.

---

## 1. Khởi chạy PostgreSQL với Docker Compose

Tất cả lệnh chạy từ thư mục gốc của dự án (`ECommerceWeb/`):

### 1.1. Bật database PostgreSQL (chạy nền)
```powershell
docker compose up -d
```

### 1.2. Bật kèm giao diện pgAdmin Web (Quản lý trực quan)
```powershell
docker compose --profile tools up -d
```
- **URL**: [http://localhost:5050](http://localhost:5050)
- **Email đăng nhập**: `admin@ecommerce.local`
- **Mật khẩu**: `admin`
- **Thông tin Add Server trong pgAdmin**:
  - Host name / address: `postgres` (hoặc `host.docker.internal` nếu kết nối từ ngoài)
  - Port: `5432`
  - Maintenance database: `ECommerceDb`
  - Username: `postgres`
  - Password: `postgres`

### 1.3. Kiểm tra trạng thái container
```powershell
docker compose ps
```

### 1.4. Xem logs database
```powershell
docker compose logs -f postgres
```

### 1.5. Dừng database
```powershell
# Dừng container (giữ nguyên dữ liệu)
docker compose down

# Dừng và XÓA SẠCH dữ liệu database (nếu muốn reset từ đầu)
docker compose down -v
```

---

## 2. Thông tin kết nối Database

Đã được cấu hình sẵn trong `ECommerceWeb/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ECommerceDb;Username=postgres;Password=postgres"
}
```

---

## 3. Chạy Migration tạo bảng vào Docker PostgreSQL

> [!IMPORTANT]
> **Lưu ý:** Hãy **dừng ứng dụng web** (Ctrl + C tại terminal đang chạy app) trước khi chạy lệnh migration để tránh lỗi bị khóa file `ECommerceWeb.exe`.

### 3.1. Tạo Migration ban đầu
Tạo mã migration dựa trên các class Entity (`Category`, `Product`, `User`, `Order`, `OrderItem`):

```powershell
dotnet ef migrations add InitialCreate --project ECommerceWeb
```

### 3.2. Cập nhật schema vào PostgreSQL
Áp dụng migration vào database PostgreSQL đang chạy trong Docker:

```powershell
dotnet ef database update --project ECommerceWeb
```

---

## 4. Các lệnh EF Core hữu ích khác

### 4.1. Xem danh sách các migration đã áp dụng
```powershell
dotnet ef migrations list --project ECommerceWeb
```

### 4.2. Xóa migration gần nhất (khi chưa update database hoặc đã rollback)
```powershell
dotnet ef migrations remove --project ECommerceWeb
```

### 4.3. Thêm migration mới khi có thay đổi Entity
```powershell
dotnet ef migrations add <TenMigrationMoi> --project ECommerceWeb
dotnet ef database update --project ECommerceWeb
```

---

## 5. Chạy ứng dụng Web

Sau khi database đã được cập nhật:

```powershell
dotnet run --project ECommerceWeb --launch-profile http
```
Truy cập: [http://localhost:5187](http://localhost:5187)

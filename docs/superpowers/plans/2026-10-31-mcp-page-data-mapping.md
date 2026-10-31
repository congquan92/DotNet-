# McpPage Data Layer Mapping & ASP.NET Core Identity Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Map domain entities from BlazorPage into McpPage, implement ASP.NET Core Identity with a separate PostgreSQL database (`McpDb`), and configure all relationships using EF Core Fluent API.

**Architecture:** ASP.NET Core Identity is integrated with `ApplicationUser : IdentityUser` replacing the legacy `Account` entity. Domain entities (`Customer`, `Category`, `Product`, `Order`, `OrderDetail`) are defined in `McpPage/Data/Entities`. Mapping, key definitions, cascading, string length, and decimal precision are configured via separate `IEntityTypeConfiguration<T>` classes. `ApplicationDbContext` inherits `IdentityDbContext<ApplicationUser>` and loads all configurations. Startup seeds roles, initial users, categories, and products.

**Tech Stack:** .NET 10.0, ASP.NET Core MVC, EF Core 10.0.13, PostgreSQL (Npgsql 10.0.3), Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.13.

**Spec:** `docs/superpowers/specs/2026-10-31-mcp-page-data-mapping-design.md`

## Global Constraints

- Target framework: `net10.0`.
- Entity Framework Core version: `10.0.13`.
- Database: PostgreSQL on `localhost:5432`, database name `McpDb`.
- All entity configurations must be declared using Fluent API (`IEntityTypeConfiguration<T>`).
- Do NOT run `git commit` or `git push` automatically (per `AGENTS.md`).

## Review Focus

1. `ApplicationDbContext` OnModelCreating must call `base.OnModelCreating(builder)` to properly register Identity schema tables (`AspNetUsers`, `AspNetRoles`, etc.).
2. Decimal properties (`Product.Price`, `Order.TotalAmount`, `OrderDetail.UnitPrice`) must have precision set to avoid PostgreSQL precision warnings (`HasPrecision(18, 2)`).
3. The relationship between `Customer` and `ApplicationUser` must use `OnDelete(DeleteBehavior.SetNull)` so deleting a user does not abruptly delete historical customer data.
4. Product to Category relationship must use `OnDelete(DeleteBehavior.Restrict)` to prevent accidental cascade deletion of products when a category is deleted.
5. Seeding logic must be idempotent, checking whether roles and initial categories already exist before inserting.

---

### Task 1: Package Dependencies in `McpPage.csproj`

**Files:**
- Modify: `McpPage/McpPage.csproj`

**Interfaces:**
- Consumes: None
- Produces: EF Core, PostgreSQL Npgsql provider, and ASP.NET Core Identity libraries available to `McpPage`

- [ ] **Step 1: Update `McpPage/McpPage.csproj` with required package references**
  Add PackageReferences for:
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (10.0.13)
  - `Microsoft.EntityFrameworkCore.Design` (10.0.13)
  - `Npgsql.EntityFrameworkCore.PostgreSQL` (10.0.3)

- [ ] **Step 2: Restore and build project to verify dependencies**
  Run: `dotnet build McpPage/McpPage.csproj`
  Expected: Build succeeded with 0 errors.

---

### Task 2: Domain Entities Definition

**Files:**
- Create: `McpPage/Data/Entities/ApplicationUser.cs`
- Create: `McpPage/Data/Entities/Category.cs`
- Create: `McpPage/Data/Entities/Product.cs`
- Create: `McpPage/Data/Entities/Customer.cs`
- Create: `McpPage/Data/Entities/Order.cs`
- Create: `McpPage/Data/Entities/OrderDetail.cs`

**Interfaces:**
- Consumes: `Microsoft.AspNetCore.Identity.IdentityUser`
- Produces: Entity types for domain model:
  - `ApplicationUser : IdentityUser`
  - `Category` (Id, Name, Description, Products)
  - `Product` (Id, Name, Description, Price, ImageUrl, Stock, CategoryId, Category, OrderDetails)
  - `Customer` (Id, FullName, Email, Phone, Address, UserId, User, Orders)
  - `Order` (Id, OrderDate, TotalAmount, Status, ShippingAddress, CustomerId, Customer, OrderDetails)
  - `OrderDetail` (Id, OrderId, Order, ProductId, Product, Quantity, UnitPrice)

- [ ] **Step 1: Create `ApplicationUser.cs`**
  Subclasses `IdentityUser`. Adds any custom profile fields if needed (e.g. `FullName`).

- [ ] **Step 2: Create `Category.cs` and `Product.cs`**
  Migrated from BlazorPage with strong typing and collections.

- [ ] **Step 3: Create `Customer.cs`**
  Contains `string? UserId` and navigation `ApplicationUser? User` alongside contact details and `Orders` navigation.

- [ ] **Step 4: Create `Order.cs` and `OrderDetail.cs`**
  Migrated from BlazorPage with navigations and collections.

- [ ] **Step 5: Verify build**
  Run: `dotnet build McpPage/McpPage.csproj`
  Expected: Build succeeded with 0 errors.

---

### Task 3: Fluent API Configurations

**Files:**
- Create: `McpPage/Data/Configurations/CategoryConfiguration.cs`
- Create: `McpPage/Data/Configurations/ProductConfiguration.cs`
- Create: `McpPage/Data/Configurations/CustomerConfiguration.cs`
- Create: `McpPage/Data/Configurations/OrderConfiguration.cs`
- Create: `McpPage/Data/Configurations/OrderDetailConfiguration.cs`

**Interfaces:**
- Consumes: Entity classes from Task 2, `Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<T>`
- Produces: Fluent API configuration classes loaded by DbContext

- [ ] **Step 1: Create `CategoryConfiguration.cs`**
  Table: `Categories`.
  `Name`: Required, MaxLength(100).
  `Description`: MaxLength(500).

- [ ] **Step 2: Create `ProductConfiguration.cs`**
  Table: `Products`.
  `Name`: Required, MaxLength(200).
  `Price`: HasPrecision(18, 2).
  `ImageUrl`: MaxLength(1000).
  Relationship: `HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict)`.

- [ ] **Step 3: Create `CustomerConfiguration.cs`**
  Table: `Customers`.
  `FullName`: Required, MaxLength(150).
  `Email`: MaxLength(150).
  `Phone`: MaxLength(20).
  `Address`: MaxLength(255).
  Relationship: `HasOne(c => c.User).WithOne().HasForeignKey<Customer>(c => c.UserId).OnDelete(DeleteBehavior.SetNull)`.

- [ ] **Step 4: Create `OrderConfiguration.cs`**
  Table: `Orders`.
  `Status`: Required, MaxLength(50).
  `ShippingAddress`: MaxLength(255).
  `TotalAmount`: HasPrecision(18, 2).
  Relationship: `HasOne(o => o.Customer).WithMany(c => c.Orders).HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.Cascade)`.

- [ ] **Step 5: Create `OrderDetailConfiguration.cs`**
  Table: `OrderDetails`.
  `UnitPrice`: HasPrecision(18, 2).
  Relationship: `HasOne(od => od.Order).WithMany(o => o.OrderDetails).HasForeignKey(od => od.OrderId).OnDelete(DeleteBehavior.Cascade)`.
  Relationship: `HasOne(od => od.Product).WithMany(p => p.OrderDetails).HasForeignKey(od => od.ProductId).OnDelete(DeleteBehavior.Restrict)`.

- [ ] **Step 6: Verify build**
  Run: `dotnet build McpPage/McpPage.csproj`
  Expected: Build succeeded with 0 errors.

---

### Task 4: ApplicationDbContext Implementation

**Files:**
- Create: `McpPage/Data/ApplicationDbContext.cs`

**Interfaces:**
- Consumes: `IdentityDbContext<ApplicationUser>`, entity configurations from Task 3
- Produces: `ApplicationDbContext` registered with DI

- [ ] **Step 1: Create `ApplicationDbContext.cs`**
  Inherits from `IdentityDbContext<ApplicationUser>`.
  Exposes:
  - `DbSet<Category> Categories => Set<Category>();`
  - `DbSet<Product> Products => Set<Product>();`
  - `DbSet<Customer> Customers => Set<Customer>();`
  - `DbSet<Order> Orders => Set<Order>();`
  - `DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();`
  In `OnModelCreating(ModelBuilder builder)`:
  - Call `base.OnModelCreating(builder);`
  - Call `builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);`

- [ ] **Step 2: Verify build**
  Run: `dotnet build McpPage/McpPage.csproj`
  Expected: Build succeeded with 0 errors.

---

### Task 5: Database Connection, Seeding & Startup Registration

**Files:**
- Modify: `McpPage/appsettings.json`
- Create: `McpPage/Data/DbInitializer.cs`
- Modify: `McpPage/Program.cs`

**Interfaces:**
- Consumes: `ApplicationDbContext`, `ApplicationUser`, `UserManager<ApplicationUser>`, `RoleManager<IdentityRole>`
- Produces: Fully wired database services and seed data execution on app launch

- [ ] **Step 1: Add PostgreSQL connection string in `McpPage/appsettings.json`**
  Add connection string for `McpDb`:
  `"ConnectionStrings": { "DefaultConnection": "Host=localhost;Port=5432;Database=McpDb;Username=postgres;Password=mysecretpassword" }`

- [ ] **Step 2: Create `DbInitializer.cs`**
  Implement `public static async Task SeedAsync(IServiceProvider serviceProvider)`:
  - Resolve `ApplicationDbContext`, `UserManager<ApplicationUser>`, `RoleManager<IdentityRole>`.
  - Ensure database is created (`await context.Database.EnsureCreatedAsync()`).
  - Create roles: `"Admin"`, `"User"` if they don't exist.
  - Create default Admin user (`admin@example.com` / `admin`) with password `Admin@123` and assign `"Admin"` role.
  - Create default Customer user (`user1@example.com` / `user1`) with password `User@123` and assign `"User"` role.
  - Seed `Customer` linked to `user1`.
  - Seed Categories and Products (migrated from BlazorPage `DbInitializer.cs`).

- [ ] **Step 3: Register services and seeding in `McpPage/Program.cs`**
  - Register DbContext:
    `builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));`
  - Register Identity:
    `builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => { ... }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();`
  - Execute seeding in application pipeline:
    `using (var scope = app.Services.CreateScope()) { await DbInitializer.SeedAsync(scope.ServiceProvider); }`

- [ ] **Step 4: Verify build and test compilation**
  Run: `dotnet build McpPage/McpPage.csproj`
  Expected: Build succeeded with 0 errors.

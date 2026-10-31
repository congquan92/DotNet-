# Design Document: Data Layer Mapping to McpPage with ASP.NET Core Identity & Fluent API

- **Date:** 2026-10-31
- **Status:** Proposed
- **Author:** Antigravity & User

## 1. Intent & Context
The project `McpPage` is an ASP.NET Core MVC application targeting .NET 10.0. It requires a complete data layer modeled after `BlazorPage`, but modernized with standard **ASP.NET Core Identity** (`Microsoft.AspNetCore.Identity.EntityFrameworkCore`) instead of the custom plain-text `Account` entity used in BlazorPage. All entity relationships, constraints, and table mappings must be configured using EF Core **Fluent API**, and use an independent PostgreSQL database (`McpDb`).

## 2. Goals & Non-Goals

### Goals
- **Package Management:** Add required packages to `McpPage.csproj`:
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (10.0.x)
  - `Npgsql.EntityFrameworkCore.PostgreSQL` (10.0.x)
  - `Microsoft.EntityFrameworkCore.Design` (10.0.x)
- **Entities:**
  - Create `ApplicationUser : IdentityUser` (supporting Roles: "Admin", "User").
  - Migrate entities: `Category`, `Product`, `Customer`, `Order`, `OrderDetail`.
  - Link `Customer` to `ApplicationUser` (1-to-1 optional via `string? UserId`).
- **Fluent API Configuration:**
  - Implement configurations using `IEntityTypeConfiguration<T>` for each entity:
    - Primary keys, foreign keys, relationships, nullability.
    - String length limits (`HasMaxLength`), decimal precision (`HasPrecision(18, 2)`).
    - Table naming and delete behaviors (`Cascade`, `Restrict`, `SetNull`).
- **DbContext:**
  - Implement `ApplicationDbContext : IdentityDbContext<ApplicationUser>`.
  - Apply all Fluent API configurations via `modelBuilder.ApplyConfigurationsFromAssembly`.
- **Database Connection & Configuration:**
  - Set connection string for PostgreSQL `McpDb` in `appsettings.json`.
  - Register DbContext and Identity (`AddIdentity<ApplicationUser, IdentityRole>`) in `Program.cs`.
- **Data Seeding:**
  - Create `DbInitializer` to ensure database creation, seed Roles ("Admin", "User"), default users (admin & customer user with hashed passwords), categories, and sample products matching `BlazorPage`.

### Non-Goals
- Full MVC CRUD UI controllers or views for products/orders (this step focuses on the data layer and Identity infrastructure).
- Modifying `BlazorPage` or `ECommerceWeb`.

### Anti-Goals
- Storing unhashed or plain-text passwords.
- Merging `Customer` into `ApplicationUser`, which would break guest checkout capabilities and deviate from the domain model.
- Using data annotations instead of Fluent API.

## 3. Approaches Considered & Trade-offs

- **Approach 1 (Selected): Domain Preservation with Separate `Customer` & `ApplicationUser`**
  - Keeps business entities intact while replacing `Account` with `ApplicationUser`.
  - Full configuration via Fluent API (`IEntityTypeConfiguration<T>`).
  - Pros: Clean separation of concerns, 100% fidelity to `BlazorPage` business model, enables guest orders and identity-agnostic customer records.
  - Cons: Requires an additional join when querying user profile with customer data.

- **Approach 2 (Rejected): Merging `Customer` into `ApplicationUser`**
  - Cons: Violates domain model compatibility with `BlazorPage`, prevents guest orders.

## 4. Architectural Details

### Directory Structure in `McpPage`:
```
McpPage/
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── DbInitializer.cs
│   ├── Configurations/
│   │   ├── CategoryConfiguration.cs
│   │   ├── ProductConfiguration.cs
│   │   ├── CustomerConfiguration.cs
│   │   ├── OrderConfiguration.cs
│   │   └── OrderDetailConfiguration.cs
│   └── Entities/
│       ├── ApplicationUser.cs
│       ├── Category.cs
│       ├── Product.cs
│       ├── Customer.cs
│       ├── Order.cs
│       └── OrderDetail.cs
```

### Key Relationships & Fluent Rules:
- `ApplicationUser` (1) <---> (0..1) `Customer` (ForeignKey: `Customer.UserId`, DeleteBehavior: `SetNull`)
- `Category` (1) <---> (N) `Product` (ForeignKey: `Product.CategoryId`, DeleteBehavior: `Restrict`)
- `Customer` (1) <---> (N) `Order` (ForeignKey: `Order.CustomerId`, DeleteBehavior: `Cascade`)
- `Order` (1) <---> (N) `OrderDetail` (ForeignKey: `OrderDetail.OrderId`, DeleteBehavior: `Cascade`)
- `Product` (1) <---> (N) `OrderDetail` (ForeignKey: `OrderDetail.ProductId`, DeleteBehavior: `Restrict`)
- Money/Decimal columns: `Price`, `TotalAmount`, `UnitPrice` set with `.HasPrecision(18, 2)`.

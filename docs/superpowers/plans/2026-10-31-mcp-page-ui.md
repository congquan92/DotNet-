# Traditional ASP.NET Core MVC UI Implementation Plan for McpPage

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement a traditional server-rendered ASP.NET Core MVC e-commerce user interface for `McpPage` using the CellphoneS / Shopee red-and-white theme, powered by the existing PostgreSQL data and ASP.NET Core Identity.

**Architecture:** Standard ASP.NET Core MVC with Razor Views, standard HTTP GET/POST forms, ViewModel data transfer, Session-backed shopping cart, and EF Core queries via `ApplicationDbContext`. All operations follow traditional MVC page reloads as requested.

**Tech Stack:** ASP.NET Core MVC 10.0, Bootstrap 5.3, Bootstrap Icons, EF Core 10.0 with PostgreSQL.

**Spec:** `docs/superpowers/specs/2026-10-31-mcp-page-ui-design.md`

## Global Constraints

- Target framework: `net10.0`.
- UI style: CellphoneS / Shopee red-and-white theme (`#d70018`).
- Architecture: Traditional basic MVC (no SPA, no AJAX required for forms; standard GET/POST with page reloads).
- Session storage must be enabled for shopping cart.
- Do NOT run `git commit` or `git push` automatically (per `AGENTS.md`).

## Review Focus

1. Session services (`AddSession`, `UseSession`) must be registered in `Program.cs` before controller routing.
2. Price formatting must be localized to VND currency string (`{0:N0} đ`).
3. Search form in header must issue a GET request to `/Products` preserving the query parameters.
4. Cart operations (`AddToCart`, `UpdateQuantity`, `RemoveFromCart`, `Checkout`) must handle missing or zero-stock products gracefully with `TempData` alert messages.
5. Checkout must create both `Order` and `OrderDetail` records in `ApplicationDbContext` and clear the cart session.

---

### Task 1: ViewModels & Session Setup in `Program.cs`

**Files:**
- Modify: `McpPage/Program.cs`
- Create: `McpPage/Models/HomeViewModel.cs`
- Create: `McpPage/Models/ProductListViewModel.cs`
- Create: `McpPage/Models/CartItem.cs`
- Create: `McpPage/Models/CartViewModel.cs`
- Create: `McpPage/Models/CheckoutViewModel.cs`

**Interfaces:**
- Consumes: `Category`, `Product`, `Order`, `OrderDetail` entities
- Produces: ViewModels and Session support for Controllers and Razor Views

- [ ] **Step 1: Add Session support in `Program.cs`**
  Add `builder.Services.AddDistributedMemoryCache();` and `builder.Services.AddSession(...)` and `app.UseSession();`.

- [ ] **Step 2: Create `CartItem.cs`, `CartViewModel.cs`, and `CheckoutViewModel.cs`**
  Define cart item properties (ProductId, ProductName, Price, ImageUrl, Quantity, SubTotal) and checkout fields.

- [ ] **Step 3: Create `HomeViewModel.cs` and `ProductListViewModel.cs`**
  Define view models transferring categorized products, banner data, search criteria, and filters.

- [ ] **Step 4: Verify build**
  Run: `dotnet build McpPage/McpPage.csproj`
  Expected: Build succeeded with 0 errors.

---

### Task 2: CellphoneS Red-White Theme & Shared Layout

**Files:**
- Modify: `McpPage/wwwroot/css/site.css`
- Modify: `McpPage/Views/Shared/_Layout.cshtml`

**Interfaces:**
- Consumes: Bootstrap Icons CDN, ViewModels, Session (`ISession`)
- Produces: Polished red-white layout with dynamic header, search form, cart counter badge, and footer

- [ ] **Step 1: Update `McpPage/wwwroot/css/site.css` with CellphoneS red palette**
  Add styles for `#d70018` brand header, product card hover animations, installment tags, price tags, and clean layout spacing.

- [ ] **Step 2: Update `McpPage/Views/Shared/_Layout.cshtml`**
  Include Bootstrap Icons (`cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css`).
  Build the red header: Brand logo "MCP Shop", category drop menu, search form `GET /Products`, hotline, cart button with badge, and user menu.
  Build the e-commerce footer: warranty policies, store info, payment badges.

- [ ] **Step 3: Verify build**
  Run: `dotnet build McpPage/McpPage.csproj`
  Expected: Build succeeded with 0 errors.

---

### Task 3: Home Page Implementation

**Files:**
- Modify: `McpPage/Controllers/HomeController.cs`
- Modify: `McpPage/Views/Home/Index.cshtml`

**Interfaces:**
- Consumes: `ApplicationDbContext`, `HomeViewModel`
- Produces: Interactive homepage with Hero banners, Category icon rows, Flash Sale section, and Category product showcases

- [ ] **Step 1: Update `HomeController.cs`**
  Inject `ApplicationDbContext`. Load categories, flash sale products, and category-grouped products into `HomeViewModel`.

- [ ] **Step 2: Update `Views/Home/Index.cshtml`**
  Render Hero carousel with promotional graphics.
  Render Category quick-select cards.
  Render "Hot Sale / Deal Sốc" with countdown banner and fire icons.
  Render sections for "Laptop & Máy tính", "Điện thoại & Tablet", "Phụ kiện & Âm thanh" with CellphoneS-style cards.

- [ ] **Step 3: Verify build**
  Run: `dotnet build McpPage/McpPage.csproj`
  Expected: Build succeeded with 0 errors.

---

### Task 4: Products Catalog & Details Pages

**Files:**
- Create: `McpPage/Controllers/ProductsController.cs`
- Create: `McpPage/Views/Products/Index.cshtml`
- Create: `McpPage/Views/Products/Details.cshtml`

**Interfaces:**
- Consumes: `ApplicationDbContext`, `ProductListViewModel`, `Product` entity
- Produces: Catalog browsing with category/price/sort filters and product details page

- [ ] **Step 1: Create `ProductsController.cs`**
  Implement `Index(int? categoryId, string? keyword, string? priceRange, string? sortBy)`.
  Implement `Details(int id)`.

- [ ] **Step 2: Create `Views/Products/Index.cshtml`**
  Render traditional sidebar with category radio/links, price filter options, sorting select, and responsive product grid.

- [ ] **Step 3: Create `Views/Products/Details.cshtml`**
  Render breadcrumb navigation, large product image preview, red price highlight, specification table, and standard HTML `<form method="post" asp-controller="Cart" asp-action="AddToCart">`.

- [ ] **Step 4: Verify build**
  Run: `dotnet build McpPage/McpPage.csproj`
  Expected: Build succeeded with 0 errors.

---

### Task 5: Shopping Cart & Checkout (Traditional MVC Session Form)

**Files:**
- Create: `McpPage/Controllers/CartController.cs`
- Create: `McpPage/Views/Cart/Index.cshtml`
- Create: `McpPage/Views/Cart/OrderSuccess.cshtml`

**Interfaces:**
- Consumes: `ISession`, `ApplicationDbContext`, `CartViewModel`, `CheckoutViewModel`
- Produces: Complete cart management, order submission, and order confirmation

- [ ] **Step 1: Create `CartController.cs`**
  Implement session serialization helpers for `List<CartItem>`.
  Implement `Index()` action.
  Implement `AddToCart(int productId, int quantity = 1)` POST action.
  Implement `UpdateQuantity(int productId, int quantity)` POST action.
  Implement `RemoveFromCart(int productId)` POST action.
  Implement `Checkout(CheckoutViewModel model)` POST action creating `Order` & `OrderDetails` in PostgreSQL.

- [ ] **Step 2: Create `Views/Cart/Index.cshtml`**
  Display cart items table with image, price, quantity update form, and remove button.
  Display order summary and customer checkout form with standard form POST.

- [ ] **Step 3: Create `Views/Cart/OrderSuccess.cshtml`**
  Display order confirmation card with order ID, customer details, and back-to-home button.

- [ ] **Step 4: Verify build**
  Run: `dotnet build McpPage/McpPage.csproj`
  Expected: Build succeeded with 0 errors.

---

### Task 6: End-to-End Verification

- [ ] **Step 1: Run comprehensive build verification**
  Run: `dotnet build McpPage/McpPage.csproj`
  Expected: Build succeeded with 0 errors, 0 warnings.

# Design Document: Traditional ASP.NET Core MVC UI for McpPage (CellphoneS Red-White Theme)

- **Date:** 2026-10-31
- **Status:** Proposed
- **Author:** Antigravity & User

## 1. Intent & Context
Build a complete, user-friendly e-commerce interface for `McpPage` using the existing PostgreSQL database data (Categories, Products, Identity Users, Orders). Following the instructor's requirement for a **basic, traditional ASP.NET Core MVC architecture**, all navigation, filtering, search, and cart operations will rely on standard server-rendered Razor views with standard HTTP GET/POST forms (full page reloads) rather than client-side SPA or complex AJAX scripts. The aesthetic will follow the dynamic **red and white** theme of **CellphoneS / Shopee** (`#d70018` brand red, `#ffffff` card backgrounds, `#f4f6f8` page backdrop).

## 2. Goals & Non-Goals

### Goals
- **Styling & Assets:**
  - Import Bootstrap 5.3 and Bootstrap Icons (`bootstrap-icons.css`).
  - Create `McpPage/wwwroot/css/site.css` with CellphoneS red palette (`#d70018`, `#b50014`), rounded card borders (12px), hover effects, red badge tags, and formatted VND currency.
- **Shared Layout (`Views/Shared/_Layout.cshtml`):**
  - Brand header with Logo ("MCP Shop"), Category quick menu, Search bar (submits `GET /Products?keyword=...`), Contact Hotline, Cart button with session item count badge, and User Login/Profile status via ASP.NET Core Identity (`SignInManager`).
  - Notification alert banner (for TempData success/error messages).
  - Professional e-commerce footer (store info, return policy, accepted payment methods, copyright).
- **Home Page (`HomeController.cs` -> `Views/Home/Index.cshtml`):**
  - Hero banner with promotional slides.
  - Horizontal category icon cards.
  - "Hot Sale / Giá Sốc" section featuring discounted products with fire icons and countdown timers.
  - Categorized product showcases (Laptops, Smartphones, Accessories, Audio, Smartwatches) rendered with red price tags, installment badges ("Trả góp 0%"), and 5-star ratings.
- **Catalog Page (`ProductsController.cs` -> `Views/Products/Index.cshtml`):**
  - Traditional sidebar/horizontal filter: Category selection, Price range filters (Dưới 5 triệu, 5-15 triệu, 15-30 triệu, Trên 30 triệu), Sorting dropdown (Giá tăng dần, Giá giảm dần, Tên A-Z).
  - Clean responsive grid of products.
- **Product Details (`ProductsController.cs` -> `Views/Products/Details.cshtml`):**
  - Breadcrumbs navigation.
  - Product image showcase.
  - Price, discount percentage, stock status.
  - Description & technical specifications table.
  - Standard `<form method="post" asp-controller="Cart" asp-action="AddToCart">` with quantity stepper and submit button.
- **Shopping Cart & Checkout (`CartController.cs` -> `Views/Cart/Index.cshtml`):**
  - Session-backed cart management using standard POST actions (`AddToCart`, `UpdateQuantity`, `RemoveFromCart`).
  - Item listing table with subtotal calculations.
  - Simple Checkout form (Customer Name, Phone, Shipping Address) that saves an `Order` and `OrderDetails` to the database and redirects to an Order Success confirmation page.

### Non-Goals
- Real third-party payment gateway integration (VNPAY/Momo webhooks).
- Heavy client-side JavaScript frameworks (React, Angular, Vue).

### Anti-Goals
- Using AJAX for core navigation or cart manipulation when traditional MVC Form submissions are requested.
- Over-complicating state management.

## 3. Approaches Considered & Trade-offs

- **Approach 1 (AJAX Mini-Cart & Reusable Partials):** Rejected because the instructor requires traditional basic MVC.
- **Approach 2 (Selected - Traditional Server-Rendered MVC GET/POST):**
  - Leverages standard Razor View Models, HTML `<form>`, `TempData` flash messages, and Session storage.
  - Perfectly matches university course requirements while delivering a polished, modern visual aesthetic.

## 4. Architectural Details

### Controllers & View Models
- `HomeController`:
  - `Index()`: Loads Categories and Featured/Flash Sale Products.
- `ProductsController`:
  - `Index(int? categoryId, string? keyword, string? priceRange, string? sortBy)`: Queries database with EF Core filters.
  - `Details(int id)`: Loads product by ID with Category info.
- `CartController`:
  - `Index()`: Displays items in cart stored in session (`ISession`).
  - `AddToCart(int productId, int quantity = 1)`: Standard POST action redirecting to Cart or back with `TempData["SuccessMessage"]`.
  - `UpdateQuantity(int productId, int quantity)`: POST action updating cart in session.
  - `RemoveFromCart(int productId)`: POST action removing item from cart.
  - `Checkout(CheckoutViewModel model)`: POST action creating `Customer` (if needed), `Order`, and `OrderDetails` in `ApplicationDbContext`.

### Directory Layout
```
McpPage/
├── Controllers/
│   ├── HomeController.cs
│   ├── ProductsController.cs
│   └── CartController.cs
├── Models/
│   ├── HomeViewModel.cs
│   ├── ProductListViewModel.cs
│   ├── CartItem.cs
│   ├── CartViewModel.cs
│   └── CheckoutViewModel.cs
├── Views/
│   ├── Shared/
│   │   └── _Layout.cshtml
│   ├── Home/
│   │   └── Index.cshtml
│   ├── Products/
│   │   ├── Index.cshtml
│   │   └── Details.cshtml
│   └── Cart/
│       ├── Index.cshtml
│       └── OrderSuccess.cshtml
└── wwwroot/
    └── css/
        └── site.css
```

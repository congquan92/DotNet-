# Dedicated Admin Portal Layout Implementation Plan for McpPage

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Separate the Admin area into an independent Admin Portal with a dedicated layout (`_AdminLayout.cshtml`), a fixed dark sidebar navigation, an admin topbar, and no customer shopping elements.

**Architecture:** Traditional ASP.NET Core MVC layout separation. `Views/Admin/_ViewStart.cshtml` applies `_AdminLayout.cshtml` to all views in `Views/Admin/`. The layout includes a 260px fixed dark sidebar, an admin topbar with logout/user info, and a full-width working canvas.

**Tech Stack:** ASP.NET Core MVC 10.0, Bootstrap 5.3, Bootstrap Icons.

## Global Constraints

- Target framework: `net10.0`.
- Dedicated layout for Admin: `_AdminLayout.cshtml`.
- No customer shopping header or customer footer in the Admin Portal.
- Do NOT run `git commit` or `git push` automatically (per `AGENTS.md`).

---

### Task 1: Create Dedicated `_AdminLayout.cshtml`

**Files:**
- Create: `McpPage/Views/Shared/_AdminLayout.cshtml`

- [ ] **Step 1: Write `_AdminLayout.cshtml`**
  Implement:
  - 260px dark sidebar with brand logo, links for Dashboard, Orders, Products, Accounts, and "Về trang bán hàng".
  - Topbar with "Admin Mode" badge, current admin user name, and logout form.
  - Body container for rendering admin view contents.
  - Minimal admin footer.

---

### Task 2: Configure Admin Views & Clean Up View Navigation

**Files:**
- Create: `McpPage/Views/Admin/_ViewStart.cshtml`
- Modify: `McpPage/Views/Admin/Index.cshtml`
- Modify: `McpPage/Views/Admin/Products.cshtml`
- Modify: `McpPage/Views/Admin/Orders.cshtml`
- Modify: `McpPage/Views/Admin/Accounts.cshtml`

- [ ] **Step 1: Create `McpPage/Views/Admin/_ViewStart.cshtml`**
  Set `Layout = "_AdminLayout";`.

- [ ] **Step 2: Clean up redundant top navbars in Admin views**
  Remove the duplicate horizontal button bar from `Index.cshtml`, `Products.cshtml`, `Orders.cshtml`, and `Accounts.cshtml` since navigation is now permanently available in the sidebar.

---

### Task 3: Build & Verification

- [ ] **Step 1: Stop any active process to avoid file locking**
- [ ] **Step 2: Build project and verify 0 errors**

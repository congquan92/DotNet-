# Đồng bộ Giao diện ECommerceWeb và BlazorPage theo phong cách CellphoneS Red-White của McpPage

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking. (Do NOT run `git commit` or `git push` per workspace rules).

**Goal:** Chuyển đổi toàn diện giao diện của 2 dự án `ECommerceWeb` (Razor Pages) và `BlazorPage` (Blazor Server) sang cùng phong cách thiết kế CellphoneS Red & White (`#d70018`) của `McpPage`: Header đỏ nổi bật, ô tìm kiếm thông minh bo tròn, Hotline, Cart badge, Menu người dùng/Admin Identity, Hero Carousel, Card sản phẩm bo góc đổ bóng, Giỏ hàng 2 cột, và Admin Portal sidebar tối sang trọng.

**Architecture:**
- **CSS & Icons:** Sử dụng hệ thống biến CSS `--cps-red`, `--cps-red-dark`, `--cps-bg`,... kết hợp thư viện **Bootstrap Icons 1.11.3**.
- **Layout:** Đồng bộ Header đỏ (`.header-cps`), Sub-nav danh mục, Announcement bar, Dropdown Menu Identity, và Footer đa cột (`.footer-cps`).
- **Product & Cart Views:** Nâng cấp card sản phẩm hiện đại kèm nhãn giảm giá/trả góp 0%, giá đỏ to đậm, giao diện chi tiết sản phẩm và giỏ hàng 2 cột chuẩn thương mại điện tử.
- **Admin Views:** Cập nhật Sidebar màu tối `#1a1e24` với điểm nhấn đỏ `#d70018`, stat cards và bảng quản lý trực quan.

**Tech Stack:** ASP.NET Core 10, Razor Pages, Blazor Server, Bootstrap 5.3, Bootstrap Icons.

---

## Danh sách Task triển khai

### Task 1: Nâng cấp Theme & Layout cho ECommerceWeb
- [ ] Đồng bộ toàn bộ CSS theme CellphoneS vào `ECommerceWeb/ECommerceWeb/wwwroot/css/site.css`.
- [ ] Thêm liên kết `bootstrap-icons.min.css` vào `Pages/Shared/_Layout.cshtml`.
- [ ] Xây dựng lại Header đỏ CellphoneS trong `_Layout.cshtml`: Logo `MOBI SHOP`, Search bar, Hotline `1800.2097`, Nút Giỏ hàng có badge số lượng thực tế, Dropdown người dùng Identity (Hồ sơ, Đơn hàng, Quản trị Admin, Đăng xuất).
- [ ] Bổ sung thanh danh mục phụ (Sub-nav) và chân trang chuyên nghiệp (`.footer-cps`).
- [ ] Cập nhật `Pages/Shared/_AdminLayout.cshtml` với Sidebar tối `#1a1e24` và theme đỏ.

### Task 2: Làm mới Trang chủ, Chi tiết sản phẩm & Giỏ hàng cho ECommerceWeb
- [ ] Nâng cấp `Pages/Index.cshtml`: Hero Carousel / Banner khuyến mãi, Danh mục trực quan (Quick Icons), Lưới sản phẩm dạng card CellphoneS (`.product-card`) với giá bán đỏ nổi bật, nhãn trả góp/giảm giá.
- [ ] Nâng cấp `Pages/Product.cshtml`: Hình ảnh lớn, hộp thông số kỹ thuật đẹp mắt, hộp ưu đãi `.product-promo-box` và các nút hành động mua hàng/thêm giỏ hàng.
- [ ] Nâng cấp `Pages/Cart.cshtml`: Bố cục 2 cột chuẩn (Danh sách sản phẩm có ảnh, đơn giá, bộ tăng giảm số lượng, tóm tắt tổng tiền bên phải).
- [ ] Build kiểm tra `ECommerceWeb`.

### Task 3: Nâng cấp Theme, Header & Layout cho BlazorPage
- [ ] Nhúng Bootstrap Icons vào `BlazorPage/Components/App.razor`.
- [ ] Đồng bộ các class CSS CellphoneS vào `BlazorPage/wwwroot/app.css`.
- [ ] Nâng cấp `Components/Layout/MainLayout.razor`: Announcement bar, Header đỏ CellphoneS với Search input, Hotline, Cart badge theo dõi reactive qua `CartService`, Dropdown Identity và Sub-nav.
- [ ] Nâng cấp `Components/Layout/AdminLayout.razor` & `AdminNavMenu.razor` sang phong cách Sidebar tối chuyên nghiệp `#1a1e24` với highlight đỏ `#d70018`.

### Task 4: Làm mới các trang Blazor (Home, Products, ProductDetail, Cart) & Nghiệm thu
- [ ] Nâng cấp `Components/Pages/Home.razor`: Hero Promo Carousel, Quick Categories, Flash Sale đỏ rực rỡ và danh sách sản phẩm nổi bật.
- [ ] Nâng cấp `Components/Pages/Products.razor` & `ProductDetail.razor`: Giao diện sản phẩm hiện đại theo phong cách McpPage.
- [ ] Nâng cấp `Components/Pages/Cart.razor`: Thiết kế giỏ hàng 2 cột với thông tin khách hàng và nút đặt hàng nổi bật.
- [ ] Build nghiệm thu toàn bộ solution: `McpPage`, `ECommerceWeb`, `BlazorPage` đều build 0 errors, 0 warnings.

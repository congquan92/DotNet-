using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceWeb.Entities;

public enum OrderStatus
{
    Pending = 0,     // Chờ xác nhận
    Processing = 1,  // Đang xử lý
    Shipped = 2,     // Đang giao hàng
    Delivered = 3,   // Đã giao thành công
    Cancelled = 4    // Đã hủy
}

public enum PaymentMethod
{
    COD = 0,           // Thanh toán khi nhận hàng
    BankTransfer = 1,  // Chuyển khoản ngân hàng
    CreditCard = 2,    // Thẻ tín dụng
    EWallet = 3        // Ví điện tử (VNPay, Momo...)
}

public enum PaymentStatus
{
    Unpaid = 0,   // Chưa thanh toán
    Paid = 1,     // Đã thanh toán
    Refunded = 2  // Đã hoàn tiền
}

public class Order : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string OrderCode { get; set; } = string.Empty;

    public int? UserId { get; set; }
    public User? User { get; set; }

    [Required]
    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(20)]
    public string CustomerPhone { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(100)]
    public string? CustomerEmail { get; set; }

    [Required]
    [StringLength(300)]
    public string ShippingAddress { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    // Navigation properties
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

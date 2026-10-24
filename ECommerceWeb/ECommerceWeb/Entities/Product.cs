using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceWeb.Entities;

public class Product : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 100_000_000_000)]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? OriginalPrice { get; set; }

    [Required]
    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Brand { get; set; } = string.Empty;

    [StringLength(50)]
    public string Storage { get; set; } = string.Empty;

    [StringLength(50)]
    public string Color { get; set; } = string.Empty;

    [StringLength(200)]
    public string Display { get; set; } = string.Empty;

    [StringLength(100)]
    public string Chip { get; set; } = string.Empty;

    [StringLength(200)]
    public string Camera { get; set; } = string.Empty;

    [StringLength(200)]
    public string Battery { get; set; } = string.Empty;

    public int StockQuantity { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    // Foreign key & navigation property
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

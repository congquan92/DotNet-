using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerceWeb.Entities;

namespace ECommerceWeb.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Property(p => p.OriginalPrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(1000);

        builder.Property(p => p.Stock)
            .HasDefaultValue(100);

        builder.Property(p => p.Brand)
            .HasMaxLength(100);

        builder.Property(p => p.Storage)
            .HasMaxLength(50);

        builder.Property(p => p.Color)
            .HasMaxLength(50);

        builder.Property(p => p.Display)
            .HasMaxLength(200);

        builder.Property(p => p.Chip)
            .HasMaxLength(100);

        builder.Property(p => p.Camera)
            .HasMaxLength(200);

        builder.Property(p => p.Battery)
            .HasMaxLength(200);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

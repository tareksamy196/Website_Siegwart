using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.DAL.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.Property(x => x.TitleEn)
                .IsRequired()
                .HasMaxLength(150)
                .UseCollation("Latin1_General_100_CI_AI");

            builder.Property(x => x.TitleAr)
                .IsRequired()
                .HasMaxLength(150)
                .UseCollation("Arabic_100_CI_AI");

            builder.Property(x => x.DescriptionEn)
                .IsRequired()
                .UseCollation("Latin1_General_100_CI_AI");

            builder.Property(x => x.DescriptionAr)
                .IsRequired()
                .UseCollation("Arabic_100_CI_AI");

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.SortOrder)
                .HasDefaultValue(100);

            // Indexes
            builder.HasIndex(x => x.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");

            builder.HasIndex(x => new { x.IsDeleted, x.IsActive, x.SortOrder })
                .HasDatabaseName("IX_Products_Deleted_Active_Sort");

            builder.HasOne(x => x.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ConfigureSeoEntity();
        }
    }
}
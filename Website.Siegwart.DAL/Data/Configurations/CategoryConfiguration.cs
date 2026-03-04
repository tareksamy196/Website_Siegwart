using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.DAL.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.Property(x => x.NameEn)
                .IsRequired()
                .HasMaxLength(150)
                .UseCollation("Latin1_General_100_CI_AI");

            builder.Property(x => x.NameAr)
                .IsRequired()
                .HasMaxLength(150)
                .UseCollation("Arabic_100_CI_AI");

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            // Unique names
            builder.HasIndex(x => x.NameEn)
                .IsUnique()
                .HasDatabaseName("IX_Categories_NameEn");

            builder.HasIndex(x => x.NameAr)
                .IsUnique()
                .HasDatabaseName("IX_Categories_NameAr");

            builder.HasIndex(x => new { x.IsDeleted, x.IsActive })
                .HasDatabaseName("IX_Categories_Deleted_Active");

            builder.HasMany(x => x.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ConfigureSeoEntity();
        }
    }
}
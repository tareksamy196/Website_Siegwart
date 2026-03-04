using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.DAL.Data.Configurations
{
    public static class EntityTypeBuilderExtensions
    {
        public static void ConfigureSeoEntity<T>(this EntityTypeBuilder<T> builder)
            where T : SeoEntity
        {
            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(220)
                .IsUnicode(false);

            builder.HasIndex(x => x.Slug)
                .IsUnique()
                .HasDatabaseName($"IX_{typeof(T).Name}_Slug");

            // SEO titles
            builder.Property(x => x.SeoTitleEn)
                .HasMaxLength(70)
                .UseCollation("Latin1_General_100_CI_AI");

            builder.Property(x => x.SeoTitleAr)
                .HasMaxLength(70)
                .UseCollation("Arabic_100_CI_AI");

            builder.Property(x => x.SeoDescriptionEn)
                .HasMaxLength(500)
                .UseCollation("Latin1_General_100_CI_AI");

            builder.Property(x => x.SeoDescriptionAr)
                .HasMaxLength(500)
                .UseCollation("Arabic_100_CI_AI");

            builder.Property(x => x.SeoKeywords)
                .HasMaxLength(500);

            builder.Property(x => x.OgImageUrl)
                .HasMaxLength(500);

            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            builder.HasIndex(x => x.IsDeleted)
                .HasDatabaseName($"IX_{typeof(T).Name}_IsDeleted");

            builder.Property(x => x.CreatedOn)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(x => x.LastModifiedOn)
                .HasColumnType("datetime2")
                .IsRequired(false);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(100);

            builder.Property(x => x.LastModifiedBy)
                .HasMaxLength(100);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
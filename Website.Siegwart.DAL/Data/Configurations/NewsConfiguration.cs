using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.DAL.Data.Configurations
{
    public class NewsConfiguration : IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.ToTable("News");

            builder.Property(n => n.TitleEn)
                .IsRequired()
                .HasMaxLength(200)
                .UseCollation("Latin1_General_100_CI_AI");

            builder.Property(n => n.TitleAr)
                .IsRequired()
                .HasMaxLength(200)
                .UseCollation("Arabic_100_CI_AI");

            // ✅ Summary fields — shown on listing cards
            builder.Property(n => n.SummaryEn)
                .HasMaxLength(300)
                .UseCollation("Latin1_General_100_CI_AI");

            builder.Property(n => n.SummaryAr)
                .HasMaxLength(300)
                .UseCollation("Arabic_100_CI_AI");

            builder.Property(n => n.ContentEn)
                .IsRequired()
                .UseCollation("Latin1_General_100_CI_AI");

            builder.Property(n => n.ContentAr)
                .IsRequired()
                .UseCollation("Arabic_100_CI_AI");

            builder.Property(n => n.ImageUrl)
                .HasMaxLength(500);

            // ✅ Nullable — only set when admin publishes
            builder.Property(n => n.PublishedOn)
                .HasColumnType("datetime2")
                .IsRequired(false);

            builder.Property(n => n.IsPublished)
                .HasDefaultValue(false);

            // Indexes — most common queries are by published status + date
            builder.HasIndex(n => new { n.IsDeleted, n.IsPublished, n.PublishedOn })
                .HasDatabaseName("IX_News_Deleted_Published_Date");

            builder.ConfigureSeoEntity();
        }
    }
}
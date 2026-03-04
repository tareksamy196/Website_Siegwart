using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.DAL.Data.Configurations
{
    public class VideoMediaConfiguration : IEntityTypeConfiguration<VideoMedia>
    {
        public void Configure(EntityTypeBuilder<VideoMedia> builder)
        {
            builder.ToTable("VideoMedia");

            builder.Property(v => v.VideoId)
                .IsRequired()
                .HasMaxLength(11)
                .IsUnicode(false);

            builder.HasIndex(v => v.VideoId)
                .IsUnique()
                .HasDatabaseName("IX_VideoMedia_VideoId");

            builder.Property(v => v.SourceUrl)
                .HasMaxLength(1000)
                .IsUnicode(false);

            builder.Property(v => v.TitleEn)
                .HasMaxLength(250)
                .UseCollation("Latin1_General_100_CI_AI");

            builder.Property(v => v.TitleAr)
                .HasMaxLength(250)
                .UseCollation("Arabic_100_CI_AI");

            builder.Property(v => v.DescriptionEn)
                .HasMaxLength(2000);

            builder.Property(v => v.DescriptionAr)
                .HasMaxLength(2000);

            builder.Property(v => v.ThumbnailUrl)
                .HasMaxLength(500)
                .IsUnicode(false);

            builder.Property(v => v.SortOrder)
                .HasDefaultValue(100);

            builder.Property(v => v.Category)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(v => v.IsPublished)
                .HasDefaultValue(true);

            // Indexes
            builder.HasIndex(v => new { v.IsDeleted, v.IsPublished, v.SortOrder })
                .HasDatabaseName("IX_VideoMedia_Deleted_Published_Sort");

            builder.HasIndex(v => v.Category)
                .HasDatabaseName("IX_VideoMedia_Category");

            builder.ConfigureSeoEntity();
        }
    }
}
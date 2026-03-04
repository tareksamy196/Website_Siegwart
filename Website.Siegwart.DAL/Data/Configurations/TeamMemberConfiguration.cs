using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.DAL.Data.Configurations
{
    public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
    {
        public void Configure(EntityTypeBuilder<TeamMember> builder)
        {
            builder.ToTable("TeamMembers");

            builder.Property(e => e.NameEn)
                .IsRequired()
                .HasMaxLength(120)
                .UseCollation("Latin1_General_100_CI_AI");

            builder.Property(e => e.NameAr)
                .IsRequired()
                .HasMaxLength(120)
                .UseCollation("Arabic_100_CI_AI");

            builder.Property(e => e.TitleEn)
                .IsRequired()
                .HasMaxLength(90)
                .UseCollation("Latin1_General_100_CI_AI");

            builder.Property(e => e.TitleAr)
                .IsRequired()
                .HasMaxLength(90)
                .UseCollation("Arabic_100_CI_AI");

            builder.Property(e => e.Category)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            builder.Property(e => e.Order)
                .HasDefaultValue(100);

            builder.Property(e => e.Email)
                .HasMaxLength(100);

            builder.Property(e => e.Phone)
                .HasMaxLength(20);

            builder.Property(e => e.LinkedInUrl)
                .HasMaxLength(300);

            builder.Property(e => e.BioEn)
                .HasMaxLength(1000);

            builder.Property(e => e.BioAr)
                .HasMaxLength(1000);

            builder.Property(e => e.IsActive)
                .HasDefaultValue(true);

            // Indexes
            builder.HasIndex(e => new { e.IsDeleted, e.IsActive, e.Order })
                .HasDatabaseName("IX_TeamMembers_Deleted_Active_Order");

            builder.HasIndex(e => e.Category)
                .HasDatabaseName("IX_TeamMembers_Category");

            builder.ConfigureSeoEntity();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.DAL.Data.Configurations
{
    public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
    {
        public void Configure(EntityTypeBuilder<ContactMessage> builder)
        {
            builder.ToTable("ContactMessages");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Phone)
                .HasMaxLength(50);

            builder.Property(e => e.Subject)
                .HasMaxLength(300);

            builder.Property(e => e.Message)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(e => e.AdminNotes)
                .HasMaxLength(1000);

            builder.Property(e => e.IpAddress)
                .HasMaxLength(50);

            builder.Property(e => e.UserAgent)
                .HasMaxLength(500);

            builder.Property(e => e.IsRead)
                .HasDefaultValue(false);

            builder.Property(e => e.IsReplied)
                .HasDefaultValue(false);

            builder.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(e => e.CreatedOn)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            // Indexes
            builder.HasIndex(e => new { e.IsRead, e.IsDeleted, e.CreatedOn })
                .HasDatabaseName("IX_ContactMessages_Read_Deleted_Date");

            builder.HasIndex(e => e.Email)
                .HasDatabaseName("IX_ContactMessages_Email");

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
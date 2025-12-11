using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Entities
{
    public class BrochureFile : SimpleBaseEntity
    {
        public string Title { get; set; }

        public string PdfFileUrl { get; set; }

        public string Slug { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }

    public class BrochureFileConfiguration : IEntityTypeConfiguration<BrochureFile>
    {
        public void Configure(EntityTypeBuilder<BrochureFile> builder)
        {
            builder.Property(b => b.Title).HasMaxLength(400).IsRequired();
            builder.Property(b => b.PdfFileUrl).HasMaxLength(1500).IsRequired();
            builder.Property(b => b.Slug).HasMaxLength(450).IsRequired();
            builder.HasIndex(b => b.Slug).IsUnique();
            builder.Property(b => b.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(b => b.UpdatedAt).IsRequired(false);
        }
    }
}

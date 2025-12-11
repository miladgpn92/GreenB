using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Entities
{
    public class SubmissionCategory : SimpleBaseEntity
    {
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }

    public class SubmissionCategoryConfiguration : IEntityTypeConfiguration<SubmissionCategory>
    {
        public void Configure(EntityTypeBuilder<SubmissionCategory> builder)
        {
            builder.Property(c => c.Title).HasMaxLength(200).IsRequired();
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.UpdatedAt).IsRequired(false);
        }
    }
}

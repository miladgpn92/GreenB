using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Entities
{
    public class UserSubmission : SimpleBaseEntity
    {
        public string Phone { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int SubmissionCategoryId { get; set; }
        public SubmissionCategory SubmissionCategory { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }

    public class UserSubmissionConfiguration : IEntityTypeConfiguration<UserSubmission>
    {
        public void Configure(EntityTypeBuilder<UserSubmission> builder)
        {
            builder.Property(u => u.Phone).HasMaxLength(50).IsRequired();
            builder.Property(u => u.FirstName).HasMaxLength(150);
            builder.Property(u => u.LastName).HasMaxLength(150);
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(u => u.UpdatedAt).IsRequired(false);

            builder.HasOne(u => u.SubmissionCategory)
                .WithMany()
                .HasForeignKey(u => u.SubmissionCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

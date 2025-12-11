using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Entities
{
    public class SmsQueue : SimpleBaseEntity
    {
        public string Phone { get; set; }
        public string Text { get; set; }
        public bool IsSent { get; set; }
        public int AttemptCount { get; set; }
        public string LastError { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? SentAt { get; set; }
        public DateTime? NextAttemptAt { get; set; }
    }

    public class SmsQueueConfiguration : IEntityTypeConfiguration<SmsQueue>
    {
        public void Configure(EntityTypeBuilder<SmsQueue> builder)
        {
            builder.Property(s => s.Phone).HasMaxLength(50).IsRequired();
            builder.Property(s => s.Text).HasMaxLength(1600).IsRequired();
            builder.Property(s => s.LastError).HasMaxLength(2000);
            builder.Property(s => s.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(s => s.IsSent).HasDefaultValue(false);
            builder.Property(s => s.AttemptCount).HasDefaultValue(0);
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.AI
{
    public class AIConversationHistory : SimpleBaseEntity
    {
        public int UserId { get; set; }

        public string ChatGuid { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Messages { get; set; } // JSON serialized messages

        public string Status { get; set; }

        public string PageSource { get; set; } // منبع درخواست (مقاله، محصول، و غیره)

        [ForeignKey("UserId")]
        #nullable enable
        public ApplicationUser? ApplicationUser { get; set; }

    }

    public class AIConversationHistoryConfiguration : IEntityTypeConfiguration<AIConversationHistory>
    {
        public void Configure(EntityTypeBuilder<AIConversationHistory> builder)
        {
            builder.Property(a => a.Title).HasMaxLength(400).IsRequired();
            builder.Property(a => a.ChatGuid).HasMaxLength(36).IsRequired();
            builder.HasIndex(a => a.ChatGuid).IsUnique();

            builder.HasOne(m => m.ApplicationUser).WithMany().HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class AspNetUserJwtRefreshToken : IEntity
    {
        public AspNetUserJwtRefreshToken()
        {
            CreateDate = DateTime.Now;
        }
        public int Id { get; set; }
        public Guid JwtRefreshToken{ get; set; }
        public string SecurityStamp { get; set; }
        public DateTime CreateDate { get; set; }
        public int UserId{ get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
    public class AspNetUserJwtRefreshTokenConfig : IEntityTypeConfiguration<AspNetUserJwtRefreshToken>
    {
        public void Configure(EntityTypeBuilder<AspNetUserJwtRefreshToken> builder)
        {
            builder.HasKey(m=>m.Id);
            builder.Property(m => m.CreateDate).HasColumnType("Date");
            builder.Property(m => m.SecurityStamp).HasColumnType("varchar(32)");
            builder.HasOne(m => m.ApplicationUser).WithMany(m => m.AspNetUserJwtRefreshTokens).HasForeignKey(m => m.UserId).IsRequired();
        }
    }
}

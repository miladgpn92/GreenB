using Common.Enums;
 
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Entities
{
    public class ApplicationUser : IdentityUser<int>, IEntity
    {
        public ApplicationUser()
        {
            CreateDate = DateTime.Now;
            LockoutEnabled = false;
        }

        public string Name { get; set; }

        public string Family { get; set; }

        public string PicUrl { get; set; }

        public Gender? Gender { get; set; }

        public bool IsActive { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime CreateDate { get; set; }

        public string NationalCode { get; set; }

        public UsersRole UserRole { get; set; }

        //This property is used to store a validation code that is used to verify a user's identity.
        public string ValidationCode { get; set; }

        // This property is used to store the date and time of the last time a validation code was sent. It is a nullable DateTime type, meaning that it can be assigned a DateTime value or null.
        public DateTime? LastSendValidationCode { get; set; }


        public bool IsRegisterComplete { get; set; }

        public ICollection<AspNetUserJwtRefreshToken> AspNetUserJwtRefreshTokens { get; set; }


        public virtual ICollection<ActiveSession> ActiveSessions { get; set; }

 

    }

    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(a => a.UserName).HasMaxLength(200);
            builder.Property(a => a.PhoneNumber).HasColumnType("varchar(11)");
            builder.Property(a => a.NationalCode).HasColumnType("varchar(10)");
            builder.Property(a => a.Name).HasMaxLength(100);
            builder.Property(a => a.Family).HasMaxLength(100);
        }
    }

}

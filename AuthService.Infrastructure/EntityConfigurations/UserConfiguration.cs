using AuthService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(t => t.Id);
            builder.Property(x=>x.UserName)
                .IsRequired();
            builder.Property(x=>x.Role)
                .IsRequired();

            builder.Property(x=>x.Email)
                .IsRequired();
            builder.Property(x=>x.PasswordHash)
                .IsRequired();

            builder.Property(x=>x.RefreshToken)
                .IsRequired();

            builder.Property(x=>x.RefreshTokenExpiration)
                .IsRequired();

            builder.Property(s => s.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();
        }
    }
}

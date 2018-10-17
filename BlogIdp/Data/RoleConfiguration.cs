using BlogDemo.Core.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogIdp.Data
{

    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            //builder.Property(prop => prop.Id).IsRequired();
            //builder.Property(prop => prop.Title).IsRequired().HasMaxLength(20);
           // builder.Property(prop => prop.Body).IsRequired().HasColumnType("nvarchar(max)");
            builder.ToTable("AspNetRoles").Property(c => c.Id).IsRequired().HasMaxLength(450);
            builder.Property(prop => prop.NormalizedName).HasMaxLength(256);
            //builder.Property(prop => prop.o).IsRequired().HasColumnType("nvarchar(max)");
        }
    }
}

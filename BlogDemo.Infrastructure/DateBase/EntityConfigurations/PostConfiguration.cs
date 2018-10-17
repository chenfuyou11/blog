using System;
using System.Collections.Generic;
using System.Text;
using BlogDemo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogDemo.Infrastructure.DateBase.EntityConfigurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            //builder.Property(prop => prop.Id).IsRequired();
            //builder.Property(prop => prop.Title).IsRequired().HasMaxLength(20);
            //builder.Property(prop => prop.Body).IsRequired().HasColumnType("nvarchar(max)");
            builder.ToTable("T_Posts").Property(c => c.Id).IsRequired();
            builder.Property(prop => prop.Remark).HasMaxLength(200);
            
        }
    }
}

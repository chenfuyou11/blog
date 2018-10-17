using AutoMapper;
using BlogDemo.Core.Entities;
using BlogDemo.Infrastructure.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogDemo.Api.Extensions
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostDTO>().ForMember(dest=>dest.Updatetime,opt=>opt.MapFrom(src=>src.LastModified));
            CreateMap<PostDTO, Post>();
            CreateMap<PostAddResource, Post>();
            CreateMap<PostUpdateResource, Post>();
            CreateMap<PostImage, PostImageResource>();
            CreateMap<PostImageResource, PostImage>();
        }
    }
}

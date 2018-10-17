using BlogDemo.Core.Entities;
using BlogDemo.Core.Interfaces;
using BlogDemo.Infrastructure.DateBase;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using BlogDemo.Infrastructure.Services;
using BlogDemo.Infrastructure.Resources;
using BlogDemo.Infrastructure.Extensions;
 
namespace BlogDemo.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
       
        
        public readonly MyContext _myContext;
        private readonly IPropertyMappingContainer _propertyMappingContainer;
        public PostRepository(MyContext myContext, IPropertyMappingContainer propertyMappingContainer)
        {
            _myContext = myContext;
            _propertyMappingContainer = propertyMappingContainer;
        }

        public void AddPost(Post post)
        {
            _myContext.Posts.Add(post);
        }

        public async Task<PaginatedList<Post>> GetPostsAsync(PostParameters parameters)
        {
           
            var Query = _myContext.Posts.AsQueryable();
            if (!string.IsNullOrEmpty(parameters.Title))
            {
                var title = parameters.Title.ToLowerInvariant();
                Query =  Query.Where(x => x.Title.ToLowerInvariant().Contains(title));
 
            }
            Query = Query.ApplySort(parameters.OrderBy, _propertyMappingContainer.Resolve<PostDTO, Post>());

           // Query = Query.OrderBy(x => x.Id);

            var count = await Query.CountAsync();
            var data = await Query.Skip(parameters.PageIndex * parameters.PageSize).Take(parameters.PageSize).ToListAsync();
            return new PaginatedList<Post>(parameters.PageIndex, parameters.PageSize,count,data);
        }

        public async Task<Post> GetPostId(int Id)
        {
             var post= await _myContext.Posts.FindAsync(Id);
            return post;
        }
        public void Delete(Post post)
        {
            _myContext.Posts.Remove(post);
        }

        public void Update(Post post)
        {
            _myContext.Entry(post).State = EntityState.Modified;
        }


    }
}

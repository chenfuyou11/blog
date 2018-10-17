using BlogDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlogDemo.Core.Interfaces
{
   public interface IPostRepository
    {
       Task<PaginatedList<Post>>  GetPostsAsync(PostParameters parameters);
        void AddPost(Post post);
        Task<Post> GetPostId(int Id);
        void Delete(Post post);
        void Update(Post post);


        

    }
}

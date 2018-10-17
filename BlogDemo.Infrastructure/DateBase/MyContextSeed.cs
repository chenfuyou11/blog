using BlogDemo.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDemo.Infrastructure.DateBase
{
   public class MyContextSeed
    {
        public static async Task SeedAsyn(MyContext myContext,ILoggerFactory loggerFactory,int retry=0)
        {
            int retryFroAvailability = retry;
            try
            {
                if(!myContext.Posts.Any())
                {
                    List<Post> posts = new List<Post>() {
                        new Post(){Title="post title1",Author="chen",Body="body1",LastModified=DateTime.Now},
                        new Post(){Title="post title2",Author="chen2",Body="body2",LastModified=DateTime.Now},
                        new Post(){Title="post title3",Author="chen3",Body="body3",LastModified=DateTime.Now},
                        new Post(){Title="post title4",Author="chen4",Body="body4",LastModified=DateTime.Now},

                    };

                    myContext.Posts.AddRange(posts);
                    await myContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if(retryFroAvailability<10)
                {
                    retryFroAvailability++;
                    var logger = loggerFactory.CreateLogger<MyContextSeed>();
                    logger.LogError(ex.Message);
                    await SeedAsyn(myContext, loggerFactory, retryFroAvailability);
                }
            }



        }





    }
}

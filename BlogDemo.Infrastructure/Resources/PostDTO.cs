using BlogDemo.Core.Entities;
using BlogDemo.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogDemo.Infrastructure.Resources
{
    public class PostDTO 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public DateTime Updatetime { get; set; }
        public string Remark { get; set; }
    }
}

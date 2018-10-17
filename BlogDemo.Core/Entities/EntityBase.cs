using BlogDemo.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogDemo.Core.Entities
{
    public abstract class EntityBase : IEntity
    {
        public int Id { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BlogDemo.Infrastructure.Resources
{
   public class PostAddOrUpdateResourceValidator<T> : AbstractValidator<T> where T: PostAddOrUpdateResource
    {
        public PostAddOrUpdateResourceValidator()
        {
            RuleFor(x => x.Title).NotEmpty()
                .WithName("标题").WithMessage("required|{PropertyName}是必须填写的")
                .MaximumLength(50).WithMessage("maxLength|{PropertyName}的最大长度是{MaxLength}");

            RuleFor(x => x.Body).NotEmpty()
                .WithName("正文").WithMessage("required|{PropertyName}是必须填写的")
                .MinimumLength(10).WithMessage("minLength|{PropertyName}的最小长度是{MinLength}");
        }
    }
}

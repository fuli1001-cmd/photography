using FluentValidation;
using Photography.Services.Post.API.Application.Commands.PublishPost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Validators
{
    public class PublishPostCommandValidator : AbstractValidator<PublishPostCommand>
    {
        public PublishPostCommandValidator()
        {
            
        }
    }
}

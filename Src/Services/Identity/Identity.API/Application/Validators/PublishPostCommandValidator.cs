using FluentValidation;
using Photography.Services.Identity.API.Application.Commands.LikePost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Identity.API.Application.Validators
{
    public class PublishPostCommandValidator : AbstractValidator<LikePostCommand>
    {
        public PublishPostCommandValidator()
        {
            
        }
    }
}

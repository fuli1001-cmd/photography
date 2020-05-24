using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Validators
{
    public class FollowCommandValidator : AbstractValidator<FollowCommand>
    {
        public FollowCommandValidator()
        {
            
        }
    }
}

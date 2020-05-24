using FluentValidation;
using Photography.Services.Order.API.Application.Commands.ConfirmShot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Application.Validators
{
    public class ConfirmShotCommandValidator : AbstractValidator<ConfirmShotCommand>
    {
        public ConfirmShotCommandValidator()
        {
            
        }
    }
}

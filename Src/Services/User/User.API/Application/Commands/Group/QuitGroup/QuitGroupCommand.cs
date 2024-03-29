﻿using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Group.QuitGroup
{
    public class QuitGroupCommand : IRequest<bool>
    {
        [Required]
        public Guid GroupId { get; set; }
    }
}

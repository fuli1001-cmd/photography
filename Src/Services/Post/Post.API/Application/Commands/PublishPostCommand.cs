﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands
{
    public class PublishPostCommand : IRequest<bool>
    {
    }
}

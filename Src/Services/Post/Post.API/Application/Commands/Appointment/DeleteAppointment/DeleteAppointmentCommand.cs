using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Application.Commands.Appointment.DeleteAppointment
{
    public class DeleteAppointmentCommand : IRequest<bool>
    {
        public Guid AppointmentId { get; set; }
    }
}

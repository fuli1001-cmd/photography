using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Identity.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nickname { get; set; }
        public string Phonenumber { get; set; }
        public string Avatar { get; set; }
        public bool Gender { get; set; }
        public DateTime Birthday { get; set; }
        public UserType UserType { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Sign { get; set; }
    }

    public enum UserType
    {
        Photographer,
        Model
    }
}

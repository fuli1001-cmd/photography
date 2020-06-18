using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.WebApps.Management.ViewModels
{
    public class User
    {
        public Guid Id { get; set; }

        public string Nickname { get; set; }

        public string Avatar { get; set; }

        public string Sign { get; set; }

        public string BackgroundImage { get; set; }

        public int? Gender { get; set; }

        public double? Birthday { get; set; }

        public int? UserType { get; set; }

        public string Province { get; set; }

        public string City { get; set; }
    }
}

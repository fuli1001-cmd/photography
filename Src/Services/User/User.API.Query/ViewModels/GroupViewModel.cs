using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.API.Query.ViewModels
{
    public class GroupViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Notice { get; set; }
        public string Avatar { get; set; }
        public bool Muted { get; set; }
        public Guid OwnerId { get; set; }
        public IEnumerable<GroupUserViewModel> Members { get; set; }
    }
}

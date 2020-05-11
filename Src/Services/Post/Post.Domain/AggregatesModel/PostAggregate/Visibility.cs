using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public enum Visibility
    {
        Public,
        AllFriends,
        SelectedFriends,
        Password
    }
}

using Newtonsoft.Json;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photography.Services.Post.API.Query.Extensions
{
    public static class PostExtensions
    {
        //public static PostViewModel ToPostViewModel(this Post.Domain.AggregatesModel.PostAggregate.Post post)
        //{
        //    var postViewModel = JsonConvert.DeserializeObject<PostViewModel>(JsonConvert.SerializeObject(post));
        //    postViewModel.PostAttachments = post.PostAttachments.Select(a => JsonConvert.DeserializeObject<PostAttachmentViewModel>(JsonConvert.SerializeObject(a))).ToList();
        //    postViewModel.User = JsonConvert.DeserializeObject<UserViewModel>(JsonConvert.SerializeObject(post.User));
        //    return postViewModel;
        //}
    }
}

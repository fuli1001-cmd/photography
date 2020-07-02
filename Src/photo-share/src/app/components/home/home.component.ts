import { Component, OnInit } from '@angular/core';
import { PostService } from 'src/app/services/post.service';
import { ActivatedRoute } from '@angular/router';
import { Post } from 'src/app/models/post';
import { ConfigService } from 'src/app/services/config.service';
import { Attachment } from 'src/app/models/attachment';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  posts: Post[];

  constructor(private postService: PostService, 
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe(async params => {
      // 获取url参数
      let postId = this.activatedRoute.snapshot.queryParamMap.get('postid');
      let sharedUserId = this.activatedRoute.snapshot.queryParamMap.get('userid');
      let privateTag = this.activatedRoute.snapshot.queryParamMap.get('privateTag');

      console.log(`postId: ${postId}, sharedUserId: ${sharedUserId}, privateTag: ${privateTag}`);

      this.posts = [];

      // 获取帖子数据
      if (sharedUserId) {
        if (postId) {
          let post = await this.postService.getPostAsync(postId, sharedUserId);
          if (post)
            this.posts.push(post);
        } else if (privateTag) {
          this.posts = await this.postService.getTagPostsAsync(privateTag, sharedUserId);
        } else {
          this.posts = await this.postService.getUserPostsAsync(sharedUserId);
        }

        if (!this.posts)
          this.posts = [];

        this.posts.forEach(p => this.setPost(p));
      }
    });
  }

  // 设置帖子的一些属性
  setPost(post: Post): void {
    if (post.updatedTime) 
      post.updatedTime = new Date(post.updatedTime * 1000);
    
    post.createdTime = new Date(post.createdTime * 1000);
    post.user.avatar = ConfigService.config.fileServer + '/' + post.user.avatar;
    post.postAttachments.forEach(a => this.setAttachment(a));
    
    if (post.forwardedPost) {
      post.forwardedPost.postAttachments.forEach(a => this.setAttachment(a));
      post.postAttachments = post.forwardedPost.postAttachments;

      if (post.showOriginalText)
        post.text += " " + post.forwardedPost.user.nickname + post.forwardedPost.text;
    }
  }

  // 设置附件文件路径
  setAttachment(attachment: Attachment): void {
    attachment.name = ConfigService.config.fileServer + '/' + attachment.name;
    attachment.thumbnail = ConfigService.config.fileServer + '/' + attachment.thumbnail;
  }

}

import { Component, OnInit } from '@angular/core';
import { PostService } from 'src/app/services/post.service';
import { ActivatedRoute } from '@angular/router';
import { Post } from 'src/app/models/post';
import { ConfigService } from 'src/app/services/config.service';
import { Attachment } from 'src/app/models/attachment';
import { PagedPost } from 'src/app/models/paged-post';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  pagedPosts: PagedPost;
  param: string;

  constructor(private postService: PostService, 
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe(async params => {
      // 获取url参数
      this.param = this.activatedRoute.snapshot.queryParamMap.get('s');

      console.log(`param: ${this.param}`);

      // 获取帖子数据
      this.pagedPosts = await this.getPagedPostsAsync(1);
    });
  }

  async getPagedPostsAsync(pageNumber: number): Promise<PagedPost> {
    var pagedPosts = await this.postService.getPostsAsync(this.param, pageNumber);

    if (pagedPosts && pagedPosts.data)
      pagedPosts.data.forEach(p => this.setPost(p));

    console.log(pagedPosts);

    return pagedPosts;
  }

  async onScroll(): Promise<void> {
    console.log('scrolled!!');

    if (this.pagedPosts.pagingInfo.currentPage < this.pagedPosts.pagingInfo.totalPages) {
      var pagedPosts = await this.getPagedPostsAsync(this.pagedPosts.pagingInfo.currentPage + 1);
      if (pagedPosts) {
        this.pagedPosts.pagingInfo = pagedPosts.pagingInfo;
        pagedPosts.data.forEach(p => this.pagedPosts.data.push(p));
      }
    }
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
    console.log(ConfigService.config.fileServer);
    attachment.name = ConfigService.config.fileServer + '/' + attachment.name;
    attachment.thumbnail = ConfigService.config.fileServer + '/' + attachment.thumbnail;
  }

}

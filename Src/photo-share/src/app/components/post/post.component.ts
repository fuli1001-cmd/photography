import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PostService } from 'src/app/services/post.service';
import { Post } from 'src/app/models/post';
import { ConfigService } from '../../services/config.service';
import { StateService } from 'src/app/services/state.service';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css']
})
export class PostComponent implements OnInit {

  @ViewChild('videoPlayer') videoplayer: ElementRef;

  post: Post;
  password: string;
  showPhotos: boolean;
  showBigPhoto: boolean;
  selectedPhotoIndex: number;

  constructor(
    private postService: PostService, 
    private stateService: StateService,
    private activatedRoute: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.showBigPhoto = false;

    this.activatedRoute.queryParams.subscribe(async params => {
      let postId = this.activatedRoute.snapshot.queryParamMap.get('postid');
      let userId = this.activatedRoute.snapshot.queryParamMap.get('userid');
      if (postId && userId) 
        this.post = await this.getPostAsync(postId, userId);

        // 设置照片的可见性，visibility为2表示需要密码查看
        this.showPhotos = this.post && !this.post.viewPassword;
    });
  }

  async getPostAsync(postId: string, userId: string): Promise<Post> {
    let post = await this.postService.getPostAsync(postId, userId);

    if (!post)
      return post;

    if (post.updatedTime) 
      post.updatedTime = new Date(post.updatedTime * 1000);
    
    post.createdTime = new Date(post.createdTime * 1000);
    post.user.avatar = ConfigService.config.fileServer + post.user.avatar;
    post.postAttachments.forEach(a => {
      if (a.attachmentType == 1) {
        let tempName = a.name;
        a.name = ConfigService.config.fileServer + a.thumbnail;
        a.thumbnail = ConfigService.config.fileServer + tempName;
      }
      else
        a.name = ConfigService.config.fileServer + a.name;
    });
    
    if (post.forwardedPost) {
      post.forwardedPost.postAttachments.forEach(a => {
        // 视频显示缩略图，为了方便显示，这里把name和thumbnail切换
        if (a.attachmentType == 1) {
          let tempName = a.name;
          a.name = ConfigService.config.fileServer + a.thumbnail;
          a.thumbnail = ConfigService.config.fileServer + tempName;
        }
        else
          a.name = ConfigService.config.fileServer + a.name;
      });
      post.postAttachments = post.forwardedPost.postAttachments;

      if (post.showOriginalText)
        post.text += " " + post.forwardedPost.user.nickname + post.forwardedPost.text;
    }

    console.log(post);

    return post;
  }

  onClickViewByPassword(): void {
    this.showPhotos = this.password == this.post.viewPassword;
  }

  onClickPhoto(index): void {
    this.showBigPhoto = true;
    this.selectedPhotoIndex = index;
    this.stateService.attachments = this.post.postAttachments;
    this.stateService.photoIndex = this.selectedPhotoIndex;
    this.router.navigate(['/photo']);
  }

  onClickVideo(): void {
    console.log("onClickVideo");
    // this.videoplayer.nativeElement.play();
  }

  goDownload(): void {
    this.router.navigate(['/download']);
  }

}

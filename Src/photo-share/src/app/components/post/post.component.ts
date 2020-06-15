import { Component, OnInit } from '@angular/core';
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
      let postId = this.activatedRoute.snapshot.queryParamMap.get('id');
      if (postId) 
        this.post = await this.getPostAsync(postId);

        // 设置照片的可见性，visibility为2表示需要密码查看
        this.showPhotos = this.post && !this.post.viewPassword;
    });
  }

  async getPostAsync(postId: string): Promise<Post> {
    let post = await this.postService.getPostAsync(postId);

    if (!post)
      return post;
      
    if (post.updatedTime) 
      post.updatedTime = new Date(post.updatedTime * 1000);
    post.createdTime = new Date(post.createdTime * 1000);
    post.user.avatar = ConfigService.config.fileServer + post.user.avatar;
    post.postAttachments.forEach(a => a.name = ConfigService.config.fileServer + a.name);

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

}

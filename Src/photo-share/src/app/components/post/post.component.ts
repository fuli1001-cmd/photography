import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PostService } from 'src/app/services/post.service';
import { Post } from 'src/app/models/post';
import { ConfigService } from '../../services/config.service';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css']
})
export class PostComponent implements OnInit {

  post: Post;

  constructor(
    private postService: PostService, 
    private router: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.router.queryParams.subscribe(async params => {
      let postId = this.router.snapshot.queryParamMap.get('id');
      if (postId) 
        this.post = await this.getPostAsync(postId);
    });

    
  }

  async getPostAsync(postId: string): Promise<Post> {
    let post = await this.postService.getPostAsync(postId);
    console.log(post);
    if (post.updatedTime) 
      post.updatedTime = new Date(post.updatedTime * 1000);
    console.log("*********createdTime: " + post.createdTime);
    post.createdTime = new Date(post.createdTime * 1000);
    post.user.avatar = ConfigService.config.fileServer + post.user.avatar;
    post.postAttachments.forEach(a => a.name = ConfigService.config.fileServer + a.name);
    return post;
  }

}

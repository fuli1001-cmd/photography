import { Component, OnInit, ElementRef, ViewChild, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Post } from 'src/app/models/post';
import { StateService } from 'src/app/services/state.service';

@Component({
  selector: 'post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css']
})
export class PostComponent implements OnInit {

  @Input() post: Post;

  password: string;
  showPhotos: boolean;
  showBigPhoto: boolean;
  selectedPhotoIndex: number;

  constructor(private stateService: StateService, private router: Router) { }

  ngOnInit(): void {
    
  }

  onClickPhoto(index): void {
    this.stateService.attachments = this.post.postAttachments;
    this.stateService.photoIndex = index;
    this.router.navigate(['/photo']);
  }

  onClickViewByPassword(): void {
    if (this.password == this.post.viewPassword)
      this.post.viewPassword = null;
  }

}

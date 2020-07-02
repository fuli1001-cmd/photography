import { Component, OnInit, Input } from '@angular/core';
import { Post } from 'src/app/models/post';
import { Router } from '@angular/router';

@Component({
  selector: 'user-bar',
  templateUrl: './user-bar.component.html',
  styleUrls: ['./user-bar.component.css']
})
export class UserBarComponent implements OnInit {

  @Input() post: Post;

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  goDownload(): void {
    this.router.navigate(['/download']);
  }

}

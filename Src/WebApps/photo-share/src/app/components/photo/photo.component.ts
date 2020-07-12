import { Component, OnInit } from '@angular/core';
import { PhotoEventService } from 'src/app/services/photo-event.service';

@Component({
  selector: 'photo',
  templateUrl: './photo.component.html',
  styleUrls: ['./photo.component.css']
})
export class PhotoComponent implements OnInit {

  constructor(public photoEventService: PhotoEventService) { }

  ngOnInit(): void { }

  close(): void {
    this.photoEventService.closePhoto();
  }

}

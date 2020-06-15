import { Component, OnInit, Input } from '@angular/core';
import { Attachment } from 'src/app/models/attachment';
import { StateService } from 'src/app/services/state.service';

@Component({
  selector: 'app-photo',
  templateUrl: './photo.component.html',
  styleUrls: ['./photo.component.css']
})
export class PhotoComponent implements OnInit {

  // attachments: Attachment[];
  // photoIndex: number;

  constructor(public stateService: StateService) { }

  ngOnInit(): void {
    // this.attachments = this.stateService.attachments;
    // this.photoIndex = this.stateService.photoIndex;
  }

}

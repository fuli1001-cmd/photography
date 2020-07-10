import { Injectable, Output, EventEmitter } from '@angular/core';
import { Attachment } from '../models/attachment';

@Injectable({
  providedIn: 'root'
})
export class PhotoEventService {
  @Output() photoSelectedEvent: EventEmitter<void> = new EventEmitter();
  @Output() photoClosedEvent: EventEmitter<void> = new EventEmitter();

  attachments: Attachment[];
  photoIndex: number;

  constructor() { }

  selectPhoto(attachments: Attachment[], photoIndex: number): void {
    this.attachments = attachments;
    this.photoIndex = photoIndex;
    this.photoSelectedEvent.emit();
  }

  closePhoto(): void {
    this.attachments = null;
    this.photoIndex = 0;
    this.photoClosedEvent.emit();
  }
}

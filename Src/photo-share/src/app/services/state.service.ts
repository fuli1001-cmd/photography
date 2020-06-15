import { Injectable } from '@angular/core';
import { Attachment } from '../models/attachment';

@Injectable({
  providedIn: 'root'
})
export class StateService {

  attachments: Attachment[];
  photoIndex: number;

  constructor() { }
}

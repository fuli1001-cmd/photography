import { Injectable, Output, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SearchEventService {
  @Output() searchEvent: EventEmitter<string> = new EventEmitter();

  constructor() { }

  search(searchKey: string): void {
    this.searchEvent.emit(searchKey);
  }
}

import { Injectable, Output, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PasswordEventService {
  @Output() enteredPasswordEvent: EventEmitter<string> = new EventEmitter();

  constructor() { }

  enterPassword(password: string): void {
    this.enteredPasswordEvent.emit(password);
  }
}

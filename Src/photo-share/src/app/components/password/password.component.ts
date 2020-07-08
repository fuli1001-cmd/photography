import { Component, OnInit } from '@angular/core';
import { PasswordEventService } from 'src/app/services/password-event.service';

@Component({
  selector: 'password',
  templateUrl: './password.component.html',
  styleUrls: ['./password.component.css']
})
export class PasswordComponent implements OnInit {

  password: string; // 用户输入的密码

  constructor(private passwordEventService: PasswordEventService) { }

  ngOnInit(): void {
  }

  onClickViewByPassword(): void {
    this.passwordEventService.enterPassword(this.password)
  }

}

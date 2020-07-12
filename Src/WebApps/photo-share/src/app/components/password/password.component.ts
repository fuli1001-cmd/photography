import { Component, OnInit, Input } from '@angular/core';
import { PasswordEventService } from 'src/app/services/password-event.service';

@Component({
  selector: 'password',
  templateUrl: './password.component.html',
  styleUrls: ['./password.component.css']
})
export class PasswordComponent implements OnInit {
  @Input() correctPassword: string;

  password: string; // 用户输入的密码
  showError: boolean; //显示密码错误提示

  constructor(private passwordEventService: PasswordEventService) { }

  ngOnInit(): void {
  }

  onClickViewByPassword(): void {
    this.showError = this.password != this.correctPassword;

    if (!this.showError)
      this.passwordEventService.enterPassword(this.password);
    else
      window.setTimeout(() => {
        this.showError = false;
      }, 1000);
  }

}

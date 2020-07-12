import { Component, OnInit } from '@angular/core';
import { ConfigService } from '../../services/config.service';

@Component({
  selector: 'app-download',
  templateUrl: './download.component.html',
  styleUrls: ['./download.component.css']
})
export class DownloadComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

  onClickDownload(): void {
    var os = this.getMobileOperatingSystem();
    if (os == 'Android')
      window.open(ConfigService.config.androidUpgradeUrl, "_blank");
    else if (os == 'iOS')
      window.open(ConfigService.config.iosUpgradeUrl, "_blank");
  }

  /**
   * Determine the mobile operating system.
   * This function returns one of 'iOS', 'Android', 'Windows Phone', or 'unknown'.
   *
   * @returns {String}
   */
   getMobileOperatingSystem(): string {
    var userAgent = navigator.userAgent || navigator.vendor;

        // Windows Phone must come first because its UA also contains "Android"
      if (/windows phone/i.test(userAgent)) {
          return "Windows Phone";
      }

      if (/android/i.test(userAgent)) {
          return "Android";
      }

      // iOS detection from: http://stackoverflow.com/a/9039885/177710
      if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
          return "iOS";
      }

      return "unknown";
  }

}

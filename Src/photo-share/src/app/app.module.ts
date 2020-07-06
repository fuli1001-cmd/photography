import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { PostComponent } from './components/post/post.component';
import { ConfigService } from './services/config.service';
import { PhotoComponent } from './components/photo/photo.component';
import { DownloadComponent } from './components/download/download.component';
import { UserBarComponent } from './components/user-bar/user-bar.component';
import { AdBarComponent } from './components/ad-bar/ad-bar.component';
import { HomeComponent } from './components/home/home.component';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';

export function initializeApp(configService: ConfigService) {
  return () => configService.load();
}
@NgModule({
  declarations: [
    AppComponent,
    PostComponent,
    PhotoComponent,
    DownloadComponent,
    UserBarComponent,
    AdBarComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([]),
    InfiniteScrollModule
  ],
  providers: [
    ConfigService, { 
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [ConfigService], multi: true 
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

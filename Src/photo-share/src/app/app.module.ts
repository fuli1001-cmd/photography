import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { PostComponent } from './components/post/post.component';
import { ConfigService } from './services/config.service';

export function initializeApp(configService: ConfigService) {
  return () => configService.load();
}
@NgModule({
  declarations: [
    AppComponent,
    PostComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    RouterModule.forRoot([])
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

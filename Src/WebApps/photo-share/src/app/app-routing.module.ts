import { PostComponent } from './components/post/post.component';
import { PhotoComponent } from './components/photo/photo.component';
import { DownloadComponent } from './components/download/download.component';

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/home/home.component';


const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'photo', component: PhotoComponent },
    { path: 'download', component: DownloadComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { anchorScrolling: 'enabled'})],
  exports: [RouterModule]
})
export class AppRoutingModule { }

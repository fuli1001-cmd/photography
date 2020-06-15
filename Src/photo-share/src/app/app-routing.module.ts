import { PostComponent } from './components/post/post.component';
import { PhotoComponent } from './components/photo/photo.component';
import { DownloadComponent } from './components/download/download.component';

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';


const routes: Routes = [
    { path: '', component: PostComponent },
    { path: 'photo', component: PhotoComponent },
    { path: 'download', component: DownloadComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

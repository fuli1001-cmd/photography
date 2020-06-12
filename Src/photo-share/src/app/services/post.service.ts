import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Post } from '../models/post';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})
export class PostService {

  constructor(private http: HttpClient) { }

  async getPostAsync(postId: string): Promise<Post> {
    try {
      let apiUrl = ConfigService.config.serviceBase + 'posts/post/' + postId;
      console.log("************" + apiUrl);
      let response = await this.http.get<any>(apiUrl).toPromise();
      return response.data;
    } catch (err) {
      console.log(`getPost error: ${err}`);
      return null;
    }
  }
}

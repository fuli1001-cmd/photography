import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Post } from '../models/post';
import { ConfigService } from './config.service';
import { PagedPost } from '../models/paged-post';

@Injectable({
  providedIn: 'root'
})
export class PostService {

  constructor(private http: HttpClient) { }

  // async getPostAsync(postId: string, sharedUserId: string): Promise<Post> {
  //   let apiUrl = ConfigService.config.serviceBase + '/posts/share/' + postId + '/' + sharedUserId;
  //   return await this.requestAsync<Post>(apiUrl);
  // }

  // async getTagPostsAsync(privateTag: string, sharedUserId: string): Promise<Post[]> {
  //   let apiUrl = ConfigService.config.serviceBase + '/posts/share/privatetag/' + privateTag + '/' + sharedUserId;
  //   return await this.requestAsync<Post[]>(apiUrl);
  // }

  // async getUserPostsAsync(sharedUserId: string): Promise<Post[]> {
  //   let apiUrl = ConfigService.config.serviceBase + '/posts/share/' + sharedUserId;
  //   return await this.requestAsync<Post[]>(apiUrl);
  // }

  async getPostsAsync(param: string, pageNumber: number): Promise<PagedPost> {
    let apiUrl = ConfigService.config.serviceBase + '/posts/share?s=' + encodeURIComponent(param) + '&pageNumber=' + pageNumber + "&pageSize=2";
    return await this.requestAsync<PagedPost>(apiUrl);
  }

  async requestAsync<T>(apiUrl: string): Promise<T> {
    try {
      let response = await this.http.get<any>(apiUrl).toPromise();

      if (response.code == 0)
        return response as T;

      console.log(`${apiUrl} error: ${response.message}`);
      return null;
    } catch (err) {
      console.log(`${apiUrl} error: ${err}`);
      return null;
    }
  }
}

import { Component, OnInit } from '@angular/core';
import { PostService } from 'src/app/services/post.service';
import { ActivatedRoute } from '@angular/router';
import { Post } from 'src/app/models/post';
import { ConfigService } from 'src/app/services/config.service';
import { Attachment } from 'src/app/models/attachment';
import { PagedPost } from 'src/app/models/paged-post';
import { SearchEventService } from 'src/app/services/search-event.service';
import * as CryptoJS from 'crypto-js';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  // 没有搜索过滤的帖子数据和搜索结果分开存储，以便在清空搜索关键字时能回到为搜索页面状态
  allPosts: PagedPost; // 没有搜索过滤的帖子数据
  searchedPosts: PagedPost; // 搜索结果
  displayData: PagedPost; // 用于显示，要么显示allPosts，要么显示searchedPosts（searchkey不为空时）
  searchkey: string; // 搜索关键字
  param: string; // 页面url中的查询参数
  noAd: boolean; // 是否显示广告栏
  showSpinner: boolean; // 是否显示等待标志
  querying: boolean; // 表示是否正在调用后台服务进行查询

  constructor(private postService: PostService, 
    private activatedRoute: ActivatedRoute,
    private searchEventService: SearchEventService) { }

  ngOnInit(): void {
    this.registerSearchEvent();

    this.activatedRoute.queryParams.subscribe(async params => {
      // 获取url参数
      this.param = this.activatedRoute.snapshot.queryParamMap.get('s');

      // 解密参数
      this.noAd = this.decryptParam()?.noAd ?? false;

      // 获取帖子数据
      this.allPosts = await this.getPagedPostsAsync(this.searchkey, 1);

      this.displayData = this.allPosts;
    });
  }

  private async getPagedPostsAsync(searchkey: string, pageNumber: number): Promise<PagedPost> {
    this.querying = true;

    var pagedPosts = await this.postService.getPostsAsync(this.param, searchkey, pageNumber);

    if (pagedPosts && pagedPosts.data)
      pagedPosts.data.forEach(p => this.setPost(p));

    this.querying = false;

    return pagedPosts;
  }

  // 设置帖子的一些属性
  private setPost(post: Post): void {
    if (post.updatedTime) 
      post.updatedTime = new Date(post.updatedTime * 1000);
    
    post.createdTime = new Date(post.createdTime * 1000);
    post.user.avatar = ConfigService.config.fileServer + '/' + post.user.avatar;
    post.postAttachments.forEach(a => this.setAttachment(a));
    
    if (post.forwardedPost) {
      post.forwardedPost.postAttachments.forEach(a => this.setAttachment(a));
      post.postAttachments = post.forwardedPost.postAttachments;

      if (post.showOriginalText)
        post.text += " " + post.forwardedPost.user.nickname + post.forwardedPost.text;
    }
  }

  // 设置附件文件路径
  private setAttachment(attachment: Attachment): void {
    attachment.name = ConfigService.config.fileServer + '/' + attachment.name;
    attachment.thumbnail = ConfigService.config.fileServer + '/' + attachment.thumbnail;
  }

  async onScroll(): Promise<void> {
    // 立即设置querying为true，避免上拉时多次触发该事件导致重复调用API
    if (this.querying)
      return;

    this.querying = true;
    this.showSpinner = true;
    let hasMore = false;
    let nextPage = 1;

    // 有搜索关键字时，使用searchResult，否则使用pagedPosts
    if (this.searchkey) {
      hasMore = this.searchedPosts.pagingInfo.currentPage < this.searchedPosts.pagingInfo.totalPages;
      nextPage = this.searchedPosts.pagingInfo.currentPage + 1;
    } else {
      hasMore = this.allPosts.pagingInfo.currentPage < this.allPosts.pagingInfo.totalPages;
      nextPage = this.allPosts.pagingInfo.currentPage + 1;
    }

    if (hasMore) {
      var pagedPosts = await this.getPagedPostsAsync(this.searchkey, nextPage);
      this.appendPosts(pagedPosts);
    }

    this.querying = false;
    this.showSpinner = false;
  }

  private appendPosts(pagedPosts: PagedPost): void {
    if (pagedPosts) {
      // 如果搜索关键字不为空，则将帖子数据加到搜索结果中，否则加到中
      if (this.searchkey) {
        this.searchedPosts.pagingInfo = pagedPosts.pagingInfo;
        pagedPosts.data.forEach(p => this.searchedPosts.data.push(p));
        this.displayData = this.searchedPosts;
      } else {
        this.allPosts.pagingInfo = pagedPosts.pagingInfo;
        pagedPosts.data.forEach(p => this.allPosts.data.push(p));
        this.displayData = this.allPosts;
      }
    }
  }

  private registerSearchEvent(): void {
    this.searchEventService.searchEvent.subscribe(async searchkey => {
      this.searchkey = searchkey;
      
      if (this.searchkey) {
        // 搜索关键字改变，重新获取搜索数据并显示
        this.searchedPosts = await this.getPagedPostsAsync(this.searchkey, 1);
        this.displayData = this.searchedPosts;
      }
      else {
        // 搜索关键字清空，重新显示为筛选过的数据
        this.searchedPosts = null;
        this.displayData = this.allPosts;
      }
    });
  }

  private decryptParam(): any {
    if (!this.param)
      return null;

    let decryptKey = 'Vs16.5.4';
    let key = CryptoJS.enc.Utf8.parse(decryptKey);
    let iv = CryptoJS.enc.Utf8.parse(decryptKey);
    let decrypted = CryptoJS.DES.decrypt(this.param, key, { iv: iv, mode: CryptoJS.mode.CBC });
    let paramObj = JSON.parse(decrypted.toString(CryptoJS.enc.Utf8));
    return paramObj;
  }

}

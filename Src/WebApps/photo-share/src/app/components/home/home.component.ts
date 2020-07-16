import { Component, OnInit } from '@angular/core';
import { PostService } from 'src/app/services/post.service';
import { ActivatedRoute } from '@angular/router';
import { Post } from 'src/app/models/post';
import { ConfigService } from 'src/app/services/config.service';
import { Attachment } from 'src/app/models/attachment';
import { PagedPost } from 'src/app/models/paged-post';
import { SearchEventService } from 'src/app/services/search-event.service';
import * as CryptoJS from 'crypto-js';
import { PasswordEventService } from 'src/app/services/password-event.service';
import { PhotoEventService } from 'src/app/services/photo-event.service';
import { ViewportScroller } from '@angular/common';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  // 未搜索过滤的帖子数据和搜索过滤的帖子数据分开存储，以便在清空搜索关键字时能回到未搜索页面状态
  allPosts: PagedPost; // 未搜索过滤的帖子数据
  searchedPosts: PagedPost; // 搜索过滤的帖子数据
  displayData: PagedPost; // 用于显示，要么显示allPosts，要么显示searchedPosts（searchkey不为空时）
  searchkey: string; // 搜索关键字
  privateTags: string[]; // 帖子类别
  selectedTag: string; // 选中的类别
  param: string; // 页面url中的查询参数
  noAd: boolean; // 是否显示广告栏
  viewPassword: string; // 查看密码
  showSpinner: boolean; // 是否显示等待标志
  querying: boolean; // 表示是否正在调用后台服务进行查询
  displayBigPhoto: boolean // 是否大图显示照片
  position: any; // 当前滚动位置

  readonly filePrefix:string = '/app/';
  readonly fileThumbnailPrefix = '/appthumbnail/';

  constructor(private postService: PostService, 
    private activatedRoute: ActivatedRoute,
    private searchEventService: SearchEventService,
    private passwordEventService: PasswordEventService,
    private photoEventService: PhotoEventService,
    private viewportScroller: ViewportScroller) { }

  ngOnInit(): void {
    this.privateTags = [];
    this.allPosts = this.getEmptyPagedPost();
    this.searchedPosts = this.getEmptyPagedPost();
    this.registerEvents();

    // 取得url param并根据param获取数据
    this.activatedRoute.queryParams.subscribe(async params => {
      // 获取url参数
      this.param = this.activatedRoute.snapshot.queryParamMap.get('s');

      // 解密参数
      let paramObj = this.decryptParam();

      // 参数中必须要有userId
      if (paramObj?.userId) {
        this.noAd = paramObj.noAd ?? false;
        this.viewPassword = paramObj.pwd;

        // 获取帖子数据
        await this.retrivePosts(1);

        // 获取帖子种类
        if (!paramObj.postId && !paramObj.privateTag) {
          this.privateTags = ['全部分类', '未分类'];
          let privateTags = await this.getPrivateTags(paramObj.userId);
          privateTags.forEach(t => this.privateTags.push(t));
          this.selectedTag = this.privateTags[0];
        }
      }
    });
  }

  private async getPrivateTags(userId: string): Promise<string[]> {
    return await this.postService.getPrivateTagsAsync(userId) ?? [];
  }

  async retrivePosts(pageNumber: number): Promise<void> {
    this.querying = true;
    this.showSpinner = true;

    let tag = this.selectedTag == '全部分类' ? null : this.selectedTag;
    var pagedPosts = await this.postService.getPostsAsync(this.param, tag, this.searchkey, pageNumber);

    if (pagedPosts && pagedPosts.data)
      pagedPosts.data.forEach(p => this.setPost(p));

    this.appendPosts(pagedPosts);

    this.querying = false;
    this.showSpinner = false;
  }

  // 设置帖子的一些属性
  private setPost(post: Post): void {
    if (post.updatedTime) 
      post.updatedTime = new Date(post.updatedTime * 1000);
    
    post.createdTime = new Date(post.createdTime * 1000);
    post.user.avatar = post.user.avatar ? ConfigService.config.fileServer + this.fileThumbnailPrefix + post.user.avatar : '../../../assets/images/default_avatar.png';
    post.postAttachments.forEach(a => this.setAttachment(a));
    
    if (post.forwardedPost) {
      post.forwardedPost.postAttachments.forEach(a => this.setAttachment(a));
      post.postAttachments = post.forwardedPost.postAttachments;

      if (post.showOriginalText)
        post.text += " " + post.forwardedPost.user.nickname + ': ' + post.forwardedPost.text;
    }
  }

  // 设置附件文件路径
  private setAttachment(attachment: Attachment): void {
    // 视频文件无缩略图
    attachment.name = attachment.attachmentType == 0 ? 
      ConfigService.config.fileServer + this.fileThumbnailPrefix + attachment.name :
      ConfigService.config.fileServer + this.filePrefix + attachment.name
    attachment.thumbnail = ConfigService.config.fileServer + this.fileThumbnailPrefix + attachment.thumbnail;
  }

  // 如果搜索关键字不为空，则将帖子数据加到搜索结果中，否则加到中
  private appendPosts(pagedPosts: PagedPost): void {
    if (pagedPosts) {
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

  async onScroll(): Promise<void> {
    if (this.querying)
      return;

    // 立即设置querying为true，避免上拉时多次触发该事件导致重复调用API
    this.querying = true;
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

    if (hasMore) 
      await this.retrivePosts(nextPage);

    this.querying = false;
  }

  private registerEvents(): void {
    this.searchEventService.searchEvent.subscribe(async searchkey => {
      this.searchkey = searchkey;

      // 搜索关键字变化，只清空搜索过滤的数据
      this.searchedPosts = this.getEmptyPagedPost();

      // 搜索关键字不为空，重新搜索数据，否则重新显示未筛选过的数据
      if (this.searchkey) {
        await this.retrivePosts(1);
      }
      else {
        // 如果未筛选数据为空，可能是在清空搜索关键字之前进行了种类选择，此种情况下需要重新获取未筛选数据
        if (this.allPosts.data.length == 0)
          await this.retrivePosts(1);

        this.displayData = this.allPosts;
      }
    });

    this.passwordEventService.enteredPasswordEvent.subscribe(password => {
      if (password == this.viewPassword)
        this.viewPassword = null;
    });

    this.photoEventService.photoSelectedEvent.subscribe(() => {
      this.displayBigPhoto = true;
      // 大图显示时，替换缩略图为大图
      this.photoEventService.attachments.forEach(a => a.name = a.name.replace(this.fileThumbnailPrefix, this.filePrefix));
      this.position = this.viewportScroller.getScrollPosition();
    });

    this.photoEventService.photoClosedEvent.subscribe(() => {
      this.displayBigPhoto = false;
      window.setTimeout(() => {
        this.viewportScroller.scrollToPosition(this.position);
      }, 0);
    });
  }

  private decryptParam(): any {
    console.log(this.param);
    this.param = decodeURIComponent(this.param);
    console.log(this.param);

    if (!this.param)
      return null;

    let decryptKey = 'Vs16.5.4';
    let key = CryptoJS.enc.Utf8.parse(decryptKey);
    let iv = CryptoJS.enc.Utf8.parse(decryptKey);
    let decrypted = CryptoJS.DES.decrypt(this.param, key, { iv: iv, mode: CryptoJS.mode.CBC });

    let test = decrypted.toString(CryptoJS.enc.Utf8);
    console.log(test);
    let paramObj = JSON.parse(decrypted.toString(CryptoJS.enc.Utf8));

    console.log(paramObj);

    return paramObj;
  }

  async onClickTag(tag): Promise<void> {
    if (tag != this.selectedTag) {
      // 种类变换，清空所有数据
      this.allPosts = this.getEmptyPagedPost();
      this.searchedPosts = this.getEmptyPagedPost();

      this.selectedTag = tag;
      await this.retrivePosts(1);
    }
  }

  private getEmptyPagedPost(): PagedPost {
    return {
      data: [],
      pagingInfo: {
        currentPage: 1,
        totalPages: 1,
        pageSize: 10, 
        totalCount: 0
      }
    };
  }
}

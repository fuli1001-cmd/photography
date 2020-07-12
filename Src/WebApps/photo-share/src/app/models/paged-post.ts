import { Post } from './post';
import { Paging } from './paging';

export interface PagedPost {
    data: Post[];
    pagingInfo: Paging;
}
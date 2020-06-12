import { Attachment } from './attachment';
import { User } from './user';

export interface Post {
    text: string;
    createdTime: any;
    updatedTime: any;
    shareType: number;
    viewPassword: string;
    showOriginalText: boolean;
    postAttachments: Attachment[];
    user: User;
    forwardedPost: Post;
}
import { Attachment } from './attachment';
import { User } from './user';

export interface Post {
    text: string;
    locationName: string;
    createdTime: any;
    updatedTime: any;
    viewPassword: string;
    showOriginalText: boolean;
    postAttachments: Attachment[];
    user: User;
    forwardedPost: Post;
}
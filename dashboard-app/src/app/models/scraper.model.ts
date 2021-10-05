export interface Author {
  id: string;
  displayName: string;
  profilePictureUrl: string;
  description: string;
}

export type PostType = "Post" | "Repost" | "Reply" | "ReplyToSelf";

export interface Post {
  url: string;
  content: string;
  type: PostType;
  isLivestream: boolean;
  authorId: string;
  creationDate: string;
  mediaItems: MediaItemWrapper[];
}

export interface MediaItemWrapper {
  $type: string;
  $value: MediaItem;
}

export interface MediaItem {
  url: string;
}

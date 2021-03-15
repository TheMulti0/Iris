export interface Update {
  author: User;
  content: string;
  creationDate: string;
  url: string;
  media: IMedia[];
  isRepost: boolean;
  isLive: boolean;
  isReply: boolean;
}

export enum Platform {
  Facebook = 0,
  Twitter = 1,
  Feeds = 2
}

export interface User {
 userId: string;
 platform: Platform;
}

export interface IMedia {
  type: string;
  url: string;
}

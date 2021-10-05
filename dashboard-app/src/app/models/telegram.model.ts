export interface TelegramSubscription {
  id: string;
  userId: string;
  platform: string;
  version: number;
  chats: UserChatSubscription[];
}

export interface UserChatSubscription {
  interval: string;
  chatInfo: ChatInfo;
  sendScreenshotOnly: boolean;
  prefix: Text;
  suffix: Text;
  showUrlPreview: boolean;
  subscriptionDate: Date;
}

export type ChatType = 'Private' | 'Group' | 'Channel' | 'Supergroup';

export interface ChatInfo {
  id: number;
  type: ChatType;
  title: string;
  description: string;
  username: string;
  firstName: string;
  lastName: string;
  allMembersAreAdministrators: boolean;
  inviteLink: string;
}

export type TextMode = 'HyperlinkedText' | 'Text' | 'Url';

export interface Text {
  enabled: boolean;
  mode: TextMode;
  content: string;
}

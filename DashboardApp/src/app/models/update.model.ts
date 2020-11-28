export interface Update {
    id: string;
    content: string;
    author_id: string;
    creation_date: string;
    url: string;
    media: Media[];
    repost: boolean;
    isExpanded: boolean;
}

export type Media = Photo | Video | Audio;

export interface Photo {
    type: string;
    url: string;
}

export interface ILinkable {
    type: string;
    url: string;
}

export interface IStreamable extends ILinkable {
    thumbnail_url: string;
    duration_seconds: number;
}

export interface Video extends IStreamable {
    width: number;
    height: number;
}

export interface Audio extends IStreamable {
    title: string;
    artist: string;
}

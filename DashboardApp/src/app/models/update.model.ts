export interface Update {
    id: number;
    content: string;
    author_id: string;
    creation_date: string;
    url: string;
    media: Media[];
    repost: boolean;
}

export type Media = Photo | Video | Audio;

export interface Photo {
    url: string;
 }

export interface Video {
    url: string;
    thumbnail_url: string;
    duration_seconds: number;
    width: number;
    height: number;
}

export interface Audio {
    url: string;
    thumbnail_url: string;
    duration_seconds: number;
    title: string;
    artist: string;
}

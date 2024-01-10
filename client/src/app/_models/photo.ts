
export interface Photo {
    id: number;
    url: string;
    isMain: boolean;
    publicId?: any;
    appUserId: number;
    isApproved: boolean
    username?: string
}

export enum ContactType {
  Customer = 0,
  Lead = 1
}

export interface Contact {
  id: number;
  name: string;
  email?: string;
  phone?: string;
  type: ContactType;
  imageCount: number;
}

export interface CreateContact {
  name: string;
  email?: string;
  phone?: string;
  type: ContactType;
}

export interface ContactImage {
  id: number;
  contactId: number;
  imageData: string;
  fileName?: string;
  contentType?: string;
  uploadedAt: Date;
}

export interface UploadImage {
  imageData: string;
  fileName?: string;
  contentType?: string;
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Contact, CreateContact, ContactImage, UploadImage } from '../models/contact.model';
import { environment } from '../../environments/environment';

// API Response interface to match backend ApiResponse<T>
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors: string[];
  timestamp: string;
}

@Injectable({
  providedIn: 'root'
})
export class ContactService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  // Contact endpoints
  getContacts(): Observable<Contact[]> {
    return this.http.get<ApiResponse<Contact[]>>(`${this.apiUrl}/contacts`)
      .pipe(map(response => response.data || []));
  }

  getContact(id: number): Observable<Contact> {
    return this.http.get<ApiResponse<Contact>>(`${this.apiUrl}/contacts/${id}`)
      .pipe(map(response => response.data!));
  }

  createContact(contact: CreateContact): Observable<Contact> {
    return this.http.post<ApiResponse<Contact>>(`${this.apiUrl}/contacts`, contact)
      .pipe(map(response => response.data!));
  }

  updateContact(id: number, contact: Partial<CreateContact>): Observable<Contact> {
    return this.http.put<ApiResponse<Contact>>(`${this.apiUrl}/contacts/${id}`, contact)
      .pipe(map(response => response.data!));
  }

  deleteContact(id: number): Observable<void> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/contacts/${id}`)
      .pipe(map(() => void 0));
  }

  canAddImages(contactId: number, additionalImages: number = 1): Observable<boolean> {
    return this.http.get<ApiResponse<boolean>>(`${this.apiUrl}/contacts/${contactId}/can-add-images?additionalImages=${additionalImages}`)
      .pipe(map(response => response.data || false));
  }

  // Image endpoints
  getContactImages(contactId: number): Observable<ContactImage[]> {
    return this.http.get<ApiResponse<ContactImage[]>>(`${this.apiUrl}/contacts/${contactId}/images`)
      .pipe(map(response => response.data || []));
  }

  getContactImage(contactId: number, imageId: number): Observable<ContactImage> {
    return this.http.get<ApiResponse<ContactImage>>(`${this.apiUrl}/contacts/${contactId}/images/${imageId}`)
      .pipe(map(response => response.data!));
  }

  uploadImage(contactId: number, image: UploadImage): Observable<ContactImage> {
    return this.http.post<ApiResponse<ContactImage>>(`${this.apiUrl}/contacts/${contactId}/images`, image)
      .pipe(map(response => response.data!));
  }

  uploadMultipleImages(contactId: number, images: UploadImage[]): Observable<ContactImage[]> {
    return this.http.post<ApiResponse<ContactImage[]>>(`${this.apiUrl}/contacts/${contactId}/images/batch`, images)
      .pipe(map(response => response.data || []));
  }

  deleteImage(contactId: number, imageId: number): Observable<void> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/contacts/${contactId}/images/${imageId}`)
      .pipe(map(() => void 0));
  }

  validateImageLimit(contactId: number, additionalImages: number = 1): Observable<boolean> {
    return this.http.get<ApiResponse<boolean>>(`${this.apiUrl}/contacts/${contactId}/images/validate-limit?additionalImages=${additionalImages}`)
      .pipe(map(response => response.data || false));
  }

  // Helper method to convert file to base64
  convertFileToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        const base64String = reader.result as string;
        // Remove the data URL prefix (e.g., "data:image/jpeg;base64,")
        const base64Data = base64String.split(',')[1];
        resolve(base64Data);
      };
      reader.onerror = error => reject(error);
    });
  }
}

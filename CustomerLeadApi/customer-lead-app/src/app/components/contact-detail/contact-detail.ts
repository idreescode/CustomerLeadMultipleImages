import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ContactService } from '../../services/contact.service';
import { Contact, ContactImage, ContactType } from '../../models/contact.model';
import { ImageGalleryComponent } from '../image-gallery/image-gallery';

@Component({
  selector: 'app-contact-detail',
  imports: [CommonModule, RouterLink, ImageGalleryComponent],
  templateUrl: './contact-detail.html',
  styleUrl: './contact-detail.scss'
})
export class ContactDetailComponent implements OnInit {
  contact = signal<Contact | null>(null);
  images = signal<ContactImage[]>([]);
  loading = signal(false);
  uploading = signal(false);
  error = signal<string | null>(null);
  success = signal<string | null>(null);

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private contactService: ContactService
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadContact(id);
      this.loadImages(id);
    }
  }

  loadContact(id: number) {
    this.loading.set(true);
    this.contactService.getContact(id).subscribe({
      next: (contact) => {
        this.contact.set(contact);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading contact:', error);
        this.loading.set(false);
        this.router.navigate(['/contacts']);
      }
    });
  }

  loadImages(contactId: number) {
    this.contactService.getContactImages(contactId).subscribe({
      next: (images) => {
        this.images.set(images);
      },
      error: (error) => {
        console.error('Error loading images:', error);
        this.error.set('Failed to load images. Please refresh the page.');
      }
    });
  }

  onFileSelected(event: any) {
    const files = event.target.files;
    if (!files || files.length === 0 || !this.contact()) return;

    const contact = this.contact()!;
    const currentImageCount = this.images().length;
    const maxAllowed = 10 - currentImageCount;
    
    if (files.length > maxAllowed) {
      this.error.set(`You can only upload ${maxAllowed} more images (maximum 10 per contact)`);
      return;
    }

    this.uploading.set(true);
    this.error.set(null);
    this.success.set(null);
    
    const uploadPromises: Promise<any>[] = [];

    for (let i = 0; i < files.length; i++) {
      const file = files[i];
      if (file.type.startsWith('image/')) {
        const promise = this.contactService.convertFileToBase64(file).then(base64Data => {
          return this.contactService.uploadImage(contact.id, {
            imageData: base64Data,
            fileName: file.name,
            contentType: file.type
          }).toPromise();
        });
        uploadPromises.push(promise);
      }
    }

    Promise.all(uploadPromises).then(results => {
      const newImages = results.filter(result => result);
      this.images.set([...this.images(), ...newImages]);
      this.uploading.set(false);
      this.success.set(`${newImages.length} image(s) uploaded successfully!`);
      setTimeout(() => this.success.set(null), 3000);
      // Reset file input
      event.target.value = '';
    }).catch(error => {
      console.error('Error uploading images:', error);
      this.error.set('Failed to upload images. Please try again.');
      this.uploading.set(false);
    });
  }

  onImageDeleted(imageId: number) {
    if (!this.contact()) return;
    
    this.error.set(null);
    this.success.set(null);
    
    this.contactService.deleteImage(this.contact()!.id, imageId).subscribe({
      next: () => {
        this.images.set(this.images().filter(img => img.id !== imageId));
        this.success.set('Image deleted successfully!');
        setTimeout(() => this.success.set(null), 3000);
      },
      error: (error) => {
        console.error('Error deleting image:', error);
        this.error.set('Failed to delete image. Please try again.');
      }
    });
  }

  getContactTypeLabel(type: ContactType): string {
    return type === ContactType.Customer ? 'Customer' : 'Lead';
  }

  canUploadMore(): boolean {
    return this.images().length < 10;
  }
}
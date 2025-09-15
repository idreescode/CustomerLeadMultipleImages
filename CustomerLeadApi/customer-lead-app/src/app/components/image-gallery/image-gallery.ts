import { Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContactImage } from '../../models/contact.model';

@Component({
  selector: 'app-image-gallery',
  imports: [CommonModule],
  templateUrl: './image-gallery.html',
  styleUrl: './image-gallery.scss'
})
export class ImageGalleryComponent {
  @Input() images: ContactImage[] = [];
  @Output() imageDeleted = new EventEmitter<number>();

  selectedImage = signal<ContactImage | null>(null);
  currentIndex = signal(0);

  openModal(image: ContactImage, index: number) {
    this.selectedImage.set(image);
    this.currentIndex.set(index);
  }

  closeModal() {
    this.selectedImage.set(null);
  }

  previousImage() {
    const current = this.currentIndex();
    const newIndex = current > 0 ? current - 1 : this.images.length - 1;
    this.currentIndex.set(newIndex);
    this.selectedImage.set(this.images[newIndex]);
  }

  nextImage() {
    const current = this.currentIndex();
    const newIndex = current < this.images.length - 1 ? current + 1 : 0;
    this.currentIndex.set(newIndex);
    this.selectedImage.set(this.images[newIndex]);
  }

  deleteImage(imageId: number, event: Event) {
    event.stopPropagation();
    if (confirm('Are you sure you want to delete this image?')) {
      this.imageDeleted.emit(imageId);
      
      // Close modal if the deleted image was selected
      if (this.selectedImage()?.id === imageId) {
        this.closeModal();
      }
    }
  }

  getImageUrl(imageData: string): string {
    return `data:image/jpeg;base64,${imageData}`;
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }
}
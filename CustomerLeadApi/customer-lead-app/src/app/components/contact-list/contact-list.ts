import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ContactService } from '../../services/contact.service';
import { Contact, ContactType, CreateContact } from '../../models/contact.model';

@Component({
  selector: 'app-contact-list',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './contact-list.html',
  styleUrl: './contact-list.scss'
})
export class ContactListComponent implements OnInit {
  contacts = signal<Contact[]>([]);
  loading = signal(false);
  showForm = signal(false);
  error = signal<string | null>(null);
  success = signal<string | null>(null);
  
  newContact: CreateContact = {
    name: '',
    email: '',
    phone: '',
    type: ContactType.Customer
  };

  ContactType = ContactType;

  constructor(private contactService: ContactService) {}

  ngOnInit() {
    this.loadContacts();
  }

  loadContacts() {
    this.loading.set(true);
    this.error.set(null);
    this.contactService.getContacts().subscribe({
      next: (contacts) => {
        this.contacts.set(contacts);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading contacts:', error);
        this.error.set('Failed to load contacts. Please try again.');
        this.loading.set(false);
      }
    });
  }

  toggleForm() {
    this.showForm.set(!this.showForm());
    if (!this.showForm()) {
      this.resetForm();
    }
  }

  createContact() {
    if (!this.newContact.name.trim()) return;

    this.error.set(null);
    this.success.set(null);
    this.contactService.createContact(this.newContact).subscribe({
      next: (contact) => {
        this.contacts.set([...this.contacts(), contact]);
        this.resetForm();
        this.showForm.set(false);
        this.success.set('Contact created successfully!');
        setTimeout(() => this.success.set(null), 3000);
      },
      error: (error) => {
        console.error('Error creating contact:', error);
        this.error.set('Failed to create contact. Please check your input and try again.');
      }
    });
  }

  deleteContact(id: number) {
    if (confirm('Are you sure you want to delete this contact?')) {
      this.error.set(null);
      this.success.set(null);
      this.contactService.deleteContact(id).subscribe({
        next: () => {
          this.contacts.set(this.contacts().filter(c => c.id !== id));
          this.success.set('Contact deleted successfully!');
          setTimeout(() => this.success.set(null), 3000);
        },
        error: (error) => {
          console.error('Error deleting contact:', error);
          this.error.set('Failed to delete contact. Please try again.');
        }
      });
    }
  }

  resetForm() {
    this.newContact = {
      name: '',
      email: '',
      phone: '',
      type: ContactType.Customer
    };
  }

  getContactTypeLabel(type: ContactType): string {
    return type === ContactType.Customer ? 'Customer' : 'Lead';
  }
}
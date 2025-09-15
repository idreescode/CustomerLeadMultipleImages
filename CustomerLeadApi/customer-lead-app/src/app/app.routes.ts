import { Routes } from '@angular/router';
import { ContactListComponent } from './components/contact-list/contact-list';
import { ContactDetailComponent } from './components/contact-detail/contact-detail';

export const routes: Routes = [
  { path: '', redirectTo: '/contacts', pathMatch: 'full' },
  { path: 'contacts', component: ContactListComponent },
  { path: 'contacts/:id', component: ContactDetailComponent },
  { path: '**', redirectTo: '/contacts' }
];

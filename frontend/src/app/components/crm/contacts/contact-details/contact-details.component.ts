import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ContactService, Contact } from '../../../../services/crm/contact.service';

@Component({
  selector: 'app-contact-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container mx-auto px-4 py-8">
      <div class="max-w-4xl mx-auto">
        <div class="flex justify-between items-center mb-6">
          <h1 class="text-2xl font-bold text-gray-800">Contact Details</h1>
          <div class="space-x-4">
            <button
              (click)="goBack()"
              class="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
            >
              Back
            </button>
            <button
              (click)="deleteContact()"
              class="px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600"
            >
              Delete Contact
            </button>
          </div>
        </div>

        <div *ngIf="contact" class="bg-white rounded-lg shadow overflow-hidden">
          <div class="p-6">
            <div class="grid grid-cols-2 gap-6">
              <div>
                <h3 class="text-sm font-medium text-gray-500">First Name</h3>
                <p class="mt-1 text-lg text-gray-900">{{ contact.firstName }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Last Name</h3>
                <p class="mt-1 text-lg text-gray-900">{{ contact.lastName }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Email</h3>
                <p class="mt-1 text-lg text-gray-900">{{ contact.email }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Phone</h3>
                <p class="mt-1 text-lg text-gray-900">{{ contact.phone || 'N/A' }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Company</h3>
                <p class="mt-1 text-lg text-gray-900">{{ contact.company || 'N/A' }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Position</h3>
                <p class="mt-1 text-lg text-gray-900">{{ contact.position || 'N/A' }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Department</h3>
                <p class="mt-1 text-lg text-gray-900">{{ contact.department || 'N/A' }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Created At</h3>
                <p class="mt-1 text-lg text-gray-900">{{ contact.createdAt | date:'medium' }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Last Updated</h3>
                <p class="mt-1 text-lg text-gray-900">{{ contact.updatedAt | date:'medium' }}</p>
              </div>
            </div>

            <div class="mt-6">
              <h3 class="text-sm font-medium text-gray-500">Address</h3>
              <p class="mt-1 text-lg text-gray-900">{{ contact.address || 'No address provided' }}</p>
            </div>

            <div class="mt-6">
              <h3 class="text-sm font-medium text-gray-500">Notes</h3>
              <p class="mt-1 text-lg text-gray-900">{{ contact.notes || 'No notes provided' }}</p>
            </div>
          </div>
        </div>

        <div *ngIf="!contact" class="text-center py-12">
          <p class="text-gray-500">Loading contact details...</p>
        </div>
      </div>
    </div>
  `,
})
export class ContactDetailsComponent implements OnInit {
  contact: Contact | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private contactService: ContactService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadContact(id);
    }
  }

  loadContact(id: string): void {
    this.contactService.getById(id).subscribe({
      next: (response) => {
        this.contact = 'data' in response ? response.data : response;
      },
      error: (error) => {
        console.error('Error loading contact:', error);
        this.router.navigate(['/crm/contacts']);
      },
    });
  }

  deleteContact(): void {
    if (this.contact && confirm('Are you sure you want to delete this contact?')) {
      this.contactService.delete(this.contact.id).subscribe({
        next: () => {
          this.router.navigate(['/crm/contacts']);
        },
        error: (error) => {
          console.error('Error deleting contact:', error);
        },
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/crm/contacts']);
  }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ContactService, CreateContactDTO } from '../../../../services/crm/contact.service';
import { CustomerService, Customer } from '../../../../services/crm/customer.service';

@Component({
  selector: 'app-contact-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="container mx-auto px-4 py-8">
      <div class="max-w-2xl mx-auto">
        <h1 class="text-2xl font-bold text-gray-800 mb-6">Add New Contact</h1>

        <form [formGroup]="contactForm" (ngSubmit)="onSubmit()" class="space-y-6">
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label for="firstName" class="block text-sm font-medium text-gray-700">First Name</label>
              <input
                type="text"
                id="firstName"
                formControlName="firstName"
                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
              <div *ngIf="contactForm.get('firstName')?.errors?.['required'] && contactForm.get('firstName')?.touched" class="text-red-500 text-sm mt-1">
                First name is required
              </div>
            </div>

            <div>
              <label for="lastName" class="block text-sm font-medium text-gray-700">Last Name</label>
              <input
                type="text"
                id="lastName"
                formControlName="lastName"
                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
              <div *ngIf="contactForm.get('lastName')?.errors?.['required'] && contactForm.get('lastName')?.touched" class="text-red-500 text-sm mt-1">
                Last name is required
              </div>
            </div>
          </div>

          <div>
            <label for="email" class="block text-sm font-medium text-gray-700">Email</label>
            <input
              type="email"
              id="email"
              formControlName="email"
              class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            />
            <div *ngIf="contactForm.get('email')?.errors?.['required'] && contactForm.get('email')?.touched" class="text-red-500 text-sm mt-1">
              Email is required
            </div>
            <div *ngIf="contactForm.get('email')?.errors?.['email'] && contactForm.get('email')?.touched" class="text-red-500 text-sm mt-1">
              Please enter a valid email address
            </div>
          </div>

          <div>
            <label for="customerId" class="block text-sm font-medium text-gray-700">Customer</label>
            <select
              id="customerId"
              formControlName="customerId"
              class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            >
              <option value="">Select a customer</option>
              <option *ngFor="let customer of customers" [value]="customer.id">
                {{ customer.name }} ({{ customer.company || 'No company' }})
              </option>
            </select>
            <div *ngIf="contactForm.get('customerId')?.errors?.['required'] && contactForm.get('customerId')?.touched" class="text-red-500 text-sm mt-1">
              Customer is required
            </div>
          </div>

          <div>
            <label for="phone" class="block text-sm font-medium text-gray-700">Phone</label>
            <input
              type="tel"
              id="phone"
              formControlName="phone"
              class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            />
          </div>

          <div>
            <label for="company" class="block text-sm font-medium text-gray-700">Company</label>
            <input
              type="text"
              id="company"
              formControlName="company"
              class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            />
          </div>

          <div>
            <label for="position" class="block text-sm font-medium text-gray-700">Position</label>
            <input
              type="text"
              id="position"
              formControlName="position"
              class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            />
          </div>

          <div>
            <label for="department" class="block text-sm font-medium text-gray-700">Department</label>
            <input
              type="text"
              id="department"
              formControlName="department"
              class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            />
          </div>

          <div>
            <label for="address" class="block text-sm font-medium text-gray-700">Address</label>
            <textarea
              id="address"
              formControlName="address"
              rows="3"
              class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            ></textarea>
          </div>

          <div>
            <label for="notes" class="block text-sm font-medium text-gray-700">Notes</label>
            <textarea
              id="notes"
              formControlName="notes"
              rows="3"
              class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            ></textarea>
          </div>

          <div class="flex justify-end space-x-4">
            <button
              type="button"
              (click)="goBack()"
              class="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              [disabled]="contactForm.invalid"
              class="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Create Contact
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
})
export class ContactCreateComponent implements OnInit {
  contactForm: FormGroup;
  customers: Customer[] = [];

  constructor(
    private fb: FormBuilder,
    private contactService: ContactService,
    private customerService: CustomerService,
    private router: Router
  ) {
    this.contactForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      customerId: ['', Validators.required],
      phone: [''],
      company: [''],
      position: [''],
      department: [''],
      address: [''],
      notes: [''],
    });
  }

  ngOnInit(): void {
    this.loadCustomers();
  }

  loadCustomers(): void {
    this.customerService.getAll().subscribe({
      next: (response) => {
        this.customers = Array.isArray(response) ? response : response.data;
      },
      error: (error) => {
        console.error('Error loading customers:', error);
      },
    });
  }

  onSubmit(): void {
    if (this.contactForm.valid) {
      const contactData: CreateContactDTO = this.contactForm.value;
      this.contactService.create(contactData).subscribe({
        next: () => {
          this.router.navigate(['/crm/contacts']);
        },
        error: (error) => {
          console.error('Error creating contact:', error);
        },
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/crm/contacts']);
  }
}

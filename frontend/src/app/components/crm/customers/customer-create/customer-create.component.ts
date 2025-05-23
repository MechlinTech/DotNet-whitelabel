import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CustomerService, CreateCustomerDTO } from '../../../../services/crm/customer.service';

@Component({
  selector: 'app-customer-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="container mx-auto px-4 py-8">
      <div class="max-w-2xl mx-auto">
        <h1 class="text-2xl font-bold text-gray-800 mb-6">Add New Customer</h1>

        <form [formGroup]="customerForm" (ngSubmit)="onSubmit()" class="space-y-6">
          <div>
            <label for="name" class="block text-sm font-medium text-gray-700">Name</label>
            <input
              type="text"
              id="name"
              formControlName="name"
              class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            />
            <div *ngIf="customerForm.get('name')?.errors?.['required'] && customerForm.get('name')?.touched" class="text-red-500 text-sm mt-1">
              Name is required
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
            <div *ngIf="customerForm.get('email')?.errors?.['required'] && customerForm.get('email')?.touched" class="text-red-500 text-sm mt-1">
              Email is required
            </div>
            <div *ngIf="customerForm.get('email')?.errors?.['email'] && customerForm.get('email')?.touched" class="text-red-500 text-sm mt-1">
              Please enter a valid email address
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
            <label for="industry" class="block text-sm font-medium text-gray-700">Industry</label>
            <input
              type="text"
              id="industry"
              formControlName="industry"
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
              [disabled]="customerForm.invalid"
              class="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Create Customer
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
})
export class CustomerCreateComponent {
  customerForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private customerService: CustomerService,
    private router: Router
  ) {
    this.customerForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      company: [''],
      industry: [''],
      address: [''],
      notes: [''],
    });
  }

  onSubmit(): void {
    if (this.customerForm.valid) {
      const customerData: CreateCustomerDTO = this.customerForm.value;
      this.customerService.create(customerData).subscribe({
        next: () => {
          this.router.navigate(['/crm/customers']);
        },
        error: (error) => {
          console.error('Error creating customer:', error);
        },
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/crm/customers']);
  }
}

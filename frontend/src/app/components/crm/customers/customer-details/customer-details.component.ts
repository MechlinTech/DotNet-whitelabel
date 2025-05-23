import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { CustomerService, Customer } from '../../../../services/crm/customer.service';

@Component({
  selector: 'app-customer-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container mx-auto px-4 py-8">
      <div class="max-w-4xl mx-auto">
        <div class="flex justify-between items-center mb-6">
          <h1 class="text-2xl font-bold text-gray-800">Customer Details</h1>
          <div class="space-x-4">
            <button
              (click)="goBack()"
              class="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
            >
              Back
            </button>
            <button
              (click)="deleteCustomer()"
              class="px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600"
            >
              Delete Customer
            </button>
          </div>
        </div>

        <div *ngIf="customer" class="bg-white rounded-lg shadow overflow-hidden">
          <div class="p-6">
            <div class="grid grid-cols-2 gap-6">
              <div>
                <h3 class="text-sm font-medium text-gray-500">Name</h3>
                <p class="mt-1 text-lg text-gray-900">{{ customer.name }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Email</h3>
                <p class="mt-1 text-lg text-gray-900">{{ customer.email }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Phone</h3>
                <p class="mt-1 text-lg text-gray-900">{{ customer.phone || 'N/A' }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Company</h3>
                <p class="mt-1 text-lg text-gray-900">{{ customer.company || 'N/A' }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Industry</h3>
                <p class="mt-1 text-lg text-gray-900">{{ customer.industry || 'N/A' }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Created At</h3>
                <p class="mt-1 text-lg text-gray-900">{{ customer.createdAt | date:'medium' }}</p>
              </div>
            </div>

            <div class="mt-6">
              <h3 class="text-sm font-medium text-gray-500">Address</h3>
              <p class="mt-1 text-lg text-gray-900">{{ customer.address || 'No address provided' }}</p>
            </div>

            <div class="mt-6">
              <h3 class="text-sm font-medium text-gray-500">Notes</h3>
              <p class="mt-1 text-lg text-gray-900">{{ customer.notes || 'No notes provided' }}</p>
            </div>
          </div>
        </div>

        <div *ngIf="!customer" class="text-center py-12">
          <p class="text-gray-500">Loading customer details...</p>
        </div>
      </div>
    </div>
  `,
})
export class CustomerDetailsComponent implements OnInit {
  customer: Customer | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private customerService: CustomerService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadCustomer(id);
    }
  }

  loadCustomer(id: string): void {
    this.customerService.getById(id).subscribe({
      next: (response) => {
        if ('data' in response) {
          this.customer = response.data;
        } else {
          this.customer = response as Customer;
        }
      },
      error: (error) => {
        console.error('Error loading customer:', error);
        this.router.navigate(['/crm/customers']);
      },
    });
  }

  deleteCustomer(): void {
    if (this.customer && confirm('Are you sure you want to delete this customer?')) {
      this.customerService.delete(this.customer.id).subscribe({
        next: () => {
          this.router.navigate(['/crm/customers']);
        },
        error: (error) => {
          console.error('Error deleting customer:', error);
        },
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/crm/customers']);
  }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DealService, CreateDealDTO, DealStage } from '../../../../services/crm/deal.service';
import { CustomerService, Customer } from '../../../../services/crm/customer.service';

@Component({
  selector: 'app-deal-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="container mx-auto px-4 py-8">
      <div class="max-w-2xl mx-auto">
        <h1 class="text-2xl font-bold text-gray-800 mb-6">Add New Deal</h1>

        <form [formGroup]="dealForm" (ngSubmit)="onSubmit()" class="space-y-6">
          <div>
            <label for="title" class="block text-sm font-medium text-gray-700">Title</label>
            <input
              type="text"
              id="title"
              formControlName="title"
              class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            />
            <div *ngIf="dealForm.get('title')?.errors?.['required'] && dealForm.get('title')?.touched" class="text-red-500 text-sm mt-1">
              Title is required
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
                {{ customer.name }}
              </option>
            </select>
            <div *ngIf="dealForm.get('customerId')?.errors?.['required'] && dealForm.get('customerId')?.touched" class="text-red-500 text-sm mt-1">
              Customer is required
            </div>
          </div>

          <div>
            <label for="value" class="block text-sm font-medium text-gray-700">Value</label>
            <input
              type="number"
              id="value"
              formControlName="value"
              class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            />
            <div *ngIf="dealForm.get('value')?.errors?.['required'] && dealForm.get('value')?.touched" class="text-red-500 text-sm mt-1">
              Value is required
            </div>
          </div>

          <div>
            <label for="stage" class="block text-sm font-medium text-gray-700">Stage</label>
            <select
              id="stage"
              formControlName="stage"
              class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            >
              <option value="">Select a stage</option>
              <option [value]="DealStage.Qualification">Qualification</option>
              <option [value]="DealStage.Proposal">Proposal</option>
              <option [value]="DealStage.Negotiation">Negotiation</option>
              <option [value]="DealStage.ClosedWon">Closed Won</option>
              <option [value]="DealStage.ClosedLost">Closed Lost</option>
            </select>
            <div *ngIf="dealForm.get('stage')?.errors?.['required'] && dealForm.get('stage')?.touched" class="text-red-500 text-sm mt-1">
              Stage is required
            </div>
          </div>

          <div>
            <label for="expectedCloseDate" class="block text-sm font-medium text-gray-700">Expected Close Date</label>
            <input
              type="date"
              id="expectedCloseDate"
              formControlName="expectedCloseDate"
              class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            />
            <div *ngIf="dealForm.get('expectedCloseDate')?.errors?.['required'] && dealForm.get('expectedCloseDate')?.touched" class="text-red-500 text-sm mt-1">
              Expected close date is required
            </div>
          </div>

          <div>
            <label for="description" class="block text-sm font-medium text-gray-700">Description</label>
            <textarea
              id="description"
              formControlName="description"
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
              [disabled]="dealForm.invalid"
              class="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Create Deal
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
})
export class DealCreateComponent implements OnInit {
  dealForm: FormGroup;
  customers: Customer[] = [];
  DealStage = DealStage; // Make enum available in template

  constructor(
    private fb: FormBuilder,
    private dealService: DealService,
    private customerService: CustomerService,
    private router: Router
  ) {
    this.dealForm = this.fb.group({
      title: ['', Validators.required],
      customerId: ['', Validators.required],
      value: ['', [Validators.required, Validators.min(0)]],
      stage: ['', Validators.required],
      expectedCloseDate: ['', Validators.required],
      description: [''],
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
    if (this.dealForm.valid) {
      const formValue = this.dealForm.value;
      const dealData: CreateDealDTO = {
        title: formValue.title,
        customerId: formValue.customerId,
        value: formValue.value,
        stage: parseInt(formValue.stage),
        expectedCloseDate: formValue.expectedCloseDate,
        description: formValue.description
      };

      this.dealService.create(dealData).subscribe({
        next: () => {
          this.router.navigate(['/crm/deals']);
        },
        error: (error) => {
          console.error('Error creating deal:', error);
        },
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/crm/deals']);
  }
}

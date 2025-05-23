import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TenantService, Tenant } from '../../../services/tenant/tenant.service';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-tenant-details',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <!-- Loading Overlay -->
    <div *ngIf="loading" class="loading-overlay">
      <div class="loading-spinner"></div>
    </div>

    <!-- Tenant Details Container -->
    <div class="min-h-screen bg-gray-100">
      <!-- Header -->
      <header class="bg-white shadow">
        <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
          <div class="flex justify-between items-center">
            <h1 class="text-3xl font-bold text-gray-900">
              {{ isEditing ? 'Edit Tenant' : 'Tenant Details' }}
            </h1>
            <div class="flex space-x-4">
            <button
                *ngIf="!isEditing"
                (click)="startEditing()"
                class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700"
            >
                Edit Tenant
            </button>
            <button
                (click)="navigateToTenantList()"
                class="inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50"
            >
                Back to List
            </button>
            </div>
          </div>
        </div>
      </header>

      <!-- Main Content -->
      <main class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <!-- Error Message -->
        <div *ngIf="errorMessage" class="mb-6 bg-red-50 border-l-4 border-red-400 p-4">
          <div class="flex">
            <div class="flex-shrink-0">
              <svg class="h-5 w-5 text-red-400" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
              </svg>
            </div>
            <div class="ml-3">
              <p class="text-sm text-red-700">{{ errorMessage }}</p>
            </div>
              </div>
              </div>

        <!-- Tenant Details Form -->
        <div class="bg-white shadow rounded-lg p-6">
          <form *ngIf="tenant" (ngSubmit)="onSubmit()" class="space-y-6">
            <!-- Name -->
              <div>
              <label for="name" class="block text-sm font-medium text-gray-700">Name</label>
              <input
                type="text"
                id="name"
                name="name"
                [(ngModel)]="tenant.name"
                [disabled]="!isEditing"
                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                required
              >
              </div>

            <!-- Identifier -->
              <div>
              <label for="identifier" class="block text-sm font-medium text-gray-700">Identifier</label>
              <input
                type="text"
                id="identifier"
                name="identifier"
                [(ngModel)]="tenant.identifier"
                [disabled]="!isEditing"
                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                required
              >
              </div>

            <!-- Domain -->
              <div>
              <label for="domain" class="block text-sm font-medium text-gray-700">Domain</label>
              <input
                type="text"
                id="domain"
                name="domain"
                [(ngModel)]="tenant.domain"
                [disabled]="!isEditing"
                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                required
              >
              </div>

            <!-- Status -->
              <div>
              <label for="isActive" class="block text-sm font-medium text-gray-700">Status</label>
              <select
                id="isActive"
                name="isActive"
                [(ngModel)]="tenant.isActive"
                [disabled]="!isEditing"
                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
              >
                <option [ngValue]="true">Active</option>
                <option [ngValue]="false">Inactive</option>
              </select>
            </div>

            <!-- Logo URL -->
            <div>
              <label for="logoUrl" class="block text-sm font-medium text-gray-700">Logo URL</label>
              <input
                type="text"
                id="logoUrl"
                name="logoUrl"
                [(ngModel)]="tenant.logoUrl"
                [disabled]="!isEditing"
                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
              >
            </div>

            <!-- Form Actions -->
            <div *ngIf="isEditing" class="flex justify-end space-x-4">
              <button
                type="button"
                (click)="cancelEditing()"
                class="inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50"
              >
                Cancel
              </button>
              <button
                type="submit"
                class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700"
              >
                Save Changes
              </button>
            </div>
          </form>
        </div>
      </main>
    </div>
  `,
  styles: [`
    .loading-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background-color: rgba(0, 0, 0, 0.5);
      display: flex;
      justify-content: center;
      align-items: center;
      z-index: 1000;
    }

    .loading-spinner {
      border: 4px solid #f3f3f3;
      border-top: 4px solid #3498db;
      border-radius: 50%;
      width: 40px;
      height: 40px;
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }
  `]
})
export class TenantDetailsComponent implements OnInit {
  tenant: Tenant | null = null;
  originalTenant: Tenant | null = null;
  loading = false;
  errorMessage: string | null = null;
  isEditing = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private tenantService: TenantService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login']);
      return;
    }
    this.loadTenant();
  }

  loadTenant(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.router.navigate(['/tenant/list']);
      return;
  }

    this.loading = true;
    this.errorMessage = null;
    this.tenantService.getTenant(id).subscribe({
      next: (response: { tenant: Tenant }) => {
        this.tenant = response.tenant;
        this.originalTenant = { ...response.tenant };
        this.loading = false;
      },
      error: (error: any) => {
        this.loading = false;
        if (error.status === 403) {
          this.errorMessage = 'You do not have permission to view this tenant. Please contact your administrator.';
        } else {
          this.errorMessage = 'Error loading tenant details. Please try again later.';
        }
        console.error('Error loading tenant:', error);
      }
    });
  }

  startEditing(): void {
    this.isEditing = true;
  }

  cancelEditing(): void {
    if (this.originalTenant) {
      this.tenant = { ...this.originalTenant };
    }
    this.isEditing = false;
  }

  onSubmit(): void {
    if (!this.tenant) return;

    this.loading = true;
    this.errorMessage = null;
    this.tenantService.updateTenant(this.tenant.id, this.tenant).subscribe({
      next: (response: { tenant: Tenant }) => {
        this.tenant = response.tenant;
        this.originalTenant = { ...response.tenant };
        this.isEditing = false;
        this.loading = false;
        },
      error: (error: any) => {
        this.loading = false;
        if (error.status === 403) {
          this.errorMessage = 'You do not have permission to update this tenant. Please contact your administrator.';
        } else {
          this.errorMessage = 'Error updating tenant. Please try again later.';
        }
        console.error('Error updating tenant:', error);
      }
      });
    }

  navigateToTenantList(): void {
    this.router.navigate(['/tenant']);
  }
}

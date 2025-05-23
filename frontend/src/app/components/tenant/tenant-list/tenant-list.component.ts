import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TenantService, Tenant } from '../../../services/tenant/tenant.service';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-tenant-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tenant-list.component.html',
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
export class TenantListComponent implements OnInit {
  tenants: Tenant[] = [];
  loading = false;
  errorMessage: string | null = null;

  constructor(
    private tenantService: TenantService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login']);
      return;
    }
    this.loadTenants();
  }

  loadTenants(): void {
    this.loading = true;
    this.errorMessage = null;
    this.tenantService.getAllTenants().subscribe({
      next: (response) => {
        this.tenants = response.tenants;
        this.loading = false;
      },
      error: (error) => {
        this.loading = false;
        if (error.status === 403) {
          this.errorMessage = 'You do not have permission to view tenants. Please contact your administrator.';
        } else {
          this.errorMessage = 'Error loading tenants. Please try again later.';
        }
        console.error('Error loading tenants:', error);
      }
    });
  }

  navigateToDetails(tenant: Tenant): void {
    this.router.navigate(['/tenant/details', tenant.id]);
  }

  navigateToEdit(tenant: Tenant): void {
    this.router.navigate(['/tenant/edit', tenant.id]);
  }

  navigateToUserManagement(tenant: Tenant): void {
    this.router.navigate(['/tenant', tenant.id, 'users']);
  }

  deleteTenant(tenant: Tenant): void {
    if (confirm(`Are you sure you want to delete ${tenant.name}?`)) {
      this.loading = true;
      this.errorMessage = null;
      this.tenantService.deleteTenant(tenant.id).subscribe({
        next: () => {
          this.tenants = this.tenants.filter(t => t.id !== tenant.id);
          this.loading = false;
        },
        error: (error) => {
          this.loading = false;
          if (error.status === 403) {
            this.errorMessage = 'You do not have permission to delete tenants. Please contact your administrator.';
          } else {
            this.errorMessage = 'Error deleting tenant. Please try again later.';
          }
          console.error('Error deleting tenant:', error);
        }
      });
    }
  }
}

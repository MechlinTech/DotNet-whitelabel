import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { TenantService, Tenant } from '../../../services/tenant/tenant.service';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-user-tenants',
  templateUrl: './user-tenants.component.html',
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class UserTenantsComponent implements OnInit {
  userId: string | null = null;
  tenants: Tenant[] = [];
  loading = false;
  error: string | null = null;
  isListView = false;

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

    this.userId = this.route.snapshot.params['userId'];
    this.isListView = !this.userId;

    if (this.isListView) {
      this.loadAllUserTenants();
    } else {
      this.loadUserTenants();
    }
  }

  loadUserTenants(): void {
    if (!this.userId) return;

    this.loading = true;
    this.tenantService.getUserTenants(this.userId).subscribe({
      next: (response) => {
        this.tenants = response.tenants;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load user tenants';
        this.loading = false;
        console.error('Error loading user tenants:', error);
      }
    });
  }

  loadAllUserTenants(): void {
    this.loading = true;
    this.tenantService.getAllUserTenants().subscribe({
      next: (response) => {
        this.tenants = response.tenants;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load all user tenants';
        this.loading = false;
        console.error('Error loading all user tenants:', error);
      }
    });
  }

  navigateToUserDetails(userId: string): void {
    this.router.navigate(['/tenant/user', userId]);
  }

  navigateToTenantDetails(tenantId: string): void {
    this.router.navigate(['/tenant/details', tenantId]);
  }

  navigateToTenantUserManagement(tenantId: string): void {
    this.router.navigate(['/tenant', tenantId, 'users']);
  }

  navigateBack(): void {
    if (this.isListView) {
      this.router.navigate(['/tenant']);
    } else {
      this.router.navigate(['/tenant/user-tenants']);
    }
  }
}

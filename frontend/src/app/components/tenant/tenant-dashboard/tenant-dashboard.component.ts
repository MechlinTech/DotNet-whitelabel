import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TenantService } from '../../../services/tenant/tenant.service';
import { AuthService } from '../../../services/auth/auth.service';
import { TenantListComponent } from '../tenant-list/tenant-list.component';

@Component({
  selector: 'app-tenant-dashboard',
  standalone: true,
  imports: [CommonModule, TenantListComponent],
  templateUrl: './tenant-dashboard.component.html'
})
export class TenantDashboardComponent implements OnInit {
  errorMessage: string | null = null;

  constructor(
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login']);
      return;
    }
  }

  navigateToCreate(): void {
    this.router.navigate(['/tenant/create']);
  }

  navigateToUserTenants(): void {
    this.router.navigate(['/tenant/user-tenants']);
  }

  navigateToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  back(): void {
    this.router.navigate(['/dashboard']);
  }
}

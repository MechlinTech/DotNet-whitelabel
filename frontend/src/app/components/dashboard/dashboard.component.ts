import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';
import { RoleManagementComponent } from '../role/role-management/role-management.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RoleManagementComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  loading = false;
  errorMessage: string | null = null;
  showRoleManagement = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login']);
    }
  }

  navigateToTenantManagement(): void {
    this.router.navigate(['/tenant']);
  }

  navigateToCrmDashboard(): void {
    this.router.navigate(['/crm']);
  }

  toggleRoleManagement(): void {
    this.showRoleManagement = !this.showRoleManagement;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }
}

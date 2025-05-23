import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TenantService, User } from '../../../services/tenant/tenant.service';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-tenant-user-management',
  templateUrl: './tenant-user-management.component.html',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class TenantUserManagementComponent implements OnInit {
  tenantId: string = '';
  tenantName: string = '';
  users: User[] = [];
  allUsers: User[] = [];
  loading = false;
  error: string | null = null;
  assignForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private tenantService: TenantService,
    private authService: AuthService,
    private fb: FormBuilder
  ) {
    this.assignForm = this.fb.group({
      userId: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login']);
      return;
    }

    this.tenantId = this.route.snapshot.params['id'];
    this.loadTenantDetails();
    this.loadAllUsers();
  }

  loadTenantDetails(): void {
    this.loading = true;
    this.tenantService.getTenant(this.tenantId).subscribe({
      next: (response) => {
        this.tenantName = response.tenant.name;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load tenant details';
        this.loading = false;
        console.error('Error loading tenant details:', error);
      }
    });
  }

  loadUsersInTenant(): void {
    this.loading = true;
    this.tenantService.getUsersInTenant(this.tenantId).subscribe({
      next: (response) => {
        console.log('Tenant users response:', response); // Debug log
        if (response.userIds && response.userIds.length > 0) {
          this.users = this.allUsers.filter(user => response.userIds?.includes(user.id));
          console.log('Filtered users:', this.users); // Debug log
        } else {
          this.users = [];
        }
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load users';
        this.loading = false;
        console.error('Error loading users:', error);
      }
    });
  }

  loadAllUsers(): void {
    this.tenantService.getAllUsers().subscribe({
      next: (response) => {
        console.log('All users response:', response); // Debug log
        this.allUsers = response.users;
        // After loading all users, load tenant users
        this.loadUsersInTenant();
      },
      error: (error) => {
        this.error = 'Failed to load all users';
        console.error('Error loading all users:', error);
      }
    });
  }

  assignUser(): void {
    if (this.assignForm.valid) {
      this.loading = true;
      const userId = this.assignForm.get('userId')?.value;

      this.tenantService.addUserToTenant(this.tenantId, userId).subscribe({
        next: () => {
          this.loadUsersInTenant();
          this.assignForm.reset();
          this.loading = false;
        },
        error: (error) => {
          this.error = 'Failed to assign user';
          this.loading = false;
          console.error('Error assigning user:', error);
        }
      });
    }
  }

  removeUser(userId: string): void {
    if (confirm('Are you sure you want to remove this user from the tenant?')) {
      this.loading = true;
      this.tenantService.removeUserFromTenant(userId, this.tenantId).subscribe({
        next: () => {
          this.loadUsersInTenant();
          this.loading = false;
        },
        error: (error) => {
          this.error = 'Failed to remove user';
          this.loading = false;
          console.error('Error removing user:', error);
        }
      });
    }
  }

  navigateBack(): void {
    this.router.navigate(['/tenant']);
  }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RoleService, Role, RoleAssignment } from '../../../services/role/role.service';

interface User {
  id: string;
  email: string;
  roles: string[];
}

@Component({
  selector: 'app-role-management',
  templateUrl: './role-management.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class RoleManagementComponent implements OnInit {
  roles: Role[] = [];
  newRoleName: string = '';
  selectedRole: string = '';
  usersInRole: User[] = [];
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;
  selectedUser: User | null = null;
  userEmail: string = '';
  userRoles: string[] = [];

  constructor(private roleService: RoleService) {}

  ngOnInit(): void {
    this.loadRoles();
  }

  loadRoles(): void {
    this.loading = true;
    this.roleService.getAllRoles().subscribe({
      next: (response) => {
        this.roles = response.roles;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load roles';
        this.loading = false;
      }
    });
  }

  createRole(): void {
    if (!this.newRoleName.trim()) {
      this.error = 'Role name cannot be empty';
      return;
    }

    this.loading = true;
    this.roleService.createRole(this.newRoleName).subscribe({
      next: (response) => {
        this.successMessage = response.message;
        this.newRoleName = '';
        this.loadRoles();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to create role';
        this.loading = false;
      }
    });
  }

  deleteRole(roleName: string): void {
    if (confirm(`Are you sure you want to delete the role "${roleName}"?`)) {
      this.loading = true;
      this.roleService.deleteRole(roleName).subscribe({
        next: (response) => {
          this.successMessage = response.message;
          this.loadRoles();
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to delete role';
          this.loading = false;
        }
      });
    }
  }

  viewUsersInRole(roleName: string): void {
    this.selectedRole = roleName;
    this.loading = true;
    this.roleService.getUsersInRole(roleName).subscribe({
      next: (response) => {
        this.usersInRole = response.users;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load users in role';
        this.loading = false;
      }
    });
  }

  loadUserRoles(userId: string): void {
    this.loading = true;
    this.selectedUser = this.usersInRole.find(user => user.id === userId) || null;

    if (!this.selectedUser) {
      this.error = 'User not found';
      this.loading = false;
      return;
    }

    this.roleService.getUserRoles(userId).subscribe({
      next: (response) => {
        this.userRoles = response.roles;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load user roles';
        this.loading = false;
      }
    });
  }

  assignRoleToUser(userId: string, roleName: string): void {
    if (!this.selectedUser) {
      this.error = 'No user selected';
      return;
    }

    this.loading = true;
    const assignment: RoleAssignment = { userId, roleName };
    this.roleService.assignRoleToUser(assignment).subscribe({
      next: (response) => {
        this.successMessage = response.message;
        this.loadUserRoles(userId);
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to assign role to user';
        this.loading = false;
      }
    });
  }

  removeRoleFromUser(userId: string, roleName: string): void {
    if (!this.selectedUser) {
      this.error = 'No user selected';
      return;
    }

    this.loading = true;
    const assignment: RoleAssignment = { userId, roleName };
    this.roleService.removeRoleFromUser(assignment).subscribe({
      next: (response) => {
        this.successMessage = response.message;
        this.loadUserRoles(userId);
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to remove role from user';
        this.loading = false;
      }
    });
  }

  clearMessages(): void {
    this.error = null;
    this.successMessage = null;
  }
}

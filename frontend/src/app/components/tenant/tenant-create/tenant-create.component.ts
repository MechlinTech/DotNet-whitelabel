import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TenantService, CreateTenantDTO } from '../../../services/tenant/tenant.service';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-tenant-create',
  templateUrl: './tenant-create.component.html',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class TenantCreateComponent implements OnInit {
  tenantForm: FormGroup;
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private tenantService: TenantService,
    public router: Router,
    private authService: AuthService
  ) {
    this.tenantForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      identifier: ['', [Validators.required, Validators.pattern('^[a-z0-9-]+$')]],
      description: [''],
      logoUrl: ['', Validators.pattern('https?://.*')],
      theme: [''],
      domain: ['', [Validators.pattern('^[a-zA-Z0-9][a-zA-Z0-9-]{1,61}[a-zA-Z0-9]\\.[a-zA-Z]{2,}$')]],
      subscriptionPlan: [''],
      subscriptionExpiry: ['']
    });
  }

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login']);
      return;
    }
  }

  onSubmit(): void {
    if (this.tenantForm.valid) {
      this.loading = true;
      this.error = null;
      this.successMessage = null;

      const formValue = this.tenantForm.value;
      const createData: CreateTenantDTO = {
        ...formValue,
        subscriptionExpiry: formValue.subscriptionExpiry
          ? new Date(formValue.subscriptionExpiry).toISOString()
          : null
      };

      console.log('Creating tenant:', createData);

      this.tenantService.createTenant(createData).subscribe({
        next: (response) => {
          console.log('Create response:', response);
          this.loading = false;
          this.successMessage = 'Tenant created successfully!';
          setTimeout(() => {
            this.router.navigate(['/tenant']);
          }, 1500);
        },
        error: (error) => {
          this.loading = false;
          if (error.status === 403) {
            this.error = 'You do not have permission to create tenants. Please contact your administrator.';
          } else {
            this.error = 'Failed to create tenant. Please try again later.';
          }
          console.error('Error creating tenant:', error);
        }
      });
    } else {
      this.markFormGroupTouched(this.tenantForm);
    }
  }

  navigateBack(): void {
    this.router.navigate(['/tenant']);
  }

  private markFormGroupTouched(formGroup: FormGroup) {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }
}

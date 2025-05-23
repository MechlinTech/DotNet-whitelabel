import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TenantService, Tenant } from '../../../services/tenant/tenant.service';
import { CommonModule } from '@angular/common';

interface TenantResponse {
  message: string;
  tenant: Tenant;
}

@Component({
  selector: 'app-tenant-edit',
  templateUrl: './tenant-edit.component.html',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class TenantEditComponent implements OnInit {
  tenantForm: FormGroup;
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;
  tenantId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private tenantService: TenantService,
    private route: ActivatedRoute,
    public router: Router
  ) {
    this.tenantForm = this.fb.group({
      name: ['', Validators.required],
      identifier: ['', Validators.required],
      description: [''],
      domain: [''],
      logoUrl: [''],
      theme: [''],
      subscriptionPlan: [''],
      subscriptionExpiry: ['']
    });
  }

  ngOnInit(): void {
    this.tenantId = this.route.snapshot.paramMap.get('id');
    if (this.tenantId) {
      this.loadTenantData();
    }
  }

  private loadTenantData(): void {
    this.loading = true;
    this.error = null;
    this.tenantService.getTenant(this.tenantId!).subscribe({
      next: (response: TenantResponse) => {
        console.log('Loaded tenant data:', response.tenant);
        this.tenantForm.patchValue({
          name: response.tenant.name,
          identifier: response.tenant.identifier,
          description: response.tenant.description || '',
          domain: response.tenant.domain || '',
          logoUrl: response.tenant.logoUrl || '',
          theme: response.tenant.theme || '',
          subscriptionPlan: response.tenant.subscriptionPlan || '',
          subscriptionExpiry: response.tenant.subscriptionExpiry ? new Date(response.tenant.subscriptionExpiry).toISOString().split('T')[0] : ''
        });
        this.loading = false;
      },
      error: (err: Error) => {
        console.error('Error loading tenant:', err);
        this.error = 'Failed to load tenant data. Please try again.';
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.tenantForm.valid && this.tenantId) {
      this.loading = true;
      this.error = null;
      this.successMessage = null;

      const formData = this.tenantForm.value;
      console.log('Submitting form data:', formData);

      this.tenantService.updateTenant(this.tenantId, formData).subscribe({
        next: (response: TenantResponse) => {
          console.log('Tenant updated successfully:', response.tenant);
          this.successMessage = response.message;
          setTimeout(() => {
            this.router.navigate(['/tenant']);
          }, 1500);
        },
        error: (err: Error) => {
          console.error('Error updating tenant:', err);
          this.error = 'Failed to update tenant. Please try again.';
          this.loading = false;
        }
      });
    }
  }

  navigateBack(): void {
    this.router.navigate(['/tenant']);
  }
}

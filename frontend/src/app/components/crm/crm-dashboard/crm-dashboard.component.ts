import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CustomerService } from '../../../services/crm/customer.service';
import { ContactService } from '../../../services/crm/contact.service';
import { DealService } from '../../../services/crm/deal.service';
import { forkJoin, catchError, finalize } from 'rxjs';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-crm-dashboard',
  templateUrl: './crm-dashboard.component.html',
  standalone: true,
  imports: [CommonModule]
})
export class CrmDashboardComponent implements OnInit {
  loading = false;
  error: string | null = null;
  stats = {
    customers: 0,
    contacts: 0,
    deals: 0,
    activeDeals: 0
  };

  constructor(
    private customerService: CustomerService,
    private contactService: ContactService,
    private dealService: DealService,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.loading = true;
    this.error = null;

    // Check if user has a tenant ID
    const tenantId = this.authService.getTenantId();
    if (!tenantId) {
      this.error = 'You are not associated with any tenant. Please contact your administrator.';
      this.loading = false;
      return;
    }

    forkJoin({
      customers: this.customerService.getAll(),
      contacts: this.contactService.getAll(),
      deals: this.dealService.getAll()
    }).pipe(
      catchError(error => {
        if (error.message.includes('Tenant identifier is required')) {
          this.error = 'You are not associated with any tenant. Please contact your administrator.';
        } else {
          this.error = error.message || 'Failed to load CRM statistics';
        }
        throw error;
      }),
      finalize(() => {
        this.loading = false;
      })
    ).subscribe({
      next: (data) => {
        this.stats = {
          customers: Array.isArray(data.customers) ? data.customers.length : (data.customers.data?.length || 0),
          contacts: Array.isArray(data.contacts) ? data.contacts.length : (data.contacts.data?.length || 0),
          deals: Array.isArray(data.deals) ? data.deals.length : (data.deals.data?.length || 0),
          activeDeals: Array.isArray(data.deals)
            ? data.deals.filter((deal: any) => deal.status === 'Active').length
            : (data.deals.data?.filter((deal: any) => deal.status === 'Active').length || 0)
        };
      }
    });
  }

  navigateToCustomers(): void {
    this.router.navigate(['/crm/customers']);
  }

  navigateToContacts(): void {
    this.router.navigate(['/crm/contacts']);
  }

  navigateToDeals(): void {
    this.router.navigate(['/crm/deals']);
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }
}

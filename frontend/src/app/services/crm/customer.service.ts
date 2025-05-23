import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseCRMService } from './base-crm.service';
import { AuthService } from '../auth/auth.service';

export interface Customer {
  id: string;
  name: string;
  email: string;
  phone?: string;
  address?: string;
  company?: string;
  industry?: string;
  status: CustomerStatus;
  notes?: string;
  createdAt: Date;
  updatedAt?: Date;
}

export enum CustomerStatus {
  Lead = 'Lead',
  Prospect = 'Prospect',
  Customer = 'Customer',
  Inactive = 'Inactive'
}

export interface CreateCustomerDTO {
  name: string;
  email: string;
  phone?: string;
  address?: string;
  company?: string;
  industry?: string;
  status: CustomerStatus;
  notes?: string;
}

export interface UpdateCustomerDTO {
  name?: string;
  email?: string;
  phone?: string;
  address?: string;
  company?: string;
  industry?: string;
  status?: CustomerStatus;
  notes?: string;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerService extends BaseCRMService<Customer, CreateCustomerDTO, UpdateCustomerDTO> {
  constructor(
    http: HttpClient,
    authService: AuthService
  ) {
    super(http, 'customers', authService);
  }
}

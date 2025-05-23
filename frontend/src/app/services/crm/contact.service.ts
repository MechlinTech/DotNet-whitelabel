import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseCRMService } from './base-crm.service';
import { AuthService } from '../auth/auth.service';

export interface Contact {
  id: string;
  firstName: string;
  lastName: string;
  name: string;
  email: string;
  phone?: string;
  position?: string;
  department?: string;
  company?: string;
  address?: string;
  customerId?: string;
  notes?: string;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateContactDTO {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  position?: string;
  department?: string;
  company?: string;
  address?: string;
  customerId?: string;
  notes?: string;
}

export interface UpdateContactDTO {
  firstName?: string;
  lastName?: string;
  email?: string;
  phone?: string;
  position?: string;
  department?: string;
  company?: string;
  address?: string;
  customerId?: string;
  notes?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ContactService extends BaseCRMService<Contact, CreateContactDTO, UpdateContactDTO> {
  constructor(
    http: HttpClient,
    authService: AuthService
  ) {
    super(http, 'contacts', authService);
  }
}

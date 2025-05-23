import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseCRMService } from './base-crm.service';
import { AuthService } from '../auth/auth.service';

export enum DealStage {
  Qualification = 0,
  Proposal = 1,
  Negotiation = 2,
  ClosedWon = 3,
  ClosedLost = 4
}

export interface Deal {
  id: string;
  title: string;
  description?: string;
  value: number;
  stage: DealStage;
  customerId: string;
  expectedCloseDate: Date;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateDealDTO {
  title: string;
  description?: string;
  value: number;
  stage: DealStage;
  customerId: string;
  expectedCloseDate: Date;
}

export interface UpdateDealDTO {
  title?: string;
  description?: string;
  value?: number;
  stage?: DealStage;
  expectedCloseDate?: Date;
}

@Injectable({
  providedIn: 'root'
})
export class DealService extends BaseCRMService<Deal, CreateDealDTO, UpdateDealDTO> {
  constructor(
    http: HttpClient,
    authService: AuthService
  ) {
    super(http, 'deals', authService);
  }
}

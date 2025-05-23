import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DealService, Deal, DealStage } from '../../../../services/crm/deal.service';

@Component({
  selector: 'app-deal-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container mx-auto px-4 py-8">
      <div class="max-w-7xl mx-auto">
        <div class="flex justify-between items-center mb-6">
          <h1 class="text-2xl font-bold text-gray-800">Deals</h1>
          <div class="space-x-4">
            <button
              (click)="goBack()"
              class="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
            >
              Back
            </button>
            <button
              (click)="createDeal()"
              class="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600"
            >
              Add Deal
            </button>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow overflow-hidden">
          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Title
                  </th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Customer
                  </th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Value
                  </th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Stage
                  </th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Expected Close
                  </th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                <tr *ngFor="let deal of deals" class="hover:bg-gray-50">
                  <td class="px-6 py-4 whitespace-nowrap">
                    <div class="text-sm font-medium text-gray-900">{{ deal.title }}</div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <div class="text-sm text-gray-500">{{ deal.customerId }}</div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <div class="text-sm text-gray-500">{{ deal.value | currency }}</div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <span
                      [class]="getStageClass(deal.stage)"
                      class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full"
                    >
                      {{ getStageName(deal.stage) }}
                    </span>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <div class="text-sm text-gray-500">{{ deal.expectedCloseDate | date:'mediumDate' }}</div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <button
                      (click)="viewDeal(deal.id)"
                      class="text-blue-600 hover:text-blue-900 mr-4"
                    >
                      View
                    </button>
                    <button
                      (click)="deleteDeal(deal.id)"
                      class="text-red-600 hover:text-red-900"
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  `,
})
export class DealListComponent implements OnInit {
  deals: Deal[] = [];
  DealStage = DealStage;

  constructor(
    private dealService: DealService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadDeals();
  }

  loadDeals(): void {
    this.dealService.getAll().subscribe({
      next: (response) => {
        this.deals = Array.isArray(response) ? response : response.data;
      },
      error: (error) => {
        console.error('Error loading deals:', error);
      },
    });
  }

  createDeal(): void {
    this.router.navigate(['/crm/deals/create']);
  }

  viewDeal(id: string): void {
    this.router.navigate(['/crm/deals', id]);
  }

  deleteDeal(id: string): void {
    if (confirm('Are you sure you want to delete this deal?')) {
      this.dealService.delete(id).subscribe({
        next: () => {
          this.loadDeals();
        },
        error: (error) => {
          console.error('Error deleting deal:', error);
        },
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/crm']);
  }

  getStageClass(stage: DealStage): string {
    const stageClasses: { [key: number]: string } = {
      [DealStage.Qualification]: 'bg-blue-100 text-blue-800',
      [DealStage.Proposal]: 'bg-yellow-100 text-yellow-800',
      [DealStage.Negotiation]: 'bg-purple-100 text-purple-800',
      [DealStage.ClosedWon]: 'bg-green-100 text-green-800',
      [DealStage.ClosedLost]: 'bg-red-100 text-red-800',
    };
    return stageClasses[stage] || 'bg-gray-100 text-gray-800';
  }

  getStageName(stage: DealStage): string {
    const stageNames: { [key: number]: string } = {
      [DealStage.Qualification]: 'Qualification',
      [DealStage.Proposal]: 'Proposal',
      [DealStage.Negotiation]: 'Negotiation',
      [DealStage.ClosedWon]: 'Closed Won',
      [DealStage.ClosedLost]: 'Closed Lost',
    };
    return stageNames[stage] || 'Unknown';
  }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { DealService, Deal, DealStage } from '../../../../services/crm/deal.service';

@Component({
  selector: 'app-deal-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container mx-auto px-4 py-8">
      <div class="max-w-4xl mx-auto">
        <div class="flex justify-between items-center mb-6">
          <h1 class="text-2xl font-bold text-gray-800">Deal Details</h1>
          <div class="space-x-4">
            <button
              (click)="goBack()"
              class="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
            >
              Back
            </button>
            <button
              (click)="deleteDeal()"
              class="px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600"
            >
              Delete Deal
            </button>
          </div>
        </div>

        <div *ngIf="deal" class="bg-white rounded-lg shadow overflow-hidden">
          <div class="p-6">
            <div class="grid grid-cols-2 gap-6">
              <div>
                <h3 class="text-sm font-medium text-gray-500">Title</h3>
                <p class="mt-1 text-lg text-gray-900">{{ deal.title }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Customer ID</h3>
                <p class="mt-1 text-lg text-gray-900">{{ deal.customerId }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Value</h3>
                <p class="mt-1 text-lg text-gray-900">{{ deal.value | currency }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Stage</h3>
                <span
                  [class]="getStageClass(deal.stage)"
                  class="px-2 py-1 inline-flex text-sm leading-5 font-semibold rounded-full"
                >
                  {{ getStageName(deal.stage) }}
                </span>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Expected Close Date</h3>
                <p class="mt-1 text-lg text-gray-900">{{ deal.expectedCloseDate | date:'mediumDate' }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Created At</h3>
                <p class="mt-1 text-lg text-gray-900">{{ deal.createdAt | date:'medium' }}</p>
              </div>

              <div>
                <h3 class="text-sm font-medium text-gray-500">Last Updated</h3>
                <p class="mt-1 text-lg text-gray-900">{{ deal.updatedAt | date:'medium' }}</p>
              </div>
            </div>

            <div class="mt-6">
              <h3 class="text-sm font-medium text-gray-500">Description</h3>
              <p class="mt-1 text-lg text-gray-900">{{ deal.description || 'No description provided' }}</p>
            </div>
          </div>
        </div>

        <div *ngIf="!deal" class="text-center py-12">
          <p class="text-gray-500">Loading deal details...</p>
        </div>
      </div>
    </div>
  `,
})
export class DealDetailsComponent implements OnInit {
  deal: Deal | null = null;
  DealStage = DealStage;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private dealService: DealService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadDeal(id);
    }
  }

  loadDeal(id: string): void {
    this.dealService.getById(id).subscribe({
      next: (response) => {
        this.deal = 'data' in response ? response.data : response;
      },
      error: (error) => {
        console.error('Error loading deal:', error);
        this.router.navigate(['/crm/deals']);
      },
    });
  }

  deleteDeal(): void {
    if (this.deal && confirm('Are you sure you want to delete this deal?')) {
      this.dealService.delete(this.deal.id).subscribe({
        next: () => {
          this.router.navigate(['/crm/deals']);
        },
        error: (error) => {
          console.error('Error deleting deal:', error);
        },
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/crm/deals']);
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

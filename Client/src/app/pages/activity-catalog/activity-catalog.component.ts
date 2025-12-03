import { Component, OnInit } from '@angular/core';
import { ActivityCatalogClient, ActivityCatalogResponseDto } from '../../shared/models/Nswag generated/NswagGenerated';
import { AccountService } from '../../account/account.service';
import { Router } from '@angular/router';
import { SharedService } from '../../shared/shared.service';

@Component({
  selector: 'app-activity-catalog',
  standalone: false,
  templateUrl: './activity-catalog.component.html',
  styleUrl: './activity-catalog.component.css'
})
export class ActivityCatalogComponent implements OnInit {
  activities: ActivityCatalogResponseDto[] = [];

  constructor(public accountService: AccountService,private router: Router,private sharedService :SharedService, private activityClient: ActivityCatalogClient) {}

  ngOnInit(): void {
    this.loadActivities();
  }

  loadActivities(): void {
    this.activityClient.getAll().subscribe({
      next: data => this.activities = data,
      error: err => console.error('Error:', err)
    });
  }
  onEdit(activity: ActivityCatalogResponseDto): void {
    if (activity.activityCatalogID) {
      this.router.navigate(['/activitycatalog/edit', activity.activityCatalogID]);
    }
}
onDelete(activity: ActivityCatalogResponseDto): void {
  if (!activity.activityCatalogID) return;

 this.sharedService.showConfirmation(
  'Confirm', 
  `Are you sure you want to delete: "${activity.name}"?`
).subscribe((confirmed: boolean) => {
  if (confirmed) {
    this.activityClient.delete(activity.activityCatalogID!).subscribe({
      next: () => {
        this.activities = this.activities.filter(a => a.activityCatalogID !== activity.activityCatalogID);
        this.sharedService.showNotification(true, 'Success', 'Success delete!');
      },
      error: (err) => {
        const errorTitle = 'Error';

        if (err.response) {
           let errorMessage = 'Error'; 
           try {
              const parsed = JSON.parse(err.response);
              errorMessage = parsed.message || parsed.title || parsed.detail || errorMessage;
           } catch {
              errorMessage = err.message || errorMessage;
           }
           this.sharedService.showNotification(false, errorTitle, errorMessage);
        }
        else {
          this.sharedService.showNotification(false, errorTitle, 'Error');
        }
      }
    });
  }
});
}


}

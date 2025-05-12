import { Component, OnInit } from '@angular/core';
import { ActivityCatalogClient, ActivityCatalogResponseDto } from '../../shared/models/Nswag generated/NswagGenerated';
import { AccountService } from '../../account/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-activity-catalog',
  standalone: false,
  templateUrl: './activity-catalog.component.html',
  styleUrl: './activity-catalog.component.css'
})
export class ActivityCatalogComponent implements OnInit {
  activities: ActivityCatalogResponseDto[] = [];

  constructor(public accountService: AccountService,private router: Router, private activityClient: ActivityCatalogClient) {}

  ngOnInit(): void {
    this.loadActivities();
  }

  loadActivities(): void {
    this.activityClient.getAll().subscribe({
      next: data => this.activities = data,
      error: err => console.error('Hiba az aktivitáskatalógus betöltésekor:', err)
    });
  }
  onEdit(activity: ActivityCatalogResponseDto): void {
    if (activity.activityCatalogID) {
      this.router.navigate(['/activitycatalog/edit', activity.activityCatalogID]);
    }
}
onDelete(activity: ActivityCatalogResponseDto): void {
  if (!activity.activityCatalogID) return;

  const confirmed = confirm(`You wanna delete this: "${activity.name}"?`);
  if (!confirmed) return;

  this.activityClient.delete(activity.activityCatalogID).subscribe({
    next: () => {
      this.activities = this.activities.filter(a => a.activityCatalogID !== activity.activityCatalogID);
    },
    error: err => {
      if (err.error instanceof Blob) {
        err.error.text().then((text: string) => {
          try {
            const parsed = JSON.parse(text);
            alert(parsed.message || 'Nem törölhető: ismeretlen hiba.');
          } catch {
            alert('Nem törölhető: nyers válasz.');
          }
        });
      } else {
        alert('You use this activity.');
      }
    }
  });
}


}

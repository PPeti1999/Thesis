import { Component, OnInit } from '@angular/core';
import { ActivityCatalogClient, ActivityCatalogResponseDto } from '../../shared/models/Nswag generated/NswagGenerated';
import { AccountService } from '../../account/account.service';

@Component({
  selector: 'app-activity-catalog',
  standalone: false,
  templateUrl: './activity-catalog.component.html',
  styleUrl: './activity-catalog.component.css'
})
export class ActivityCatalogComponent implements OnInit {
  activities: ActivityCatalogResponseDto[] = [];

  constructor(public accountService: AccountService, private activityClient: ActivityCatalogClient) {}

  ngOnInit(): void {
    this.loadActivities();
  }

  loadActivities(): void {
    this.activityClient.getAll().subscribe({
      next: data => this.activities = data,
      error: err => console.error('Hiba az aktivitáskatalógus betöltésekor:', err)
    });
  }
  onEdit(activity: ActivityCatalogResponseDto): void{
    
  }
}

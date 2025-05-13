import { Component, OnInit } from '@angular/core';
import { AccountClient, ActivityCatalogClient, ActivityCatalogResponseDto, DailyNoteClient, DailyNoteResponseDto, MealEntriesClient, MealEntryResponseDto, UserActivityClient, UserActivityCreateDto, UserActivityResponseDto, WeightUpdateDto } from '../../shared/models/Nswag generated/NswagGenerated';
import { ActivatedRoute, Router } from '@angular/router';
import * as bootstrap from 'bootstrap';


@Component({
  selector: 'app-daily-note',
  standalone: false,
  templateUrl: './daily-note.component.html',
  styleUrl: './daily-note.component.css'
})
export class DailyNoteComponent implements OnInit{
  editingActivityId: string | null = null;
isEditMode: boolean = false;
  dailyNote!: DailyNoteResponseDto;
  updatedWeight: number = 0;
  burnedCalories: number = 0;
  mealEntries: MealEntryResponseDto[] = [];
  userActivities: UserActivityResponseDto[] = [];
  activityCatalog: ActivityCatalogResponseDto[] = [];

  selectedActivityId: string = '';
  selectedDuration: number = 0;
  durationModal!: bootstrap.Modal;

  macroNutrients: {
    label: string;
    actual: number;
    target: number;
  }[] = [];

  constructor(
    private route: ActivatedRoute,
    private userClient: AccountClient,
    private router: Router,
    private dailyNoteClient: DailyNoteClient,
    private mealEntriesClient: MealEntriesClient,
    private userActivityClient: UserActivityClient,
    private activityCatalogClient: ActivityCatalogClient
  ) {}

  ngOnInit(): void {
    this.userClient.getProfile().subscribe(profile => {
      const missingProfile = !profile.age || !profile.height || !profile.weight;
      if (missingProfile) {
        this.router.navigate(['/create-profile']);
      } else {
        this.loadDailyNote();
      }
    });
  }

  loadDailyNote(): void {
    this.dailyNoteClient.getToday().subscribe({
      next: note => {
        this.dailyNote = note;
        this.updatedWeight = note.dailyWeight ?? 0;
        this.setupMacroNutrients();
        this.loadMealEntries();
        this.loadUserActivities();
        this.loadActivityCatalog();
      },
      error: err => console.error('Error loading daily note:', err)
    });
  }

  loadMealEntries(): void {
    if (!this.dailyNote?.dailyNoteID) return;
    this.mealEntriesClient.getByDailyNote(this.dailyNote.dailyNoteID).subscribe({
      next: entries => {
        this.mealEntries = entries;
      },
      error: err => console.error('Error loading meal entries:', err)
    });
  }

  loadUserActivities(): void {
    this.userActivityClient.getAll().subscribe({
      next: all => {
        this.userActivities = all.filter(a => a.dailyNoteID === this.dailyNote.dailyNoteID);
        this.burnedCalories = this.userActivities.reduce((sum, a) => sum + (a.calories ?? 0), 0);
      },
      error: err => console.error('Error loading user activities:', err)
    });
  }

  loadActivityCatalog(): void {
    this.activityCatalogClient.getAll().subscribe({
      next: list => this.activityCatalog = list,
      error: err => console.error('Error loading activity catalog:', err)
    });
  }

  setupMacroNutrients(): void {
    this.macroNutrients = [
      {
        label: 'Protein',
        actual: this.dailyNote.actualSumProtein ?? 0,
        target: this.dailyNote.dailyTargetProtein ?? 0
      },
      {
        label: 'Carbohydrates',
        actual: this.dailyNote.actualSumCarb ?? 0,
        target: this.dailyNote.dailyTargetCarb ?? 0
      },
      {
        label: 'Fat',
        actual: this.dailyNote.actualSumFat ?? 0,
        target: this.dailyNote.dailyTargetFat ?? 0
      }
    ];
  }

  getPercentage(actual: number, target: number): number {
    if (target === 0) return 0;
    return Math.min((actual / target) * 100, 100);
  }

  updateWeight(): void {
    const dto = new WeightUpdateDto();
    dto.weight = this.updatedWeight;
    this.dailyNoteClient.updateWeight(this.dailyNote.dailyNoteID ?? '', dto).subscribe({
      next: updated => {
        this.dailyNote = updated;
        this.setupMacroNutrients();
      },
      error: err => console.error('Error updating weight:', err)
    });
  }

  openActivityCatalogPopup(): void {
    const modalElement = document.getElementById('activityCatalogModal');
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
  }

  selectActivity(activity: ActivityCatalogResponseDto): void {
    this.selectedActivityId = activity.activityCatalogID ?? '';
    this.selectedDuration = 30;

    const modalElement = document.getElementById('durationModal');
    if (modalElement) {
      this.durationModal = new bootstrap.Modal(modalElement);
      this.durationModal.show();
    }
  }

  saveActivity(): void {
    if (!this.dailyNote?.dailyNoteID || !this.selectedActivityId || this.selectedDuration <= 0) return;
  
    const dto = new UserActivityCreateDto();
    dto.dailyNoteID = this.dailyNote.dailyNoteID;
    dto.activityCatalogID = this.selectedActivityId;
    dto.duration = this.selectedDuration;
  
    if (this.isEditMode && this.editingActivityId) {
      this.userActivityClient.update(this.editingActivityId, dto).subscribe({
        next: () => {
          this.closeModals();
          this.loadDailyNote();
        },
        error: err => console.error('Error updating activity:', err)
      });
    } else {
      this.userActivityClient.create(dto).subscribe({
        next: () => {
          this.closeModals();
          this.loadDailyNote();
        },
        error: err => console.error('Error creating activity:', err)
      });
    }
  }
  
  closeModals(): void {
    bootstrap.Modal.getInstance(document.getElementById('activityCatalogModal')!)?.hide();
    bootstrap.Modal.getInstance(document.getElementById('durationModal')!)?.hide();
  }
  
  deleteActivity(id?: string): void {
    if (!id) return;
    this.userActivityClient.delete(id).subscribe({
      next: () => {
        this.loadDailyNote();
      },
      error: err => console.error('Error deleting activity:', err)
    });
  }

  editActivity(activity: UserActivityResponseDto): void {
    this.selectedActivityId = activity.activityCatalogID ?? '';
    this.selectedDuration = activity.duration ?? 0;
    this.editingActivityId = activity.userActivityID ?? null;
    this.isEditMode = true;
  
    const modalElement = document.getElementById('durationModal');
    if (modalElement) {
      this.durationModal = new bootstrap.Modal(modalElement);
      this.durationModal.show();
    }
  }
  
}

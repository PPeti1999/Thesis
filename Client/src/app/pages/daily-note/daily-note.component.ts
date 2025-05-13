import { Component, OnInit } from '@angular/core';
import { AccountClient, DailyNoteClient, DailyNoteResponseDto, MealEntriesClient, MealEntryResponseDto, WeightUpdateDto } from '../../shared/models/Nswag generated/NswagGenerated';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-daily-note',
  standalone: false,
  templateUrl: './daily-note.component.html',
  styleUrl: './daily-note.component.css'
})
export class DailyNoteComponent implements OnInit{
  dailyNote!: DailyNoteResponseDto;
  updatedWeight: number = 0;
  burnedCalories: number = 0;
  mealEntries: MealEntryResponseDto[] = [];

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
    private mealEntriesClient: MealEntriesClient
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
}

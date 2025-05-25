import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { AccountClient, ActivityCatalogClient, ActivityCatalogResponseDto, DailyNoteClient, DailyNoteResponseDto, FoodClient, FoodResponseDto, MealEntriesClient, MealEntryResponseDto, MealFoodResponseDto, MealFoodsClient, MealRecipeResponseDto, MealRecipesClient, UserActivityClient, UserActivityCreateDto, UserActivityResponseDto, WeightUpdateDto } from '../../shared/models/Nswag generated/NswagGenerated';
import { ActivatedRoute, Router } from '@angular/router';
import * as bootstrap from 'bootstrap';
import { FormsModule } from '@angular/forms';
import { MealItemSearchComponent } from '../meal-item-search/meal-item-search.component';

@Component({
  selector: 'app-daily-note',
  standalone: false,
  templateUrl: './daily-note.component.html',
  styleUrl: './daily-note.component.css'
})
export class DailyNoteComponent implements OnInit{
  @Input() mealEntryId!: string | undefined;
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
  @ViewChild(MealItemSearchComponent) searchComponent?: MealItemSearchComponent;
  selectedMeal!: MealEntryResponseDto;
  mealFoods: MealFoodResponseDto[] = [];
  mealRecipes: MealRecipeResponseDto[] = [];
  
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
    private activityCatalogClient: ActivityCatalogClient,
    private mealFoodsClient: MealFoodsClient,
    private mealRecipesClient: MealRecipesClient,
    private foodClient: FoodClient // 💡 EZ HIÁNYZOTT
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
  selectedFood?: FoodResponseDto;
  editingMealFoodId?: string; // ⬅️ Ezt add hozzá a DailyNoteComponent osztályhoz
  showFoodModal = false;
  
  handleMealItemSelected(event: { type: 'food' | 'recipe'; item: any }) {
    if (event.type === 'food') {
      this.selectedFood = undefined;
  
      setTimeout(() => {
        this.selectedFood = event.item as FoodResponseDto;
  
        const modalElement = document.getElementById('foodQuantityModal');
        if (modalElement) {
          const modal = new bootstrap.Modal(modalElement);
          modal.show();
        }
      }, 300);
    }
  
    // 🔁 Végén hívjuk meg a closeModals-t, hogy biztosan ne nyissa vissza a mealDetailsModal-t
    this.closeModals();
  }
  
  
    // (ha később lesz recipe modal, ott is majd)
  
  
  
  
  
  
  
  
    onFoodAdded(food: MealFoodResponseDto) {
      console.log('[ADDED] Food added/updated:', food);
    
      const mealId = this.selectedMeal.mealEntryID!;
      this.loadMealFoods(mealId);         // frissíti a foods listát
      this.loadMealRecipes(mealId);       // ha esetleg recept is van
      
      // újra lekérjük a meal entry-t és beállítjuk
      this.mealEntriesClient.getByDailyNote(this.dailyNote.dailyNoteID!).subscribe({
        next: entries => {
          this.mealEntries = entries;
          const updated = entries.find(e => e.mealEntryID === mealId);
          if (updated) {
            this.selectedMeal = updated;
    
            // 💡 Modal újratöltés: friss adatokkal
            const modalEl = document.getElementById('mealDetailsModal');
            if (modalEl) {
              const modal = new bootstrap.Modal(modalEl);
              modal.show();
            }
          }
        }
      });
    
      this.loadDailyNote(); // fő makró értékek is frissülnek (kártyák)
      this.editingMealFoodId = undefined;
    }
    
    
    
    
    
  
  loadMealEntries(): void {
    if (!this.dailyNote?.dailyNoteID) return;
    this.mealEntriesClient.getByDailyNote(this.dailyNote.dailyNoteID).subscribe({
      next: entries => this.mealEntries = entries,
      error: err => console.error('Error loading meal entries:', err)
    });
  }

  openMealDetails(meal: MealEntryResponseDto): void {
    this.selectedMeal = meal;
    if (meal.mealEntryID) {
      this.loadMealFoods(meal.mealEntryID);
      this.loadMealRecipes(meal.mealEntryID);
    }
    

    const modalElement = document.getElementById('mealDetailsModal');
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
  }

  foodLookup: { [key: string]: FoodResponseDto } = {};

  loadMealFoods(mealEntryId: string): void {
    this.mealFoodsClient.getByMealEntry(mealEntryId).subscribe({
      next: foods => {
        this.mealFoods = foods;
  
        for (const food of foods) {
          if (food.foodID){
            this.foodClient.getFood(food.foodID).subscribe(result => {
              this.foodLookup[food.foodID!] = result; // mindig frissítjük
            });
          }
        }
      },
      error: err => console.error('Error loading meal foods:', err)
    });
  }
  

  loadMealRecipes(mealEntryId: string): void {
    this.mealRecipesClient.getByMealEntry(mealEntryId).subscribe({
      next: recipes => this.mealRecipes = recipes,
      error: err => console.error('Error loading meal recipes:', err)
    });
  }

  editMealFood(food: MealFoodResponseDto): void {
    if (!food.foodID) return;
  
    this.foodClient.getFood(food.foodID).subscribe(result => {
      const patched = Object.assign(result, { gram: food.quantity });
this.selectedFood = patched;

      this.editingMealFoodId = food.mealFoodID;
  
      const modalEl = document.getElementById('foodQuantityModal');
      if (modalEl) {
        const modal = new bootstrap.Modal(modalEl);
        modal.show();
      }
    });
  }
  
  
  
  
  

  deleteMealFood(id?: string): void {
    if (!id) return;
    this.mealFoodsClient.delete(id).subscribe({
      next: () => this.openMealDetails(this.selectedMeal),
      error: err => console.error('Error deleting meal food:', err)
    });
  }

  editMealRecipe(recipe: MealRecipeResponseDto): void {
    // TODO: implement modal editor
  }

  deleteMealRecipe(id?: string): void {
    if (!id) return;
    this.mealRecipesClient.delete(id).subscribe({
      next: () => this.openMealDetails(this.selectedMeal),
      error: err => console.error('Error deleting meal recipe:', err)
    });
  }


  openAddMealItemModal(): void {
    bootstrap.Modal.getInstance(document.getElementById('mealDetailsModal')!)?.hide();
    const modalElement = document.getElementById('mealItemSearchModal');
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
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
  
  closeModals(keepMealDetailsOpen: boolean = true): void {
    const searchModalEl = document.getElementById('mealItemSearchModal');
    const wasSearchOpen = searchModalEl?.classList.contains('show');
  
    bootstrap.Modal.getInstance(searchModalEl!)?.hide();
    bootstrap.Modal.getInstance(document.getElementById('activityCatalogModal')!)?.hide();
    bootstrap.Modal.getInstance(document.getElementById('durationModal')!)?.hide();
    bootstrap.Modal.getInstance(document.getElementById('mealDetailsModal')!)?.hide();
    bootstrap.Modal.getInstance(document.getElementById('foodQuantityModal')!)?.hide();
    bootstrap.Modal.getInstance(document.getElementById('recipeQuantityModal')!)?.hide();
  
    this.searchComponent?.clear();
  
    // 💡 biztos ami biztos: töröljük az árnyékot is
    const backdropEls = document.querySelectorAll('.modal-backdrop');
    backdropEls.forEach(el => el.remove());
  
    // csak akkor nyisd vissza ha szükséges (opcionális)
    /*
    if (wasSearchOpen && this.selectedMeal && keepMealDetailsOpen) {
      const detailsEl = document.getElementById('mealDetailsModal');
      if (detailsEl) {
        const modal = new bootstrap.Modal(detailsEl);
        modal.show();
      }
    }
    */
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

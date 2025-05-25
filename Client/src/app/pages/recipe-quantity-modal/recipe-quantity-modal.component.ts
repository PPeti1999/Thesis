import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MealRecipeCreateDto, MealRecipeResponseDto, MealRecipesClient, RecipeResponseDto } from '../../shared/models/Nswag generated/NswagGenerated';
import bootstrap from 'bootstrap';

@Component({
  selector: 'app-recipe-quantity-modal',
  standalone: false,
  templateUrl: './recipe-quantity-modal.component.html',
  styleUrl: './recipe-quantity-modal.component.css'
})
export class RecipeQuantityModalComponent implements OnInit {
  @Input() recipe?: RecipeResponseDto;
  @Input() mealEntryId!: string;
  @Input() editingMealRecipeId?: string;
  @Output() added = new EventEmitter<MealRecipeResponseDto>();

  quantity: number = 1;
  protein = 0;
  carb = 0;
  fat = 0;
  calorie = 0;

  constructor(private mealRecipesClient: MealRecipesClient, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    if (!this.recipe) return;

    this.quantity = 1;
    this.recalculateMacros();

    const modalEl = document.getElementById('recipeQuantityModal');
    if (modalEl) {
      const modal = new bootstrap.Modal(modalEl);
      modal.show();
    }
  }

  recalculateMacros(): void {
    if (!this.recipe) return;

    const f = this.quantity;
    this.protein = +((this.recipe.sumProtein ?? 0) * f).toFixed(1);
    this.carb    = +((this.recipe.sumCarb ?? 0) * f).toFixed(1);
    this.fat     = +((this.recipe.sumFat ?? 0) * f).toFixed(1);
    this.calorie = +((this.recipe.sumCalorie ?? 0) * f).toFixed(0);
    this.cdr.detectChanges();
  }

  onQuantityChange(val: number): void {
    if (val > 0) {
      this.quantity = val;
      this.recalculateMacros();
    }
  }

  save(): void {
    if (!this.recipe || !this.mealEntryId) return;

    const dto = new MealRecipeCreateDto();
    dto.recipeID = this.recipe.recipeID!;
    dto.mealEntryID = this.mealEntryId;
    dto.quantity = this.quantity;

    if (this.editingMealRecipeId) {
      this.mealRecipesClient.update(this.editingMealRecipeId, dto).subscribe({
        next: res => {
          this.added.emit(res);
          this.reset();
          this.closeModal();
        },
        error: err => console.error(err)
      });
    } else {
      this.mealRecipesClient.create(dto).subscribe({
        next: res => {
          this.added.emit(res);
          this.reset();
          this.closeModal();
        },
        error: err => console.error(err)
      });
    }
  }

  cancel(): void {
    this.reset();
    this.closeModal();
  }

  reset(): void {
    this.editingMealRecipeId = undefined;
    this.quantity = 1;
  }

  private closeModal(): void {
    const modalEl = document.getElementById('recipeQuantityModal');
    if (modalEl) {
      bootstrap.Modal.getInstance(modalEl)?.hide();

      setTimeout(() => {
        document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
        document.body.classList.remove('modal-open');
        document.body.style.removeProperty('padding-right');
      }, 300);
    }
  }
}

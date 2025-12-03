import { ChangeDetectorRef, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { MealRecipeCreateDto, MealRecipeResponseDto, MealRecipesClient, RecipeIngredientDetailDto, RecipeResponseDto, RecipesClient } from '../../shared/models/Nswag generated/NswagGenerated';
import * as bootstrap from 'bootstrap';

@Component({
  selector: 'app-recipe-quantity-modal',
  standalone: false,
  templateUrl: './recipe-quantity-modal.component.html',
  styleUrl: './recipe-quantity-modal.component.css'
})
export class RecipeQuantityModalComponent implements OnInit,OnChanges {
  @Input() recipe?: RecipeResponseDto;
  @Input() mealEntryId!: string | undefined;
  @Input() editingMealRecipeId?: string;
  @Output() added = new EventEmitter<MealRecipeResponseDto>();
  @Input() initialQuantity: number = 1;
  quantity: number = 1;
  protein = 0;
  carb = 0;
  fat = 0;
  calorie = 0;

  ingredients: RecipeIngredientDetailDto[] = [];

  constructor(private recipesClient: RecipesClient,
              private mealRecipesClient: MealRecipesClient,
              private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    if (!this.recipe) return;
  
    if (this.editingMealRecipeId) {
      this.quantity = this.initialQuantity ?? 1;
    }
  
    this.recalculateMacros();
  
    const modalEl = document.getElementById('recipeQuantityModal');
    if (modalEl) {
      const modal = new bootstrap.Modal(modalEl);
      modal.show();
    }
  }
  
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['recipe'] && this.recipe) {
      if (this.editingMealRecipeId) {
        this.quantity = this.initialQuantity ?? 1;  
      }
      this.recalculateMacros();
    }
  }
  onQuantityChange(val: number): void {
    if (val > 0) {
      this.quantity = val;
      this.recalculateMacros(); 
    }
  }
  
  recalculateMacros(): void {
    if (!this.recipe) return;
  
    const f = this.quantity;

    this.protein = +(((this.recipe.sumProtein ?? 0) * f).toFixed(1));
    this.carb    = +(((this.recipe.sumCarb ?? 0) * f).toFixed(2));
    this.fat     = +(((this.recipe.sumFat ?? 0) * f).toFixed(2));
    this.calorie = +(((this.recipe.sumCalorie ?? 0) * f).toFixed(0));

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
  reset(): void {
    this.editingMealRecipeId = undefined;
    this.quantity = 1;
  }
  cancel() {
    const modalEl = document.getElementById('recipeQuantityModal');
    if (modalEl) {
      bootstrap.Modal.getInstance(modalEl)?.hide();
    }
  }

  private openModal(): void {
    const modalEl = document.getElementById('recipeQuantityModal');
    if (modalEl) {
      const modal = new bootstrap.Modal(modalEl);
      modal.show();
    }
  }

  private closeModal(): void {
    const modalEl = document.getElementById('recipeQuantityModal');
    if (modalEl) {
      const instance = bootstrap.Modal.getInstance(modalEl);
      bootstrap.Modal.getInstance(modalEl)?.hide();

      const backdrops = document.querySelectorAll('.modal-backdrop');
      backdrops.forEach(el => el.remove());
      document.body.classList.remove('modal-open');
      document.body.style.overflow = '';
    }
  }
  
}
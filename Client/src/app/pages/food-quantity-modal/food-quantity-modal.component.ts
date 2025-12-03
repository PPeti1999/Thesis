import { ChangeDetectorRef, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FoodResponseDto, MealFoodCreateDto, MealFoodResponseDto, MealFoodsClient } from '../../shared/models/Nswag generated/NswagGenerated';
import * as bootstrap from 'bootstrap';

@Component({
  selector: 'app-food-quantity-modal',
  standalone: false,
  templateUrl: './food-quantity-modal.component.html',
  styleUrl: './food-quantity-modal.component.css'
})
export class FoodQuantityModalComponent implements OnInit, OnChanges  {
  @Input() food?: FoodResponseDto;
  @Input() mealEntryId!: string | undefined;
  @Input() editingMealFoodId?: string;
  @Output() added = new EventEmitter<MealFoodResponseDto>();
  quantity: number = 100;
  protein = 0;
  carb = 0;
  fat = 0;
  calorie = 0;
  modalInstance!: bootstrap.Modal;

  constructor(private mealFoodsClient: MealFoodsClient,
    private cdr: ChangeDetectorRef) {}



  ngOnInit() {
  
    if (!this.food) return;
  
    this.quantity = this.food.gram ?? 100;
    this.recalculateMacros();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['food'] && this.food) {
      this.quantity = this.food.gram ?? 100;
      this.recalculateMacros();
    }
  }
  
  recalculateMacros() {
    if (!this.food) return;
  
    const food = this.food;
    const base = food.gram || 100;
    const f = this.quantity / base;
  
    this.protein = +((food.protein ?? 0) * f).toFixed(1);
    this.carb    = +((food.carb ?? 0) * f).toFixed(1);
    this.fat     = +((food.fat ?? 0) * f).toFixed(1);
    this.calorie = +((food.calorie ?? 0) * f).toFixed(0);
this.cdr.detectChanges();

  }
  

  onQuantityChange(val: number) {
    if (val > 0) {
      this.quantity = val;
      this.recalculateMacros();
    }
  }

  save() {
  
    if (!this.food || !this.mealEntryId) return;
  
    const dto = new MealFoodCreateDto();
    dto.foodID = this.food.foodID!;
    dto.mealEntryID = this.mealEntryId;
    dto.quantity = this.quantity;
  
    if (this.editingMealFoodId) {
      this.mealFoodsClient.update(this.editingMealFoodId, dto).subscribe({
        next: res => {
          this.added.emit(res);
          this.reset(); 
          this.closeModal();
        },
        error: err => console.error(err)
      });
    } else {
      this.mealFoodsClient.create(dto).subscribe({
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
    this.editingMealFoodId = undefined;
    this.quantity = 100;
  }
  

  private closeModal(): void {
    const modalEl = document.getElementById('foodQuantityModal');
    if (modalEl) {
      const instance = bootstrap.Modal.getInstance(modalEl);
      bootstrap.Modal.getInstance(modalEl)?.hide();
      instance?.hide();
      const backdrops = document.querySelectorAll('.modal-backdrop');
      backdrops.forEach(el => el.remove());
      document.body.classList.remove('modal-open');
      document.body.style.overflow = '';
    }
  }

  cancel() {
    const modalEl = document.getElementById('foodQuantityModal');
    if (modalEl) {
      bootstrap.Modal.getInstance(modalEl)?.hide();
    }
  }
  
}
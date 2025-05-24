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
    console.log('MODAL INIT FOOD:', this.food); // 👈 itt már az Angularhoz érkezett input érték
  
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
  
    console.log('RECALCULATE FROM FOOD:', food);
  
    this.protein = +((food.protein ?? 0) * f).toFixed(1);
    this.carb    = +((food.carb ?? 0) * f).toFixed(1);
    this.fat     = +((food.fat ?? 0) * f).toFixed(1);
    this.calorie = +((food.calorie ?? 0) * f).toFixed(0);
    // 💡 erőltetett újrarender
this.cdr.detectChanges();
console.log('protein: ', this.protein,
  ',carb: ', this.carb,
  ',fat: ', this.fat,
  ',calorie: ', this.calorie

);
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
  
    this.mealFoodsClient.create(dto).subscribe({
      next: res => {
        this.added.emit(res);
  
        const modalEl = document.getElementById('foodQuantityModal');
        if (modalEl) {
          bootstrap.Modal.getInstance(modalEl)?.hide();
        }
      },
      error: err => console.error(err)
    });
  }
  

  cancel() {
    const modalEl = document.getElementById('foodQuantityModal');
    if (modalEl) {
      bootstrap.Modal.getInstance(modalEl)?.hide();
    }
  }
  
}
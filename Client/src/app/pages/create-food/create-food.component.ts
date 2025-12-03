import { Component } from '@angular/core';
import { FoodClient, FoodCreateDto, FoodResponseDto } from '../../shared/models/Nswag generated/NswagGenerated';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs';

@Component({
  selector: 'app-create-food',
  standalone: false,
  templateUrl: './create-food.component.html',
  styleUrl: './create-food.component.css'
})
export class CreateFoodComponent {
  food: FoodCreateDto = new FoodCreateDto();
  editing = false;
  foodId: string | null = null;
  showAlert= false;

  constructor(
    private foodClient: FoodClient,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.route.paramMap.pipe(take(1)).subscribe(params => {
      this.foodId = params.get('id');
      if (this.foodId) {
        this.editing = true;
        this.foodClient.getFood(this.foodId).subscribe({
          next: (data: FoodResponseDto) => {
            this.food = new FoodCreateDto();
            Object.assign(this.food, {
              title: data.title,
              protein: data.protein,
              fat: data.fat,
              carb: data.carb,
              calorie: data.calorie,
              gram: data.gram
            });
          },
          error: err => {
            console.error('Error', err);
            this.router.navigate(['/food']);
          }
        });
      } 
    });
  }

  beforeSendClean(dto: any): void {
    for (const key in dto) {
      if (dto[key] === undefined || dto[key] === null || dto[key] === '') {
        delete dto[key];
      }
    }
  }

  createOrUpdate(): void {

   
    const cleanDto = new FoodCreateDto();
    Object.assign(cleanDto, this.food);
    this.beforeSendClean(cleanDto);

  
    if (this.editing && this.foodId) {
      const updateDto = new FoodCreateDto();
      Object.assign(updateDto, cleanDto);
      this.foodClient.updateFood(this.foodId, updateDto).subscribe({
        next: () => {
          this.router.navigate(['/food']);
        },
        error: err => {
          console.error( err);
        }
      });
    } else {
      this.foodClient.addFood(cleanDto.toJSON()).subscribe({
        next: () => {
          this.router.navigate(['/food']);
        },
        error: err => {
          console.error( err);
        }
      });
    }
  }
}

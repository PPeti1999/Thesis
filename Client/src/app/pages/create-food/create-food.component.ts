import { Component } from '@angular/core';
import { FoodClient, FoodCreateDto, FoodResponseDto, FoodUpdateDto } from '../../shared/models/Nswag generated/NswagGenerated';
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
        console.log('Szerkesztés módban vagyunk, ID:', this.foodId);
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
            console.error('Nem sikerült lekérni az ételt:', err);
            this.router.navigate(['/food']);
          }
        });
      } else {
        console.log('Új étel létrehozása módban vagyunk.');
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
      const updateDto = new FoodUpdateDto();
      Object.assign(updateDto, cleanDto);
      console.log('Frissítés DTO:', updateDto);
      this.foodClient.updateFood(this.foodId, updateDto).subscribe({
        next: () => {
          console.log('Sikeres frissítés');
          this.router.navigate(['/food']);
        },
        error: err => {
          console.error('Frissítés sikertelen:', err);
        }
      });
    } else {
      console.log('Létrehozás DTO:', cleanDto);
      console.log('Beküldendő JSON:', JSON.stringify(cleanDto));

      this.foodClient.addFood(cleanDto.toJSON()).subscribe({
        next: () => {
          console.log('Sikeres létrehozás');
          this.router.navigate(['/food']);
        },
        error: err => {
          console.error('Létrehozás sikertelen:', err);
        }
      });
    }
  }
}

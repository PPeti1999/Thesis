import { Component, OnInit } from '@angular/core';
import { FoodClient, FoodCreateDto, FoodResponseDto } from '../../shared/models/Nswag generated/NswagGenerated';

@Component({
  selector: 'app-food',
  standalone: false,
  templateUrl: './food.component.html',
  styleUrl: './food.component.css'
})
export class FoodComponent implements OnInit {
  foods: FoodResponseDto[] = [];
  newFood: FoodCreateDto = new FoodCreateDto();

  constructor(private foodClient: FoodClient) {}

  ngOnInit(): void {
    this.loadFoods();
  }

  loadFoods(): void {
    this.foodClient.getAllFoods().subscribe({
      next: foods => this.foods = foods,
      error: err => console.error('Hiba az ételek betöltésekor:', err)
    });
  }

  createFood(): void {
    this.foodClient.addFood(this.newFood).subscribe({
      next: () => {
        this.newFood = new FoodCreateDto(); // ürítés
        this.loadFoods(); // újratöltés
      },
      error: err => console.error('Hiba az étel létrehozásakor:', err)
    });
  }

  deleteFood(id: string): void {
    this.foodClient.deleteFood(id).subscribe({
      next: () => this.loadFoods(),
      error: err => console.error('Hiba a törlésnél:', err)
    });
  }
}

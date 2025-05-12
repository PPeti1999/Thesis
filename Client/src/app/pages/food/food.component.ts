import { Component, OnInit } from '@angular/core';
import { FoodClient, FoodCreateDto, FoodResponseDto, FoodUpdateDto } from '../../shared/models/Nswag generated/NswagGenerated';
import { AccountService } from '../../account/account.service';
import { User } from '../../shared/models/account/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-food',
  standalone: false,
  templateUrl: './food.component.html',
  styleUrl: './food.component.css'
})
export class FoodComponent implements OnInit {
  foods: FoodResponseDto[] = [];
  constructor(private foodClient: FoodClient, public _accountService: AccountService,
    private router: Router) {}

  ngOnInit(): void {
    this.loadFoods();
  }

  loadFoods(): void {
    this.foodClient.getAllFoods().subscribe({
      next: foods => this.foods = foods,
      error: err => console.error('Hiba az ételek betöltésekor:', err)
    });
  }
  onEdit(food: FoodResponseDto): void {
    if (food.foodID) {
      this.router.navigate(['/food/edit', food.foodID]);
    }
}
onDelete(food: FoodResponseDto): void {
  if (!food.foodID) return;

  const confirmed = confirm(`You wanna delete this food: "${food.title}"?`);
  if (!confirmed) return;

  this.foodClient.deleteFood(food.foodID).subscribe({
    next: () => {
      console.log('Törlés sikeres');
      this.foods = this.foods.filter(f => f.foodID !== food.foodID);
    },
    error: err => {
      if (err.error instanceof Blob) {
        err.error.text().then((text: string) => {
          try {
            const parsed = JSON.parse(text);
            alert(parsed.message || 'Nem törölhető. Lehet, hogy használatban van.');
          } catch {
            alert('Törlés sikertelen. (nyers válasz)');
          }
        });
      } else {
        alert('You use this food.');
      }
    }
  });
}
}
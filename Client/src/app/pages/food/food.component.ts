import { Component, OnInit } from '@angular/core';
import { FoodClient, FoodCreateDto, FoodResponseDto} from '../../shared/models/Nswag generated/NswagGenerated';
import { AccountService } from '../../account/account.service';
import { User } from '../../shared/models/account/user';
import { Router } from '@angular/router';
import { SharedService } from '../../shared/shared.service';

@Component({
  selector: 'app-food',
  standalone: false,
  templateUrl: './food.component.html',
  styleUrl: './food.component.css'
})
export class FoodComponent implements OnInit {
  foods: FoodResponseDto[] = [];
  constructor(private foodClient: FoodClient,private sharedService: SharedService, public _accountService: AccountService,
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

  this.sharedService.showConfirmation(
    'Confirm',
    `Are you sure you want to delete: "${food.title}"?`
  ).subscribe((confirmed: boolean) => {
    if (confirmed) {
      this.foodClient.deleteFood(food.foodID!).subscribe({
        next: () => {
          this.foods = this.foods.filter(f => f.foodID !== food.foodID);
          this.sharedService.showNotification(true, 'Success', 'Success delete!');
        },
        error: (err) => {
          const errorTitle = 'Error';

          if (err.response) {
            let errorMessage = 'Error';
            try {
              const parsed = JSON.parse(err.response);
              errorMessage = parsed.message || parsed.title || parsed.detail || errorMessage;
            } catch {
              errorMessage = err.message || errorMessage;
            }
            this.sharedService.showNotification(false, errorTitle, errorMessage);
          } else {
            this.sharedService.showNotification(false, errorTitle, 'Error');
          }
        }
      });
    }
  });
}
}
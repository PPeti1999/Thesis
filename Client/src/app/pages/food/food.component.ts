import { Component, OnInit } from '@angular/core';
import { FoodClient, FoodCreateDto, FoodResponseDto, FoodUpdateDto } from '../../shared/models/Nswag generated/NswagGenerated';
import { AccountService } from '../../account/account.service';
import { User } from '../../shared/models/account/user';

@Component({
  selector: 'app-food',
  standalone: false,
  templateUrl: './food.component.html',
  styleUrl: './food.component.css'
})
export class FoodComponent implements OnInit {
  foods: FoodResponseDto[] = [];
  constructor(private foodClient: FoodClient, public _accountService: AccountService) {}

  ngOnInit(): void {
    this.loadFoods();
  }

  loadFoods(): void {
    this.foodClient.getAllFoods().subscribe({
      next: foods => this.foods = foods,
      error: err => console.error('Hiba az ételek betöltésekor:', err)
    });
  }
  onEdit(food:FoodUpdateDto):void{

  }

 
}

import { Component, EventEmitter, Output } from '@angular/core';
import { FoodClient, FoodResponseDto, RecipeResponseDto, RecipesClient } from '../../shared/models/Nswag generated/NswagGenerated';
import { debounceTime, Subject } from 'rxjs';
import { OnChanges, SimpleChanges } from '@angular/core';
@Component({
  selector: 'app-meal-item-search',
  standalone: false,
  templateUrl: './meal-item-search.component.html',
  styleUrl: './meal-item-search.component.css'
})
export class MealItemSearchComponent {
  query: string = '';
  foodResults: FoodResponseDto[] = [];
  recipeResults: RecipeResponseDto[] = [];
  searchQuery: string = '';
  searchResults: (FoodResponseDto | RecipeResponseDto)[] = [];

clear(): void {
  this.query = '';
  this.foodResults = [];
  this.recipeResults = [];
}
  private searchSubject = new Subject<string>();

  @Output() itemSelected = new EventEmitter<{ type: 'food' | 'recipe', item: FoodResponseDto | RecipeResponseDto }>();


  constructor(private foodClient: FoodClient, private recipeClient: RecipesClient) {
    this.searchSubject.pipe(debounceTime(300)).subscribe(q => this.search(q));
  }
  resetSearch(): void {
    this.searchQuery = '';
    this.searchResults = [];
  }
  onInputChange(value: string) {
    this.query = value;
    this.searchSubject.next(value);
  }
selectFood(food: FoodResponseDto): void {

  this.itemSelected.emit({ type: 'food', item: food });
}

selectRecipe(recipeId: string): void {
  this.recipeClient.getById(recipeId).subscribe(fullRecipe => {

    this.itemSelected.emit({ type: 'recipe', item: fullRecipe });
  });
}
  search(query: string) {
    if (!query.trim()) {
      this.foodResults = [];
      this.recipeResults = [];
      return;
    }

    this.foodClient.search(query).subscribe(res => {
      this.foodResults = res;
    });
  
    this.recipeClient.search(query).subscribe(res => {
      this.recipeResults = res;
    });
  }

}

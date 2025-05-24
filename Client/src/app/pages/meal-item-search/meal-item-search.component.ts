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

  search(query: string) {
    if (!query.trim()) {
      this.foodResults = [];
      this.recipeResults = [];
      return;
    }

    this.foodClient.search(query).subscribe(res => {
      this.foodResults = res;
      console.log('FOOD RESULTS:', res); // 👈 itt listázzuk az étel találatokat
    });
  
    this.recipeClient.search(query).subscribe(res => {
      this.recipeResults = res;
      console.log('RECIPE RESULTS:', res); // 👈 itt a recept találatok
    });
  }

 /* selectItem(item: FoodResponseDto | RecipeResponseDto) {
    this.itemSelected.emit(item);
  }*/
}

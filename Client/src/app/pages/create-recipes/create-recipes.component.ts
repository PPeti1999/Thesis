import { Component, OnInit } from '@angular/core';
import { FoodClient, FoodResponseDto, RecipeCreateDto, RecipeFoodItemDto, RecipesClient, RecipeUpdateDto } from '../../shared/models/Nswag generated/NswagGenerated';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs';

@Component({
  selector: 'app-create-recipes',
  standalone: false,
  templateUrl: './create-recipes.component.html',
  styleUrl: './create-recipes.component.css'
})
export class CreateRecipesComponent  {
  recipe = new RecipeCreateDto();
  recipeId: string | null = null;
  editing = false;

  foodOptions: FoodResponseDto[] = [];
  foodIDInput?: string;
  quantityInput = 0;
  editingIngredientIndex: number | null = null;
  ingredients: RecipeFoodItemDto[] = [];

  sumCalorie = 0;
  sumProtein = 0;
  sumCarb = 0;
  sumFat = 0;

  constructor(
    private recipesClient: RecipesClient,
    private foodClient: FoodClient,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadFoods();

    this.route.paramMap.pipe(take(1)).subscribe(params => {
      this.recipeId = params.get('id');
      if (this.recipeId) {
        this.editing = true;
        this.recipesClient.getById(this.recipeId).subscribe({
          next: data => {
            this.recipe.title = data.title;
            this.recipe.description = data.description;
            this.ingredients = (data.ingredients || []).map(i => {
              const item = new RecipeFoodItemDto();
              item.foodID = i.foodID;
              item.quantity = i.quantity;
              return item;
            });
            this.updateNutrition();
          },
          error: err => {
            console.error('Nem sikerült lekérni a receptet:', err);
            this.router.navigate(['/recipes']);
          }
        });
      }
    });
  }

  loadFoods(): void {
    this.foodClient.getAllFoods().subscribe({
      next: foods => this.foodOptions = foods,
      error: err => console.error('Hiba az ételek lekérdezésénél:', err)
    });
  }

  addOrUpdateIngredient(): void {
    if (!this.foodIDInput || this.quantityInput <= 0) return;

    const item = new RecipeFoodItemDto();
    item.foodID = this.foodIDInput;
    item.quantity = this.quantityInput;

    if (this.editingIngredientIndex !== null) {
      this.ingredients[this.editingIngredientIndex] = item;
      this.editingIngredientIndex = null;
    } else {
      this.ingredients.push(item);
    }

    this.foodIDInput = undefined;
    this.quantityInput = 0;
    this.updateNutrition();
  }

  loadIngredientForEdit(index: number): void {
    const ing = this.ingredients[index];
    this.foodIDInput = ing.foodID;
    this.quantityInput = ing.quantity ?? 0;
    this.editingIngredientIndex = index;
  }

  removeIngredient(index: number): void {
    this.ingredients.splice(index, 1);
    this.updateNutrition();
  }

  createOrUpdate(): void {
    const dto = this.editing ? new RecipeUpdateDto() : new RecipeCreateDto();
    dto.title = this.recipe.title;
    dto.description = this.recipe.description;
    dto.ingredients = this.ingredients;

    const request = this.editing && this.recipeId
      ? this.recipesClient.update(this.recipeId, dto as RecipeUpdateDto)
      : this.recipesClient.create(dto as RecipeCreateDto);

    request.subscribe({
      next: () => this.router.navigate(['/recipes']),
      error: err => console.error('Mentés sikertelen:', err)
    });
  }

  getFoodTitleById(id?: string): string {
    return this.foodOptions.find(f => f.foodID === id)?.title || 'Unknown';
  }

  updateNutrition(): void {
    this.sumCalorie = 0;
    this.sumProtein = 0;
    this.sumFat = 0;
    this.sumCarb = 0;

    this.ingredients.forEach(ing => {
      const food = this.foodOptions.find(f => f.foodID === ing.foodID);
      if (!food || !ing.quantity) return;

      const factor = ing.quantity / 100;
      this.sumCalorie += (food.calorie || 0) * factor;
      this.sumProtein += (food.protein || 0) * factor;
      this.sumFat += (food.fat || 0) * factor;
      this.sumCarb += (food.carb || 0) * factor;
    });
  }
}
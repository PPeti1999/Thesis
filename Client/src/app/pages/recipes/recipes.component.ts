import { Component, OnInit } from '@angular/core';
import { RecipeResponseDto, RecipesClient } from '../../shared/models/Nswag generated/NswagGenerated';
import { AccountService } from '../../account/account.service';

@Component({
  selector: 'app-recipes',
  standalone: false,
  templateUrl: './recipes.component.html',
  styleUrl: './recipes.component.css'
})
export class RecipesComponent implements OnInit {
  recipes: RecipeResponseDto[] = [];

  constructor(
    private recipesClient: RecipesClient,
    public accountService: AccountService
  ) {}

  ngOnInit(): void {
    this.loadRecipes();
  }

  loadRecipes(): void {
    this.recipesClient.getAll().subscribe({
      next: data => this.recipes = data,
      error: err => console.error('Hiba a receptek lekérdezésénél:', err)
    });
  }

  onEdit(recipe: RecipeResponseDto): void {
    // TODO: navigáció a szerkesztő oldalra vagy modal megnyitása
    console.log('Edit:', recipe);
  }

  onDelete(recipe: RecipeResponseDto): void {
   /* if (confirm(`Biztosan törlöd a(z) "${recipe.title}" receptet?`)) {
      this.recipesClient.delete(recipe.recipeID).subscribe({
        next: () => this.loadRecipes(),
        error: err => console.error('Törlés sikertelen:', err)
      });
    }*/
  }
}

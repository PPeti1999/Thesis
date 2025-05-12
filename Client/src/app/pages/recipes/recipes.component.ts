import { Component, OnInit } from '@angular/core';
import { RecipeResponseDto, RecipesClient } from '../../shared/models/Nswag generated/NswagGenerated';
import { AccountService } from '../../account/account.service';
import { Router } from '@angular/router';

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
    public accountService: AccountService, private router: Router
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
    if (recipe.recipeID) {
      this.router.navigate(['/recipes/edit', recipe.recipeID]);
  }
  }

  onDelete(recipe: RecipeResponseDto): void {
    if (!recipe.recipeID) return;
  
    const confirmed = confirm(`You wanna delete the "${recipe.title}" receptet?`);
    if (!confirmed) return;
  
    this.recipesClient.delete(recipe.recipeID).subscribe({
      next: () => {
        console.log('Recept törölve');
        this.recipes = this.recipes.filter(r => r.recipeID !== recipe.recipeID);
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
          alert('You use this recipe.');
        }
      }
    });
  }
}

import { Component, OnInit } from '@angular/core';
import { RecipeResponseDto, RecipesClient } from '../../shared/models/Nswag generated/NswagGenerated';
import { AccountService } from '../../account/account.service';
import { Router } from '@angular/router';
import { SharedService } from '../../shared/shared.service';

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
    public accountService: AccountService, private router: Router,
    private sharedService : SharedService
  ) {}

  ngOnInit(): void {
    this.loadRecipes();
  }

  loadRecipes(): void {
    this.recipesClient.getAll().subscribe({
      next: data => this.recipes = data,
      error: err => console.error( err)
    });
  }

  onEdit(recipe: RecipeResponseDto): void {
    if (recipe.recipeID) {
      this.router.navigate(['/recipes/edit', recipe.recipeID]);
  }
  }
  onDelete(recipe: RecipeResponseDto): void {
    if (!recipe.recipeID) return;
  
    this.sharedService.showConfirmation(
      'Confirm',
      `Are you sure you want to delete: "${recipe.title}"?`
    ).subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.recipesClient.delete(recipe.recipeID!).subscribe({
          next: () => {
            this.recipes = this.recipes.filter(r => r.recipeID !== recipe.recipeID);
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

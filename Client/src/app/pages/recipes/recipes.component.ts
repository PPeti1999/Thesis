import { Component, OnInit } from '@angular/core';
import { RecipeResponseDto, RecipesClient } from '../../shared/models/Nswag generated/NswagGenerated';

@Component({
  selector: 'app-recipes',
  standalone: false,
  templateUrl: './recipes.component.html',
  styleUrl: './recipes.component.css'
})
export class RecipesComponent implements OnInit {
  recipes: RecipeResponseDto[] = [];

  constructor(private recipesClient: RecipesClient) {}

  ngOnInit(): void {
    this.loadRecipes();
  }

  loadRecipes(): void {
    this.recipesClient.getAll().subscribe({
      next: data => this.recipes = data,
      error: err => console.error('Hiba a receptek lekérdezésénél:', err)
    });
  }
}

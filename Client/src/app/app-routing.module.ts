import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundComponent } from './shared/components/errors/not-found/not-found.component';
import { AuthorizationGuard } from './shared/guards/authorization.guard';
import { FoodComponent } from './pages/food/food.component';
import { ActivityCatalogComponent } from './pages/activity-catalog/activity-catalog.component';
import { RecipesComponent } from './pages/recipes/recipes.component';
import { CreateFoodComponent } from './pages/create-food/create-food.component';
import { CreateActivityCatalogComponent } from './pages/create-activity-catalog/create-activity-catalog.component';
import { CreateRecipesComponent } from './pages/create-recipes/create-recipes.component';
import { CreateProfileComponent } from './pages/create-profile/create-profile.component';
import { DailyNoteComponent } from './pages/daily-note/daily-note.component';
import { WeightHistoryComponent } from './pages/weight-history/weight-history.component';

const routes: Routes = [
  { path: '', redirectTo: 'food', pathMatch: 'full' }, 
{ path: 'food', component: FoodComponent, 
}, 
{ path: 'activitycatalog', component: ActivityCatalogComponent, 
}, 
{ path: 'recipes', component: RecipesComponent, 
}, 
{ path: 'w', component: WeightHistoryComponent, 
}, 
{ path: '', 
runGuardsAndResolvers:'always',
canActivate:[AuthorizationGuard],
children:[// ide kell tenni azokat az elereseket amiket csak belepes utan szabad latni 
{ path: 'activitycatalog/create', component: CreateActivityCatalogComponent },
{ path: 'activitycatalog/edit/:id', component: CreateActivityCatalogComponent },
{ path: 'food/create', component: CreateFoodComponent },
{ path: 'food/edit/:id', component: CreateFoodComponent },
{ path: 'recipes/create', component: CreateRecipesComponent },
{ path: 'recipes/edit/:id', component: CreateRecipesComponent },
{ path: 'edit-profile', component: CreateProfileComponent },
{ path: 'dailynote', component: DailyNoteComponent },
],

},


{path: 'account',loadChildren:()=>import('./account/account.module').then(module=>module.AccountModule) },
{ path: 'not-found', component: NotFoundComponent,},
{ path: '**', component: NotFoundComponent,pathMatch:'full'                               // érvénytelen kérés esetén ezt tölti be 
},
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

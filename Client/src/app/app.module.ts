import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';
import { SharedModule } from './shared/shared.module';
import { NotificationComponent } from './shared/components/modals/notification/notification.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from '@angular/common/http';
import { JwtInterceptor } from './shared/interceptors/jwt.interceptor';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { API_BASE_URL } from './shared/models/Nswag generated/NswagGenerated';
import { environment } from '../environments/environment.development';
import { FoodComponent } from './pages/food/food.component';
import { ActivityCatalogComponent } from './pages/activity-catalog/activity-catalog.component';
import { RecipesComponent } from './pages/recipes/recipes.component';
import { CreateFoodComponent } from './pages/create-food/create-food.component';
import { CreateActivityCatalogComponent } from './pages/create-activity-catalog/create-activity-catalog.component';
import { CreateRecipesComponent } from './pages/create-recipes/create-recipes.component';
import { CreateProfileComponent } from './pages/create-profile/create-profile.component';
import { DailyNoteComponent } from './pages/daily-note/daily-note.component';
import { MealItemSearchComponent } from './pages/meal-item-search/meal-item-search.component';
import { FoodQuantityModalComponent } from './pages/food-quantity-modal/food-quantity-modal.component';
import { RecipeQuantityModalComponent } from './pages/recipe-quantity-modal/recipe-quantity-modal.component';
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';
import { WeightHistoryComponent } from './pages/weight-history/weight-history.component';
import { ConfirmDialogComponent } from './shared/components/modals/confirm-dialog/confirm-dialog.component';
@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    FooterComponent,
    FoodComponent,
    ActivityCatalogComponent,
    RecipesComponent,
    CreateFoodComponent,
    CreateActivityCatalogComponent,
    CreateRecipesComponent,
    CreateProfileComponent,
    DailyNoteComponent,
    MealItemSearchComponent,
    FoodQuantityModalComponent,
    RecipeQuantityModalComponent,
    WeightHistoryComponent,
  ],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    ModalModule.forRoot(),
    ReactiveFormsModule,
    SharedModule,
    FormsModule,
    NgbDatepickerModule, 
    
  ],
  providers: [ {provide:HTTP_INTERCEPTORS, useClass:JwtInterceptor,multi:true},
    { provide: API_BASE_URL, useValue: environment.appUrl } 
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
 
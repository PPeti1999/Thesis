import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';
import { HomeComponent } from './home/home.component';
import { PlayComponent } from './play/play.component';
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

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    FooterComponent,
    HomeComponent,
    PlayComponent,
    FoodComponent,
    ActivityCatalogComponent,
    RecipesComponent,
    
  ],
  imports: [
    FormsModule,
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    ModalModule.forRoot(),
    ReactiveFormsModule,
    SharedModule
  ],
  providers: [ {provide:HTTP_INTERCEPTORS, useClass:JwtInterceptor,multi:true},
    { provide: API_BASE_URL, useValue: environment.appUrl } 
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
 
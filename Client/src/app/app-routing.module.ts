import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NotFoundComponent } from './shared/components/errors/not-found/not-found.component';
import { PlayComponent } from './play/play.component';
import { AuthorizationGuard } from './shared/guards/authorization.guard';
import { FoodComponent } from './pages/food/food.component';

const routes: Routes = [
  { path: '', component: HomeComponent, //canActivate: [AuthGuard]
},
{ path: 'food', component: FoodComponent, //canActivate: [AuthGuard]
}, 
{ path: '', 
runGuardsAndResolvers:'always',
canActivate:[AuthorizationGuard],
children:[// ide kell tenni azokat az elereseket amiket csak belepes utan szabad latni 
/*{path: 'bodydiary',component: HomeBodydiaryComponent
},
{path: 'create',component: CreateExerciseComponent
},
{path: 'exercise/:id/edit',component: CreateExerciseComponent
},
{path:'createbodydiary',component:CreateBodydiaryComponent},
{path:'createbodydiaryweekly',component:CreateBodydiaryweeklyComponent},
{path:'createworkoutplan',component:CreateWorkoutplanComponent},
*/
{path: 'play',component: PlayComponent
},
],

},

//path: 'exercise/:id',component: ExerciseDetailsPageComponent,},

{path: 'account',loadChildren:()=>import('./account/account.module').then(module=>module.AccountModule) },
{ path: 'not-found', component: NotFoundComponent,},
{ path: '**', component: NotFoundComponent,pathMatch:'full'// érvénytelen kérés esetén ezt tölti be 
},
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

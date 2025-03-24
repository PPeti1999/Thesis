import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { RouterModule, Routes } from '@angular/router';


const routes:Routes=[
  { path: 'login', component: LoginComponent, //canActivate: [AuthGuard]
 },
 { path: 'register', component: RegisterComponent, //canActivate: [AuthGuard]
},
]
@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ],
  exports:[
    RouterModule
  ]
})
export class AccountRoutingModule { }

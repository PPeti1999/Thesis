import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { RouterModule, Routes } from '@angular/router';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { SendEmailComponent } from './send-email/send-email.component';


const routes:Routes=[
  { path: 'login', component: LoginComponent, //canActivate: [AuthGuard]
 },
 { path: 'register', component: RegisterComponent, //canActivate: [AuthGuard]
},
{ path: 'confirm-email', component: ConfirmEmailComponent, //canActivate: [AuthGuard]
},
{ path: 'send-email/:mode', component: SendEmailComponent, //canActivate: [AuthGuard]
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

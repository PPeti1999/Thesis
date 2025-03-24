import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';
import { AccountService } from '../../account/account.service';
import { SharedService } from '../shared.service';
import { map, Observable } from 'rxjs';
import { User } from '../models/account/user';


@Injectable({
  providedIn: 'root'
})
export class AuthorizationGuard {
  constructor(private accountService:AccountService,
    private sharedService: SharedService,
    private router: Router) {}

  canActivate(// guard h ha olyan helyre akar menni az illeto ami nem all jogaban akkor loginra dobb 
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> {
    return this.accountService.user$.pipe(
      map((user: User | null) => {
        if (user) {
          return true;
        } else {
          this.sharedService.showNotification(false, 'Restricted Area', 'Leave immediately!');
          this.router.navigate(['account/login'], {queryParams: {returnUrl: state.url}});
          return false;
        }
      })
    );
  }
  
}
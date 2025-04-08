import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of, ReplaySubject } from 'rxjs';
import { User } from '../shared/models/account/user';
import { Router } from '@angular/router';
import { Login } from '../shared/models/account/login';
import { environment } from '../../environments/environment.development';
import { Register } from '../shared/models/account/register';
import { ConfirmEmail } from '../shared/models/account/confirmEmail';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  
  private userSource = new ReplaySubject<User | null>(1);// OBSERVER AMIRE FELIRATKOZUNK ami egy lehet //jo
  user$ = this.userSource.asObservable();// itt feliratkozunk ra//jo
 
  constructor(private http: HttpClient, //jo
    private router: Router,
    ) { }
    login(model: Login) {// konkret user belepes
     
     // return this.http.post<User>(`${environment.appUrl}account/login`, model)
     return this.http.post<User>(`${environment.appUrl}/api/account/login`, model).pipe(
        map((user: User) => {
          if (user) {
            this.setUser(user);
          
          }
      
        })
      );
    }
    register(model: Register) {
      return this.http.post(`${environment.appUrl}/api/account/register`, model);//jo
    }
    confirmEmail(model: ConfirmEmail) {
      return this.http.put(`${environment.appUrl}/api/account/confirm-email`, model);
    }









    getJWT() {// belepet tokent ementi szoval ha frissitek akk belepve marad

      const key = localStorage.getItem(environment.userKey);
      if (key) {
        const user: User = JSON.parse(key);
        return user.jwt;
      } else {
        return null;
      }
    }
    refreshUser(jwt: string | null) {
      if (jwt === null) {
        this.userSource.next(null);
        return of(undefined);
      }
  
      let headers = new HttpHeaders();
      headers = headers.set('Authorization', 'Bearer ' + jwt);
  //return this.http.get<User>(`${environment.appUrl}/api/account/refresh-user-token`, {headers, withCredentials: true}).pipe(
      return this.http.get<User>(`${environment.appUrl}/api/account/refresh-user-token`, {headers}).pipe(
        map((user: User) => {
          if (user) {
            this.setUser(user);
          }
        })
      )
    }
    private setUser(user: User) {
     // this.stopRefreshTokenTimer();
     // this.startRefreshTokenTimer(user.jwt);
      localStorage.setItem(environment.userKey, JSON.stringify(user));// enviroment userkey e local storageben eltaroljuk //jo
      this.userSource.next(user);// itt is
      
      //this.sharedService.displayingExpiringSessionModal = false;
      //this.checkUserIdleTimout();
    }
    logout() {
      localStorage.removeItem(environment.userKey);
      this.userSource.next(null);
      this.router.navigateByUrl('/');
     // this.stopRefreshTokenTimer();
    }
}

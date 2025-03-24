import { Component } from '@angular/core';
import { AccountService } from '../account/account.service';
import { User } from '../shared/models/account/user';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  currentUser: User | null = null;
  isLoading: Boolean = true; // Flag to track loading state
 /* constructor(  private _homeBodyDiaryService: HomeBodydiaryService, public _accountService: AccountService
    )  {}
*/
constructor(   public _accountService: AccountService
)  {}
  logout() {
    this._accountService.logout();

  }

  ngOnInit(): void {
  }
}
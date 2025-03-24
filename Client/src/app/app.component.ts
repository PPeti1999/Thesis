import { Component } from '@angular/core';
import { AccountService } from './account/account.service';
import { Router } from '@angular/router';
import { SharedService } from './shared/shared.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent {
  router;
  title = 'healthy.client';
  constructor(private accountService: AccountService,
    private sharedService: SharedService,router : Router) {this.router = router;}
  
  ngOnInit(): void {
    this.refreshUser();
  }
  
  
  private refreshUser() {
    const jwt = this.accountService.getJWT();
    if (jwt) {
      this.accountService.refreshUser(jwt).subscribe({
        next: _ => {},
        error: error => {
          this.accountService.logout();
  
          if (error.status === 401) {
            this.sharedService.showNotification(false, 'Account blocked', error.error);
          }
        }
      })
    } else {
      this.accountService.refreshUser(null).subscribe();
    }
  }
}

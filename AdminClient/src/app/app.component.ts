import { Component, OnInit } from '@angular/core';
import { TokenService } from './services/token.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  isLoggedIn = false;
  username?: string;
  
  constructor(private tokenService: TokenService) { }

  ngOnInit(): void {
    this.isLoggedIn = !!this.tokenService.getToken();

    if (this.isLoggedIn) {
      const user = this.tokenService.getUser();

      this.username = user.name;
    }
  }

  logout(): void {
    this.tokenService.signOut();
    window.location.reload();
  }
}

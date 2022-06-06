import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

const TOKEN_KEY = 'auth-token';
const REFRESHTOKEN_KEY = 'auth-refreshtoken';
const USER_KEY = 'auth-user';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  jwtHelper = new JwtHelperService();
  constructor() { }

  signOut(): void {
    window.localStorage.clear();
    
  }
  
  public saveData(data)
  {
    this.saveToken(data.accessToken);
    this.saveRefreshToken(data.refreshToken);

    var decoded = this.jwtHelper.decodeToken(data.accessToken);

    this.saveUser(decoded);
  }

  public saveToken(token: string): void {
    window.localStorage.removeItem(TOKEN_KEY);
    window.localStorage.setItem(TOKEN_KEY, token);
  }

  public getToken(): string | null {
    return window.localStorage.getItem(TOKEN_KEY);
  }

  public saveRefreshToken(token: string): void {
    window.localStorage.removeItem(REFRESHTOKEN_KEY);
    window.localStorage.setItem(REFRESHTOKEN_KEY, token);
  }

  public getRefreshToken(): string | null {
    return window.localStorage.getItem(REFRESHTOKEN_KEY);
  }

  public saveUser(data: any): void {
    window.localStorage.removeItem(USER_KEY);
    var user = {
      name : data["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
      roles : data["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
      id : data["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]
    }
    window.localStorage.setItem(USER_KEY, JSON.stringify(user));
  }

  public getUser(): any {
    const user = window.localStorage.getItem(USER_KEY);
    if (user) {
      return JSON.parse(user);
    }

    return {};
  }
}

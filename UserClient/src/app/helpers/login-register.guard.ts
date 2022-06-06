import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { JwtHelperService } from "@auth0/angular-jwt";
import { BehaviorSubject } from "rxjs";
import { switchMap } from "rxjs/operators";
import { AuthService } from "../services/auth.service";
import { TokenService } from "../services/token.service";

@Injectable()
export class LoginRegisterGuard implements CanActivate {
  constructor(private jwtHelper: JwtHelperService, private router: Router,
              private tokenService: TokenService, private authService: AuthService) {
  }
  canActivate() {
    const token = this.tokenService.getToken();
    const refreshToken = this.tokenService.getRefreshToken();

    if (!token){
      return true;
    }

    if (token && refreshToken && this.jwtHelper.isTokenExpired(token))
    {
      try{
        console.log("login-register.guard refreshing");

        this.authService.refreshToken(token, refreshToken).subscribe(newTokens => this.tokenService.saveData(newTokens));
        this.router.navigate(["challenges"]);
        return false;
      }
      catch(err){        
        this.tokenService.signOut();
        console.log("login-register.guard refreshing failed");

        return true;
      }
    }

    this.router.navigate(["challenges"]);
    return false;
  }
}
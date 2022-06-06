import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { JwtHelperService } from "@auth0/angular-jwt";
import { BehaviorSubject } from "rxjs";
import { catchError, switchMap } from "rxjs/operators";
import { AuthService } from "../services/auth.service";
import { TokenService } from "../services/token.service";

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(private jwtHelper: JwtHelperService, private router: Router,
              private tokenService: TokenService, private authService: AuthService) {
  }
  canActivate() {
    const token = this.tokenService.getToken();

    if (token){
      return true;
    }

    this.router.navigate(["login"]);
    return false;
  }
}
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Config } from '../config';

const TOKEN_API = Config.apiUrl + '/api/token/';
const AUTH_API = Config.apiUrl + '/api/auth/';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient) { }

  login(username: string, password: string): Observable<any> {
    return this.http.post(AUTH_API + 'login', {
      UserName : username,
      Password : password
    }, httpOptions);
  }

  register(username: string, email: string, password: string): Observable<any> {
    return this.http.post(AUTH_API + 'register', {
      username,
      email,
      password
    }, httpOptions);
  }

  refreshToken(token: string, refreshToken : string){
    return this.http.post(TOKEN_API + 'refresh',
    {
      accessToken : token, 
      refreshToken : refreshToken
    }, httpOptions);
  }
}

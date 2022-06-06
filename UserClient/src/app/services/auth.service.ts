import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, Observable } from 'rxjs';
import { Config } from '../config';
import { Settings } from 'http2';
import { User, NotificationSettings } from '../Shared/interfaces';

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
      UserName: username,
      Password: password
    }, httpOptions);
  }

  getUser(): Observable<User> {
    const url = AUTH_API + 'userInfo';
    return this.http.get<User>(url);
  }

  getNotificationSettings(): Observable<NotificationSettings> {
    const url = AUTH_API + 'notificationSettings';
    return this.http.get<NotificationSettings>(url);
  }

  updateNotificationSettings(NotificationSettings: NotificationSettings): Observable<NotificationSettings> {
    const url = AUTH_API + 'notificationSettings';
    return this.http.put<NotificationSettings>(url, NotificationSettings, httpOptions).pipe(
      catchError(err => {
        console.log(err)
        throw err;
      })
    )
  }

  getContestantsOfChallenge(challengeId: any): Observable<User[]> {
    const url = AUTH_API + 'contestants/' + challengeId;
    return this.http.get<User[]>(url)
      .pipe(
        catchError(err => {
          console.log(err)
          throw err;
        })
      );
  }

  updateUser(user: User): Observable<any> {
    const url = AUTH_API;
    return this.http.put(url, user, httpOptions).pipe(
      catchError(err => {
        console.log(err)
        throw err;
      })
    )
  }

  register(username: string, email: string, password: string): Observable<any> {
    return this.http.post(AUTH_API + 'register', {
      username,
      email,
      password
    }, httpOptions);
  }

  refreshToken(token: string, refreshToken: string) {
    return this.http.post(TOKEN_API + 'refresh',
      {
        accessToken: token,
        refreshToken: refreshToken
      }, httpOptions).pipe(
        catchError((err) => {
          throw err;
        })
      );
  }
}

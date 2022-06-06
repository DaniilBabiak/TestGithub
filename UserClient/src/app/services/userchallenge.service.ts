import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { Config } from '../config';
import { PaginatedResult, SortConfig, UserChallenge } from '../Shared/interfaces';

@Injectable({
  providedIn: 'root'
})
export class UserChallengeService {
  private userChallengesUrl = 'api/UserChallenge';
  private apiUrl = Config.apiUrl;

  httpOptionsJson = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  constructor(
    private http: HttpClient) { }

    getUserChallenges(pageNumber?, pageSize?, sortConfig?: SortConfig): Observable<PaginatedResult<UserChallenge[]>> {
      const paginatedResult: PaginatedResult<UserChallenge[]> = new PaginatedResult<UserChallenge[]>();
  
      let params = new HttpParams();
  
      if (pageNumber != null && pageSize != null) {
        params = params.append('pageNumber', pageNumber);
        params = params.append('pageSize', pageSize);
      }

      if (sortConfig.isSortAscending) {
        params = params.append('orderBy', sortConfig.sortBy)
      }
      else{
        params = params.append('orderBy', sortConfig.sortBy + ' desc');
      }
  
      const url = this.apiUrl + '/' + this.userChallengesUrl;
      
      return this.http.get<UserChallenge[]>(
        url,
        { responseType: "json", observe: 'response', params })
        .pipe(
          map(res => {
          paginatedResult.resut = res.body;
          if (res.headers.get('X-Pagination') != null) {
            paginatedResult.pagination = JSON.parse(res.headers.get('X-Pagination'))
          }
          return paginatedResult;
        }),
          catchError(this.handleError<PaginatedResult<UserChallenge[]>>(`getUserChallenges`))
        );
    }

    addUserChallenge(userChallenge: UserChallenge): Observable<any> {
      const url = this.apiUrl + '/' + this.userChallengesUrl;
      return this.http.post(url, userChallenge, this.httpOptionsJson).pipe(
        catchError(this.handleError<UserChallenge>('addUserChallenge'))
      );
    }

  /** GET challenge by id. Will 404 if id not found */
  getUserChallenge(id: any): Observable<UserChallenge> {
    const url = this.apiUrl + '/' + this.userChallengesUrl + `/${id}`;
    return this.http.get<UserChallenge>(url).pipe(
      catchError(this.handleError<UserChallenge>(`getUserChallenge id=${id}`))
    );
  }

  deleteUserChallenge(id: any): Observable<UserChallenge> {
    const url = this.apiUrl + '/' + this.userChallengesUrl + `/${id}`;

    return this.http.delete<UserChallenge>(url, this.httpOptionsJson).pipe(
      catchError(this.handleError<UserChallenge>('deleteUserChallenge'))
    );
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}

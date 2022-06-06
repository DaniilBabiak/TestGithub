import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { Challenge, PaginatedResult, SortConfig } from '../Shared/interfaces';
import { Config } from '../config';
import { TokenService } from './token.service';


@Injectable({ providedIn: 'root' })
export class ChallengeService {

  private challengesUrl = 'api/challenge';
  private apiUrl = Config.apiUrl;

  httpOptionsJson = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  constructor(
    private http: HttpClient,
    private tokenService: TokenService) { }

  getChallenges(pageNumber?, pageSize?, sortConfig? : SortConfig): Observable<PaginatedResult<Challenge[]>> {
    const paginatedResult: PaginatedResult<Challenge[]> = new PaginatedResult<Challenge[]>();

    let params = new HttpParams();

    if (pageNumber != null && pageSize != null) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
      params = params.append('isAdmin', true);
      params = params.append('isLoggedIn', true);
      params = params.append('userId', this.tokenService.getUser().id);
    }

    if (sortConfig.isSortAscending) {
      params = params.append('orderBy', sortConfig.sortBy)
    }
    else{
      params = params.append('orderBy', sortConfig.sortBy + ' desc');
    }

    const url = this.apiUrl + '/' + this.challengesUrl;
    
    return this.http.get<Challenge[]>(
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
        catchError(this.handleError<PaginatedResult<Challenge[]>>(`getChallenges`))
      );
  }

  getChallenge(id: any): Observable<Challenge> {
    const url = this.apiUrl + '/' + this.challengesUrl + `/${id}`;
    return this.http.get<Challenge>(url).pipe(
      catchError(this.handleError<Challenge>(`getChallenge id=${id}`))
    );
  }

  addChallenge(challenge: Challenge): Observable<any> {
    const url = this.apiUrl + '/api/admin/challenge/';
    return this.http.post(url, challenge, this.httpOptionsJson).pipe(
      catchError(this.handleError<Challenge>('addChallenge'))
    );
  }

  deleteChallenge(id: any): Observable<Challenge> {
    const url = this.apiUrl + '/api/admin/challenge/';

    return this.http.delete<Challenge>(url, this.httpOptionsJson).pipe(
      catchError(this.handleError<Challenge>('deleteChallenge'))
    );
  }

  updateChallenge(challenge: Challenge): Observable<Challenge> {
    const url = this.apiUrl + '/api/admin/challenge/';
    return this.http.put(url, challenge, this.httpOptionsJson).pipe(
      catchError(this.handleError<any>('updateChallenge'))
    );
  }

  /**
   * Handle Http operation that failed.
   * Let the app continue.
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}
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

  getChallenges(pageNumber?, pageSize?, sortConfig?: SortConfig): Observable<PaginatedResult<Challenge[]>> {
    const paginatedResult: PaginatedResult<Challenge[]> = new PaginatedResult<Challenge[]>();

    let params = new HttpParams();
    var userId = this.tokenService.getUser().id;

    if (userId == undefined) {
      params.append('userId', 0);
      params.append('isLoggedIn', false);
    }
    else {
      params = params.append('userId', userId);
      params = params.append('isLoggedIn', true);
    }

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
  /** GET challenge by id. Will 404 if id not found */
  getChallenge(id: any): Observable<Challenge> {
    const url = this.apiUrl + '/' + this.challengesUrl + `/${id}`;
    return this.http.get<Challenge>(url).pipe(
      catchError(this.handleError<Challenge>(`getChallenge id=${id}`))
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
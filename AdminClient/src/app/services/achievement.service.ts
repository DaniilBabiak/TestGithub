import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Config } from '../config';
import { Achievement, PaginatedResult, SortConfig } from '../Shared/interfaces';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class AchievementService {

  private achievementsUrl = 'api/achievement';
  private apiUrl = Config.apiUrl;

  httpOptionsJson = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  constructor(
    private http: HttpClient,
    private tokenService: TokenService) { }

  getAchievements(pageNumber?, pageSize?, sortConfig? : SortConfig): Observable<PaginatedResult<Achievement[]>> {
    const paginatedResult: PaginatedResult<Achievement[]> = new PaginatedResult<Achievement[]>();

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

    const url = this.apiUrl + '/' + this.achievementsUrl;
    
    return this.http.get<Achievement[]>(
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
        catchError(this.handleError<PaginatedResult<Achievement[]>>(`getAchievements`))
      );
  }

  getAchievement(id: any): Observable<Achievement> {
    const url = this.apiUrl + '/' + this.achievementsUrl + `/${id}`;
    return this.http.get<Achievement>(url).pipe(
      catchError(this.handleError<Achievement>(`getAchievement id=${id}`))
    );
  }

  addAchievement(achievement: Achievement): Observable<any> {
    const url = this.apiUrl + '/api/admin/achievement/';
    return this.http.post(url, achievement, this.httpOptionsJson).pipe(
      catchError(this.handleError<Achievement>('addAchievement'))
    );
  }

  deleteAchievement(id: any): Observable<Achievement> {
    const url = this.apiUrl + '/api/admin/achievement/';

    return this.http.delete<Achievement>(url, this.httpOptionsJson).pipe(
      catchError(this.handleError<Achievement>('deleteAchievement'))
    );
  }

  updateAchievement(achievement: Achievement): Observable<Achievement> {
    const url = this.apiUrl + '/api/admin/achievement/';
    return this.http.put(url, achievement, this.httpOptionsJson).pipe(
      catchError(this.handleError<any>('updateAchievement'))
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

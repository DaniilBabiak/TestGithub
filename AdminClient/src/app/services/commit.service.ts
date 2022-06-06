import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { Commit, PaginatedResult, SortConfig } from '..//Shared/interfaces';
import { Config } from '../config';

@Injectable({
  providedIn: 'root'
})
export class CommitService {
  private commitsUrl = 'api/challengecommit';
  private apiUrl = Config.apiUrl;

  httpOptionsJson = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  constructor(private http: HttpClient)
  {    
  }

  getCommits(pageNumber?, pageSize?, sortConfig?:SortConfig): Observable<PaginatedResult<Commit[]>> {
    const paginatedResult: PaginatedResult<Commit[]> = new PaginatedResult<Commit[]>();

    let params = new HttpParams();

    if (pageNumber != null && pageSize != null) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }

    const url = this.apiUrl + '/api/admin/challengecommit/';

    if (sortConfig.isSortAscending) {
      params = params.append('orderBy', sortConfig.sortBy)
    }
    else{
      params = params.append('orderBy', sortConfig.sortBy + ' desc');
    }

    return this.http.get<Commit[]>(
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
        catchError(this.handleError<PaginatedResult<Commit[]>>(`getCommits`))
      );
  }

  getCommit(id: any): Observable<Commit> {
    const url = this.apiUrl + '/' + this.commitsUrl + `/${id}`;
    return this.http.get<Commit>(url).pipe(
      catchError(this.handleError<Commit>(`getCommit id=${id}`))
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

  updateCommit(commit: Commit): Observable<Commit> {
    const url = this.apiUrl + '/api/admin/challengecommit/';
    return this.http.put(url, commit, this.httpOptionsJson).pipe(
      catchError(this.handleError<any>('updateCommit'))
    );
  }
}

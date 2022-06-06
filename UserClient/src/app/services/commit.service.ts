import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { Config } from '../config';
import { Commit, PaginatedResult, SortConfig } from '../Shared/interfaces';

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

  getCommits(userChallengeId: any, pageNumber?, pageSize?, sortConfig?: SortConfig): Observable<PaginatedResult<Commit[]>> {
    const paginatedResult: PaginatedResult<Commit[]> = new PaginatedResult<Commit[]>();

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

    const url = this.apiUrl + `/userChallengeCommits/${userChallengeId}`;
    
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
  addCommit(commit: Commit): Observable<any> {
    const url = this.apiUrl + '/' + this.commitsUrl;
    return this.http.post(url, commit, this.httpOptionsJson).pipe(
      catchError(this.handleError<Commit>('addCommit'))
    );
  }

  updateCommit(commit: Commit): Observable<any>{
    const url = this.apiUrl + '/' + this.commitsUrl;
    return this.http.put(url, commit, this.httpOptionsJson).pipe(
      catchError(this.handleError<Commit>('updateCommit'))
    )
  }

  deleteCommit(id: any): Observable<any>{
    const url = this.apiUrl + '/' + this.commitsUrl + `/${id}`;
    return this.http.delete(url).pipe(
      catchError(this.handleError<Commit>(`deleteCommit id=${id}`))
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

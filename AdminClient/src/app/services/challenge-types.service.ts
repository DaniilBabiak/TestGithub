import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Config } from '../config';
import { ChallengeType } from '../Shared/interfaces';

@Injectable({
  providedIn: 'root'
})
export class ChallengeTypesService {
  private typesUrl = 'api/challenge/types';
  private apiUrl = Config.apiUrl;

  constructor(private http: HttpClient) { }

  getChallengeTypes(): Observable<ChallengeType[]> {
    const url = this.apiUrl + '/' + this.typesUrl;
    
    return this.http.get<ChallengeType[]>(url)
      .pipe(       
        catchError(this.handleError<ChallengeType[]>('getChallengeTypes', []))
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

import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { Config } from '../config';
import { PaginatedResult, Venue } from '../Shared/interfaces';

@Injectable({
  providedIn: 'root'
})
export class VenueService {
  private venuesUrl = 'api/venue';
  private apiUrl = Config.apiUrl;

  httpOptionsJson = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  constructor(private http: HttpClient) { }

  getVenue(id: any): Observable<Venue> {
    const url = this.apiUrl + '/' + this.venuesUrl + `/${id}`;
    return this.http.get<Venue>(url).pipe(
      catchError(this.handleError<Venue>(`getVenue id=${id}`))
    );
  }

  getNearestVenues(): Observable<Venue[]> {
    const url = this.apiUrl + '/' + this.venuesUrl + '/nearest';

    return this.http.get<Venue[]>(url).pipe(
      catchError(this.handleError<Venue[]>(`getNearestVenue`))
    );
  }

  getPagedVenues(pageNumber?, pageSize?): Observable<PaginatedResult<Venue[]>> {
    const paginatedResult: PaginatedResult<Venue[]> = new PaginatedResult<Venue[]>();

    let params = new HttpParams();

    if (pageNumber != null && pageSize != null) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }

    const url = this.apiUrl + '/' + this.venuesUrl;

    return this.http.get<Venue[]>(
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
        catchError(this.handleError<PaginatedResult<Venue[]>>(`getVenues`))
      );
  }
  createVenue(venue: Venue) {
    const url = this.apiUrl + '/' + this.venuesUrl;
    return this.http.post(url, venue, this.httpOptionsJson).pipe(
      catchError(this.handleError<Venue>('addVenue'))
    );
  }

  deleteVenue(id: any) {
    const url = this.apiUrl + '/' + this.venuesUrl + `/${id}`;
    return this.http.delete(url).pipe(
      catchError(this.handleError<Venue>('deleteVenue'))
    );
  }

  assignUser(id: any) {
    const url = this.apiUrl + '/' + this.venuesUrl + `/assign/${id}`;
    return this.http.post(url, null).pipe(
      catchError(this.handleError<Venue>('assignToVenue'))
    );
  }

  unassignUser(id: any){
    const url = this.apiUrl + '/' + this.venuesUrl + `/unassign/${id}`;
    return this.http.post(url, null).pipe(
      catchError(this.handleError<Venue>('unassignToVenue'))
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

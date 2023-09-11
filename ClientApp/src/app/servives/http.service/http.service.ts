import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpError } from '../../../common/http-error'
import { Variable } from '../../models/variable';

@Injectable()
export class HttpService {
  apiRoot: string = "https://localhost:7251/api/";

  constructor(private http: HttpClient) {
  }

  getVariables(variableIds?: number[]) {
    return this.post(`${this.apiRoot}variables/get`, variableIds);
  }

  changeVariables(variables: Variable[]) {
    return this.post(`${this.apiRoot}variables/change`, variables);
  }

  getEvents(variableId?: number, start?: Date, end?: Date, type?: number[], count?: number) {
    return this.post(`${this.apiRoot}events/get`,
      {
        variableId: variableId,
        start: start?.toLocaleString(),
        end: end?.toLocaleString(),
        type: type,
        count: count
      }
    );
  }

  getEventTypes() {
    return this.get(`${this.apiRoot}events/get-event-types`);
  }

  getTrends(variableId?: number, start?: Date, end?: Date) {
    return this.post(`${this.apiRoot}trends/get`,
      {
        variableId: variableId,
        start: start?.toLocaleString(),
        end: end?.toLocaleString()
      }
    );
  }

  getTrendingVariables() {
    return this.get(`${this.apiRoot}trends/get-variables`);
  }

  post(address: string, input: any) {
    return this.http.post(address, input)
      .pipe(
        map((result: any) => {
          return result;
        }
        ),
        catchError((error) => {
          throw new HttpError(error.status, error.message);
        })
      );
  }

  get(address: string) {
    return this.http.get(address).
      pipe(
        map((result: any) => {
          return result;
        }
        ),
        catchError((error) => {
          throw new HttpError(error.status, error.message);
        })
      );
  }


}

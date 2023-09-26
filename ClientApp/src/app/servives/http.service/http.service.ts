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

  getGroups = () => this.post(`${this.apiRoot}groups/get`, null);

  getSubgroups = (id: number) => this.post(`${this.apiRoot}subgroups/get-by-groupId`, { groupid: id });

  getVariables = (variableIds?: number[]) => this.post(`${this.apiRoot}variables/get`, variableIds);

  getVariableEntitiesBySubgroupId = (id: number) => this.post(`${this.apiRoot}subgroups/get-variables-entities-by-subgroup`, { subgroupid: id });

  changeVariables = (variables: Variable[]) => this.post(`${this.apiRoot}variables/change`, variables);

  getEvents = (variableId?: number, start?: Date, end?: Date, type?: number[], count?: number) =>
    this.post(`${this.apiRoot}events/get`,
      {
        variableId: variableId,
        start: start?.toLocaleString(),
        end: end?.toLocaleString(),
        type: type,
        count: count
      }
    );

  getEventTypes = () => this.get(`${this.apiRoot}events/get-event-types`);

  getTrends = (variableId?: number, start?: Date, end?: Date) =>
    this.post(`${this.apiRoot}trends/get`,
      {
        variableId: variableId,
        start: start?.toLocaleString(),
        end: end?.toLocaleString()
      }
    );

  getTrendingVariables = () => this.get(`${this.apiRoot}trends/get-variables`);

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
    ).toPromise();
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
    ).toPromise();
  }
}

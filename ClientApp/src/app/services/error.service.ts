import { Injectable } from '@angular/core';
import { HttpError } from '../../common/http-error'
import { HttpService } from './http.service';

@Injectable()
export class ErrorService {

  constructor(private httpService: HttpService) {
  }

  handle(message: string) {
    console.log(message);
    this.httpService.sendError(message);
  }
}

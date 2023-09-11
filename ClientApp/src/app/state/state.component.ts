import { Component } from '@angular/core';
import { HttpService } from '../servives/http.service/http.service';
import { Observable } from 'rxjs';
import { Variable } from '../models/variable';
import { ChangeDetectorRef } from '@angular/core';
import { HttpError } from '../../common/http-error';

@Component({
  selector: 'app-state-component',
  templateUrl: './state.component.html',
  providers: [HttpService]
})
export class StateComponent {

  variables: Variable[] = [];

  constructor(private httpService: HttpService, private cdr: ChangeDetectorRef) {
    this.httpService.getVariables()
      .toPromise()
      .then(v => {
        this.variables = v;
      },
        ex => console.log(ex.message));
  }
}

import { Component, OnInit } from '@angular/core';
import { HttpService } from './services/http.service';
import { ErrorService } from './services/error.service';
import { Group } from '../app/models/group';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  providers: [HttpService, ErrorService]
})
export class AppComponent {
  title = 'app';

  groups: Group[] = [];

  constructor(
    private httpService: HttpService,
    private errorService: ErrorService) { }

  async ngOnInit() {
    this.httpService.getGroups()
      .then(groups => {
        this.groups = groups;
      })
      .catch(ex => this.errorService.handle(ex.message));
  }

}

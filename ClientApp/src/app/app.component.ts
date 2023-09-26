import { Component, OnInit } from '@angular/core';
import { HttpService } from '../app/servives/http.service/http.service'
import { Group } from '../app/models/group';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  providers: [HttpService]
})
export class AppComponent {
  title = 'app';

  groups: Group[] = [];

  constructor(private httpService: HttpService) {}

  async ngOnInit() {
    this.httpService.getGroups()
      .then(groups => {
        this.groups = groups;
      })
      .catch(ex => console.log(ex.message));
  }

}

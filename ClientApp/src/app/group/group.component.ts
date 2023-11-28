import { Component } from '@angular/core';
import { HttpService } from '../services/http.service';
import { ErrorService } from '../services/error.service';
import { Observable } from 'rxjs';
import { Variable } from '../models/variable';
import { HttpError } from '../../common/http-error';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Group, Subgroup } from '../models/group'

@Component({
  selector: 'group-component',
  templateUrl: './group.component.html',
  providers: [HttpService, ErrorService]
})

export class GroupComponent {
  id?: number;
  name?: string;
  group: Group;
  subgroups?: Subgroup[];
  selectedSubgroup?: any;

  private routeSubscription: Subscription;
  private querySubscription: Subscription;

  constructor(
    private route: ActivatedRoute,
    private httpService: HttpService,
    private errorService: ErrorService)
  {
    this.group = new Group(0, "");

    this.routeSubscription = route.params.subscribe(params => {
      this.group.id = params['id'];
      this.getSubgroups();
    });
    this.querySubscription = route.queryParams.subscribe(
      (queryParam: any) => {
        this.group.name = queryParam['name'];
      }
    );
  }

  getSubgroups() {
    this.httpService.getSubgroups(this.group.id).then(subgroups => {
      this.subgroups = subgroups;
      this.subgroups?.forEach(subgroup => subgroup.group = this.group);
      this.refreshSubgroups();

      if (this.subgroups && this.subgroups[0])
        this.selectedSubgroup = this.subgroups[0];
    })
      .catch(ex => this.errorService.handle(ex.message));
  }

  refreshSubgroups() {

  }

  selectSubgroup(id: number) {
    if (this.subgroups
      && this.subgroups.filter(v => v.id == id)[0])

    this.selectedSubgroup = this.subgroups.filter(v => v.id == id)[0];
  }

}

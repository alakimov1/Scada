import { Component, Input } from '@angular/core';
import { HttpService } from '../../servives/http.service/http.service';
import { Observable } from 'rxjs';
import { Variable } from '../../models/variable';
import { HttpError } from '../../../common/http-error';
import { Subgroup } from '../../models/group';
import { VariableEntity } from '../../models/variable';
import { IndicatorComponent } from '../../common-components/indicator/indicator.component';
import { CheckComponent } from '../../common-components/check/check.component';

@Component({
  selector: 'subgroup-component',
  templateUrl: './subgroup.component.html',
  providers: [HttpService]
})

export class SubgroupComponent {
  _subgroup: Subgroup = { id: 0, name: "", group: undefined };
  variableEntities?: VariableEntity[];

  @Input() public set subgroup(value: any) {
    if (!value)
      return;

    this._subgroup = value;
    this.refreshVariables();
  }

  constructor(
    private httpService: HttpService) { }

  refreshVariables() {
    this.httpService.getVariableEntitiesBySubgroupId(this._subgroup.id).then(entities => {
      if (entities) {
        this.variableEntities = entities;
        this.variableEntities?.forEach(entity => entity.subgroup = this._subgroup);
        console.log(this.variableEntities);
      }
    })
      .catch(ex => console.log(ex.message));
  }

  onChange(v: any) {
    console.log("a");
    console.log(v);
  }
}

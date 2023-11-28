import { Component, Input, OnDestroy } from '@angular/core';
import { HttpService } from '../../services/http.service';
import { ErrorService } from '../../services/error.service';
import { Observable, Subscription, interval } from 'rxjs';
import { Variable, VariableEntity, VariableType } from '../../models/variable';
import { HttpError } from '../../../common/http-error';
import { Subgroup } from '../../models/group';

@Component({
  selector: 'subgroup-component',
  templateUrl: './subgroup.component.html',
  providers: [HttpService, ErrorService]
})

export class SubgroupComponent {
  _subgroup: Subgroup = { id: 0, name: "", group: undefined };
  subgroupSelected: boolean = false;
  variableEntities?: VariableEntity[];
  validationErrors = new Map<number, string>();
  refreshVariablesSub: Subscription;
  busyRefreshingVariables: boolean;

  @Input() public set subgroup(value: any) {
    if (!value)
      return;
    this._subgroup = value;
    this.subgroupSelected = true;
    this.refreshVariables();
  }

  constructor(
    private httpService: HttpService,
    private errorService: ErrorService) {
    this.busyRefreshingVariables = false;
    this.refreshVariablesSub = interval(200).subscribe((func => {
      if (this.subgroupSelected && !this.busyRefreshingVariables) {
        this.refreshVariablesValues();
      }
    }));
  }

  ngOnDestroy() {
    this.refreshVariablesSub.unsubscribe();
  }

  refreshVariablesValues() {
    if (!this.variableEntities)
      return;

    this.busyRefreshingVariables = true;
    this.httpService.getVariablesValues(this.variableEntities.map(_ => _.variable.id)).then((variables :[number, any][]) => {
      if (variables) {
        variables?.forEach(variable => {
          this.variableEntities?.filter(entity =>
            !entity.writable && entity?.variable?.id == variable[0]
          )?.forEach(
            entity => entity.variable.value = variable[1]
          );
        });
        this.busyRefreshingVariables = false;
      }
    })
      .catch(ex => {
        this.errorService.handle(ex.message);
        this.busyRefreshingVariables = false;
      }
      );
  }

  refreshVariables() {
    this.busyRefreshingVariables = true;
    this.httpService.getVariableEntitiesBySubgroupId(this._subgroup.id).then(entities => {
      if (entities) {
        this.variableEntities = entities;
        this.variableEntities?.forEach(entity => { entity.subgroup = this._subgroup; });
        this.busyRefreshingVariables = false;
      }
    })
      .catch(ex => {
        this.errorService.handle(ex.message);
        this.busyRefreshingVariables = false;
      }
      );
  }

  public isInputText(entity: VariableEntity): boolean {
    return entity && entity.variable && entity.writable && entity.variable.type > 1;
  }

  public isCheckBox(entity: VariableEntity): boolean {
    return entity && entity.variable && entity.writable && entity.variable.type == VariableType.Bool;
  }

  public isIndicator(entity: VariableEntity): boolean {
    return entity && entity.variable && !entity.writable && entity.variable.type == VariableType.Bool;
  }

  public isInfoText = (entity: VariableEntity) => entity && entity.variable && !entity.writable && entity.variable.type > 1; 

  onChange(value: any, id: number) {
    const variables = [...new Set(this.variableEntities?.filter(ve => ve.variable.id == id))] ;

    if (!variables
      || variables.length == 0)
      return;

    const variable = variables![0].variable;
    variable.value = value[1];

    this.httpService.changeVariables([{ id: variable.id, value: variable.value.toString() }]).then(results => {
      if (results) {
        results.forEach((result: any) => {
          if (result.result == 0) {
            this.variableEntities?.filter(_ => _.variable.id == result.id)
              .forEach(ve => {
                ve.variable.value = variable.value
                this.validationErrors.delete(variable.id);
              });
          }
          else {
            this.variableEntities?.filter(_ => _.variable.id == result.id)
              .forEach(ve => {
                this.validationErrors.set(variable.id, "Некорректное значение");
              });
          }
        });
      }
    })
      .catch(ex => this.errorService.handle(ex.message));
  }
}

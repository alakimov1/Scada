import { Component, OnInit } from '@angular/core';
import { Trend } from './chart-trends/trend';
import { TrendPoint } from './chart-trends/trend-point';
import { HttpService } from '../services/http.service';
import { ErrorService } from '../services/error.service';
import { Variable } from '../models/variable';
import { number } from 'echarts';

@Component({
  selector: 'app-trends-component',
  templateUrl: './trends.component.html',
  providers: [HttpService, ErrorService]
})
export class TrendsComponent {
  private _trends : Trend[]=[];
  trends: Trend[] = [];// [new Trend('Temp', 'rgb(55,25,25)', [new TrendPoint(new Date(2023, 1, 1), 1), new TrendPoint(new Date(2023, 1, 2), 2)])];
  startDate?: Date;
  endDate?: Date;
  variablesList?: any[];
  variableId: number | undefined;
  
  constructor(
    private httpService: HttpService,
    private errorService: ErrorService) { }

  ngOnInit(): void {
    this.startDate = new Date();
    this.endDate?.setDate(this.startDate.getDate() - 1);
    this.getVariableList();
  }

  onStartDateChange(date?: Date) {
    this.startDate = date;
    this.getTrends();
  }

  onEndDateChange(date?: Date) {
    this.endDate = date;
    this.getTrends();
  }

  onVariableChange(id?: number) {
    this.variableId = id;
    this.getTrends();
  }

  getTrends() {
    if (!this.variableId)
      return;
    this._trends = [];
    this.httpService.getTrends(this.variableId, this.startDate, this.endDate).then(
      res => {
        res.forEach((data: any) => {
          this._trends.push(
            new Trend(
              data?.trend?.variable?.name,
              "",
              data?.trend?.data?.map((point: any) => new TrendPoint(point?.date, point?.value))
          ));
          data?.eventLines?.forEach((eventLine: any) => {
            this._trends.push(
              new Trend(
                eventLine?.eventType?.name,
                `rgb(${eventLine?.eventType?.color?.red}, ${eventLine?.eventType?.color?.green}, ${eventLine?.eventType?.color?.blue})`,
                [new TrendPoint(this.startDate!, eventLine?.value), new TrendPoint(this.endDate!, eventLine?.value)]
              )
            );
          });
          
        });
        this.trends = [];
        this.trends = this._trends;
      }
    ).catch(error => this.errorService.handle(error.message));
  }

  getVariableList() {
    this.variablesList = [];
    this.httpService.getTrendingVariables().then(
      res => {
        this.variablesList = res.map((variable: any) => { return { id: variable.id, name: variable.name } });
      }
    ).catch(error => this.errorService.handle(error.message));

  }
}

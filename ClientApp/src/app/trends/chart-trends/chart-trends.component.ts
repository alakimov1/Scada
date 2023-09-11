import { Component, OnInit, Input } from '@angular/core';
import type { EChartsOption } from 'echarts';
import { TrendPoint } from './trend-point';
import { Trend } from '../chart-trends/trend'

@Component({
  selector: 'chart-trends-component',
  templateUrl: './chart-trends.component.html'
})

export class ChartTrendsComponent {
  @Input() public set data(value: Trend[] | undefined) {
    if (!value)
      return;
    this._data = value.map(line => {
      return {
        name : line.name,
        color: line.color,
        data: line.data,
        showSymbol: false,
        type :'line'
      }
    });
    if (!this.options)
      this.optionsSet();
    this.updateOptions = { series: this._data };
  }

  options!: EChartsOption;
  updateOptions!: EChartsOption;
  private _data!: DataLine[];

  initOpts = {
    locale: 'RU',
  };

  ngOnInit(): void {
    this.optionsSet();
  }

  optionsSet() {
    this.options = {
      tooltip: {
        trigger: 'axis',
        axisPointer: {
          animation: false,
        },
      },
      xAxis: {
        type: 'time',
        splitLine: {
          show: false,
        },
      },
      yAxis: {
        type: 'value',
        boundaryGap: [0, '100%'],
        splitLine: {
          show: false,
        },
      },
      series: this._data
    };
  }
}

type DataLine = {
  name: string,
  type: 'line',
  showSymbol: false,
  data: TrendPoint[],
  color: string,
};

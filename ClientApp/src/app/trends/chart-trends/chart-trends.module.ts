import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgxEchartsModule } from 'ngx-echarts';

import * as echarts from 'echarts/core';
import langRU from '../chart-trends/langRU';

import { ChartTrendsComponent } from '../chart-trends/chart-trends.component';

echarts.registerLocale("RU", langRU);

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    BrowserAnimationsModule,
    CommonModule,
    NgxEchartsModule.forRoot({ echarts: () => import('echarts')})
  ],
  declarations: [ChartTrendsComponent],
  exports: [ChartTrendsComponent],
  bootstrap: [ChartTrendsComponent]
})
export class ChartTrendsModule {
}


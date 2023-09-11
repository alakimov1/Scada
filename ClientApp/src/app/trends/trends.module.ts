import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DateTimePickerModule } from '.././common-components/date-time-picker/date-time-picker.module'
import { CheckboxListModule } from '../common-components/checkbox-list/checkbox-list.module'
//import { NgxEchartsModule } from 'ngx-echarts';
import { ChartTrendsModule } from './chart-trends/chart-trends.module';
import { RadioListModule } from '../common-components/radio-list/radio-list.module';

import { TrendsComponent } from './trends.component';

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    BrowserAnimationsModule,
    CommonModule,
    DateTimePickerModule,
    CheckboxListModule,
    ChartTrendsModule,
    RadioListModule
    //NgxEchartsModule.forRoot({ echarts: () => import('echarts') })
  ],
  declarations: [TrendsComponent],
  bootstrap: [TrendsComponent]
})
export class TrendsModule {
}


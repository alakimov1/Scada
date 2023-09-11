import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { ruLocale } from 'ngx-bootstrap/locale';
import { BsDatepickerModule, BsLocaleService } from 'ngx-bootstrap/datepicker'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';

import { DateTimePickerComponent } from './date-time-picker.component';



@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    BrowserAnimationsModule,
    CommonModule,
    BsDatepickerModule.forRoot(),
    TimepickerModule.forRoot()
  ],
  declarations: [DateTimePickerComponent],
  exports: [DateTimePickerComponent],
  bootstrap: [DateTimePickerComponent]
})
export class DateTimePickerModule {
  constructor(localeservice: BsLocaleService) {
    defineLocale('ru', ruLocale);
    localeservice.use('ru');
  }
}

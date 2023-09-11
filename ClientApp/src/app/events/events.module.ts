import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DateTimePickerModule } from '.././common-components/date-time-picker/date-time-picker.module'
import { EventsTableModule } from './events-table/events-table.module'
import { CheckboxListModule } from '../common-components/checkbox-list/checkbox-list.module'

import { EventsComponent } from './events.component';


@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    BrowserAnimationsModule,
    CommonModule,
    DateTimePickerModule,
    EventsTableModule,
    CheckboxListModule
  ],
  declarations: [EventsComponent],
  bootstrap: [EventsComponent]
})
export class EventsModule { }

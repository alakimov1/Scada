import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { EventsTableComponent } from './events-table.component';
import { TableModule } from 'ngx-easy-table';
@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    CommonModule,
    TableModule
  ],
  declarations: [EventsTableComponent],
  exports: [EventsTableComponent],
  bootstrap: [EventsTableComponent]
})
export class EventsTableModule {}

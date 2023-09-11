import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { CheckboxListComponent } from './checkbox-list.component';

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    CommonModule,
  ],
  declarations: [CheckboxListComponent],
  exports: [CheckboxListComponent],
  bootstrap: [CheckboxListComponent]
})
export class CheckboxListModule { }

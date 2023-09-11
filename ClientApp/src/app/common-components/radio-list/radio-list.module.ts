import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { RadioListComponent } from './radio-list.component';

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    CommonModule,
  ],
  declarations: [RadioListComponent],
  exports: [RadioListComponent],
  bootstrap: [RadioListComponent]
})
export class RadioListModule { }

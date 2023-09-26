import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { IndicatorComponent } from './indicator.component';

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    CommonModule,
  ],
  declarations: [IndicatorComponent],
  exports: [IndicatorComponent],
  bootstrap: [IndicatorComponent]
})
export class IndicatorModule { }

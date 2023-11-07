import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { LabelComponent } from './label.component';

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    CommonModule,
  ],
  declarations: [LabelComponent],
  exports: [LabelComponent]
})
export class LabelModule { }

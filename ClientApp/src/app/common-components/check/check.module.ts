import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { CheckComponent } from './check.component';

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    CommonModule,
  ],
  declarations: [CheckComponent],
  exports: [CheckComponent],
  bootstrap: [CheckComponent]
})
export class CheckModule { }

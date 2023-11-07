import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { TextInputComponent } from './text-input.component';

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    CommonModule,
  ],
  declarations: [TextInputComponent],
  exports: [TextInputComponent],
  bootstrap: [TextInputComponent]
})
export class TextInputModule { }

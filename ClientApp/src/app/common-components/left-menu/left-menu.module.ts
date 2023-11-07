import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { LeftMenuComponent } from './left-menu.component';

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    CommonModule,
  ],
  declarations: [LeftMenuComponent],
  exports: [LeftMenuComponent]
})
export class LeftMenuModule { }

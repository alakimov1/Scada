import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SubgroupComponent } from './subgroup.component';
import { IndicatorModule } from '../../common-components/indicator/indicator.module';
import { CheckModule } from '../../common-components/check/check.module';

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    BrowserAnimationsModule,
    CommonModule,
    IndicatorModule,
    CheckModule
  ],
  declarations: [SubgroupComponent],
  bootstrap: [SubgroupComponent],
  exports: [SubgroupComponent]
})
export class SubgroupModule { }

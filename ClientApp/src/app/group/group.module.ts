import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { SubgroupModule } from './subgroup/subgroup.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { GroupComponent } from './group.component';


@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    BrowserAnimationsModule,
    CommonModule,
    SubgroupModule
  ],
  declarations: [GroupComponent],
  bootstrap: [GroupComponent],
  exports: [GroupComponent]
})
export class GroupModule { }

import { Component,Input } from '@angular/core';

@Component({
  selector: 'indicator',
  templateUrl: './indicator.component.html'
})

export class IndicatorComponent {
  @Input() public set value(value: boolean) {
    this.selected = value;
  }
  @Input() public set text(value: string) {
    this.label = value;
  }

  label: string = "";
  selected: boolean = false;
}

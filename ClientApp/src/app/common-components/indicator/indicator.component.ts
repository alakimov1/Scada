import { Component,Input } from '@angular/core';

@Component({
  selector: 'indicator',
  templateUrl: './indicator.component.html'
})

export class IndicatorComponent {
  @Input() public set value(value: boolean) {
    this.selected = value;
  }

  selected: boolean = false;
}

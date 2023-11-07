import { Component,Input } from '@angular/core';

@Component({
  selector: 'label',
  templateUrl: './label.component.html'
})

export class LabelComponent {
  @Input() public set text(value: string) {
    this._text = value;
  }

  _text: string = "";
}

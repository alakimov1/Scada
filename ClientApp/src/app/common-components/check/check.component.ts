import { Component, Input, Output, EventEmitter } from '@angular/core';
import { VariableEntity } from '../../models/variable';

@Component({
  selector: 'check',
  templateUrl: './check.component.html'
})

export class CheckComponent {
  @Input() public set value(value: boolean) {
    this._value = value;
  }

  @Input() public set id(value: number) {
    this._id = value;
  }

  _id: number = 0;
  _value: boolean = false;

  handleSelected(v: any) {
    var value = v.target.checked;
    this._value = value;
    this.onChanged.emit([this._id, this._value]);
  }

  @Output() onChanged = new EventEmitter<[number, boolean]>();
}

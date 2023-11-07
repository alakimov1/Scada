import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'text-input',
  templateUrl: './text-input.component.html'
})

export class TextInputComponent {
  @Input() public set value(value: string) {
    this._value = value;
  }

  @Input() public set id(value: number) {
    this._id = value;
  }

  @Input() public set error(value: string | undefined) {
    this._error = value ?? "";
  }

  _id: number = 0;
  _value: string = "";
  _error: string = "";

  handleChange(v: any) {
    var value = v.target.value;
    this._value = value;
    this.onChanged.emit([this._id, this._value]);
  }

  @Output() onChanged = new EventEmitter<[number, string]>();
}

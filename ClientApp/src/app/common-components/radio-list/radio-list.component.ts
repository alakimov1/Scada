import { Component,Output,Input, EventEmitter } from '@angular/core';

@Component({
  selector: 'radio-list',
  templateUrl: './radio-list.component.html'
})

export class RadioListComponent {
  _radioListModel: { id: number, name: string }[] = [];

  @Input() public set radioListModel(value: any) {
    if (!value)
      return;

    this._radioListModel = value;
    if (this._radioListModel
      && this._radioListModel.length > 0
      && this._radioListModel[0]) {
      this.change(this._radioListModel[0].id);
    }
  }

  @Output() onChanged = new EventEmitter<number | undefined>();

  selected: number | undefined;

  change(itemId: number | undefined) {
    if (!this._radioListModel)
      return;

    this.selected = itemId;
    this.onChanged.emit(this.selected);
  }
}

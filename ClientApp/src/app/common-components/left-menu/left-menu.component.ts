import { Component,Output,Input, EventEmitter } from '@angular/core';

@Component({
  selector: 'left-menu',
  templateUrl: './left-menu.component.html'
})

export class LeftMenuComponent {
  _items: { id: number, name: string }[] = [];

  @Input() public set items(value: any) {
    if (!value)
      return;

    this._items = value;
    if (this._items
      && this._items.length > 0
      && this._items[0]) {
      this.change(this._items[0].id);
    }
  }

  @Output() onChanged = new EventEmitter<number>();

  selected: number | undefined;

  change(itemId: number) {
    if (!this._items)
      return;

    this.selected = itemId;
    this.onChanged.emit(itemId);
  }
}

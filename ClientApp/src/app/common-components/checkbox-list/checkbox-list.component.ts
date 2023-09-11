import { Component,Output,Input, EventEmitter } from '@angular/core';

@Component({
  selector: 'checkbox-list',
  templateUrl: './checkbox-list.component.html'
})

export class CheckboxListComponent {
  @Input() checkboxListModel?: any[] = [];
  @Output() onChanged = new EventEmitter<any[]>();

  change(itemId: number) {
    if (!this.checkboxListModel)
      return;

    this.checkboxListModel
      .filter(item => item.id == itemId)
      .forEach(item => {
        item.value = !item.value;
        item.color = item.value
          ? item.onColor
          : null
      });
    
    this.onChanged.emit(this.checkboxListModel.filter(item=>item.value));
  }
}

import { ChangeDetectionStrategy, Component, OnInit,Input, TemplateRef, ViewChild } from '@angular/core';
import { Columns, Config, DefaultConfig } from 'ngx-easy-table';

@Component({
  selector: 'events-table',
  templateUrl: './events-table.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})

export class EventsTableComponent implements OnInit {
  @ViewChild('startDateTpl', { static: true }) startDateTpl!: TemplateRef<any>;
  @ViewChild('endDateTpl', { static: true }) endDateTpl!: TemplateRef<any>;
  @ViewChild('typeTpl', { static: true }) typeTpl!: TemplateRef<any>;

  _data: any;

  @Input() public set data(value:any) {
    this._data = value;
  }

  configuration: Config = {...DefaultConfig };
  columns: Columns[]=[];
  
  ngOnInit(): void {
    this.columns = [
      { key: 'event', title: 'Событие' },
      { key: 'type', title: 'Тип', cellTemplate: this.typeTpl },
      { key: 'start', title: 'Начало', cellTemplate: this.startDateTpl },
      { key: 'end', title: 'Окончание', cellTemplate: this.endDateTpl },
    ];
    this.configuration = { ...DefaultConfig };
  }
}

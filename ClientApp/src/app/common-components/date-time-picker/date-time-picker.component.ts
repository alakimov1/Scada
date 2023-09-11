import { Component,Input,Output, EventEmitter, OnInit } from '@angular/core';

@Component({
  selector: 'date-time-picker',
  templateUrl: './date-time-picker.component.html'
})
export class DateTimePickerComponent {
  @Input() text?: string;
  @Input() minDate?: Date;
  @Input() maxDate?: Date;
  @Input() initDate?: Date;
  @Output() onChanged = new EventEmitter<Date>();

  data?: Date = new Date();
  isMeridian = false;
  showSpinners = true;

  ngOnInit() {
    if (this.initDate)
      this.date = this.checkDate(this.initDate);
  }

  private _time?: Date = new Date();

  public get time(): Date | undefined {
    return this._time;
  }

  public set time(date: Date | undefined) {
    this._time = date;
    this.onValueChange();
  }

  private _date?: Date = new Date();

  public get date(): Date | undefined {
    return this._date;
  }

  public set date(date: Date | undefined) {
    this._date = date;
    this.onValueChange();
  }

  onValueChange(): void {
    let newDate: Date | undefined = undefined;
    newDate = this._date;
    
    if (this._time) {
      newDate?.setHours(this._time.getHours());
      newDate?.setMinutes(this._time.getMinutes());
      newDate?.setSeconds(0);
    }

    this.data = this.checkDate(newDate);
    this.onChanged.emit(this.data);
  }

  checkDate(date: Date |undefined): Date | undefined {
    if (!date)
      return undefined;

    if (this.minDate
      && this.maxDate
      && this.minDate > this.maxDate)
      throw new Error("Минимальная разрешенная дата больше максимальной")

    if (this.minDate
      && date < this.minDate)
      date = this.minDate;

    if (this.maxDate
      && date > this.maxDate)
      date = this.maxDate;

    return date;
  }
}

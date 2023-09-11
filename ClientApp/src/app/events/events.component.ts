import { Component, OnInit } from '@angular/core';
import { HttpService } from '../servives/http.service/http.service';

@Component({
  selector: 'app-events-component',
  templateUrl: './events.component.html',
  providers: [HttpService]
})

export class EventsComponent {
  startDate?: Date;
  endDate?: Date;
  eventTypes?: any[];
  selectedEventTypes?: any[];
  eventsData?: any[];
  private _eventsData?: any[];

  ngOnInit(): void {
    this.getEventTypes();
    this.startDate = new Date();
    this.endDate?.setDate(this.startDate.getDate() - 1);
    this.getEvents();
  }

  constructor(private httpService: HttpService) { }

  onStartDateChange(date?: Date) {
    this.startDate = date;
    this.getEvents();
  }

  onEndDateChange(date?: Date) {
    this.endDate = date;
    this.getEvents();
  }

  onEventTypeChange(eventTypes?: any[]) {
    this.selectedEventTypes = eventTypes;
    this.getEvents();
  }

  getEventTypes() {
    this.eventTypes = [];
    this.httpService
      .getEventTypes().toPromise().then(
        res => {
          res.forEach((eventType: any) => {
            this.eventTypes?.push(
              {
                id: eventType?.id,
                name: eventType?.name,
                onColor: `rgb(${eventType?.color?.red}, ${eventType?.color?.green}, ${eventType?.color?.blue})`,
              }
            );
          });
        }
      ).catch(error => console.log(error.message));
  }

  getEvents() {
    this._eventsData = [];
    this.eventsData = [];
    this.httpService
      .getEvents(undefined, this.startDate, this.endDate, this.selectedEventTypes?.map(_ => _.id)).toPromise().then(
        res => {
          res.forEach((ev: any) => {
            let startDate = ev?.startTime
              ? new Date(ev?.startTime)
              : null;
            let endDate = ev?.endTime
              ? new Date(ev?.endTime)
              : null;
            this._eventsData?.push(
              {
                event: ev?.event?.message,
                type: ev?.event?.type?.name,
                start: startDate?.getTime(),
                startText: startDate?.toLocaleString(),
                end: endDate?.getTime(),
                endText: endDate?.toLocaleString(),
                color: `rgb(${ev?.event?.type?.color?.red}, ${ev?.event?.type?.color?.green}, ${ev?.event?.type?.color?.blue})`,
              }
            );
          });
          this.eventsData = this._eventsData;
        }
      ).catch(error => console.log(error.message));
  }
}

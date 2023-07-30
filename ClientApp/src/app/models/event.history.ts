import { Event } from './event';

export class EventHistory {
  constructor(
    public Id: number,
    public Event: Event,
    public StartTime:string,
    public EndTime:string
  ) { }
}

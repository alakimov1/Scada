import { Variable } from './variable';

export enum EventType {
  Alarm,
  Warning,
  Event
}

export enum EventVariableComparison {
  None,
  Equal,
  NotEqual,
  GreaterThan,
  LessThan,
  GreaterThanOrEqual,
  LessThanOrEqual,
  Empty,
  Contain,
  NotContain
}

export class Event {
  constructor(
    public Id: number,
    public Type:EventType,
    public Variable: Variable,
    public Limit:object,
    public Comparison: EventVariableComparison,
    public Message:string
  ) { }
}

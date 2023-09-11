import { TrendPoint } from './trend-point';

export class Trend {
  name: string;
  color: string;
  data: TrendPoint[];

  constructor(name: string, color: string, data: TrendPoint[]) {
    this.name = name;
    this.color = color;
    this.data = data;
  }
}

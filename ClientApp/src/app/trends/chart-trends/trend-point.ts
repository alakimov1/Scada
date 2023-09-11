export class TrendPoint{
  public name: string;
  public value: [string, number];

  constructor(date: Date, value: number) {
    this.name = new Date(date).toISOString();
    this.value = [
      this.name,
      value
    ];
  }
}

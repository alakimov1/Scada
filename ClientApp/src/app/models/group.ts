export class Group {
  constructor(
    public id: number,
    public name: string
  ) { }
}

export class Subgroup {
  constructor(
    public id: number,
    public name: string,
    public group?: Group
  ) { }
}

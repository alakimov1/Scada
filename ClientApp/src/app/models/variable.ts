export enum VariableType {
  None,
  Bool,
  Byte,
  Word,
  Dword,
  Real,
  String
}

export class Variable {
  constructor(
    public id: number,
    public address: number,
    public type: VariableType,
    public name: string,
    public value: object,
    public active: number,
    public trendingPeriod:number
  ) { }
}

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
    public Id: number,
    public Address: number,
    public Type: VariableType,
    public Name: string,
    public Value: object,
    public Active: number,
    public TrendingPeriod:number
  ) { }
}

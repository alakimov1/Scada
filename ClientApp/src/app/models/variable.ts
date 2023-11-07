import { Subgroup } from "./group";

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

export class VariableEntity {
  constructor(
    public variable: Variable,
    public subgroup: Subgroup,
    public writable: boolean
  ) { }

}

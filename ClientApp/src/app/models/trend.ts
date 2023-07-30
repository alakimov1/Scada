import { Variable } from './variable';

export class Trend {
  constructor(
    public Variable: Variable,
    public Data: [string, object][],
    public Start: string,
    public End:string
  ) { }
}

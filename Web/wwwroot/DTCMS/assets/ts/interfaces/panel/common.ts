import {Languages, Operator} from '../../Enums';

interface ListInput {
  arg: {
    pageNumber: number;
    pageSize: number;
  };
  lang?: Languages;
  filters?:ApiFilter[];
  sortField?: string;
  ascending?: boolean;
}

interface ApiFilter {
  field: string;
  operator: Operator;
  value: string;
}


export {ListInput,ApiFilter}
 

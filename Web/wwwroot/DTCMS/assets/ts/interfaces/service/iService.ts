import { AxiosResponse } from "axios";
import { ListInput } from "../panel/common";

export interface IService<TResponse, TModel> {
  paginatedList(input: ListInput): Promise<AxiosResponse<TResponse[]>>;
  getById(id: number | string): Promise<AxiosResponse<TResponse>>;
  create(userInput: TModel): Promise<AxiosResponse<TModel>>;
  update(userInput: TModel): Promise<AxiosResponse>;
  remove(id: number[] | string[]): Promise<AxiosResponse>;
}

 
import axios, { AxiosInstance, AxiosResponse, AxiosRequestConfig } from "axios";
import axiosInstance from "./index";
import { IService } from "../interfaces/service/iService";
import { ListInput } from "../interfaces/panel/common";

abstract class BaseService<TResponse, TModel>
  implements IService<TResponse, TModel>
{
  private axiosInstance: AxiosInstance;
  private apiUrl: string; // Add a property to store the API address

  constructor(apiUrl: string) {
    this.axiosInstance = axiosInstance;
    this.apiUrl = apiUrl;
  }

  public async paginatedList(
    input: ListInput
  ): Promise<AxiosResponse<TResponse[]>> {
    const { data } = await this.axiosInstance.post<AxiosResponse<TResponse[]>>(
      `${this.apiUrl}/PagedList/`,
      input
    );
    return data;
  }

  public async getById(id: number | string): Promise<AxiosResponse<TResponse>> {
    const { data } = await this.axiosInstance.get<AxiosResponse<TResponse>>(
      `${this.apiUrl}/${id}`
    );
    return data;
  }

  public async create(userInput: TModel): Promise<AxiosResponse<TModel>> {
    const { data } = await this.axiosInstance.post<AxiosResponse<TModel>>(
      `${this.apiUrl}/`,
      { ...userInput }
    );
    return data;
  }

  public async update(userInput: TModel): Promise<AxiosResponse> {
    const { data } = await this.axiosInstance.put(`${this.apiUrl}/`, {
      ...userInput,
    });
    return data;
  }

  public async remove(id: number[] | string[]): Promise<AxiosResponse> {
    const config: AxiosRequestConfig = {
      data: id,
    };
    const response = await this.axiosInstance.delete<AxiosResponse>(
      `${this.apiUrl}`,
      config
    );
    return response;
  }
}

export default BaseService;

import axiosInstance from "./index";
import { AxiosResponse } from "axios";
import { AxiosRequestConfig } from "axios";
 
async function multiRemove(
  apiUrl: string,
  id: number[] | string[]
): Promise<AxiosResponse> {
  const config: AxiosRequestConfig = {
    data: id,
  };
  const response = await axiosInstance.delete<AxiosResponse>(
    `${apiUrl}`,
    config
  );
  return response;
}

export { multiRemove };

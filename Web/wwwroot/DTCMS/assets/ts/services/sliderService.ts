import axios from "./index";
import { AxiosResponse } from "axios";
import { SliderModel, SliderResponse } from "../interfaces/panel/slider";
import { ListInput } from "../interfaces/panel/common";

/**
 *
 * @param input which is based on ListInput
 * @returns a list of sliders
 */
const getSliders = async (
  input: ListInput
): Promise<AxiosResponse<AxiosResponse<SliderResponse[]>>> => {
  let res = await axios.post<AxiosResponse<Array<SliderResponse>>>(
    "/slider/PagedList/",
    { input }
    
  );
  return res;
};

/**
 *
 * @param id string or number
 * @returns returns a slider
 */
const getSlider = async (
  id: number | string
): Promise<AxiosResponse<SliderResponse>> => {
  let token = localStorage.getItem("token");
  let res = await axios.get<SliderResponse>("/slider/" + id, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  return res;
};

/**
 *
 * @param userInput which is type of slider model
 * @returns the created slider
 */
const createSlider = async (
  userInput: SliderModel
): Promise<AxiosResponse<SliderResponse>> => {
  let token = localStorage.getItem("token");
  let res = await axios.post<SliderResponse>(
    "/slider/",
    { userInput },
    {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );
  return res;
};

/**
 *
 * @param userInput which is type of slider model
 * @returns a normal http response (may change in the future)
 */
const updateSlider = async (userInput: SliderModel): Promise<AxiosResponse> => {
  let token = localStorage.getItem("token");
  let res = await axios.put(
    "/slider/",
    { userInput },
    {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );
  return res;
};

/**
 * @param id which is a list of either numbers or strings
 * @returns a normal http response (may change in the future)
 */
const deleteSlider = async (
  id: number[] | string[]
): Promise<AxiosResponse> => {
  let token = localStorage.getItem("token");
  let data = JSON.stringify(id);

  let res = await axios.delete("/slider/", {
    headers: {
      Authorization: `Bearer ${token}`,
    },
    data: data,
  });
  return res;
};

export { getSliders, getSlider, createSlider, updateSlider, deleteSlider };

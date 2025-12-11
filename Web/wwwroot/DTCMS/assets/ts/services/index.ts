import axios, { AxiosInstance } from "axios";
import { serverLang } from "../utils/index";

const axiosInstance: AxiosInstance = axios.create({
  // baseURL: "/api/v1/",

  baseURL: "/api/",
  headers: {
    "Accept-Language": serverLang(),
  },
});
// eslint-disable-next-line import/no-anonymous-default-export

axiosInstance.interceptors.request.use(
  function (config) {
    // Do something before request is sent
    return config;
  },
  function (error) {
    // Do something with request error
    return Promise.reject(error);
  }
);

// Add a response interceptor
axiosInstance.interceptors.response.use(
  function (response) {
    // Any status code that lie within the range of 2xx cause this function to trigger
    // Do something with response data
    return response;
  },
  async function (error) {
    // Any status codes that falls outside the range of 2xx cause this function to trigger
    // Do something with response error
    if (!error.response) {
    } else {
      if (error.response.status === 401) {
        // Remove the token from localStorage

        // Redirect the user to /auth/login
        window.location.replace("/auth/login");
      }
      if (error.response.status === 403) {
      }

      if (error.response.status === 400) {
        //error 400
      }

      if (error.response.status === 402 || error.response.status === 404) {
      }

      if (error.response.status === 500) {
        //error 500
      }
    }
    return Promise.reject(error);
  }
);

export default axiosInstance;

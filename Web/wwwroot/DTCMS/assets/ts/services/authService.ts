import axios from "./index";
import { AxiosResponse } from "axios";
import {
  LoginModel,
  TokenModel,
  CodeModel,
  PassowrdModel,
  VerificationModel,
} from "../interfaces/auth";

/**
 * @param userInput:LoginModel
 * @returns TokenModel
 */
const loginUser = async (
  userInput: LoginModel
): Promise<AxiosResponse<TokenModel>> => {
  let formData = new FormData();
  formData.append("username", userInput.userName);
  formData.append("password", userInput.passsword);
  formData.append("userRole", userInput.userRole);
  let res = await axios.post<TokenModel>("/auth/login", formData);
  return res;
};

/**
 * @param userInput:CodeModel
 * @returns Standard Axios Response
 */
const sendCode = async (userInput: CodeModel): Promise<AxiosResponse> => {
  let formData = new FormData();
  formData.append("username", userInput.userName);
  formData.append("IsForgotpass", JSON.stringify(userInput.isForgotpass));
  let res = await axios.post<AxiosResponse>("/auth/sendCode", formData);
  return res;
};

/**
 * @param userInput:CodeModel
 * @returns Standard Axios Response
 */
const register = async (userInput: CodeModel): Promise<AxiosResponse> => {
  let formData = new FormData();
  formData.append("username", userInput.userName);
  let res = await axios.post<AxiosResponse>("/auth/register", formData);
  return res;
};

/**
 * @param userInput:VerificationModel
 * @returns TokenModel
 */
const verifyUser = async (
  userInput: VerificationModel
): Promise<AxiosResponse<AxiosResponse<TokenModel, any>, any>> => {
  let res = await axios.post<AxiosResponse<TokenModel>>(
    "/auth/AccountVerification",
    userInput
  );
  return res;
};

/**
 * @param userInput:PassowrdModel
 * @returns Standard Axios Response
 */
const setPassword = async (userInput: PassowrdModel) => {
  let token: string | null = localStorage.getItem("token");
  let res = await axios.post<AxiosResponse>(
    "/auth/SetPassword",
    {
      password: userInput.password,
    },
    {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );
  return res;
};

export { loginUser, sendCode, verifyUser, setPassword, register };

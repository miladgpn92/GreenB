import {
  FileApiInput,
  FileResponse,
  FolderApiInput,
  FolderResponse,
} from "../interfaces/panel/files";
import axios from "./index";
import { AxiosResponse } from "axios";

/**
 *
 * @param userInput which is type of slider model
 * @returns the created slider
 */
const uploadFiles = async (
  files: any | string[] = [],
  filePath: string = "",
  onProgress: (percent: number) => void
): Promise<AxiosResponse> => {
  let formData = new FormData();
  files.forEach((file) => {
    formData.append("files", file);
  });
  formData.append("filepath", filePath);
  let res = await axios.post("/admin/Filemanager/Uplaod", formData, {
    onUploadProgress: (progressEvent) => {
      const { loaded, total } = progressEvent;
      let percent = Math.round((loaded / total) * 100);
      onProgress(percent);
    },
  });
  return res;
};

/**
 *
 * @param apiInput which is the type of FileApiInput
 * @returns  an array of FileResponse
 */
const getFiles = async (
  apiInput: FileApiInput
): Promise<AxiosResponse<AxiosResponse<FileResponse[]>>> => {
  const data = await axios.post<AxiosResponse<FileResponse[]>>(
    `FileManager/GetFiles/`,
    apiInput
  );
  return data;
};
/**
 *
 * @param apiInput which is the type of FileApiInput
 * @returns  an array of FileResponse
 */
const getFolders = async (
  apiInput: FolderApiInput
): Promise<AxiosResponse<AxiosResponse<FolderResponse[]>>> => {
  const data = await axios.post<AxiosResponse<FolderResponse[]>>(
    `FileManager/GetFolders/`,
    apiInput
  );
  return data;
};
const createFolder = async (folderName: string): Promise<AxiosResponse> => {
  const data = await axios.post<AxiosResponse>(
    `Filemanager/CreateFolder?FolderName=${folderName}`
  );
  return data;
};
const removeFolder = async (folderName: string[]): Promise<AxiosResponse> => {
  const payload = {
    folderPath: folderName,
  };
  const data = await axios.delete<AxiosResponse>(`Filemanager/RemoveFolder`, {
    data: payload,
  });
  return data;
};
const removeFile = async (filepath: string[]): Promise<AxiosResponse> => {
  const payload = {
    filepath: filepath,
  };
  const data = await axios.delete<AxiosResponse>(`Filemanager/RemoveFiles`, {
    data: payload,
  });
  return data;
};
export {
  uploadFiles,
  getFiles,
  getFolders,
  createFolder,
  removeFolder,
  removeFile,
};

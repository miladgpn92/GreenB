import { uploadFiles } from "../../../services/fileManagerService";

export const uploadFile = async (
  image: File[],
  subAddress: string,
  handleProgress: Function,
  handleError:Function
) => {
  try{
   return await uploadFiles(image, subAddress, (progress) => {
      handleProgress(progress);
    });
  }
  catch{
    handleError()
  }
};

import ImageCropper from "../imageCropper";
import { uploadFile } from "../uploaderService";

/**
 * variables
 */
let cropper: ImageCropper;
let file: File;
const overlay: HTMLElement = document.getElementById("preview__overlay");
const loading: HTMLElement = document.getElementById("preview__loading");
const percentageView: HTMLElement = document.getElementById("percentage__view");
const retryandCancelButtons: HTMLElement =
  document.getElementById("retry__button");
const pencilIcon: HTMLElement = document.getElementById("edit__pencil__icon");
let imagePreview: HTMLImageElement =
  document.querySelector("#cropper__preview");
let fileInput: HTMLInputElement;
let inputValue: HTMLInputElement;

(<any>window).initProfileCropper = async (parentId: string) => {
  fileInput = event.target as HTMLInputElement;
  let file = fileInput.files[0];
  if (file && file.type.includes("image")) {
    let parent: HTMLElement = document.querySelector(`#${parentId}`);
    let cropIsNotNeeded = parent.getAttribute("data-not-crop");
    let compressNotNeeded = parent.getAttribute("data-not-compress");
    let rangeInput: HTMLInputElement = parent.querySelector("input");
    let prevCropper = parent.querySelector(".cropper-container");
    inputValue = document.querySelector(".round__input__value");
    let maxHeight = parent.getAttribute("data-max-height");
    let maxSize = parent.getAttribute("data-max-size");
    let aspectRatio = parent.getAttribute("data-aspect-ratio");
    if (
      (cropIsNotNeeded &&
        cropIsNotNeeded === "true" &&
        compressNotNeeded &&
        compressNotNeeded === "true") ||
      file.type.includes("svg") ||
      file.type.includes("gif") 
    ) {
      handleUpload();
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = (e) => {
        imagePreview.src = e.target.result as string;
      };
    } else if (
      (!compressNotNeeded || compressNotNeeded !== "true") &&
      cropIsNotNeeded &&
      cropIsNotNeeded === "true"
    ) {
      ImageCropper.CompressImage(
        fileInput.files[0],
        0.6,
        maxHeight ? parseInt(maxHeight) : 1024,
        maxSize ? parseInt(maxSize) : 80000,
        (item: File) => {
          file = item;
          handleUpload(item);
          const reader = new FileReader();
          reader.readAsDataURL(item);
          reader.onload = (e) => {
            imagePreview.src = e.target.result as string;
          };
        }
      );
    } else {
      if (prevCropper) {
        prevCropper.remove();
      }
      cropper = new ImageCropper(
        file,
        parent,
        maxSize ? parseInt(maxSize) : 80000,
        maxHeight ? parseInt(maxHeight) : 1024,
        0.6,
        aspectRatio ? eval(aspectRatio) : 1,
        rangeInput,
        imagePreview,
        true,
        false
      );
      cropper.initCropper();
    }
  }
};
(<any>window).handleProfileZoom = () => {
  cropper.handleZoom();
};

(<any>window).handleProfileCrop = async (): Promise<void> => {
  cropper.handleCrop((item) => {
    file = item;
    handleUpload();
  });
};
const handleUpload = async (item?: File) => {
  retryandCancelButtons.classList.add("hidden");
  percentageView.classList.remove("hidden");
  overlay.classList.remove("hidden");
  loading.classList.remove("hidden");
  let apiCall = await uploadFile(
    [item ? item : file],
    "/profile",
    handleProgress,
    handleError
  );
  setValue(apiCall.data.data[0]);
};
const setValue = (uploadPath: string) => {
  inputValue.value = uploadPath;
};
(<any>window).setProfileCompressStatus = (): void => {
  cropper.setCompressStatus();
  let compressorStatuses: NodeListOf<HTMLElement> = document.querySelectorAll(
    ".compressor__status"
  );
  Array.from(compressorStatuses).forEach((item) => {
    item.classList.toggle("hidden");
  });
};
const handleProgress = (progress: number) => {
  retryandCancelButtons.classList.add("hidden");
  percentageView.innerHTML = progress + "%";
  if (progress === 100) {
    overlay.classList.remove("bg-gray-700");
    overlay.classList.remove("opacity-80");
    overlay.className =
      overlay.className + " bg-gradient-to-t from-gray-800 from-10% to-40%";
    loading.classList.add("hidden");
    percentageView.classList.add("hidden");
    pencilIcon.classList.remove("hidden");
  }
};
const handleError = () => {
  overlay.classList.add("bg-gray-700");
  overlay.classList.add("opacity-80");
  pencilIcon.classList.add("hidden");
  percentageView.classList.add("hidden");
  loading.classList.add("hidden");
  retryandCancelButtons.classList.remove("hidden");
  overlay.className.replace(
    " bg-gradient-to-t from-gray-800 from-10% to-40%",
    ""
  );
};

(<any>window).handleRemove = () => {
  fileInput.value = null;
  inputValue.value = null;
  overlay.classList.add("hidden");
  imagePreview.src = "";
};

(<any>window).handleUpload = handleUpload;
(() => {
  const input: HTMLInputElement = document.querySelector(
    ".round__input__value"
  );
  if (input && input.value && input.value !== "") {
    imagePreview.src = input.value;
    imagePreview.classList.remove("hidden");
    overlay.classList.remove("bg-gray-700");
    overlay.classList.remove("opacity-80");
    overlay.classList.remove("hidden");
    percentageView.classList.add("hidden")
    overlay.className =
      overlay.className + " bg-gradient-to-t from-gray-800 from-10% to-40%";
    pencilIcon.classList.remove("hidden");
  }
})();

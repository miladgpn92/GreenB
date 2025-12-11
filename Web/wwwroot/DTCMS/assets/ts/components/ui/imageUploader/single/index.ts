import ImageCropper from "../../imageCropper";
import { uploadFile } from "../../uploaderService";

/**
 * variables
 */
let cropper: ImageCropper;
let file: File;
let overlay: HTMLElement;
let loading: HTMLElement;
let percentageView: HTMLElement;
let retryandCancelButtons: HTMLElement;
let imagePreview: HTMLImageElement;
let progessBar: HTMLElement;
let removeBtn: HTMLElement;
let fileInput: HTMLInputElement;
let inputValue: HTMLInputElement;
let parent: HTMLElement;

(<any>window).initMediaCropper = async (parentId: string) => {
  setInstance(parentId);
  parent = document.querySelector(`#${parentId}`);
  fileInput = event.target as HTMLInputElement;
  file = fileInput.files[0];
  if (file && file.type.includes("image")) {
    const cropIsNotNeeded = parent.getAttribute("data-not-crop");
    const compressNotNeeded = parent.getAttribute("data-not-compress");
    const maxHeight = parent.getAttribute("data-max-height");
    const maxSize = parent.getAttribute("data-max-size");
    const aspectRatio = parent.getAttribute("data-aspect-ratio");
    if (
      (cropIsNotNeeded &&
        cropIsNotNeeded === "true" &&
        compressNotNeeded &&
        compressNotNeeded === "true") ||
      file.type.includes("svg") ||
      file.type.includes("gif")
    ) {
      setTimeout(() => {
        Array.from(
          document.querySelectorAll(".overlay") as NodeListOf<HTMLElement>
        ).forEach((item) => {
          item.click();
        });
      }, 100);
      handleMediaUpload();
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
        (item) => {
          file = item;
          handleMediaUpload();
          const reader = new FileReader();
          reader.readAsDataURL(item);
          reader.onload = (e) => {
            imagePreview.src = e.target.result as string;
          };
        }
      );
    } else {
      let rangeInput: HTMLInputElement = parent.querySelector("input");
      let prevCropper = parent.querySelector(".cropper-container");
      if (prevCropper) {
        prevCropper.remove();
      }
      cropper = new ImageCropper(
        file,
        parent,
        maxSize ? parseInt(maxSize) : 80000,
        maxHeight ? parseInt(maxHeight) : 1024,
        0.6,
        aspectRatio ? eval(aspectRatio) : 20 / 9,
        rangeInput,
        imagePreview,
        false,
        true
      );
      cropper.initCropper();
      resetCropper();
    }
  }
};
const setInstance = (parentId) => {
  parent = document.querySelector(`#${parentId}`);
  let previewTarget = parent.getAttribute("data-preview-target");
  let retryTarget = parent.getAttribute("data-retry-target");
  let prgoressTarget = parent.getAttribute("data-progress-target");
  let removeTarget = parent.getAttribute("data-remove-target");
  let percentageTarget = parent.getAttribute("data-percentage-target");
  let overlayTarget = parent.getAttribute("data-overlay-target");
  let loadingTarget = parent.getAttribute("data-loading-target");
  let inputValueTarget = parent.getAttribute("data-input-target");
  imagePreview = document.querySelector("#" + previewTarget);
  retryandCancelButtons = document.querySelector("#" + retryTarget);
  progessBar = document.querySelector("#" + prgoressTarget);
  removeBtn = document.querySelector("#" + removeTarget);
  percentageView = document.querySelector("#" + percentageTarget);
  inputValue = document.querySelector("." + inputValueTarget);
  overlay = document.querySelector("#" + overlayTarget);
  loading = document.querySelector("#" + loadingTarget);
};
(<any>window).handleMediaZoom = () => {
  cropper.handleZoom();
};

(<any>window).handleMediaCrop = async (): Promise<void> => {
  cropper.handleCrop((item) => {
    file = item;
    handleMediaUpload();
  });
};
const handleMediaUpload = async () => {
  retryandCancelButtons.classList.add("hidden");
  percentageView.classList.remove("hidden");
  overlay.classList.remove("hidden");
  overlay.classList.add("bg-gray-700");
  overlay.classList.remove("bg-gradient-to-b");
  overlay.classList.remove("from-red-500");
  overlay.classList.remove("to-gray-800");
  imagePreview.classList.remove("hidden");
  loading.classList.remove("hidden");
  let apiCall = await uploadFile([file], "/media", handleProgress, handleError);
  setValue(apiCall.data.data[0]);
};
const setValue = (uploadPath: string) => {
  inputValue.value = uploadPath;
};
(<any>window).setMediaCompressStatus = (): void => {
  cropper.setCompressStatus();
  let compressorStatuses: NodeListOf<HTMLElement> = parent.querySelectorAll(
    ".media__compressor__status"
  );
  Array.from(compressorStatuses).forEach((item) => {
    item.classList.toggle("hidden");
  });
};
const resetCropper = () => {
  cropper.isCompressed = true;
  let compressorStatuses: NodeListOf<HTMLElement> = parent.querySelectorAll(
    ".media__compressor__status"
  );
  let statusArray = Array.from(compressorStatuses);
  statusArray[0].classList.remove("hidden");
  statusArray[1].classList.add("hidden");
};
const handleProgress = (progress: number) => {
  overlay.classList.replace("z-[0]", "z-[3]");
  overlay.classList.replace("opacity-100", "opacity-50");
  retryandCancelButtons.classList.add("hidden");
  progessBar.classList.remove("hidden");
  percentageView.innerHTML = progress + "%";
  (progessBar.children[0] as HTMLElement).style.width = progress + "%";
  if (progress === 100) {
    //  overlay.classList.add("hidden");
    overlay.classList.replace("z-[3]", "z-[0]");
    overlay.classList.replace("opacity-50", "opacity-100");
    removeBtn.classList.remove("hidden");
    loading.classList.add("hidden");
    percentageView.classList.add("hidden");
    progessBar.classList.add("hidden");
  }
};
const handleError = () => {
  percentageView.classList.add("hidden");
  loading.classList.add("hidden");
  progessBar.classList.add("hidden");
  retryandCancelButtons.classList.remove("hidden");
  overlay.classList.replace("z-[0]", "z-[3]");
  overlay.classList.replace("opacity-100", "opacity-50");
  overlay.classList.remove("bg-gray-700");
  overlay.classList.add("bg-gradient-to-b");
  overlay.classList.add("from-red-500");
  overlay.classList.add("to-gray-800");
};

(<any>window).handleMediaRemove = (parentId) => {
  setInstance(parentId);
  inputValue.value = "";
  if (fileInput) fileInput.value = "";
  overlay.classList.add("hidden");
  imagePreview.src = "";
  imagePreview.classList.add("hidden");
  overlay.classList.add("hidden");
  removeBtn.classList.add("hidden");
  progessBar.classList.add("hidden");
  loading.classList.add("hidden");
  percentageView.classList.add("hidden");
  retryandCancelButtons.classList.add("hidden");
};

(<any>window).handleMediaUpload = handleMediaUpload;
(() => {
  const inputList: NodeListOf<HTMLInputElement> =
    document.querySelectorAll("[data-parent-id]");
  if (inputList) {
    Array.from(inputList).forEach((element) => {
      let parentId = element.getAttribute("data-parent-id");
      if (parentId) {
        setInstance(parentId);
        if (inputValue.value && inputValue.value !== "") {
          imagePreview.src = inputValue.value;
          imagePreview.classList.remove("hidden");
          removeBtn.classList.remove("hidden");
        }
      }
    });
  }
})();

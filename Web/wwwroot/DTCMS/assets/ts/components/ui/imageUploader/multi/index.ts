import ImageCropper from "../../imageCropper";
import { uploadFile } from "../../uploaderService";
import { v4 as uuidv4 } from "uuid";

/**
 * variables
 */
let cropper: ImageCropper;
let files: File[];
let croppedFiles: File[] = [];

let imagePreview: HTMLImageElement;

let fileInput: HTMLInputElement;
let parent: HTMLElement;
let fileCountHolder: HTMLElement;
let fileIndexHolder: HTMLElement;
let currentIndex = 0;
let maxHeight: string;
let maxSize: string;
let aspectRatio: string;
(<any>window).initMultiCropper = (parentId: string) => {
  parent = document.querySelector(`#${parentId}`);
  fileInput = event.target as HTMLInputElement;
  files = Array.from(fileInput.files)
    .filter((file) => file.type.includes("image"))
    .map((file) => {
      let ext = file.name.split(".").slice(-1);
      let newFile = new File([file], uuidv4() + "." + ext, {
        type: file.type,
      });
      return newFile;
    });
  currentIndex = 0;
  maxHeight = parent.getAttribute("data-max-height");
  maxSize = parent.getAttribute("data-max-size");
  aspectRatio = parent.getAttribute("data-aspect-ratio");
  parent.classList.remove("hidden");
  if (files.length !== 0) {
    const cropIsNotNeeded = parent.getAttribute("data-not-crop");
    const compressNotNeeded = parent.getAttribute("data-not-compress");
    const maxHeight = parent.getAttribute("data-max-height");
    const maxSize = parent.getAttribute("data-max-size");
    if (
      cropIsNotNeeded &&
      cropIsNotNeeded === "true" &&
      compressNotNeeded &&
      compressNotNeeded === "true"
    ) {
      croppedFiles = files;
      generatePreviews();
    } else if (
      (!compressNotNeeded || compressNotNeeded !== "true") &&
      cropIsNotNeeded &&
      cropIsNotNeeded === "true"
    ) {
      files.forEach((file) => {
        ImageCropper.CompressImage(
          file,
          0.6,
          maxHeight ? parseInt(maxHeight) : 1024,
          maxSize ? parseInt(maxSize) : 80000,
          (item) => {
            croppedFiles.push(item);
            if (croppedFiles.length === files.length) {
              generatePreviews();
            }
          }
        );
      });
    } else {
      startCropper();
    }
  }
};
const startCropper = () => {
  fileCountHolder = parent.querySelector(".file__count");
  fileCountHolder.innerText = files.length.toString();
  fileIndexHolder = parent.querySelector(".file__index");
  fileIndexHolder.innerText = (currentIndex + 1).toString();
  let rangeInput: HTMLInputElement = parent.querySelector("input");
  let prevCropper = parent.querySelector(".cropper-container");
  if (prevCropper) {
    cropper.destroyInstance();
    prevCropper.remove();
  }

  cropper = new ImageCropper(
    files[currentIndex],
    parent,
    maxSize ? parseInt(maxSize) : 80000,
    maxHeight ? parseInt(maxHeight) : 1024,
    0.6,
    aspectRatio ? eval(aspectRatio) : 20 / 9,
    rangeInput,
    null,
    false,
    true
  );
  cropper.initCropper();
  resetCropper();
};
const resetCropper = () => {
  cropper.isCompressed = true;
  let compressorStatuses: NodeListOf<HTMLElement> = parent.querySelectorAll(
    ".multi__media__compressor__status"
  );
  let statusArray = Array.from(compressorStatuses);
  statusArray[0].classList.remove("hidden");
  statusArray[1].classList.add("hidden");
};
(<any>window).handleMultiMediaZoom = () => {
  cropper.handleZoom();
};
(<any>window).setMultiMediaCompressStatus = () => {
  cropper.setCompressStatus();
  let compressorStatuses: NodeListOf<HTMLElement> = parent.querySelectorAll(
    ".multi__media__compressor__status"
  );
  Array.from(compressorStatuses).forEach((item) => {
    item.classList.toggle("hidden");
  });
};
(<any>window).handleMultiMediaCrop = async (): Promise<void> => {
  cropper.handleCrop((item) => {
    croppedFiles.push(item);
    if (currentIndex + 1 === files.length) {
      let dissmiss: HTMLElement = document.getElementById("cropper__dismisser");

      dissmiss.click();
      generatePreviews();
    } else {
      currentIndex = currentIndex + 1;

      let exepFiles = files.filter(
        (a) => a.type.includes("svg") || a.type.includes("gif")
      );
      croppedFiles.push(...exepFiles);
      files = files.filter(function (x) {
        return exepFiles.indexOf(x) < 0;
      });
      startCropper();
    }
  });
};
(<any>window).handleMultiCropperRemove = (fileName: string, itemId: string) => {
 
  croppedFiles = croppedFiles.filter((file) => file.name !== fileName);
  document.querySelector(`#${itemId}`).remove();
  let inputArea = parent.getAttribute("data-input-area");
  let target = document.querySelector(`#${inputArea}`).querySelector("input");
  let jsonValue: string[] = [];
  if (target.value !== "") {
    jsonValue = JSON.parse(target.value);
  }
  let check = jsonValue.find((item) => item.includes(fileName));
  if (check) {
    jsonValue = jsonValue.filter((item) => item !== check);
    target.value = JSON.stringify(jsonValue);
  }
};

(<any>window).cancelAllCrops = (inputId: string) => {
  let input: HTMLInputElement = document.querySelector(`#${inputId}`);
  files = [];
  croppedFiles = [];
  input.files = undefined;
  input.value = "";
};
const generatePreviews = () => {
  let previewContainerAddress: string = parent.getAttribute(
    "data-preview-container"
  );
  let container: HTMLElement = document.querySelector(
    `#${previewContainerAddress}`
  );
  croppedFiles.forEach((file, index) => {
    handleUploadFiles(previewContainerAddress + index, file);
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = (e) => {
      container.innerHTML =
        container.innerHTML +
        ` 
    <div class="w-28 h-[120px] relative rounded-md mx-0.5 mb-1" id='${
      previewContainerAddress + index
    }'>
     <svg id='primary__remove' onclick="handleMultiCropperRemove('${
       file.name
     }','${
          previewContainerAddress + index
        }')" class="cursor-pointer z-[3] absolute bottom-2 left-2"
       width="17" height="17" viewBox="0 0 17 17" fill="none" xmlns="http://www.w3.org/2000/svg">
       <circle cx="8.40517" cy="8.40517" r="8.40517" fill="#DC2626" />
       <path
         d="M12.2716 6.22037L11.8342 12.344C11.7965 12.8718 11.3573 13.2807 10.8281 13.2807H6.65468C6.12551 13.2807 5.68632 12.8718 5.64862 12.344L5.21122 6.22037M7.73277 8.23761V11.2635M9.75001 8.23761V11.2635M10.2543 6.22037V4.70744C10.2543 4.42891 10.0285 4.20312 9.75001 4.20312H7.73277C7.45425 4.20312 7.22846 4.42891 7.22846 4.70744V6.22037M4.70691 6.22037H12.7759"
         stroke="white" stroke-width="1.34483" stroke-linecap="round" stroke-linejoin="round" />
     </svg>
     <img src='${
       e.target.result
     }' class="rounded-md absolute block w-full h-full object-cover" id="media__cropper__preview" />
     <div id="preview__overlay" class="hidden z-[3] bg-gray-700 absolute h-full w-full opacity-50 rounded-md">
   
       <svg aria-hidden="true" id="preview__loading"
         class="  absolute bottom-4 right-1 w-6 h-6 mr-2 text-transparent animate-spin dark:text-gray-600 fill-white"
         viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
         <path
           d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z"
           fill="currentColor" />
         <path
           d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z"
           fill="currentFill" />
       </svg>
       <div class="absolute bottom-4 left-2 text-white" id="percentage__view">0%</div>
   
     </div>
     <div id="progress__bar" dir="ltr"
       class="z-[3] hidden absolute bottom-0 w-full bg-gray-200 rounded-full h-1.5 dark:bg-gray-700">
       <div class="bg-green-500 h-1.5 rounded-full" style="width: 33%">
       </div>
     </div>
     <div class="hidden" id="retry__button">
       <svg onclick="handleRetry('${file.name}','${
          previewContainerAddress + index
        }')" class="z-[4]  cursor-pointer absolute bottom-2 right-2" width="24" height="24"
         viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
         <mask id="mask0_1250_32739" style="mask-type:alpha" maskUnits="userSpaceOnUse" x="0" y="0" width="24" height="24">
           <rect width="24" height="24" fill="#D9D9D9" />
         </mask>
         <g mask="url(#mask0_1250_32739)">
           <path
             d="M12 22C10.75 22 9.57933 21.7627 8.488 21.288C7.396 20.8127 6.446 20.1707 5.638 19.362C4.82933 18.554 4.18733 17.604 3.712 16.512C3.23733 15.4207 3 14.25 3 13H5C5 14.95 5.67933 16.604 7.038 17.962C8.396 19.3207 10.05 20 12 20C13.95 20 15.604 19.3207 16.962 17.962C18.3207 16.604 19 14.95 19 13C19 11.05 18.3207 9.39567 16.962 8.037C15.604 6.679 13.95 6 12 6H11.85L13.4 7.55L12 9L8 5L12 1L13.4 2.45L11.85 4H12C13.25 4 14.421 4.23767 15.513 4.713C16.6043 5.18767 17.5543 5.829 18.363 6.637C19.171 7.44567 19.8127 8.39567 20.288 9.487C20.7627 10.579 21 11.75 21 13C21 14.25 20.7627 15.4207 20.288 16.512C19.8127 17.604 19.171 18.554 18.363 19.362C17.5543 20.1707 16.6043 20.8127 15.513 21.288C14.421 21.7627 13.25 22 12 22Z"
             fill="white" />
         </g>
       </svg>
       <svg onclick="handleMultiCropperRemove('${file.name}','${
          previewContainerAddress + index
        }')" class=" z-[4]  cursor-pointer absolute bottom-2 left-2" width="25" height="25"
         viewBox="0 0 25 25" fill="none" xmlns="http://www.w3.org/2000/svg">
         <circle cx="12.5" cy="12.5" r="12.5" fill="#DC2626" />
         <path
           d="M18.25 9.25L17.5995 18.3569C17.5434 19.1418 16.8903 19.75 16.1033 19.75H9.89668C9.10972 19.75 8.45656 19.1418 8.40049 18.3569L7.75 9.25M11.5 12.25V16.75M14.5 12.25V16.75M15.25 9.25V7C15.25 6.58579 14.9142 6.25 14.5 6.25H11.5C11.0858 6.25 10.75 6.58579 10.75 7V9.25M7 9.25H19"
           stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
       </svg>
     </div>
    </div>
    `;
    };
  });
  files = [];

  fileInput.files = undefined;
  fileInput.value = "";
};
(<any>window).handleRetry = (fileName: string, parentId: string) => {
  let file = croppedFiles.find((f) => f.name === fileName);
  handleUploadFiles(parentId, file);
};
const handleUploadFiles = async (parentId: string, file: File) => {
  await uploadFile(
    [file],
    "/media",
    (progress) => handleProgress(parentId, progress),
    () =>
      setTimeout(() => {
        handleError(parentId);
      }, 200)
  )
    .then((data) => {
      let inputArea = parent.getAttribute("data-input-area");
      let target = document
        .querySelector(`#${inputArea}`)
        .querySelector("input");
      let jsonValue: string[] = [];
      if (target.value !== "") {
        jsonValue = JSON.parse(target.value);
      }
      jsonValue.push(data.data.data[0]);
      target.value = JSON.stringify(jsonValue);
    })
    .catch();
  croppedFiles = croppedFiles.filter((f) => f.name !== file.name);
};
const handleProgress = (parentId: string, progress: number) => {
  let container = document.querySelector(`#${parentId}`);
  let previewOverlay = container.querySelector(`#preview__overlay`);
  let retryBtns = container.querySelector("#retry__button");
  let primaryRemoveBtn = container.querySelector(`#primary__remove`);
  let previewLoading = container.querySelector("#preview__loading");
  let percentageContainer = container.querySelector("#percentage__view");
  let progressBarContainer = container.querySelector("#progress__bar");
  previewOverlay.classList.remove("hidden");
  previewOverlay.classList.add("bg-gray-700");
  previewOverlay.classList.remove("bg-gradient-to-b");
  previewOverlay.classList.remove("from-red-500");
  previewOverlay.classList.remove("to-gray-800");
  primaryRemoveBtn.classList.add("hidden");
  previewLoading.classList.remove("hidden");
  retryBtns.classList.add("hidden");
  progressBarContainer.classList.remove("hidden");
  (progressBarContainer.children[0] as HTMLElement).style.width =
    progress + "%";
  percentageContainer.classList.remove("hidden");
  percentageContainer.innerHTML = `${progress}`;
  if (progress === 100) {
    previewOverlay.classList.add("hidden");
    progressBarContainer.classList.add("hidden");
    primaryRemoveBtn.classList.remove("hidden");
  }
};
const handleError = (parentId: string) => {
  let container = document.querySelector(`#${parentId}`);
  let previewOverlay = container.querySelector(`#preview__overlay`);
  let primaryRemoveBtn = container.querySelector(`#primary__remove`);
  let previewLoading = container.querySelector("#preview__loading");
  let percentageContainer = container.querySelector("#percentage__view");
  let progressBarContainer = container.querySelector("#progress__bar");
  let retryBtns = container.querySelector("#retry__button");
  primaryRemoveBtn.classList.add("hidden");
  previewLoading.classList.add("hidden");
  percentageContainer.classList.add("hidden");
  progressBarContainer.classList.add("hidden");
  retryBtns.classList.remove("hidden");
  previewOverlay.classList.remove("hidden");
  previewOverlay.classList.remove("bg-gray-700");
  previewOverlay.classList.add("bg-gradient-to-b");
  previewOverlay.classList.add("from-red-500");
  previewOverlay.classList.add("to-gray-800");
};
(<any>window).cancelImage = () => {
  files = files.filter((file) => file.name !== files[currentIndex].name);
  fileCountHolder = parent.querySelector(".file__count");
  fileCountHolder.innerText = files.length.toString();
  fileIndexHolder = parent.querySelector(".file__index");
  fileIndexHolder.innerText = currentIndex.toString();
  if (currentIndex === files.length || files.length === 0) {
    let dissmiss: HTMLElement = document.getElementById("cropper__dismisser");
    files = [];
    fileInput.files = undefined;
    fileInput.value = "";
    dissmiss.click();
    generatePreviews();
  } else {
    startCropper();
  }
};

(() => {
  let inputDefault: NodeListOf<HTMLInputElement> = document.querySelectorAll(
    "[data-multi-parentId]"
  );
  if (inputDefault) {
    inputDefault.forEach((item, index) => {
      let parentId = item.getAttribute("data-multi-parentId");
      let parent = document.querySelector(`#${parentId}`);
      let defaultItems: string[] = [];
      if (item.value !== "") {
        defaultItems = JSON.parse(item.value);
      }
      defaultItems.forEach((img) => {
        parent.innerHTML =
          parent.innerHTML +
          ` 
      <div class="w-28 h-[120px] relative rounded-md mx-0.5 mb-1" id='${
        parentId + (index + 100)
      }'>
       <svg id='primary__remove' onclick="handleRemoveMultiPrevFile( '${
         parentId + (index + 100)
       }','${
            item.id
          }','${img}')" class="cursor-pointer z-[3] absolute bottom-2 left-2"
         width="17" height="17" viewBox="0 0 17 17" fill="none" xmlns="http://www.w3.org/2000/svg">
         <circle cx="8.40517" cy="8.40517" r="8.40517" fill="#DC2626" />
         <path
           d="M12.2716 6.22037L11.8342 12.344C11.7965 12.8718 11.3573 13.2807 10.8281 13.2807H6.65468C6.12551 13.2807 5.68632 12.8718 5.64862 12.344L5.21122 6.22037M7.73277 8.23761V11.2635M9.75001 8.23761V11.2635M10.2543 6.22037V4.70744C10.2543 4.42891 10.0285 4.20312 9.75001 4.20312H7.73277C7.45425 4.20312 7.22846 4.42891 7.22846 4.70744V6.22037M4.70691 6.22037H12.7759"
           stroke="white" stroke-width="1.34483" stroke-linecap="round" stroke-linejoin="round" />
       </svg>
       <img src='${img}' class="rounded-md absolute block w-full h-full object-cover" id="media__cropper__preview" />
          </div>
      `;
      });
    });
  }
})();
(<any>window).handleRemoveMultiPrevFile = (
  previewId: string,
  inputId: string,
  imgUrl: string
) => {
  document.querySelector(`#${previewId}`).remove();
  let input: HTMLInputElement = document.querySelector(`#${inputId}`);
  let parsedValue: string[] = JSON.parse(input.value);
  parsedValue = parsedValue.filter((a) => a !== imgUrl);
  input.value = JSON.stringify(parsedValue);
};

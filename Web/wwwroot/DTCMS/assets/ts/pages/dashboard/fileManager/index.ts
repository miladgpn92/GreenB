import ImageCropper from "../../../components/ui/imageCropper";
import { uploadFile } from "../../../components/ui/uploaderService";
import {
  FileApiInput,
  FolderResponse,
  FileResponse,
  FolderApiInput,
} from "../../../interfaces/panel/files";
import {
  getFiles,
  getFolders,
  removeFolder,
  removeFile,
  createFolder,
} from "../../../services/fileManagerService";
let moment = require("moment-jalaali");
import { v4 as uuidv4 } from "uuid";

/**
 * true for box view and false for row view
 */
let viewType = true;
let uploadedFiles: File[] = [];
let selectedFile: File;
let cropper: ImageCropper;
let uploaderInput: HTMLInputElement;
let imagePreview: HTMLImageElement;
let uploadButton: HTMLButtonElement = document.querySelector("#uploadButton");
let previewParentEl: HTMLElement = document.querySelector(".fm__preview__area");
let uploadInitiator: NodeListOf<HTMLElement> =
  document.querySelectorAll(".upload__initiator");
let uploadLoadings: NodeListOf<HTMLElement> =
  document.querySelectorAll(".upload__loading");
let activeItem: { path: string; type: "folder" | "file" } = {
  path: "",
  type: "file",
};
let breadCrumbContainer: HTMLElement = document.querySelector(
  "#breadCrumbContainer"
);
let parentContainer: HTMLElement = document.querySelector("#parentContainer");
let fileControls: FileApiInput = {
  pager: {
    pageNumber: 1,
    pageSize: 50,
  },
  filePath: "/",
  searchText: "",
};

let pageLoading = document.querySelector("#page__loading");
let files: FileResponse[] = [];
let folders: FolderResponse[] = [];
let getPageFiles = async () => {
  try {
    let apiCall = await getFiles(fileControls);
    files = [...files, ...apiCall.data.data];
  } catch (ex) {
    console.log(ex);
  }
};

let getPageFolders = async () => {
  try {
    let apiCall = await getFolders({
      filePath: fileControls.filePath,
      searchText: fileControls.searchText,
    });
    folders = [...folders, ...apiCall.data.data];
  } catch (ex) {
    console.log(ex);
  }
};
let generateBoxFolderView = (): void => {
  folders.forEach((folder) => {
    parentContainer.innerHTML += `
    <div onclick="selectAtciveItem('${folder.name}','folder','folder__options')" class="w-[171px] h-[163px] mx-1 my-2 cursor-pointer">
    <div class="w-full h-[115px] bg-orange-200 flex items-center justify-center rounded-lg">
        <svg width="58" height="58" viewBox="0 0 58 58" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M7.25 16.9173V41.084C7.25 43.7534 9.41396 45.9173 12.0833 45.9173H45.9167C48.586 45.9173 50.75 43.7534 50.75
41.084V21.7507C50.75 19.0813 48.586 16.9173 45.9167 16.9173H31.4167L26.5833 12.084H12.0833C9.41396 12.084 7.25 14.2479
7.25 16.9173Z" stroke="#FB923C" stroke-width="4.83333" stroke-linecap="round" stroke-linejoin="round" />
        </svg>
    </div>
    <div class="text-sm font-normal mt-2">${folder.name}</div>
</div>
    `;
  });
};
const generateBoxFileView = (): void => {
  files.forEach((file) => {
    if (file.fileType.toLowerCase().includes("image")) {
      parentContainer.innerHTML += `
    <div onclick="selectAtciveItem('${
      file.name
    }','file','file__options')" class="w-[171px] h-[163px] mx-1 my-2 cursor-pointer">
        <img src="${file.fileUrl}" class="rounded-md w-full h-[115px]" />
        <div class="text-sm font-normal mt-2 text-gray-900" >${
          file.name.length > 20 ? file.name.slice(0, 20) + "..." : file.name
        }</div>
        <div class="text-sm font-normal text-gray-500 text-right" dir="ltr"  >
      
        ${
          file.length < 1000
            ? file.length
            : file.length >= 1000 && file.length <= 999999
            ? (file.length / 1000).toFixed(2)
            : (file.length / 1000000).toFixed(2)
        }
        ${
          file.length < 1000
            ? "Byte"
            : file.length >= 1000 && file.length <= 999999
            ? "KB"
            : "MB"
        } </div>
    </div>`;
    } else {
      let icon = getBoxViewIcon(file.fileType.toLowerCase());
      parentContainer.innerHTML += `
        <div onclick="selectAtciveItem('${
          file.name
        }','file','file__options')" class="w-[171px] h-[163px] mx-1 my-2 cursor-pointer">
            ${icon}
            <div class="text-sm font-normal mt-2 text-gray-900" >${
              file.name.length > 20 ? file.name.slice(0, 20) + "..." : file.name
            }</div>
            <div class="text-sm font-normal text-gray-500 text-right" dir="ltr"  >
          
            ${
              file.length < 1000
                ? file.length
                : file.length >= 1000 && file.length <= 999999
                ? (file.length / 1000).toFixed(2)
                : (file.length / 1000000).toFixed(2)
            }
            ${
              file.length < 1000
                ? "Byte"
                : file.length >= 1000 && file.length <= 999999
                ? "KB"
                : "MB"
            } </div>
        </div>`;
    }
  });
};

const getBoxViewIcon = (type: string): string => {
  let icon: string;
  if (type.includes("zip")) {
    icon = `
    <div class="rounded-md w-full h-[115px] bg-purple-900 flex justify-center items-center">
      <svg width="171" height="115" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
      <rect x="1" y="1" width="46" height="46" rx="6" fill="#4C1D95"/>
      <path d="M20.6186 26.7V29H11.4386V27.46L16.9786 19.22H11.7186V16.76H20.5386V18.54L15.0586 26.7H20.6186ZM25.398 16.76V29H22.658V16.76H25.398ZM33.3728 25.38H30.6128V29H27.8728V16.76H33.4328C34.7661 16.76 35.7661 17.16 36.4328 17.96C37.1128 18.76 37.4528 19.8133 37.4528 21.12C37.4528 21.9467 37.2861 22.6867 36.9528 23.34C36.6195 23.98 36.1461 24.48 35.5328 24.84C34.9195 25.2 34.1995 25.38 33.3728 25.38ZM34.2528 19.64C33.9461 19.3067 33.4928 19.14 32.8928 19.14H30.6128V23.02H32.7928C33.4061 23.02 33.8795 22.8533 34.2128 22.52C34.5461 22.1733 34.7128 21.7067 34.7128 21.12C34.7128 20.4667 34.5595 19.9733 34.2528 19.64Z" fill="white"/>
      </svg>
    </div>
  `;
  } else if (type.includes("pdf")) {
    icon = `
    <div class="rounded-md w-full h-[115px] bg-red-600 flex justify-center items-center">
    <svg width="171" height="115" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
      <rect width="48" height="48" rx="6" fill="#DC2626"/>
      <path d="M13.99 25.38H11.23V29H8.49V16.76H14.05C15.3833 16.76 16.3833 17.16 17.05 17.96C17.73 18.76 18.07 19.8133 18.07 21.12C18.07 21.9467 17.9033 22.6867 17.57 23.34C17.2367 23.98 16.7633 24.48 16.15 24.84C15.5367 25.2 14.8167 25.38 13.99 25.38ZM14.87 19.64C14.5633 19.3067 14.11 19.14 13.51 19.14H11.23V23.02H13.41C14.0233 23.02 14.4967 22.8533 14.83 22.52C15.1633 22.1733 15.33 21.7067 15.33 21.12C15.33 20.4667 15.1767 19.9733 14.87 19.64ZM28.7972 27.54C27.9172 28.5133 26.5772 29 24.7772 29H19.8572V16.76H24.7772C26.5639 16.76 27.8972 17.2533 28.7772 18.24C29.6705 19.2133 30.1172 20.7867 30.1172 22.96C30.1172 25.04 29.6772 26.5667 28.7972 27.54ZM26.2172 19.62C25.7905 19.3533 25.1839 19.22 24.3972 19.22H22.5972V26.7H24.3972C25.0905 26.7 25.6439 26.6 26.0572 26.4C26.4839 26.1867 26.8039 25.8133 27.0172 25.28C27.2439 24.7333 27.3572 23.96 27.3572 22.96C27.3572 22.0533 27.2705 21.3333 27.0972 20.8C26.9372 20.2667 26.6439 19.8733 26.2172 19.62ZM34.9409 24.02V29H32.2009V16.76H40.4809V18.96H34.9409V21.74H39.8009V24.02H34.9409Z" fill="white"/>
      </svg>
      </div>
      `;
  } else if (type.includes("video") || type.includes("mp4")) {
    icon = `
    <div class="rounded-md w-full h-[115px] bg-purple-600 flex justify-center items-center">
      <svg width="171" height="115" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
      <rect width="48" height="48" rx="6" fill="#7C3AED"/>
      <path fill-rule="evenodd" clip-rule="evenodd" d="M24 36.8002C31.0693 36.8002 36.8 31.0694 36.8 24.0002C36.8 16.931 31.0693 11.2002 24 11.2002C16.9308 11.2002 11.2 16.931 11.2 24.0002C11.2 31.0694 16.9308 36.8002 24 36.8002ZM23.2875 19.4689C22.7966 19.1416 22.1653 19.1111 21.645 19.3895C21.1248 19.6679 20.8 20.2101 20.8 20.8002V27.2002C20.8 27.7903 21.1248 28.3324 21.645 28.6109C22.1653 28.8893 22.7966 28.8588 23.2875 28.5315L28.0875 25.3315C28.5326 25.0347 28.8 24.5352 28.8 24.0002C28.8 23.4652 28.5326 22.9657 28.0875 22.6689L23.2875 19.4689Z" fill="white"/>
      </svg>
      </div>
  `;
  } else if (type.includes("audio")) {
    icon = `
    <div class="rounded-md w-full h-[115px] bg-orange-500 flex justify-center items-center">
         <svg width="43" height="44" viewBox="0 0 43 44" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M27.8345 15.6664C31.3329 19.1649 31.3329 24.837 27.8345 28.3354M32.9022 10.5987C39.1994 16.8959 39.1994 27.1057 32.9022 33.4029M10.0079 27.376H7.16667C6.17716 27.376 5.375 26.5738 5.375 25.5843V18.4177C5.375 17.4281 6.17716 16.626 7.16667 16.626H10.0079L18.4414 8.19242C19.5701 7.06373 21.5 7.86312 21.5 9.45932V34.5427C21.5 36.1389 19.5701 36.9382 18.4414 35.8096L10.0079 27.376Z" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
         </svg>

      </div>
      `;
  } else if (type.includes("word")) {
    icon = `
    <div class="rounded-md w-full h-[115px] bg-blue-700 flex justify-center items-center">
    <svg width="171" height="115" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
     <rect width="48" height="48" rx="6" fill="#1D4ED8"/>
     <path d="M11.6899 23.474L10.1779 28.5H8.42788L6.53788 19.932H8.41388L9.53388 25.182L10.7099 21.108V19.932H12.2499L13.7059 25.182L14.8539 19.932H16.7159L14.8399 28.5H13.0899L11.6899 23.474ZM25.5371 24.258C25.5371 27.1793 24.2117 28.64 21.5611 28.64C18.9011 28.64 17.5711 27.1793 17.5711 24.258C17.5711 22.6993 17.9024 21.57 18.5651 20.87C19.2277 20.1607 20.2264 19.806 21.5611 19.806C22.8957 19.806 23.8897 20.1607 24.5431 20.87C25.2057 21.57 25.5371 22.6993 25.5371 24.258ZM20.0071 26.232C20.3431 26.652 20.8611 26.862 21.5611 26.862C22.2611 26.862 22.7744 26.652 23.1011 26.232C23.4371 25.8027 23.6051 25.1447 23.6051 24.258C23.6051 23.2967 23.4417 22.6107 23.1151 22.2C22.7884 21.78 22.2704 21.57 21.5611 21.57C20.8517 21.57 20.3337 21.78 20.0071 22.2C19.6804 22.6107 19.5171 23.2967 19.5171 24.258C19.5171 25.1447 19.6804 25.8027 20.0071 26.232ZM29.8943 25.756H28.9423V28.5H27.0243V19.932H30.9863C31.9009 19.932 32.5869 20.1933 33.0443 20.716C33.5016 21.2387 33.7303 21.948 33.7303 22.844C33.7303 23.4507 33.5903 23.9687 33.3103 24.398C33.0303 24.8273 32.6383 25.1493 32.1343 25.364C32.2649 25.4293 32.3629 25.5133 32.4283 25.616C32.5029 25.7093 32.5729 25.84 32.6383 26.008L33.6743 28.5H31.6723L30.7763 26.288C30.6923 26.0827 30.5896 25.9427 30.4683 25.868C30.3563 25.7933 30.1649 25.756 29.8943 25.756ZM31.8123 22.844C31.8123 22.0133 31.4109 21.598 30.6083 21.598H28.9423V24.09H30.4963C31.3736 24.09 31.8123 23.6747 31.8123 22.844ZM41.4854 27.478C40.8694 28.1593 39.9314 28.5 38.6714 28.5H35.2274V19.932H38.6714C39.922 19.932 40.8554 20.2773 41.4714 20.968C42.0967 21.6493 42.4094 22.7507 42.4094 24.272C42.4094 25.728 42.1014 26.7967 41.4854 27.478ZM39.6794 21.934C39.3807 21.7473 38.956 21.654 38.4054 21.654H37.1454V26.89H38.4054C38.8907 26.89 39.278 26.82 39.5674 26.68C39.866 26.5307 40.09 26.2693 40.2394 25.896C40.398 25.5133 40.4774 24.972 40.4774 24.272C40.4774 23.6373 40.4167 23.1333 40.2954 22.76C40.1834 22.3867 39.978 22.1113 39.6794 21.934Z" fill="white"/>
     </svg>
     </div>
     `;
  } else if (type.includes("excel")) {
    icon = `
    <div class="rounded-md w-full h-[115px] bg-[#10B981] flex justify-center items-center">
    <svg width="171" height="115" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
      <rect width="48" height="48" rx="6" fill="#10B981"/>
      <path d="M13.9114 25.26L11.3514 29H8.07141L12.2514 22.82L8.21141 16.76H11.4314L13.9114 20.46L16.3714 16.76H19.5914L15.5314 22.82L19.7314 29H16.4714L13.9114 25.26ZM29.9131 26.7V29H21.6931V16.76H24.4331V26.7H29.9131ZM38.8922 28.26C38.0922 28.8867 36.9189 29.2 35.3722 29.2C34.6255 29.2 33.9255 29.1333 33.2722 29C32.6189 28.88 32.0589 28.7067 31.5922 28.48V26.2C32.1389 26.44 32.7189 26.64 33.3322 26.8C33.9455 26.9467 34.5322 27.02 35.0922 27.02C36.5722 27.02 37.3122 26.5467 37.3122 25.6C37.3122 25.16 37.1122 24.82 36.7122 24.58C36.3122 24.3267 35.6055 24.0467 34.5922 23.74C33.7789 23.4867 33.1389 23.2067 32.6722 22.9C32.2189 22.5933 31.8855 22.2267 31.6722 21.8C31.4722 21.36 31.3722 20.8133 31.3722 20.16C31.3722 19 31.7589 18.1133 32.5322 17.5C33.3055 16.8867 34.4655 16.58 36.0122 16.58C36.5722 16.58 37.1855 16.6333 37.8522 16.74C38.5189 16.8467 39.0522 16.9667 39.4522 17.1V19.4C38.4922 18.9733 37.4989 18.76 36.4722 18.76C35.6989 18.76 35.1189 18.8733 34.7322 19.1C34.3455 19.3133 34.1522 19.6533 34.1522 20.12C34.1522 20.5067 34.3189 20.8067 34.6522 21.02C34.9989 21.2333 35.6455 21.4667 36.5922 21.72C37.5122 21.96 38.2255 22.2533 38.7322 22.6C39.2522 22.9467 39.6055 23.36 39.7922 23.84C39.9922 24.3067 40.0922 24.8867 40.0922 25.58C40.0922 26.74 39.6922 27.6333 38.8922 28.26Z" fill="white"/>
      </svg>
      </div>
      `;
  } else {
    icon = `
    <div class="rounded-md w-full h-[115px] bg-[#9ca3af] justify-center items-center">

      <svg width="171" height="115" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
      <rect width="48" height="48" rx="6" fill="#9CA3AF"/>
      <g clip-path="url(#clip0_2986_4833)">
      <path d="M24 40C28.2435 40 32.3131 38.3143 35.3137 35.3137C38.3143 32.3131 40 28.2435 40 24C40 19.7565 38.3143 15.6869 35.3137 12.6863C32.3131 9.68571 28.2435 8 24 8C19.7565 8 15.6869 9.68571 12.6863 12.6863C9.68571 15.6869 8 19.7565 8 24C8 28.2435 9.68571 32.3131 12.6863 35.3137C15.6869 38.3143 19.7565 40 24 40ZM18.6125 18.3313C19.1062 16.9375 20.4313 16 21.9125 16H25.5562C27.7375 16 29.5 17.7688 29.5 19.9438C29.5 21.3563 28.7437 22.6625 27.5187 23.3687L25.5 24.525C25.4875 25.3375 24.8188 26 24 26C23.1687 26 22.5 25.3312 22.5 24.5V23.6562C22.5 23.1187 22.7875 22.625 23.2563 22.3562L26.025 20.7688C26.3188 20.6 26.5 20.2875 26.5 19.95C26.5 19.425 26.075 19.0063 25.5562 19.0063H21.9125C21.7 19.0063 21.5125 19.1375 21.4438 19.3375L21.4187 19.4125C21.1438 20.1938 20.2812 20.6 19.5063 20.325C18.7313 20.05 18.3188 19.1875 18.5938 18.4125L18.6187 18.3375L18.6125 18.3313ZM22 30C22 29.4696 22.2107 28.9609 22.5858 28.5858C22.9609 28.2107 23.4696 28 24 28C24.5304 28 25.0391 28.2107 25.4142 28.5858C25.7893 28.9609 26 29.4696 26 30C26 30.5304 25.7893 31.0391 25.4142 31.4142C25.0391 31.7893 24.5304 32 24 32C23.4696 32 22.9609 31.7893 22.5858 31.4142C22.2107 31.0391 22 30.5304 22 30Z" fill="white"/>
      </g>
      <defs>
      <clipPath id="clip0_2986_4833">
      <rect width="32" height="32" fill="white" transform="translate(8 8)"/>
      </clipPath>
      </defs>
      </svg>
      </div>
     `;
  }
  return icon;
};
const generateRowFolderView = (): void => {
  folders.forEach((folder) => {
    parentContainer.innerHTML += `<div  
    class=" w-full h-[100px] mx-1 my-2 flex justify-between items-center px-2 py-3 border-gray-200 border-solid border-[1px] shadow-sm rounded-lg">
    <div class="flex justify-start">
        <div onclick="selectFolder('${
          folder.name
        }')" class="cursor-pointer w-[76px] h-[75px] bg-orange-200 flex items-center justify-center rounded-lg">
            <svg width="38" height="38" viewBox="0 0 58 58" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path
                    d="M7.25 16.9173V41.084C7.25 43.7534 9.41396 45.9173 12.0833 45.9173H45.9167C48.586 45.9173 50.75 43.7534 50.75 41.084V21.7507C50.75 19.0813 48.586 16.9173 45.9167 16.9173H31.4167L26.5833 12.084H12.0833C9.41396 12.084 7.25 14.2479 7.25 16.9173Z"
                    stroke="#FB923C" stroke-width="4.83333" stroke-linecap="round" stroke-linejoin="round" />
            </svg>
        </div>
        <div class="flex flex-col mx-3">
            <div class="text-sm font-bold cursor-pointer" onclick="selectFolder('${
              folder.name
            }')">${folder.name}</div>
            <div class="mt-2">
                <svg class="inline" width="18" height="18" viewBox="0 0 18 18" fill="none"
                    xmlns="http://www.w3.org/2000/svg">
                    <path
                        d="M6 5.25V2.25M12 5.25V2.25M5.25 8.25H12.75M3.75 15.75H14.25C15.0784 15.75 15.75 15.0784 15.75 14.25V5.25C15.75 4.42157 15.0784 3.75 14.25 3.75H3.75C2.92157 3.75 2.25 4.42157 2.25 5.25V14.25C2.25 15.0784 2.92157 15.75 3.75 15.75Z"
                        stroke="#6B7280" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                </svg>
                <span class="text-xs font-light">${moment(folder.lastModified)
                  .endOf("jMonth")
                  .format("HH:mm jYYYY/jM/jD ")} </span>
            </div>
        </div>
       </div>
       <div class="mb-auto cursor-pointer " onclick="selectAtciveItem('${
         folder.name
       }','folder','removeSheetTrigger')">
       <svg width="15" height="15" viewBox="0 0 15 15" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M3.125 4.375L3.66708 11.9641C3.7138 12.6182 4.2581 13.125 4.9139 13.125H10.0861C10.7419 13.125 11.2862 12.6182 11.3329 11.9641L11.875 4.375M8.75 6.875V10.625M6.25 6.875V10.625M5.625 4.375V2.5C5.625 2.15482 5.90482 1.875 6.25 1.875H8.75C9.09518 1.875 9.375 2.15482 9.375 2.5V4.375M12.5 4.375H2.5" stroke="#DC2626" stroke-width="1.25" stroke-linecap="round" stroke-linejoin="round"/>
       </svg>
      </div>
    </div>`;
  });
};
const generateRowFileView = (): void => {
  files.forEach((file) => {
    if (file.fileType.toLowerCase().includes("image")) {
      parentContainer.innerHTML += `
      <div
      class="  w-full h-[100px] mx-1 my-2 flex justify-between items-center px-2 py-3 border-gray-200 border-solid border-[1px] shadow-sm rounded-lg">
      <div class="flex justify-start">
          <a href="${file.fileUrl}" target="_blank">
          <img src="${file.fileUrl}"
              class="w-[76px] h-[75px]  rounded-lg object-cover" />
          </a>

          <div class="flex flex-col mx-3">
              <a class="text-sm font-bold " href="${
                file.fileUrl
              }" target="_blank">
              ${
                file.name.length > 100
                  ? file.name.slice(0, 100) + "..."
                  : file.name
              }</a>
              <div class="my-2 text-xs font-light">
                  <span>حجم:</span><span>
                  ${
                    file.length < 1000
                      ? file.length
                      : file.length >= 1000 && file.length <= 999999
                      ? (file.length / 1000).toFixed(2)
                      : (file.length / 1000000).toFixed(2)
                  }
                  ${
                    file.length < 1000
                      ? "بایت"
                      : file.length >= 1000 && file.length <= 999999
                      ? "کیلوبایت"
                      : "مگابایت"
                  }</span>
              </div>
              <div>
                  <svg class="inline" width="18" height="18" viewBox="0 0 18 18" fill="none"
                      xmlns="http://www.w3.org/2000/svg">
                      <path
                          d="M6 5.25V2.25M12 5.25V2.25M5.25 8.25H12.75M3.75 15.75H14.25C15.0784 15.75 15.75 15.0784 15.75 14.25V5.25C15.75 4.42157 15.0784 3.75 14.25 3.75H3.75C2.92157 3.75 2.25 4.42157 2.25 5.25V14.25C2.25 15.0784 2.92157 15.75 3.75 15.75Z"
                          stroke="#6B7280" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                  </svg>
                  <span class="text-xs font-light">${moment(file.lastModified)
                    .endOf("jMonth")
                    .format("HH:mm jYYYY/jM/jD ")} </span>
              </div>
          </div>
      </div>
      <div class="h-full flex flex-col justify-between">
      <svg class="cursor-pointer mt-1" width="15" height="15" viewBox="0 0 15 15" fill="none"
      onclick="selectAtciveItem('${file.name}','file','removeSheetTrigger')"
          xmlns="http://www.w3.org/2000/svg">
          <path
              d="M3.125 4.375L3.66708 11.9641C3.7138 12.6182 4.2581 13.125 4.9139 13.125H10.0861C10.7419 13.125 11.2862 12.6182 11.3329 11.9641L11.875 4.375M8.75 6.875V10.625M6.25 6.875V10.625M5.625 4.375V2.5C5.625 2.15482 5.90482 1.875 6.25 1.875H8.75C9.09518 1.875 9.375 2.15482 9.375 2.5V4.375M12.5 4.375H2.5"
              stroke="#DC2626" stroke-width="1.25" stroke-linecap="round" stroke-linejoin="round" />
      </svg>

      </div>
  </div>`;
    } else {
      let icon = getRowViewIcon(file.fileType.toLowerCase());
      parentContainer.innerHTML += `
      <div
      class="w-full h-[100px]  my-2 flex justify-between items-center px-2 py-3 border-gray-200 border-solid border-[1px] shadow-sm rounded-lg">
      <div class="flex justify-start">
        <a href=${file.fileUrl} target="_blank">${icon}</a>

        <div class="flex flex-col mx-3">
              <a href="${
                file.fileUrl
              }" target="_blank" class="text-sm font-bold ">${file.name}</a>
              <div class="my-2 text-xs font-light">
                  <span>حجم:</span><span>  ${
                    file.length < 1000
                      ? file.length
                      : file.length >= 1000 && file.length <= 999999
                      ? (file.length / 1000).toFixed(2)
                      : (file.length / 1000000).toFixed(2)
                  }
                  ${
                    file.length < 1000
                      ? "بایت"
                      : file.length >= 1000 && file.length <= 999999
                      ? "کیلوبایت"
                      : "مگابایت"
                  }</span>
              </div>
              <div>
                  <svg class="inline" width="18" height="18" viewBox="0 0 18 18" fill="none"
                      xmlns="http://www.w3.org/2000/svg">
                      <path
                          d="M6 5.25V2.25M12 5.25V2.25M5.25 8.25H12.75M3.75 15.75H14.25C15.0784 15.75 15.75 15.0784 15.75 14.25V5.25C15.75 4.42157 15.0784 3.75 14.25 3.75H3.75C2.92157 3.75 2.25 4.42157 2.25 5.25V14.25C2.25 15.0784 2.92157 15.75 3.75 15.75Z"
                          stroke="#6B7280" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                  </svg>
                  <span class="text-xs font-light">${moment(file.lastModified)
                    .endOf("jMonth")
                    .format("HH:mm jYYYY/jM/jD ")} </span>
              </div>
          </div>
      </div>
      <div class="h-full flex flex-col justify-between">
          <svg class="cursor-pointer mt-1" width="15" height="15" viewBox="0 0 15 15" fill="none"
          onclick="selectAtciveItem('${file.name}','file','removeSheetTrigger')"
              xmlns="http://www.w3.org/2000/svg">
              <path
                  d="M3.125 4.375L3.66708 11.9641C3.7138 12.6182 4.2581 13.125 4.9139 13.125H10.0861C10.7419 13.125 11.2862 12.6182 11.3329 11.9641L11.875 4.375M8.75 6.875V10.625M6.25 6.875V10.625M5.625 4.375V2.5C5.625 2.15482 5.90482 1.875 6.25 1.875H8.75C9.09518 1.875 9.375 2.15482 9.375 2.5V4.375M12.5 4.375H2.5"
                  stroke="#DC2626" stroke-width="1.25" stroke-linecap="round" stroke-linejoin="round" />
          </svg>
     
      </div>
  </div>`;
    }
    //     <svg class="cursor-pointer" width="13" height="13" viewBox="0 0 13 13" fill="none"
    //     xmlns="http://www.w3.org/2000/svg">
    //     <path
    //         d="M8.50528 7.38506C8.37973 7.13447 8.30908 6.85161 8.30908 6.55225C8.30908 6.25289 8.37973 5.97003 8.50528 5.71943M8.50528 7.38506C8.81094 7.99521 9.44201 8.41406 10.1709 8.41406C11.1992 8.41406 12.0327 7.5805 12.0327 6.55225C12.0327 5.52399 11.1992 4.69043 10.1709 4.69043C9.44201 4.69043 8.81094 5.10929 8.50528 5.71943M8.50528 7.38506L4.38926 9.44307M8.50528 5.71943L4.38926 3.66142M4.38926 3.66142C4.08359 4.27157 3.45252 4.69043 2.72363 4.69043C1.69538 4.69043 0.861816 3.85687 0.861816 2.82861C0.861816 1.80036 1.69538 0.966797 2.72363 0.966797C3.75189 0.966797 4.58545 1.80036 4.58545 2.82861C4.58545 3.12797 4.5148 3.41083 4.38926 3.66142ZM4.38926 9.44307C4.5148 9.69366 4.58545 9.97652 4.58545 10.2759C4.58545 11.3041 3.75189 12.1377 2.72363 12.1377C1.69538 12.1377 0.861816 11.3041 0.861816 10.2759C0.861816 9.24763 1.69538 8.41406 2.72363 8.41406C3.45252 8.41406 4.08359 8.83292 4.38926 9.44307Z"
    //         stroke="#4B5563" stroke-width="1.24121" stroke-linecap="round" stroke-linejoin="round" />
    // </svg>
  });
  if (files.length === 0 && folders.length === 0) {
    parentContainer.innerHTML = ``;
  }
};
const getRowViewIcon = (type: string): string => {
  let icon: string;
  if (type.includes("zip")) {
    icon = `
    <svg width="75" height="75" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <rect x="1" y="1" width="46" height="46" rx="6" fill="#4C1D95"/>
    <path d="M20.6186 26.7V29H11.4386V27.46L16.9786 19.22H11.7186V16.76H20.5386V18.54L15.0586 26.7H20.6186ZM25.398 16.76V29H22.658V16.76H25.398ZM33.3728 25.38H30.6128V29H27.8728V16.76H33.4328C34.7661 16.76 35.7661 17.16 36.4328 17.96C37.1128 18.76 37.4528 19.8133 37.4528 21.12C37.4528 21.9467 37.2861 22.6867 36.9528 23.34C36.6195 23.98 36.1461 24.48 35.5328 24.84C34.9195 25.2 34.1995 25.38 33.3728 25.38ZM34.2528 19.64C33.9461 19.3067 33.4928 19.14 32.8928 19.14H30.6128V23.02H32.7928C33.4061 23.02 33.8795 22.8533 34.2128 22.52C34.5461 22.1733 34.7128 21.7067 34.7128 21.12C34.7128 20.4667 34.5595 19.9733 34.2528 19.64Z" fill="white"/>
    </svg>
`;
  } else if (type.includes("pdf")) {
    icon = `<svg width="75" height="75" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <rect width="48" height="48" rx="6" fill="#DC2626"/>
    <path d="M13.99 25.38H11.23V29H8.49V16.76H14.05C15.3833 16.76 16.3833 17.16 17.05 17.96C17.73 18.76 18.07 19.8133 18.07 21.12C18.07 21.9467 17.9033 22.6867 17.57 23.34C17.2367 23.98 16.7633 24.48 16.15 24.84C15.5367 25.2 14.8167 25.38 13.99 25.38ZM14.87 19.64C14.5633 19.3067 14.11 19.14 13.51 19.14H11.23V23.02H13.41C14.0233 23.02 14.4967 22.8533 14.83 22.52C15.1633 22.1733 15.33 21.7067 15.33 21.12C15.33 20.4667 15.1767 19.9733 14.87 19.64ZM28.7972 27.54C27.9172 28.5133 26.5772 29 24.7772 29H19.8572V16.76H24.7772C26.5639 16.76 27.8972 17.2533 28.7772 18.24C29.6705 19.2133 30.1172 20.7867 30.1172 22.96C30.1172 25.04 29.6772 26.5667 28.7972 27.54ZM26.2172 19.62C25.7905 19.3533 25.1839 19.22 24.3972 19.22H22.5972V26.7H24.3972C25.0905 26.7 25.6439 26.6 26.0572 26.4C26.4839 26.1867 26.8039 25.8133 27.0172 25.28C27.2439 24.7333 27.3572 23.96 27.3572 22.96C27.3572 22.0533 27.2705 21.3333 27.0972 20.8C26.9372 20.2667 26.6439 19.8733 26.2172 19.62ZM34.9409 24.02V29H32.2009V16.76H40.4809V18.96H34.9409V21.74H39.8009V24.02H34.9409Z" fill="white"/>
    </svg>
    `;
  } else if (type.includes("video") || type.includes("mp4")) {
    icon = `
    <svg width="75" height="75" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <rect width="48" height="48" rx="6" fill="#7C3AED"/>
    <path fill-rule="evenodd" clip-rule="evenodd" d="M24 36.8002C31.0693 36.8002 36.8 31.0694 36.8 24.0002C36.8 16.931 31.0693 11.2002 24 11.2002C16.9308 11.2002 11.2 16.931 11.2 24.0002C11.2 31.0694 16.9308 36.8002 24 36.8002ZM23.2875 19.4689C22.7966 19.1416 22.1653 19.1111 21.645 19.3895C21.1248 19.6679 20.8 20.2101 20.8 20.8002V27.2002C20.8 27.7903 21.1248 28.3324 21.645 28.6109C22.1653 28.8893 22.7966 28.8588 23.2875 28.5315L28.0875 25.3315C28.5326 25.0347 28.8 24.5352 28.8 24.0002C28.8 23.4652 28.5326 22.9657 28.0875 22.6689L23.2875 19.4689Z" fill="white"/>
    </svg>
`;
  } else if (type.includes("audio")) {
    icon = `<svg width="75" height="75" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <rect width="48" height="48" rx="6" fill="#F97316"/>
    <path d="M28.714 19.2863C31.3175 21.8898 31.3175 26.1109 28.714 28.7144M32.4854 15.515C37.1716 20.2013 37.1716 27.7992 32.4854 32.4855M15.4477 28.0004H13.3333C12.597 28.0004 12 27.4034 12 26.6671V21.3337C12 20.5973 12.597 20.0004 13.3333 20.0004H15.4477L21.7239 13.7243C22.5638 12.8843 24 13.4792 24 14.6671V33.3337C24 34.5216 22.5638 35.1165 21.7239 34.2765L15.4477 28.0004Z" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    </svg>
    `;
  } else if (type.includes("word")) {
    icon = `<svg width="75" height="75" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
   <rect width="48" height="48" rx="6" fill="#1D4ED8"/>
   <path d="M11.6899 23.474L10.1779 28.5H8.42788L6.53788 19.932H8.41388L9.53388 25.182L10.7099 21.108V19.932H12.2499L13.7059 25.182L14.8539 19.932H16.7159L14.8399 28.5H13.0899L11.6899 23.474ZM25.5371 24.258C25.5371 27.1793 24.2117 28.64 21.5611 28.64C18.9011 28.64 17.5711 27.1793 17.5711 24.258C17.5711 22.6993 17.9024 21.57 18.5651 20.87C19.2277 20.1607 20.2264 19.806 21.5611 19.806C22.8957 19.806 23.8897 20.1607 24.5431 20.87C25.2057 21.57 25.5371 22.6993 25.5371 24.258ZM20.0071 26.232C20.3431 26.652 20.8611 26.862 21.5611 26.862C22.2611 26.862 22.7744 26.652 23.1011 26.232C23.4371 25.8027 23.6051 25.1447 23.6051 24.258C23.6051 23.2967 23.4417 22.6107 23.1151 22.2C22.7884 21.78 22.2704 21.57 21.5611 21.57C20.8517 21.57 20.3337 21.78 20.0071 22.2C19.6804 22.6107 19.5171 23.2967 19.5171 24.258C19.5171 25.1447 19.6804 25.8027 20.0071 26.232ZM29.8943 25.756H28.9423V28.5H27.0243V19.932H30.9863C31.9009 19.932 32.5869 20.1933 33.0443 20.716C33.5016 21.2387 33.7303 21.948 33.7303 22.844C33.7303 23.4507 33.5903 23.9687 33.3103 24.398C33.0303 24.8273 32.6383 25.1493 32.1343 25.364C32.2649 25.4293 32.3629 25.5133 32.4283 25.616C32.5029 25.7093 32.5729 25.84 32.6383 26.008L33.6743 28.5H31.6723L30.7763 26.288C30.6923 26.0827 30.5896 25.9427 30.4683 25.868C30.3563 25.7933 30.1649 25.756 29.8943 25.756ZM31.8123 22.844C31.8123 22.0133 31.4109 21.598 30.6083 21.598H28.9423V24.09H30.4963C31.3736 24.09 31.8123 23.6747 31.8123 22.844ZM41.4854 27.478C40.8694 28.1593 39.9314 28.5 38.6714 28.5H35.2274V19.932H38.6714C39.922 19.932 40.8554 20.2773 41.4714 20.968C42.0967 21.6493 42.4094 22.7507 42.4094 24.272C42.4094 25.728 42.1014 26.7967 41.4854 27.478ZM39.6794 21.934C39.3807 21.7473 38.956 21.654 38.4054 21.654H37.1454V26.89H38.4054C38.8907 26.89 39.278 26.82 39.5674 26.68C39.866 26.5307 40.09 26.2693 40.2394 25.896C40.398 25.5133 40.4774 24.972 40.4774 24.272C40.4774 23.6373 40.4167 23.1333 40.2954 22.76C40.1834 22.3867 39.978 22.1113 39.6794 21.934Z" fill="white"/>
   </svg>
   `;
  } else if (type.includes("excel")) {
    icon = `<svg width="75" height="75" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <rect width="48" height="48" rx="6" fill="#10B981"/>
    <path d="M13.9114 25.26L11.3514 29H8.07141L12.2514 22.82L8.21141 16.76H11.4314L13.9114 20.46L16.3714 16.76H19.5914L15.5314 22.82L19.7314 29H16.4714L13.9114 25.26ZM29.9131 26.7V29H21.6931V16.76H24.4331V26.7H29.9131ZM38.8922 28.26C38.0922 28.8867 36.9189 29.2 35.3722 29.2C34.6255 29.2 33.9255 29.1333 33.2722 29C32.6189 28.88 32.0589 28.7067 31.5922 28.48V26.2C32.1389 26.44 32.7189 26.64 33.3322 26.8C33.9455 26.9467 34.5322 27.02 35.0922 27.02C36.5722 27.02 37.3122 26.5467 37.3122 25.6C37.3122 25.16 37.1122 24.82 36.7122 24.58C36.3122 24.3267 35.6055 24.0467 34.5922 23.74C33.7789 23.4867 33.1389 23.2067 32.6722 22.9C32.2189 22.5933 31.8855 22.2267 31.6722 21.8C31.4722 21.36 31.3722 20.8133 31.3722 20.16C31.3722 19 31.7589 18.1133 32.5322 17.5C33.3055 16.8867 34.4655 16.58 36.0122 16.58C36.5722 16.58 37.1855 16.6333 37.8522 16.74C38.5189 16.8467 39.0522 16.9667 39.4522 17.1V19.4C38.4922 18.9733 37.4989 18.76 36.4722 18.76C35.6989 18.76 35.1189 18.8733 34.7322 19.1C34.3455 19.3133 34.1522 19.6533 34.1522 20.12C34.1522 20.5067 34.3189 20.8067 34.6522 21.02C34.9989 21.2333 35.6455 21.4667 36.5922 21.72C37.5122 21.96 38.2255 22.2533 38.7322 22.6C39.2522 22.9467 39.6055 23.36 39.7922 23.84C39.9922 24.3067 40.0922 24.8867 40.0922 25.58C40.0922 26.74 39.6922 27.6333 38.8922 28.26Z" fill="white"/>
    </svg>
    `;
  } else {
    icon = `
    <svg width="75" height="75" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <rect width="48" height="48" rx="6" fill="#9CA3AF"/>
    <g clip-path="url(#clip0_2986_4833)">
    <path d="M24 40C28.2435 40 32.3131 38.3143 35.3137 35.3137C38.3143 32.3131 40 28.2435 40 24C40 19.7565 38.3143 15.6869 35.3137 12.6863C32.3131 9.68571 28.2435 8 24 8C19.7565 8 15.6869 9.68571 12.6863 12.6863C9.68571 15.6869 8 19.7565 8 24C8 28.2435 9.68571 32.3131 12.6863 35.3137C15.6869 38.3143 19.7565 40 24 40ZM18.6125 18.3313C19.1062 16.9375 20.4313 16 21.9125 16H25.5562C27.7375 16 29.5 17.7688 29.5 19.9438C29.5 21.3563 28.7437 22.6625 27.5187 23.3687L25.5 24.525C25.4875 25.3375 24.8188 26 24 26C23.1687 26 22.5 25.3312 22.5 24.5V23.6562C22.5 23.1187 22.7875 22.625 23.2563 22.3562L26.025 20.7688C26.3188 20.6 26.5 20.2875 26.5 19.95C26.5 19.425 26.075 19.0063 25.5562 19.0063H21.9125C21.7 19.0063 21.5125 19.1375 21.4438 19.3375L21.4187 19.4125C21.1438 20.1938 20.2812 20.6 19.5063 20.325C18.7313 20.05 18.3188 19.1875 18.5938 18.4125L18.6187 18.3375L18.6125 18.3313ZM22 30C22 29.4696 22.2107 28.9609 22.5858 28.5858C22.9609 28.2107 23.4696 28 24 28C24.5304 28 25.0391 28.2107 25.4142 28.5858C25.7893 28.9609 26 29.4696 26 30C26 30.5304 25.7893 31.0391 25.4142 31.4142C25.0391 31.7893 24.5304 32 24 32C23.4696 32 22.9609 31.7893 22.5858 31.4142C22.2107 31.0391 22 30.5304 22 30Z" fill="white"/>
    </g>
    <defs>
    <clipPath id="clip0_2986_4833">
    <rect width="32" height="32" fill="white" transform="translate(8 8)"/>
    </clipPath>
    </defs>
    </svg>
    
   `;
  }
  return icon;
};
const changeView = (
  type: boolean,
  svg: HTMLElement,
  sibling: HTMLElement
): void => {
  Array.from(
    svg.querySelectorAll("path") as NodeListOf<SVGPathElement>
  ).forEach((item) => {
    item.classList.add("active__view");
  });
  const targetSibling: HTMLElement = document.querySelector(`#${sibling}`);
  Array.from(
    targetSibling.querySelectorAll("path") as NodeListOf<SVGPathElement>
  ).forEach((item) => {
    item.classList.remove("active__view");
  });

  viewType = type;
  updateView();
};
const updateView = (): void => {
  parentContainer.innerHTML = "";
  if (files.length === 0 && folders.length === 0) {
    document.querySelector("#no__file__found").classList.remove("hidden");
  } else {
    document.querySelector("#no__file__found").classList.add("hidden");

    if (viewType) {
      generateBoxFolderView();
      generateBoxFileView();
    } else {
      generateRowFolderView();
      generateRowFileView();
    }
  }
  generateBreadCrumb();
};
const generateBreadCrumb = () => {
  let breadCrumb: string[] = fileControls.filePath.split("/");
  breadCrumbContainer.innerHTML = `
  <div onclick="changeLocation('/')" class="text-xs font-light text-gray-600 cursor-pointer">خانه</div>
  `;
  let templocation = "/";
  breadCrumb
    .filter((crumb) => crumb !== "")
    .forEach((crumb) => {
      templocation += "/" + crumb;
      templocation.replace("//", "/");
      breadCrumbContainer.innerHTML += `
   <div class="mx-0.5 mt-0.5" >
   <svg width="10" height="10" viewBox="0 0 10 10" fill="none" xmlns="http://www.w3.org/2000/svg">
     <path d="M6.25 7.91732L3.33333 5.00065L6.25 2.08398" stroke="#6B7280" stroke-width="0.833333"
         stroke-linecap="round" stroke-linejoin="round" />
   </svg>
   </div>
   <div onclick="changeLocation('${templocation}')" class="text-xs font-light text-gray-600 cursor-pointer">${crumb}</div>  
   `;
    });
};
const loadMore = async () => {
  if (
    window.innerHeight + window.scrollY ===
    document.documentElement.scrollHeight
  ) {
    pageLoading.classList.remove("hidden");
    fileControls.pager.pageNumber += 1;
    getPageFiles().then(() => {
      updateView();
      pageLoading.classList.add("hidden");
    });
  }
};

const selectAtciveItem = (
  itemName: string,
  type: "folder" | "file",
  btnId: string
): void => {
  activeItem = {
    path:
      fileControls.filePath === "/"
        ? itemName
        : fileControls.filePath + itemName,
    type: type,
  };
  let btn: HTMLButtonElement = document.querySelector(`#${btnId}`);
  if (type === "file") {
    document.querySelector("#fileNameContainer").innerHTML = itemName;
  }
  btn.click();
};
const confirmRemove = async () => {
  document.querySelector("#removeText").classList.toggle("hidden");
  document.querySelector("#removeLoading").classList.toggle("hidden");
  try {
    if (activeItem.type === "folder") {
      await removeFolder([activeItem.path]);
    } else {
      await removeFile([activeItem.path]);
    }
    files = [];
    folders = [];
    fileControls.pager.pageNumber = 1;
    await getPageFolders();
    await getPageFiles();
    updateView();
  } catch (ex) {
    console.log(ex);
  }
  document.querySelector("#removeText").classList.toggle("hidden");
  document.querySelector("#removeLoading").classList.toggle("hidden");
  closeSheets();
};
const closeSheets = () => {
  let sheets: NodeListOf<HTMLElement> = document.querySelectorAll(".overlay");
  Array.from(sheets).forEach((item) => {
    item.click();
  });
};

const selectFolder = async (folderName?: string) => {
  let chosenPath = folderName
    ? fileControls.filePath + folderName
    : activeItem.path;
  fileControls.filePath = chosenPath + "/";
  fileControls.pager.pageNumber = 1;
  fileControls.searchText = "";
  files = [];
  folders = [];

  await getPageFiles();
  await getPageFolders();
  updateView();
  closeSheets();
};
const openFile = () => {
  let anchor = document.createElement("a");
  anchor.href = "/" + activeItem.path;
  anchor.setAttribute("target", "_blank");
  anchor.click();
};
const changeLocation = async (location: string) => {
  fileControls.filePath = location;
  fileControls.pager.pageNumber = 1;
  fileControls.searchText = "";
  files = [];
  folders = [];
  await getPageFiles();
  await getPageFolders();
  updateView();
};
const createPageFolder = async (folderInputId: string) => {
  let folderInput: HTMLInputElement = document.querySelector(
    `.${folderInputId}`
  );
  if (!folderInput.value || folderInput.value === "") {
    return;
  } else {
    try {
      await createFolder(fileControls.filePath + folderInput.value);
      files = [];
      folders = [];
      folderInput.value = "";
      fileControls.pager.pageNumber = 1;
      await getPageFolders();
      await getPageFiles();
      updateView();
      closeSheets();
    } catch (ex) {}
  }
};

const initFMUploader = (input: HTMLInputElement) => {
  uploadedFiles = Array.from(input.files);
  uploaderInput = input;
  generateFMUploadPreview();
};
const generateFMUploadPreview = () => {
  uploadButton.disabled = true;
  previewParentEl.innerHTML = null;
  uploadedFiles.forEach((file, index) => {
    let uniqueId = "file__" + uuidv4();
    if (file.type.toLowerCase().includes("image")) {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = (e) => {
        previewParentEl.innerHTML += `
    <div
    id="${uniqueId}"
    data-file-name="${file.name}"
    class="relative bg-white w-full h-[100px]   mb-4 flex justify-between items-center px-2 py-3 border-gray-200 border-solid border-[1px] shadow-sm rounded-lg">
    <div class="flex justify-start">

        <img src="${e.target.result}"
            class="w-[76px] h-[75px]  rounded-lg object-cover fm__preview__container" />
        <div id="preview__overlay" class="hidden z-[3] bg-gray-700 absolute top-0 right-0 h-full w-full opacity-70 rounded-md">

            <svg aria-hidden="true" id="preview__loading"
                class="hidden  absolute bottom-2 right-1 w-6 h-6 mr-2 text-transparent animate-spin dark:text-gray-600 fill-white"
                viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path
                    d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z"
                    fill="currentColor" />
                <path
                    d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z"
                    fill="currentFill" />
            </svg>
            <div class="hidden absolute bottom-1 left-2 text-white" id="percentage__view">0%</div>
        </div>
        <div id="successUpload" class="hidden flex flex-col justify-between items-center z-10 absolute bottom-2.5 right-[38%]">
            <svg class="mb-2.5" width="40" height="40" viewBox="0 0 40 40" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M8.33325 21.666L14.9999 28.3327L31.6666 11.666" stroke="white"
                    stroke-width="3.33333" stroke-linecap="round" stroke-linejoin="round" />
            </svg>

            <div class="text-white font-normal text-xs">بارگزاری موفق آمیز بود</div>
        </div>
        <div id="progress__bar" dir="ltr"
            class=" hidden z-[3]  absolute bottom-0 right-0 w-full bg-gray-200 rounded-full h-1.5 dark:bg-gray-700">
            <div class="bg-green-500 h-1.5 rounded-full" style="width: 33%">
            </div>
        </div>
        <div class="hidden flex justify-between items-center z-[4] w-full absolute bottom-2.5 px-2"
            id="retry__button" >
            <div class="text-white font-normal text-xs">متاسفانه بارگزاری با مشکل مواجه شد</div>
            <div class="flex">
                <svg onclick="handleRetry('${file.name}',${
          file.size
        },'${uniqueId}')" class="cursor-pointer" width="24" height="24" viewBox="0 0 24 24" fill="none"
                    xmlns="http://www.w3.org/2000/svg">
                    <mask id="mask0_1250_32739" style="mask-type:alpha" maskUnits="userSpaceOnUse" x="0"
                        y="0" width="24" height="24">
                        <rect width="24" height="24" fill="#D9D9D9" />
                    </mask>
                    <g mask="url(#mask0_1250_32739)">
                        <path
                            d="M12 22C10.75 22 9.57933 21.7627 8.488 21.288C7.396 20.8127 6.446 20.1707 5.638 19.362C4.82933 18.554 4.18733 17.604 3.712 16.512C3.23733 15.4207 3 14.25 3 13H5C5 14.95 5.67933 16.604 7.038 17.962C8.396 19.3207 10.05 20 12 20C13.95 20 15.604 19.3207 16.962 17.962C18.3207 16.604 19 14.95 19 13C19 11.05 18.3207 9.39567 16.962 8.037C15.604 6.679 13.95 6 12 6H11.85L13.4 7.55L12 9L8 5L12 1L13.4 2.45L11.85 4H12C13.25 4 14.421 4.23767 15.513 4.713C16.6043 5.18767 17.5543 5.829 18.363 6.637C19.171 7.44567 19.8127 8.39567 20.288 9.487C20.7627 10.579 21 11.75 21 13C21 14.25 20.7627 15.4207 20.288 16.512C19.8127 17.604 19.171 18.554 18.363 19.362C17.5543 20.1707 16.6043 20.8127 15.513 21.288C14.421 21.7627 13.25 22 12 22Z"
                            fill="white" />
                    </g>
                </svg>
                <svg class=" cursor-pointer mx-2 " width="24" height="24" viewBox="0 0 25 25" fill="none"
                    xmlns="http://www.w3.org/2000/svg">
                    <circle cx="12.5" cy="12.5" r="12.5" fill="#DC2626" />
                    <path
                        d="M18.25 9.25L17.5995 18.3569C17.5434 19.1418 16.8903 19.75 16.1033 19.75H9.89668C9.10972 19.75 8.45656 19.1418 8.40049 18.3569L7.75 9.25M11.5 12.25V16.75M14.5 12.25V16.75M15.25 9.25V7C15.25 6.58579 14.9142 6.25 14.5 6.25H11.5C11.0858 6.25 10.75 6.58579 10.75 7V9.25M7 9.25H19"
                        stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                </svg>
            </div>
        </div>


        <div class="flex flex-col mx-3">
            <div class="text-sm font-bold " >
            ${
              file.name.length > 20 ? file.name.slice(0, 20) + "..." : file.name
            }</div>
            <div class="my-2 text-xs font-light">
            <span>حجم:</span><span>  ${
              file.size < 1000
                ? file.size
                : file.size >= 1000 && file.size <= 999999
                ? (file.size / 1000).toFixed(2)
                : (file.size / 1000000).toFixed(2)
            }
            ${
              file.size < 1000
                ? "بایت"
                : file.size >= 1000 && file.size <= 999999
                ? "کیلوبایت"
                : "مگابایت"
            }</span>
            </div>

        </div>
    </div>
    <div class="h-full flex flex-col justify-between">
        <svg onclick="handlePreUploadRemove('${file.name}','${
          file.size
        }','${uniqueId}')" class="cursor-pointer mt-1" width="15" height="15" viewBox="0 0 15 15" fill="none"
            xmlns="http://www.w3.org/2000/svg">
            <path
                d="M3.125 4.375L3.66708 11.9641C3.7138 12.6182 4.2581 13.125 4.9139 13.125H10.0861C10.7419 13.125 11.2862 12.6182 11.3329 11.9641L11.875 4.375M8.75 6.875V10.625M6.25 6.875V10.625M5.625 4.375V2.5C5.625 2.15482 5.90482 1.875 6.25 1.875H8.75C9.09518 1.875 9.375 2.15482 9.375 2.5V4.375M12.5 4.375H2.5"
                stroke="#DC2626" stroke-width="1.25" stroke-linecap="round" stroke-linejoin="round" />
        </svg>
        <svg class="cursor-pointer" onclick="initFMCropper('${file.name}',${
          file.size
        },'${uniqueId}')" width="15" height="15" viewBox="0 0 15 15" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path
                d="M7.44727 3.8291V2.58789M7.44727 3.8291C8.13277 3.8291 8.68848 4.38481 8.68848 5.07031C8.68848 5.75581 8.13277 6.31152 7.44727 6.31152M7.44727 3.8291C6.76176 3.8291 6.20605 4.38481 6.20605 5.07031C6.20605 5.75581 6.76176 6.31152 7.44727 6.31152M11.1709 11.2764C10.4854 11.2764 9.92969 10.7207 9.92969 10.0352C9.92969 9.34965 10.4854 8.79395 11.1709 8.79395M11.1709 11.2764C11.8564 11.2764 12.4121 10.7207 12.4121 10.0352C12.4121 9.34965 11.8564 8.79395 11.1709 8.79395M11.1709 11.2764V12.5176M11.1709 8.79395V2.58789M7.44727 6.31152V12.5176M3.72363 11.2764C3.03813 11.2764 2.48242 10.7207 2.48242 10.0352C2.48242 9.34965 3.03813 8.79395 3.72363 8.79395M3.72363 11.2764C4.40913 11.2764 4.96484 10.7207 4.96484 10.0352C4.96484 9.34965 4.40913 8.79395 3.72363 8.79395M3.72363 11.2764V12.5176M3.72363 8.79395V2.58789"
                stroke="#4B5563" stroke-width="1.24121" stroke-linecap="round" stroke-linejoin="round" />
        </svg>

    </div>
</div>`;
      };
    } else {
      let icon = getRowViewIcon(file.type);
      previewParentEl.innerHTML += `
      <div
      id="${uniqueId}"
      data-file-name="${file.name}"
      class="relative bg-white w-full h-[100px]   mb-4 flex justify-between items-center px-2 py-3 border-gray-200 border-solid border-[1px] shadow-sm rounded-lg">
      <div class="flex justify-start">
             ${icon}
          <div id="preview__overlay" class="hidden z-[3] bg-gray-700 absolute top-0 right-0 h-full w-full opacity-70 rounded-md">
  
              <svg aria-hidden="true" id="preview__loading"
                  class="hidden  absolute bottom-2 right-1 w-6 h-6 mr-2 text-transparent animate-spin dark:text-gray-600 fill-white"
                  viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <path
                      d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z"
                      fill="currentColor" />
                  <path
                      d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z"
                      fill="currentFill" />
              </svg>
              <div class="hidden absolute bottom-1 left-2 text-white" id="percentage__view">0%</div>
          </div>
          <div id="successUpload" class="hidden flex flex-col justify-between items-center z-10 absolute bottom-2.5 right-[38%]">
              <svg class="mb-2.5" width="40" height="40" viewBox="0 0 40 40" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <path d="M8.33325 21.666L14.9999 28.3327L31.6666 11.666" stroke="white"
                      stroke-width="3.33333" stroke-linecap="round" stroke-linejoin="round" />
              </svg>
  
              <div class="text-white font-normal text-xs">بارگزاری موفق آمیز بود</div>
          </div>
          <div id="progress__bar" dir="ltr"
              class=" hidden z-[3]  absolute bottom-0 right-0 w-full bg-gray-200 rounded-full h-1.5 dark:bg-gray-700">
              <div class="bg-green-500 h-1.5 rounded-full" style="width: 33%">
              </div>
          </div>
          <div class="hidden flex justify-between items-center z-[4] w-full absolute bottom-2.5 px-2"
              id="retry__button">
              <div class="text-white font-normal text-xs">متاسفانه بارگزاری با مشکل مواجه شد</div>
              <div class="flex">
                  <svg onclick="handleRetry('${file.name}',${
        file.size
      },'${uniqueId}')" class="  cursor-pointer  " width="24" height="24" viewBox="0 0 24 24" fill="none"
                      xmlns="http://www.w3.org/2000/svg">
                      <mask id="mask0_1250_32739" style="mask-type:alpha" maskUnits="userSpaceOnUse" x="0"
                          y="0" width="24" height="24">
                          <rect width="24" height="24" fill="#D9D9D9" />
                      </mask>
                      <g mask="url(#mask0_1250_32739)">
                          <path
                              d="M12 22C10.75 22 9.57933 21.7627 8.488 21.288C7.396 20.8127 6.446 20.1707 5.638 19.362C4.82933 18.554 4.18733 17.604 3.712 16.512C3.23733 15.4207 3 14.25 3 13H5C5 14.95 5.67933 16.604 7.038 17.962C8.396 19.3207 10.05 20 12 20C13.95 20 15.604 19.3207 16.962 17.962C18.3207 16.604 19 14.95 19 13C19 11.05 18.3207 9.39567 16.962 8.037C15.604 6.679 13.95 6 12 6H11.85L13.4 7.55L12 9L8 5L12 1L13.4 2.45L11.85 4H12C13.25 4 14.421 4.23767 15.513 4.713C16.6043 5.18767 17.5543 5.829 18.363 6.637C19.171 7.44567 19.8127 8.39567 20.288 9.487C20.7627 10.579 21 11.75 21 13C21 14.25 20.7627 15.4207 20.288 16.512C19.8127 17.604 19.171 18.554 18.363 19.362C17.5543 20.1707 16.6043 20.8127 15.513 21.288C14.421 21.7627 13.25 22 12 22Z"
                              fill="white" />
                      </g>
                  </svg>
                  <svg class=" cursor-pointer mx-2 " width="24" height="24" viewBox="0 0 25 25" fill="none"
                      xmlns="http://www.w3.org/2000/svg">
                      <circle cx="12.5" cy="12.5" r="12.5" fill="#DC2626" />
                      <path
                          d="M18.25 9.25L17.5995 18.3569C17.5434 19.1418 16.8903 19.75 16.1033 19.75H9.89668C9.10972 19.75 8.45656 19.1418 8.40049 18.3569L7.75 9.25M11.5 12.25V16.75M14.5 12.25V16.75M15.25 9.25V7C15.25 6.58579 14.9142 6.25 14.5 6.25H11.5C11.0858 6.25 10.75 6.58579 10.75 7V9.25M7 9.25H19"
                          stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                  </svg>
              </div>
          </div>
  
  
          <div class="flex flex-col mx-3">
          <div  class="text-sm font-bold " >
          ${
            file.name.length > 20 ? file.name.slice(0, 20) + "..." : file.name
          }</div>
              <div class="my-2 text-xs font-light">
              <span>حجم:</span><span>  ${
                file.size < 1000
                  ? file.size
                  : file.size >= 1000 && file.size <= 999999
                  ? (file.size / 1000).toFixed(2)
                  : (file.size / 1000000).toFixed(2)
              }
              ${
                file.size < 1000
                  ? "بایت"
                  : file.size >= 1000 && file.size <= 999999
                  ? "کیلوبایت"
                  : "مگابایت"
              }</span>
              </div>
  
          </div>
      </div>
      <div class="h-full flex flex-col justify-between">
          <svg onclick="handlePreUploadRemove('${file.name}','${
        file.size
      }','${uniqueId}')" class="cursor-pointer mt-1" width="15" height="15" viewBox="0 0 15 15" fill="none"
              xmlns="http://www.w3.org/2000/svg">
              <path
                  d="M3.125 4.375L3.66708 11.9641C3.7138 12.6182 4.2581 13.125 4.9139 13.125H10.0861C10.7419 13.125 11.2862 12.6182 11.3329 11.9641L11.875 4.375M8.75 6.875V10.625M6.25 6.875V10.625M5.625 4.375V2.5C5.625 2.15482 5.90482 1.875 6.25 1.875H8.75C9.09518 1.875 9.375 2.15482 9.375 2.5V4.375M12.5 4.375H2.5"
                  stroke="#DC2626" stroke-width="1.25" stroke-linecap="round" stroke-linejoin="round" />
          </svg>
          </div>
  </div>`;
    }
    if (index === uploadedFiles.length - 1) {
      uploadButton.disabled = false;
    }
  });
};
const handlePreUploadRemove = (
  fileName: string,
  fileSize: number,
  parentId: string
) => {
  uploadedFiles = uploadedFiles.filter(
    (file) => file.name !== fileName && file.size !== fileSize
  );
  document.querySelector(`#${parentId}`).remove();
};
const handleFMUpload = async () => {
  let fileMarkups: HTMLCollection = previewParentEl.children;

  uploadInitiator[0].classList.add("hidden");
  uploadInitiator[1].classList.add("hidden");
  uploadLoadings[0].classList.remove("hidden");
  uploadLoadings[1].classList.remove("hidden");
  uploadedFiles.forEach(async (file, index) => {
    let targetMarkup = Array.from(fileMarkups).find(
      (m) => m.getAttribute("data-file-name") === file.name
    );
    await uploadFile(
      [file],
      fileControls.filePath,
      (progress) => handleProgress(targetMarkup as HTMLElement, progress, file),
      () =>
        setTimeout(() => {
          handleError(targetMarkup as HTMLElement);
        }, 200)
    )
      .then(async (data) => {
        if (uploadedFiles.length === 0) {
          uploadInitiator[0].classList.remove("hidden");
          uploadInitiator[1].classList.remove("hidden");
          uploadLoadings[0].classList.add("hidden");
          uploadLoadings[1].classList.add("hidden");
          uploaderInput.value = null;
          closeSheets();
          files = [];
          folders = [];
          fileControls.pager.pageNumber = 1;
          await getPageFolders();
          await getPageFiles();
          updateView();
        }
      })
      .catch((ex) => {
        console.log(ex);
      });
  });
};
const handleProgress = (
  container: HTMLElement,
  progress: number,
  file: File
) => {
  let previewOverlay = container.querySelector(`#preview__overlay`);
  let retryBtns = container.querySelector("#retry__button");
  let previewLoading = container.querySelector("#preview__loading");
  let percentageContainer = container.querySelector("#percentage__view");
  let progressBarContainer = container.querySelector("#progress__bar");
  let successUpload = container.querySelector("#successUpload");
  previewOverlay.classList.remove("hidden");
  previewOverlay.classList.add("bg-gray-700");
  previewOverlay.classList.remove("bg-gradient-to-b");
  previewOverlay.classList.remove("from-red-500");
  previewOverlay.classList.remove("to-gray-800");
  previewLoading.classList.remove("hidden");
  retryBtns.classList.add("hidden");
  progressBarContainer.classList.remove("hidden");
  (progressBarContainer.children[0] as HTMLElement).style.width =
    progress + "%";
  percentageContainer.classList.remove("hidden");
  percentageContainer.innerHTML = `${progress}`;
  if (progress === 100) {
    uploadedFiles = uploadedFiles.filter(
      (f) => f.name !== file.name && f.size !== file.size
    );
    previewLoading.classList.add("hidden");
    successUpload.classList.remove("hidden");
    progressBarContainer.classList.add("hidden");
    percentageContainer.classList.add("hidden");
    previewOverlay.classList.remove("bg-gray-700");
    previewOverlay.classList.add("bg-gradient-to-b");
    previewOverlay.classList.add("from-green-600");
    previewLoading.classList.add("from-0%");
    previewOverlay.classList.add("to-gray-800");
    previewLoading.classList.add("to-50%");
  }
};
const handleError = (container: HTMLElement) => {
  let previewOverlay = container.querySelector(`#preview__overlay`);
  let previewLoading = container.querySelector("#preview__loading");
  let percentageContainer = container.querySelector("#percentage__view");
  let progressBarContainer = container.querySelector("#progress__bar");
  let retryBtns = container.querySelector("#retry__button");
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
const handleRetry = async (
  fileName: string,
  fileSize: number,
  parentId: string
) => {
  let targetMarkup = document.querySelector(`#${parentId}`);
  let file = uploadedFiles.find(
    (f) => f.name === fileName && f.size === fileSize
  );
  await uploadFile(
    [file],
    fileControls.filePath,
    (progress) => handleProgress(targetMarkup as HTMLElement, progress, file),
    () =>
      setTimeout(() => {
        handleError(targetMarkup as HTMLElement);
      }, 200)
  )
    .then(async (data) => {
      if (uploadedFiles.length === 0) {
        uploadInitiator[0].classList.remove("hidden");
        uploadInitiator[1].classList.remove("hidden");
        uploadLoadings[0].classList.add("hidden");
        uploadLoadings[1].classList.add("hidden");
        uploaderInput.value = null;
        closeSheets();
        files = [];
        folders = [];
        fileControls.pager.pageNumber = 1;
        await getPageFolders();
        await getPageFiles();
        updateView();
      }
    })
    .catch((ex) => {
      console.log(ex);
    });
};
const initFMCropper = (
  fileName: string,
  fileSize: number,
  holderId: string
) => {
  let initer: HTMLElement = document.querySelector("#cropper_initer");
  initer.click();
  let cropperParent: HTMLElement = document.querySelector(
    `#fm__cropperjs__container`
  );

  let file = uploadedFiles.find(
    (a) => a.name === fileName && a.size === fileSize
  );
  selectedFile = file;
  const maxHeight = cropperParent.getAttribute("data-max-height");
  const maxSize = cropperParent.getAttribute("data-max-size");
  const aspectRatio = cropperParent.getAttribute("data-aspect-ratio");
  imagePreview = document.querySelector(`#${holderId}`).querySelector("img");
  let rangeInput: HTMLInputElement = cropperParent.querySelector("input");
  let prevCropper = cropperParent.querySelector(".cropper-container");
  if (prevCropper) {
    prevCropper.remove();
  }
  cropper = new ImageCropper(
    file,
    cropperParent,
    maxSize ? parseInt(maxSize) : 80000,
    maxHeight ? parseInt(maxHeight) : 1024,
    0.6,
    0,
    rangeInput,
    imagePreview,
    false,
    true
  );
  cropper.initCropper();
  resetFMCropper();
};

const resetFMCropper = () => {
  cropper.isCompressed = true;
  let compressorStatuses: NodeListOf<HTMLElement> = document
    .querySelector(`#fm__cropperjs__container`)
    .querySelectorAll(".media__compressor__status");
  let statusArray = Array.from(compressorStatuses);
  statusArray[0].classList.remove("hidden");
  statusArray[1].classList.add("hidden");
};
const handleFMCrop = async (): Promise<void> => {
  cropper.handleCrop((item: File) => {
    uploadedFiles = uploadedFiles.filter((file) => file !== selectedFile);
    uploadedFiles.push(item);
    let fm = new FileReader();
    fm.readAsDataURL(item);
    fm.onload = (res) => {
      imagePreview.src = res.target.result as string;
      generateFMUploadPreview();
    };
  });
};
const cancelAllFMFiles = () => {
  uploaderInput.files = null;
  uploaderInput.value = null;
  uploadedFiles = [];
};
(async () => {
  if (window.location.href.includes("filemanager")) {
    document.addEventListener("scroll", () => loadMore());
    pageLoading.classList.remove("hidden");
    await getPageFolders();
    getPageFiles().then(() => {
      updateView();
      pageLoading.classList.add("hidden");
    });
  }
})();

/**
 * making functions available for window
 */
(<any>window).changeView = (
  type: boolean,
  svg: HTMLElement,
  sibling: HTMLElement
) => changeView(type, svg, sibling);
(<any>window).selectAtciveItem = (
  folderName: string,
  type: "file" | "folder",
  btn: string
) => selectAtciveItem(folderName, type, btn);
(<any>window).confirmRemove = () => confirmRemove();
(<any>window).selectFolder = (folderName?: string) => selectFolder(folderName);
(<any>window).changeLocation = async (location: string) =>
  changeLocation(location);
(<any>window).openFile = () => openFile();
(<any>window).createPageFolder = (folderInputId: string) =>
  createPageFolder(folderInputId);
(<any>window).initFMUploader = (input: HTMLInputElement) =>
  initFMUploader(input);
(<any>window).handlePreUploadRemove = (
  fileName: string,
  fileSize: number,
  parentId: string
) => handlePreUploadRemove(fileName, fileSize, parentId);

(<any>window).handleFMUpload = async () => handleFMUpload();
(<any>window).handleRetry = async (
  fileName: string,
  fileSize: number,
  parentId: string
) => handleRetry(fileName, fileSize, parentId);
(<any>window).initFMCropper = (
  fileName: string,
  fileSize: number,
  holderId: string
) => initFMCropper(fileName, fileSize, holderId);
(<any>window).handleFMCrop = () => handleFMCrop();
(<any>window).cancelAllFMFiles = () => cancelAllFMFiles();

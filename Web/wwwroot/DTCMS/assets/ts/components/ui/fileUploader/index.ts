import mime from "mime";
import { uploadFile } from "../uploaderService";
import { v4 as uuidv4 } from "uuid";

let input: HTMLInputElement;
let files: File[];
let acceptSingle = false;
(<any>window).initFileUploader = (previewId: string) => {
  input = event.target as HTMLInputElement;
  let acceptSingleAtt = input.getAttribute("data-accept-single");
  acceptSingle = acceptSingleAtt === "true";

  files = Array.from(input.files).map((file) => {
    let ext = file.name.split(".").slice(-1);
    let newFile = new File([file], uuidv4() + "." + ext, {
      type: file.type,
    });
    return newFile;
  });
  if (acceptSingle) {
    let hiddenInputClass = input.getAttribute("data-doc-target");
    let hiddenInput: HTMLInputElement = document.querySelector(
      `.${hiddenInputClass}`
    );
    hiddenInput.value = "";
    let prev = document.querySelector(`#${previewId}`);
    prev.innerHTML = "";
  }
  generatePreview(files, previewId);
};

const generatePreview = (files: File[], previewId: string) => {
  let previewContainer = document.querySelector(`#${previewId}`);
  files.forEach(async (file) => {
    let type = mime.getType(file.name);
    handleUpladDocs("doc__container__" + file.size, file);
    if (type.includes("image")) {
      var reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = function () {
        previewContainer.innerHTML += `
             <div id="doc__container__${
               file.size
             }" class="w-full h-[72px] relative shadow-sm mb-2 py-3 px-2 border-solid border-1 rounded-md border-gray-200 bg-white">
             <div class="flex justify-between items-center">
               <a  target="_blank" class="flex items-center icon__container">
                 <img src='${reader.result}' class="h-12 w-12"/>
                 <span class="font-bold text-sm px-3">
                   ${file.name.slice(0, 20)}
                 </span>
         
               </a>
               <svg onclick="handleRemoveDoc('${file.name}','doc__container__${
          file.size
        }')" id="primary__remove" class="cursor-pointer" width="15" height="16" viewBox="0 0 15 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                 <path
                   d="M3.125 4.875L3.66708 12.4641C3.7138 13.1182 4.2581 13.625 4.9139 13.625H10.0861C10.7419 13.625 11.2862 13.1182 11.3329 12.4641L11.875 4.875M8.75 7.375V11.125M6.25 7.375V11.125M5.625 4.875V3C5.625 2.65482 5.90482 2.375 6.25 2.375H8.75C9.09518 2.375 9.375 2.65482 9.375 3V4.875M12.5 4.875H2.5"
                   stroke="#DC2626" stroke-width="1.25" stroke-linecap="round" stroke-linejoin="round" />
               </svg>
         
             </div>
             <div id="preview__overlay" class="hidden z-[3] bg-gray-700 absolute top-0 right-0  h-full w-full opacity-50 rounded-md">
   
             <svg aria-hidden="true" id="preview__loading"
               class="  absolute top-1/3 right-1/2 w-6 h-6 mr-2 text-transparent animate-spin dark:text-gray-600 fill-white"
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
             class="z-[4] hidden absolute bottom-0 right-0 w-full bg-gray-200 rounded-full h-1.5 dark:bg-gray-700">
             <div class="bg-green-500 h-1.5 rounded-full" style="width: 33%">
             </div>
           </div>
           <div class="hidden" id="retry__button">
             <svg onclick="handleDocRetry('${file.name}','doc__container__${
          file.size
        }')" class="z-[4]  cursor-pointer absolute top-1/2 right-1/2" width="16" height="16"
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
             <svg onclick="handleRemoveDoc('${file.name}','doc__container__${
          file.size
        }')" class=" z-[4]  cursor-pointer absolute bottom-2 left-2" width="25" height="25"
               viewBox="0 0 25 25" fill="none" xmlns="http://www.w3.org/2000/svg">
               <circle cx="12.5" cy="12.5" r="12.5" fill="#DC2626" />
               <path
                 d="M18.25 9.25L17.5995 18.3569C17.5434 19.1418 16.8903 19.75 16.1033 19.75H9.89668C9.10972 19.75 8.45656 19.1418 8.40049 18.3569L7.75 9.25M11.5 12.25V16.75M14.5 12.25V16.75M15.25 9.25V7C15.25 6.58579 14.9142 6.25 14.5 6.25H11.5C11.0858 6.25 10.75 6.58579 10.75 7V9.25M7 9.25H19"
                 stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
             </svg>
           </div>
          </div>
           </div>`;
      };
    } else {
      let icon =  getIcon(type);
      previewContainer.innerHTML += `
             <div id="doc__container__${
               file.size
             }" class="w-full h-[72px] relative shadow-sm mb-2 py-3 px-2 border-solid border-1 rounded-md border-gray-200 bg-white">
             <div class="flex justify-between items-center">
               <a target="_blank" class="flex items-center icon__container">
                 ${icon}
                 <span class="font-bold text-sm px-3">
                   ${file.name.slice(0, 20)}
                 </span>
         
               </a>
               <svg onclick="handleRemoveDoc('${file.name}','doc__container__${
        file.size
      }')"  id="primary__remove" class="cursor-pointer" width="15" height="16" viewBox="0 0 15 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                 <path
                   d="M3.125 4.875L3.66708 12.4641C3.7138 13.1182 4.2581 13.625 4.9139 13.625H10.0861C10.7419 13.625 11.2862 13.1182 11.3329 12.4641L11.875 4.875M8.75 7.375V11.125M6.25 7.375V11.125M5.625 4.875V3C5.625 2.65482 5.90482 2.375 6.25 2.375H8.75C9.09518 2.375 9.375 2.65482 9.375 3V4.875M12.5 4.875H2.5"
                   stroke="#DC2626" stroke-width="1.25" stroke-linecap="round" stroke-linejoin="round" />
               </svg>
         
             </div>
             <div id="preview__overlay" class=" z-[3] bg-gray-700 absolute top-0 right-0 h-full w-full opacity-50 rounded-md">
   
             <svg aria-hidden="true" id="preview__loading"
               class="  absolute top-1/3 right-1/2 w-6 h-6 mr-2 text-transparent animate-spin dark:text-gray-600 fill-white"
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
             class="z-[4]  absolute bottom-0 hidden right-0 w-full bg-gray-200 rounded-full h-1.5 dark:bg-gray-700">
             <div class="bg-green-500 h-1.5 rounded-full" style="width: 33%">
             </div>
           </div>
           <div class="hidden" id="retry__button">
             <svg onclick="handleDocRetry('${file.name}','doc__container__${
        file.size
      }')" class="z-[4]  cursor-pointer absolute top-1/2 right-1/2" width="16" height="16"
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
             <svg onclick="handleRemoveDoc('${file.name}','doc__container__${
        file.size
      }')" class=" z-[4]  cursor-pointer absolute bottom-2 left-2" width="25" height="25"
               viewBox="0 0 25 25" fill="none" xmlns="http://www.w3.org/2000/svg">
               <circle cx="12.5" cy="12.5" r="12.5" fill="#DC2626" />
               <path
                 d="M18.25 9.25L17.5995 18.3569C17.5434 19.1418 16.8903 19.75 16.1033 19.75H9.89668C9.10972 19.75 8.45656 19.1418 8.40049 18.3569L7.75 9.25M11.5 12.25V16.75M14.5 12.25V16.75M15.25 9.25V7C15.25 6.58579 14.9142 6.25 14.5 6.25H11.5C11.0858 6.25 10.75 6.58579 10.75 7V9.25M7 9.25H19"
                 stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
             </svg>
           </div>
          </div>
           </div>`;
    }
  });
};

const getIcon =  (type: string): string => {
  let icon: string;
  if (type.includes("zip")) {
    icon = `
    <svg width="48" height="48" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <rect x="1" y="1" width="46" height="46" rx="6" fill="#4C1D95"/>
    <path d="M20.6186 26.7V29H11.4386V27.46L16.9786 19.22H11.7186V16.76H20.5386V18.54L15.0586 26.7H20.6186ZM25.398 16.76V29H22.658V16.76H25.398ZM33.3728 25.38H30.6128V29H27.8728V16.76H33.4328C34.7661 16.76 35.7661 17.16 36.4328 17.96C37.1128 18.76 37.4528 19.8133 37.4528 21.12C37.4528 21.9467 37.2861 22.6867 36.9528 23.34C36.6195 23.98 36.1461 24.48 35.5328 24.84C34.9195 25.2 34.1995 25.38 33.3728 25.38ZM34.2528 19.64C33.9461 19.3067 33.4928 19.14 32.8928 19.14H30.6128V23.02H32.7928C33.4061 23.02 33.8795 22.8533 34.2128 22.52C34.5461 22.1733 34.7128 21.7067 34.7128 21.12C34.7128 20.4667 34.5595 19.9733 34.2528 19.64Z" fill="white"/>
    </svg>
`;
  } else if (type.includes("pdf")) {
    icon = `<svg width="48" height="48" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <rect width="48" height="48" rx="6" fill="#DC2626"/>
    <path d="M13.99 25.38H11.23V29H8.49V16.76H14.05C15.3833 16.76 16.3833 17.16 17.05 17.96C17.73 18.76 18.07 19.8133 18.07 21.12C18.07 21.9467 17.9033 22.6867 17.57 23.34C17.2367 23.98 16.7633 24.48 16.15 24.84C15.5367 25.2 14.8167 25.38 13.99 25.38ZM14.87 19.64C14.5633 19.3067 14.11 19.14 13.51 19.14H11.23V23.02H13.41C14.0233 23.02 14.4967 22.8533 14.83 22.52C15.1633 22.1733 15.33 21.7067 15.33 21.12C15.33 20.4667 15.1767 19.9733 14.87 19.64ZM28.7972 27.54C27.9172 28.5133 26.5772 29 24.7772 29H19.8572V16.76H24.7772C26.5639 16.76 27.8972 17.2533 28.7772 18.24C29.6705 19.2133 30.1172 20.7867 30.1172 22.96C30.1172 25.04 29.6772 26.5667 28.7972 27.54ZM26.2172 19.62C25.7905 19.3533 25.1839 19.22 24.3972 19.22H22.5972V26.7H24.3972C25.0905 26.7 25.6439 26.6 26.0572 26.4C26.4839 26.1867 26.8039 25.8133 27.0172 25.28C27.2439 24.7333 27.3572 23.96 27.3572 22.96C27.3572 22.0533 27.2705 21.3333 27.0972 20.8C26.9372 20.2667 26.6439 19.8733 26.2172 19.62ZM34.9409 24.02V29H32.2009V16.76H40.4809V18.96H34.9409V21.74H39.8009V24.02H34.9409Z" fill="white"/>
    </svg>
    `;
  } else if (type.includes("video") || type.includes("mp4")) {
    icon = `
    <svg width="48" height="48" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <rect width="48" height="48" rx="6" fill="#7C3AED"/>
    <path fill-rule="evenodd" clip-rule="evenodd" d="M24 36.8002C31.0693 36.8002 36.8 31.0694 36.8 24.0002C36.8 16.931 31.0693 11.2002 24 11.2002C16.9308 11.2002 11.2 16.931 11.2 24.0002C11.2 31.0694 16.9308 36.8002 24 36.8002ZM23.2875 19.4689C22.7966 19.1416 22.1653 19.1111 21.645 19.3895C21.1248 19.6679 20.8 20.2101 20.8 20.8002V27.2002C20.8 27.7903 21.1248 28.3324 21.645 28.6109C22.1653 28.8893 22.7966 28.8588 23.2875 28.5315L28.0875 25.3315C28.5326 25.0347 28.8 24.5352 28.8 24.0002C28.8 23.4652 28.5326 22.9657 28.0875 22.6689L23.2875 19.4689Z" fill="white"/>
    </svg>
`;
  } else if (type.includes("audio")) {
    icon = `<svg width="48" height="48" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <rect width="48" height="48" rx="6" fill="#F97316"/>
    <path d="M28.714 19.2863C31.3175 21.8898 31.3175 26.1109 28.714 28.7144M32.4854 15.515C37.1716 20.2013 37.1716 27.7992 32.4854 32.4855M15.4477 28.0004H13.3333C12.597 28.0004 12 27.4034 12 26.6671V21.3337C12 20.5973 12.597 20.0004 13.3333 20.0004H15.4477L21.7239 13.7243C22.5638 12.8843 24 13.4792 24 14.6671V33.3337C24 34.5216 22.5638 35.1165 21.7239 34.2765L15.4477 28.0004Z" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    </svg>
    `;
  } else if (type.includes("word")) {
    icon = `<svg width="48" height="48" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
   <rect width="48" height="48" rx="6" fill="#1D4ED8"/>
   <path d="M11.6899 23.474L10.1779 28.5H8.42788L6.53788 19.932H8.41388L9.53388 25.182L10.7099 21.108V19.932H12.2499L13.7059 25.182L14.8539 19.932H16.7159L14.8399 28.5H13.0899L11.6899 23.474ZM25.5371 24.258C25.5371 27.1793 24.2117 28.64 21.5611 28.64C18.9011 28.64 17.5711 27.1793 17.5711 24.258C17.5711 22.6993 17.9024 21.57 18.5651 20.87C19.2277 20.1607 20.2264 19.806 21.5611 19.806C22.8957 19.806 23.8897 20.1607 24.5431 20.87C25.2057 21.57 25.5371 22.6993 25.5371 24.258ZM20.0071 26.232C20.3431 26.652 20.8611 26.862 21.5611 26.862C22.2611 26.862 22.7744 26.652 23.1011 26.232C23.4371 25.8027 23.6051 25.1447 23.6051 24.258C23.6051 23.2967 23.4417 22.6107 23.1151 22.2C22.7884 21.78 22.2704 21.57 21.5611 21.57C20.8517 21.57 20.3337 21.78 20.0071 22.2C19.6804 22.6107 19.5171 23.2967 19.5171 24.258C19.5171 25.1447 19.6804 25.8027 20.0071 26.232ZM29.8943 25.756H28.9423V28.5H27.0243V19.932H30.9863C31.9009 19.932 32.5869 20.1933 33.0443 20.716C33.5016 21.2387 33.7303 21.948 33.7303 22.844C33.7303 23.4507 33.5903 23.9687 33.3103 24.398C33.0303 24.8273 32.6383 25.1493 32.1343 25.364C32.2649 25.4293 32.3629 25.5133 32.4283 25.616C32.5029 25.7093 32.5729 25.84 32.6383 26.008L33.6743 28.5H31.6723L30.7763 26.288C30.6923 26.0827 30.5896 25.9427 30.4683 25.868C30.3563 25.7933 30.1649 25.756 29.8943 25.756ZM31.8123 22.844C31.8123 22.0133 31.4109 21.598 30.6083 21.598H28.9423V24.09H30.4963C31.3736 24.09 31.8123 23.6747 31.8123 22.844ZM41.4854 27.478C40.8694 28.1593 39.9314 28.5 38.6714 28.5H35.2274V19.932H38.6714C39.922 19.932 40.8554 20.2773 41.4714 20.968C42.0967 21.6493 42.4094 22.7507 42.4094 24.272C42.4094 25.728 42.1014 26.7967 41.4854 27.478ZM39.6794 21.934C39.3807 21.7473 38.956 21.654 38.4054 21.654H37.1454V26.89H38.4054C38.8907 26.89 39.278 26.82 39.5674 26.68C39.866 26.5307 40.09 26.2693 40.2394 25.896C40.398 25.5133 40.4774 24.972 40.4774 24.272C40.4774 23.6373 40.4167 23.1333 40.2954 22.76C40.1834 22.3867 39.978 22.1113 39.6794 21.934Z" fill="white"/>
   </svg>
   `;
  } else if (type.includes("spreadsheet")) {
    icon = `<svg width="48" height="48" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <rect width="48" height="48" rx="6" fill="#10B981"/>
    <path d="M13.9114 25.26L11.3514 29H8.07141L12.2514 22.82L8.21141 16.76H11.4314L13.9114 20.46L16.3714 16.76H19.5914L15.5314 22.82L19.7314 29H16.4714L13.9114 25.26ZM29.9131 26.7V29H21.6931V16.76H24.4331V26.7H29.9131ZM38.8922 28.26C38.0922 28.8867 36.9189 29.2 35.3722 29.2C34.6255 29.2 33.9255 29.1333 33.2722 29C32.6189 28.88 32.0589 28.7067 31.5922 28.48V26.2C32.1389 26.44 32.7189 26.64 33.3322 26.8C33.9455 26.9467 34.5322 27.02 35.0922 27.02C36.5722 27.02 37.3122 26.5467 37.3122 25.6C37.3122 25.16 37.1122 24.82 36.7122 24.58C36.3122 24.3267 35.6055 24.0467 34.5922 23.74C33.7789 23.4867 33.1389 23.2067 32.6722 22.9C32.2189 22.5933 31.8855 22.2267 31.6722 21.8C31.4722 21.36 31.3722 20.8133 31.3722 20.16C31.3722 19 31.7589 18.1133 32.5322 17.5C33.3055 16.8867 34.4655 16.58 36.0122 16.58C36.5722 16.58 37.1855 16.6333 37.8522 16.74C38.5189 16.8467 39.0522 16.9667 39.4522 17.1V19.4C38.4922 18.9733 37.4989 18.76 36.4722 18.76C35.6989 18.76 35.1189 18.8733 34.7322 19.1C34.3455 19.3133 34.1522 19.6533 34.1522 20.12C34.1522 20.5067 34.3189 20.8067 34.6522 21.02C34.9989 21.2333 35.6455 21.4667 36.5922 21.72C37.5122 21.96 38.2255 22.2533 38.7322 22.6C39.2522 22.9467 39.6055 23.36 39.7922 23.84C39.9922 24.3067 40.0922 24.8867 40.0922 25.58C40.0922 26.74 39.6922 27.6333 38.8922 28.26Z" fill="white"/>
    </svg>
    `;
  } else {
    icon = `
    <svg width="48" height="48" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
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

const handleUpladDocs = async (parentId: string, file: File) => {
  await uploadFile(
    [file],
    "/files",
    (progress) => handleProgress(parentId, progress),
    () =>
      setTimeout(() => {
        handleError(parentId);
      }, 200)
  )
    .then((data) => {
      let hiddenInputClass = input.getAttribute("data-doc-target");
      let hiddenInput: HTMLInputElement = document.querySelector(
        `.${hiddenInputClass}`
      );
      let prevValue: string[] = [];
      document.querySelector(`#${parentId}`).querySelector("a").href =
        data.data.data[0];
      if (hiddenInput.value !== "") prevValue = JSON.parse(hiddenInput.value);
      prevValue.push(data.data.data[0]);
      hiddenInput.value = JSON.stringify(prevValue);
    })
    .catch();
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
  percentageContainer.innerHTML = `${progress} %`;
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
  previewOverlay.classList.add("from-transparent");
  previewOverlay.classList.add("to-red-500");
};

(<any>window).handleRemoveDoc = (fileName: string, identifier: string, hiddenInputClass?: string) => {
  if (files && files.length) {
      files = files.filter((f) => f.name !== fileName);
  }
    if (!hiddenInputClass) hiddenInputClass = input.getAttribute("data-doc-target");
    let hiddenInput: HTMLInputElement = document.querySelector(
      `.${hiddenInputClass}`
    );
    let prevValue: string[] = [];
    if (hiddenInput.value) prevValue = JSON.parse(hiddenInput.value);
    let check = prevValue.find((item) => item.includes(fileName));
    if (check) {
      prevValue = prevValue.filter((item) => item !== check);
      hiddenInput.value = JSON.stringify(prevValue);
    }
    hiddenInput.value = JSON.stringify(prevValue);
  
  let parentView: HTMLElement = document.querySelector(`#${identifier}`);
  if (parentView) parentView.remove();
};
(<any>window).handleDocRetry = (fileName: string, parentId: string) => {
  let file = files.find((f) => f.name === fileName);
  handleUpladDocs(parentId, file);
};

(() => {
  let inputList: NodeListOf<HTMLInputElement> = document.querySelectorAll(
    "[data-defaultvalue-target]"
  );
  Array.from(inputList).forEach((input) => {
    let parentId = input.getAttribute("data-defaultvalue-target");
    let defaultFiles: string[] = [];
    let previewContainer = document.querySelector(`#${parentId}`);
    if (input.value) defaultFiles = JSON.parse(input.value);
    defaultFiles.forEach((file) => {
      let splitted = file.split("/");
      let fileName = splitted[splitted.length - 1];
      let type = mime.getType(fileName);
      let randomNumber = uuidv4();
      if (type.includes("image")) {
        previewContainer.innerHTML += `
           <div id="doc__container__${randomNumber}" class="w-full h-[72px] relative shadow-sm mb-2 py-3 px-2 border-solid border-1 rounded-md border-gray-200 bg-white">
           <div class="flex justify-between items-center">
             <a href="${file}" target="_blank" class="flex items-center icon__container">
               <img src='${file}' class="h-12 w-12"/>
               <span class="font-bold text-sm px-3">
                 ${fileName.slice(0, 20)}
               </span>
       
             </a>
             <svg onclick="handleRemoveDoc('${fileName}','doc__container__${randomNumber}','${input.className}')" id="primary__remove" class="cursor-pointer" width="15" height="16" viewBox="0 0 15 16" fill="none" xmlns="http://www.w3.org/2000/svg">
               <path
                 d="M3.125 4.875L3.66708 12.4641C3.7138 13.1182 4.2581 13.625 4.9139 13.625H10.0861C10.7419 13.625 11.2862 13.1182 11.3329 12.4641L11.875 4.875M8.75 7.375V11.125M6.25 7.375V11.125M5.625 4.875V3C5.625 2.65482 5.90482 2.375 6.25 2.375H8.75C9.09518 2.375 9.375 2.65482 9.375 3V4.875M12.5 4.875H2.5"
                 stroke="#DC2626" stroke-width="1.25" stroke-linecap="round" stroke-linejoin="round" />
             </svg>
       
           </div>
         </div>`;
      } else {
        let icon = getIcon(type);
        previewContainer.innerHTML += `
           <div id="doc__container__${randomNumber}" class="w-full h-[72px] relative shadow-sm mb-2 py-3 px-2 border-solid border-1 rounded-md border-gray-200 bg-white">
           <div class="flex justify-between items-center">
             <a href="${file}" target="_blank" class="flex items-center icon__container">
               ${icon}
               <span class="font-bold text-sm px-3">
                 ${fileName.slice(0, 20)}
               </span>
             </a>
             <svg onclick="handleRemoveDoc('${fileName}','doc__container__${randomNumber}','${input.className}')"  id="primary__remove" class="cursor-pointer" width="15" height="16" viewBox="0 0 15 16" fill="none" xmlns="http://www.w3.org/2000/svg">
               <path
                 d="M3.125 4.875L3.66708 12.4641C3.7138 13.1182 4.2581 13.625 4.9139 13.625H10.0861C10.7419 13.625 11.2862 13.1182 11.3329 12.4641L11.875 4.875M8.75 7.375V11.125M6.25 7.375V11.125M5.625 4.875V3C5.625 2.65482 5.90482 2.375 6.25 2.375H8.75C9.09518 2.375 9.375 2.65482 9.375 3V4.875M12.5 4.875H2.5"
                 stroke="#DC2626" stroke-width="1.25" stroke-linecap="round" stroke-linejoin="round" />
             </svg>
       
           </div>
         </div>`;
      }
    });
  });
})();

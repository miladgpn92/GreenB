enum language {
  fa = "fa-IR",
  en = "en-US",
}
const setSearchIcon = () => {
  let iconContainer = document.querySelector(".cdx-search-field__icon");
  iconContainer.innerHTML = `
       <svg width="20" height="20" viewBox="0 0 17 17" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M8.14616 15.4066C4.14408 15.4066 0.885742 12.1482 0.885742 8.14616C0.885742 4.14408 4.14408 0.885742 8.14616 0.885742C12.1482 0.885742 15.4066 4.14408 15.4066 8.14616C15.4066 12.1482 12.1482 15.4066 8.14616 15.4066ZM8.14616 1.94824C4.72491 1.94824 1.94824 4.73199 1.94824 8.14616C1.94824 11.5603 4.72491 14.3441 8.14616 14.3441C11.5674 14.3441 14.3441 11.5603 14.3441 8.14616C14.3441 4.73199 11.5674 1.94824 8.14616 1.94824Z" fill="#4B5563"/>
        <path d="M15.5838 16.1151C15.4493 16.1151 15.3147 16.0655 15.2084 15.9593L13.7918 14.5426C13.5863 14.3372 13.5863 13.9972 13.7918 13.7918C13.9972 13.5863 14.3372 13.5863 14.5426 13.7918L15.9593 15.2084C16.1647 15.4138 16.1647 15.7538 15.9593 15.9593C15.853 16.0655 15.7184 16.1151 15.5838 16.1151Z" fill="#4B5563"/>
       </svg>`;
};
const replaceToolBarIcons = () => {
  const lang = document.querySelector("html").getAttribute("lang");
  let toolList: NodeListOf<HTMLElement> =
    document.querySelectorAll(".ce-popover-item");
  toolList.forEach((element) => {
    let elementType = element.getAttribute("data-item-name");
    console.log(elementType);
    if (elementType === "paragraph") {
      element.innerHTML = `
          <div class="flex my-1">
          <svg width="46" height="46" viewBox="0 0 46 46" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="0.5" y="0.5" width="45" height="45" rx="3.5" stroke="#D1D5DB"/>
          <path d="M13.6699 18.1703V16.3503C13.6699 15.2003 14.5999 14.2803 15.7399 14.2803H30.2599C31.4099 14.2803 32.3299 15.2103 32.3299 16.3503V18.1703" stroke="#374151" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M23 31.7204V15.1104" stroke="#374151" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M19.0596 31.7197H26.9396" stroke="#374151" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          <div class="flex flex-col justify-between mx-1.5">
          <div class="text-sm font-black text-gray-700">
          ${lang === language.fa ? "متن" : "Text"}
          </div>
          <div class="text-xs font-light text-gray-700">
          ${lang === language.fa ? "افزودن یک متن ساده" : "Add a simple text"}
          </div>
        </div>
          </div>
          `;
    } else if (elementType === "video") {
      element.innerHTML = `
          <div class="flex my-1">
          <svg width="46" height="46" viewBox="0 0 46 46" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="0.5" y="0.5" width="45" height="45" rx="3.5" stroke="#D1D5DB"/>
          <path d="M20 33H26C31 33 33 31 33 26V20C33 15 31 13 26 13H20C15 13 13 15 13 20V26C13 31 15 33 20 33Z" stroke="#374151" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M20.0996 23.0005V21.5205C20.0996 19.6105 21.4496 18.8405 23.0996 19.7905L24.3796 20.5305L25.6596 21.2705C27.3096 22.2205 27.3096 23.7805 25.6596 24.7305L24.3796 25.4705L23.0996 26.2105C21.4496 27.1605 20.0996 26.3805 20.0996 24.4805V23.0005Z" stroke="#374151" stroke-width="1.5" stroke-miterlimit="10" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          
          <div class="flex flex-col justify-between mx-1.5">
          <div class="text-sm font-black text-gray-700">
          ${lang === language.fa ? "ویدئو" : "Video"}
          </div>
          <div class="text-xs font-light text-gray-700">
          ${lang === language.fa ? "بارگزاری ویدئو" : "Upload a video"}
          </div>
        </div>
          </div>
          `;
    } else if (elementType === "image") {
      element.innerHTML = `
          <div class="flex my-1">
          <svg width="46" height="46" viewBox="0 0 46 46" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="0.5" y="0.5" width="45" height="45" rx="3.5" stroke="#D1D5DB"/>
          <path d="M32.6799 27.9596L29.5499 20.6496C28.4899 18.1696 26.5399 18.0696 25.2299 20.4296L23.3399 23.8396C22.3799 25.5696 20.5899 25.7196 19.3499 24.1696L19.1299 23.8896C17.8399 22.2696 16.0199 22.4696 15.0899 24.3196L13.3699 27.7696C12.1599 30.1696 13.9099 32.9996 16.5899 32.9996H29.3499C31.9499 32.9996 33.6999 30.3496 32.6799 27.9596Z" stroke="#374151" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M17.9697 19C19.6266 19 20.9697 17.6569 20.9697 16C20.9697 14.3431 19.6266 13 17.9697 13C16.3129 13 14.9697 14.3431 14.9697 16C14.9697 17.6569 16.3129 19 17.9697 19Z" stroke="#374151" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          <div class="flex flex-col justify-between mx-1.5">
          <div class="text-sm font-black text-gray-700">
          ${lang === language.fa ? "عکس" : "Photo"}
          </div>
          <div class="text-xs font-light text-gray-700">
          ${lang === language.fa ? "بارگزاری عکس" : "Upload a photo"}
          </div>
        </div>
          </div>
          `;
    } else if (elementType === "audio") {
      element.innerHTML = `
          <div class="flex my-1">
          <svg class="w-[46px] h-[46px]" width="46" height="46" viewBox="0 0 46 46" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="0.5" y="0.5" width="45" height="45" rx="3.5" stroke="#D1D5DB"/>
          <path d="M17.2802 33.2498C15.4263 33.2498 13.9102 31.7336 13.9102 29.8798C13.9102 28.0259 15.4263 26.5098 17.2802 26.5098C19.134 26.5098 20.6502 28.0259 20.6502 29.8798C20.6502 31.7336 19.134 33.2498 17.2802 33.2498ZM17.2802 27.0098C15.694 27.0098 14.4102 28.2936 14.4102 29.8798C14.4102 31.4659 15.694 32.7498 17.2802 32.7498C18.8663 32.7498 20.1502 31.4659 20.1502 29.8798C20.1502 28.2936 18.8663 27.0098 17.2802 27.0098Z" fill="#292D32" stroke="#374151"/>
          <path d="M22.2506 14.5524L22.2518 14.5521L28.4918 12.8521L28.4932 12.8517C29.6714 12.527 30.5611 12.674 31.1445 13.1252L31.1445 13.1252L31.1471 13.1271C31.738 13.5781 32.0904 14.3683 32.0904 15.6096V27.8096C32.0904 27.9435 31.9742 28.0596 31.8404 28.0596C31.7065 28.0596 31.5904 27.9435 31.5904 27.8096V15.5996C31.5904 15.286 31.5639 14.9035 31.4673 14.5409C31.3729 14.1866 31.1948 13.7887 30.8443 13.5226C30.4684 13.2324 30.0007 13.1713 29.6246 13.1785C29.2387 13.1858 28.8695 13.2665 28.6167 13.3378L22.379 15.0372L22.379 15.0372L22.3774 15.0376C21.8519 15.1826 21.4123 15.4607 21.1057 15.8651C20.7993 16.2692 20.6504 16.767 20.6504 17.3096V29.8896C20.6504 30.0092 20.5386 30.1296 20.4004 30.1296C20.2665 30.1296 20.1504 30.0135 20.1504 29.8796V17.2996C20.1504 15.9452 20.9493 14.9034 22.2506 14.5524Z" fill="#292D32" stroke="#374151"/>
          <path d="M28.7196 31.1697C26.8658 31.1697 25.3496 29.6535 25.3496 27.7997C25.3496 25.9458 26.8658 24.4297 28.7196 24.4297C30.5735 24.4297 32.0896 25.9458 32.0896 27.7997C32.0896 29.6535 30.5735 31.1697 28.7196 31.1697ZM28.7196 24.9297C27.1335 24.9297 25.8496 26.2135 25.8496 27.7997C25.8496 29.3858 27.1335 30.6697 28.7196 30.6697C30.3058 30.6697 31.5896 29.3858 31.5896 27.7997C31.5896 26.2135 30.3057 24.9297 28.7196 24.9297Z" fill="#292D32" stroke="#374151"/>
          <path d="M20.3998 21.2703C20.0698 21.2703 19.7698 21.0503 19.6798 20.7203C19.5698 20.3203 19.7998 19.9003 20.1998 19.7903L31.6398 16.6703C32.0398 16.5603 32.4498 16.8003 32.5598 17.2003C32.6698 17.6003 32.4298 18.0103 32.0298 18.1203L20.5998 21.2403C20.5298 21.2603 20.4598 21.2703 20.3998 21.2703Z" fill="#292D32"/>
          </svg>
          
          <div class="flex flex-col justify-between mx-1.5">
          <div class="text-sm font-black text-gray-700">
          ${lang === language.fa ? "صوت" : "Audio"}
          </div>
          <div class="text-xs font-light text-gray-700">
          ${lang === language.fa ? "بارگزاری فایل صوتی " : "Upload an audio"}
          </div>
        </div>
          </div>
          `;
    } else if (elementType === "table") {
      element.innerHTML = `
          <div class="flex my-1">
          <svg width="46" height="46" viewBox="0 0 46 46" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="0.5" y="0.5" width="45" height="45" rx="3.5" stroke="#D1D5DB"/>
          <path d="M23.029 33.0877C23.029 33.0876 23.0302 33.0861 23.0325 33.0835C23.0302 33.0866 23.029 33.0879 23.029 33.0877ZM23.4675 33.0835C23.4698 33.0861 23.471 33.0876 23.471 33.0877C23.471 33.0879 23.4698 33.0866 23.4675 33.0835ZM23.471 12.9123C23.471 12.9124 23.4698 12.9139 23.4675 12.9165C23.4698 12.9134 23.471 12.9121 23.471 12.9123ZM23.0325 12.9165C23.0302 12.9139 23.029 12.9124 23.029 12.9123C23.029 12.9121 23.0302 12.9134 23.0325 12.9165Z" fill="#292D32" stroke="#374151"/>
          <path d="M13.415 19.2832C13.4151 19.2832 13.4163 19.2841 13.4185 19.2861C13.416 19.2842 13.4149 19.2832 13.415 19.2832ZM13.4185 19.7139C13.4163 19.7159 13.4151 19.7168 13.415 19.7168C13.4149 19.7168 13.416 19.7158 13.4185 19.7139ZM33.085 19.7168C33.0849 19.7168 33.0837 19.7159 33.0815 19.7139C33.084 19.7158 33.0851 19.7168 33.085 19.7168ZM33.0815 19.2861C33.0837 19.2841 33.0849 19.2832 33.085 19.2832C33.0851 19.2832 33.084 19.2842 33.0815 19.2861Z" fill="#292D32" stroke="#374151"/>
          <path d="M13.415 23.2832C13.4151 23.2832 13.4163 23.2841 13.4185 23.2861C13.416 23.2842 13.4149 23.2832 13.415 23.2832ZM13.4185 23.7139C13.4163 23.7159 13.4151 23.7168 13.415 23.7168C13.4149 23.7168 13.416 23.7158 13.4185 23.7139ZM33.085 23.7168C33.0849 23.7168 33.0837 23.7159 33.0815 23.7139C33.084 23.7158 33.0851 23.7168 33.085 23.7168ZM33.0815 23.2861C33.0837 23.2841 33.0849 23.2832 33.085 23.2832C33.0851 23.2832 33.084 23.2842 33.0815 23.2861Z" fill="#292D32" stroke="#374151"/>
          <path d="M31.8816 28H14.6184C13.7337 28 13 27.7733 13 27.5C13 27.2267 13.7337 27 14.6184 27H31.8816C32.7663 27 33.5 27.2267 33.5 27.5C33.5 27.7733 32.7663 28 31.8816 28Z" fill="#292D32"/>
          <path d="M26 33.75H20C14.57 33.75 12.25 31.43 12.25 26V20C12.25 14.57 14.57 12.25 20 12.25H26C31.43 12.25 33.75 14.57 33.75 20V26C33.75 31.43 31.43 33.75 26 33.75ZM20 13.75C15.39 13.75 13.75 15.39 13.75 20V26C13.75 30.61 15.39 32.25 20 32.25H26C30.61 32.25 32.25 30.61 32.25 26V20C32.25 15.39 30.61 13.75 26 13.75H20Z" fill="#292D32"/>
          </svg>
          
          
          <div class="flex flex-col justify-between mx-1.5">
          <div class="text-sm font-black text-gray-700">
          ${lang === language.fa ? "جدول" : "Table"}
          </div>
          <div class="text-xs font-light text-gray-700">
          ${lang === language.fa ? "ساخت جدول" : "Add a table"}
          </div>
        </div>
          </div>
          `;
    } else if (elementType === "attaches") {
      element.innerHTML = `
          <div class="flex my-1">
          <svg width="46" height="46" viewBox="0 0 46 46" fill="none" xmlns="http://www.w3.org/2000/svg">
            <rect x="0.5" y="0.5" width="45" height="45" rx="3.5" stroke="#D1D5DB"/>
            <path d="M20 28V22L18 24" stroke="#374151" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            <path d="M20 22L22 24" stroke="#374151" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            <path d="M33 21V26C33 31 31 33 26 33H20C15 33 13 31 13 26V20C13 15 15 13 20 13H25" stroke="#374151" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            <path d="M33 21H29C26 21 25 20 25 17V13L33 21Z" stroke="#292D32" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
           </svg>
           <div class="flex flex-col justify-between mx-1.5">
          <div class="text-sm font-black text-gray-700">
          ${lang === language.fa ? "فایل" : "File"}
          </div>
          <div class="text-xs font-light text-gray-700">
          ${lang === language.fa ? "آپلود یک فایل" : "Upload a file"}
          </div>
        </div>
          </div>
          `;
    } else if (elementType === "raw") {
      element.innerHTML = `
          <div class="flex my-1">
          <svg width="46" height="46" viewBox="0 0 46 46" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="0.5" y="0.5" width="45" height="45" rx="3.5" stroke="#D1D5DB"/>
          <path d="M15.0996 13H30.8996C31.4996 13 31.9996 13.5 31.8996 14.1L30.0996 30.3C30.0996 30.7 29.7996 31 29.3996 31.2L23.2996 32.9C23.0996 33 22.8996 33 22.7996 32.9L16.6996 31.2C16.2996 31.1 15.9996 30.8 15.9996 30.3L14.0996 14.1C14.0996 13.5 14.4996 13 15.0996 13Z" stroke="#374151" stroke-width="1.5" stroke-miterlimit="10" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M27.1998 17.7998H18.7998L19.1998 22.1998H26.7998L26.1998 27.1998L22.7998 28.1998L19.1998 27.1998V25.1998" stroke="#374151" stroke-width="1.5" stroke-miterlimit="10" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          <div class="flex flex-col justify-between mx-1.5">
          <div class="text-sm font-black text-gray-700">
          ${lang === language.fa ? "کد خام" : "Raw Code"}
          </div>
          <div class="text-xs font-light text-gray-700">
          ${lang === language.fa ? "کد Html بنویسید" : "write Html code"}
          </div>
        </div>
          </div>
          `;
    } else if (elementType === "list") {
      element.innerHTML = `
          <div class="flex my-1">
          <svg width="46" height="46" viewBox="0 0 46 46" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="0.5" y="0.5" width="45" height="45" rx="3.5" stroke="#D1D5DB"/>
          <path d="M32 18.25H14C13.8661 18.25 13.75 18.1339 13.75 18C13.75 17.8661 13.8661 17.75 14 17.75H32C32.1339 17.75 32.25 17.8661 32.25 18C32.25 18.1339 32.1339 18.25 32 18.25Z" fill="#292D32" stroke="#374151"/>
          <path d="M32 23.25H14C13.8661 23.25 13.75 23.1339 13.75 23C13.75 22.8661 13.8661 22.75 14 22.75H32C32.1339 22.75 32.25 22.8661 32.25 23C32.25 23.1339 32.1339 23.25 32 23.25Z" fill="#292D32" stroke="#374151"/>
          <path d="M32 28.25H14C13.8661 28.25 13.75 28.1339 13.75 28C13.75 27.8661 13.8661 27.75 14 27.75H32C32.1339 27.75 32.25 27.8661 32.25 28C32.25 28.1339 32.1339 28.25 32 28.25Z" fill="#292D32" stroke="#374151"/>
          </svg>        
          <div class="flex flex-col justify-between mx-1.5">
          <div class="text-sm font-black text-gray-700">
          ${lang === language.fa ? "لیست" : "List"}
          </div>
          <div class="text-xs font-light text-gray-700">
          ${lang === language.fa ? "یک لیست ایجاد کنید" : "Create a list"}
          </div>
        </div>
          </div>
          `;
    } else if (elementType === "quote") {
      element.innerHTML = `
          <div class="flex my-1">
          <svg width="46" height="46" viewBox="0 0 46 46" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="0.5" y="0.5" width="45" height="45" rx="3.5" stroke="#D1D5DB"/>
          <path d="M33.0001 22.6495H27.2001C25.6701 22.6495 24.6201 21.4895 24.6201 20.0695V16.8495C24.6201 15.4295 25.6701 14.2695 27.2001 14.2695H30.4201C31.8401 14.2695 33.0001 15.4295 33.0001 16.8495V22.6495Z" stroke="#374151" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M32.9997 22.6494C32.9997 28.6994 31.8697 29.6994 28.4697 31.7194" stroke="#374151" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M21.3702 22.6495H15.5702C14.0402 22.6495 12.9902 21.4895 12.9902 20.0695V16.8495C12.9902 15.4295 14.0402 14.2695 15.5702 14.2695H18.8002C20.2202 14.2695 21.3802 15.4295 21.3802 16.8495V22.6495" stroke="#374151" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M21.3698 22.6494C21.3698 28.6994 20.2398 29.6994 16.8398 31.7194" stroke="#292D32" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
               
          <div class="flex flex-col justify-between mx-1.5">
          <div class="text-sm font-black text-gray-700">
          ${lang === language.fa ? "نقل قول" : "Quote"}
          </div>
          <div class="text-xs font-light text-gray-700">
          ${lang === language.fa ? "یک نقل قول بنویسید" : "Write a quote"}
          </div>
        </div>
          </div>
          `;
    } else if (elementType === "header") {
      element.innerHTML = `
          <div class="flex my-1">
          <svg width="46" height="46" viewBox="0 0 46 46" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="0.5" y="0.5" width="45" height="45" rx="3.5" stroke="#D1D5DB"/>
          <path d="M30.7683 13.4277V32.9997H28.8083V23.8437H18.1963V32.9997H16.2363V13.4277H18.1963V22.2197H28.8083V13.4277H30.7683Z" fill="#374151"/>
          </svg>
          <div class="flex flex-col justify-between mx-1.5">
          <div class="text-sm font-black text-gray-700">
          ${lang === language.fa ? "عنوان" : "Header"}
          </div>
          <div class="text-xs font-light text-gray-700">
          ${lang === language.fa ? "عنوان H1 تا H6 بسازید" : "Create a header"}
          </div>
          </div>
          </div>
          `;
    }
     else if (elementType === "checklist") {
      element.innerHTML = `
          <div class="flex my-1">
          <svg width="46" height="46" viewBox="0 0 46 46" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="0.5" y="0.5" width="45" height="45" rx="3.5" stroke="#D1D5DB"/>
          <path d="M26 33.25H20C17.3424 33.25 15.5649 32.6803 14.4423 31.5577C13.3197 30.4351 12.75 28.6576 12.75 26V20C12.75 17.3424 13.3197 15.5649 14.4423 14.4423C15.5649 13.3197 17.3424 12.75 20 12.75H26C28.6576 12.75 30.4351 13.3197 31.5577 14.4423C32.6803 15.5649 33.25 17.3424 33.25 20V26C33.25 28.6576 32.6803 30.4351 31.5577 31.5577C30.4351 32.6803 28.6576 33.25 26 33.25ZM20 13.25C17.6597 13.25 15.9243 13.6611 14.7927 14.7927C13.6611 15.9243 13.25 17.6597 13.25 20V26C13.25 28.3403 13.6611 30.0757 14.7927 31.2073C15.9243 32.3389 17.6597 32.75 20 32.75H26C28.3403 32.75 30.0757 32.3389 31.2073 31.2073C32.3389 30.0757 32.75 28.3403 32.75 26V20C32.75 17.6597 32.3389 15.9243 31.2073 14.7927C30.0757 13.6611 28.3403 13.25 26 13.25H20Z" fill="#292D32" stroke="#374151"/>
          <path d="M21.2269 25.1232L21.5804 25.4767L21.934 25.1232L27.074 19.9832C27.1687 19.8884 27.3321 19.8884 27.4269 19.9832C27.5216 20.0779 27.5216 20.2413 27.4269 20.3361L21.7569 26.0061C21.7096 26.0534 21.6463 26.0796 21.5804 26.0796C21.5145 26.0796 21.4513 26.0534 21.404 26.0061L18.574 23.1761C18.4792 23.0813 18.4792 22.9179 18.574 22.8232C18.6687 22.7284 18.8321 22.7284 18.9269 22.8232L21.2269 25.1232Z" fill="#292D32" stroke="#374151"/>
          </svg>
          <div class="flex flex-col justify-between mx-1.5">
          <div class="text-sm font-black text-gray-700">
          ${lang === language.fa ? "چک لیست" : "Checklist"}
          </div>
          <div class="text-xs font-light text-gray-700">
          ${lang === language.fa ? "لیستی از اطلاعات بسازید": "Create a check list"}
          </div>
          </div>
          </div>
          `;
    }
  });
};

export { replaceToolBarIcons, setSearchIcon };

import { Operator, Languages } from "../Enums";
import { ApiFilter } from "../interfaces/panel/common";
import { FileResponse } from "../interfaces/panel/files";
import ValidationModel from "../interfaces/validation";
import * as shamsi from "shamsi-date-converter";
import type { MonthType, DayType } from "./../interfaces/panel/date";
function duplicateRemover(arr: ValidationModel[]): ValidationModel[] {
  return arr.filter(
    (value: ValidationModel, index: number, self: ValidationModel[]) =>
      index === self.findIndex((t: ValidationModel) => t.path === value.path)
  );
}

function replaceTexts(
  originalText: string,
  textArray: string[],
  replaceArray: string[]
): string {
  let outPut = "";
  textArray.forEach((element, index) => {
    outPut = originalText.replace(element, replaceArray[index]);
  });
  return outPut;
}

const getJalaaliPresenterDate = (date: string): string => {
  if (!date) return;

  return new Intl.DateTimeFormat("fa", {
    year: "numeric",
    month: "long",
    day: "numeric",
    calendar: "persian",
  }).format(new Date(date));
};

const base64ToFile = (base64String: string, fileName: string): File => {
  // Remove the data:image/<image-extension>;base64 prefix
  const base64Data = base64String.replace(/^data:image\/\w+;base64,/, "");

  // Convert the Base64 data to a binary format
  const binaryData = atob(base64Data);

  // Create a Uint8Array to hold the binary data
  const bytes = new Uint8Array(binaryData.length);
  for (let i = 0; i < binaryData.length; i++) {
    bytes[i] = binaryData.charCodeAt(i);
  }

  // Create a Blob from the binary data
  const blob = new Blob([bytes], { type: "image/png" }); // Change the type if your image is of a different format (e.g., 'image/jpeg')
  // Create a File from the Blob
  const file = new File([blob], fileName, { type: blob.type });

  return file;
};

/**
 *
 * @param time time variable
 * @param format the required format from moment
 * @returns moment time
 */
const jalaliToMiladi = (time: string, format?: string) => {
  let locale = "fa";
  var moment = require("moment-jalaali");
  let formatted = format
    ? format
    : locale === "fa"
    ? "jYYYY/jMM/jD"
    : "YYYY/MMMM/D";
  return moment(time).format(formatted);
};

const langDirection = (): "rtl" | "ltr" => {
  let lang = localStorage.getItem("langConf");
  const rtlLanguages = ["fa", "ar"];
  const ltrLanguages = ["en", "fr"];

  if (rtlLanguages.includes(lang.toLowerCase())) {
    return "rtl";
  } else if (ltrLanguages.includes(lang.toLowerCase())) {
    return "ltr";
  } else {
    // Default to "ltr" for any other language code not specified above
    return "ltr";
  }
};

const serverLang = (): string => {
  let lang = document.querySelector("html").getAttribute("lang");

  return lang;
};

/**
 * returns suitable language for server based on Language Enum
 */
// const getApiLang = ():Languages => {
//   let langCOnf = localStorage.getItem("langConf");
//  return Languages[langCOnf === "fa" ? "Persian" : "English"]
// }
// const greetUser = (userName: string): string => {
//   const currentDate = new Date();
//   const currentHour = currentDate.getHours();

//   if (userName === "" || userName === null) {
//     userName = i18n.t("constant.no_name_user");
//   }

//   let greetingMessage = "";

//   if (currentHour >= 5 && currentHour < 11) {
//     greetingMessage = i18n.t("constant.good_morning");
//   } else if (currentHour >= 11 && currentHour < 14) {
//     greetingMessage = i18n.t("constant.good_afternoon");
//   } else if (currentHour >= 14 && currentHour < 119) {
//     greetingMessage = i18n.t("constant.good_evening");
//   } else {
//     greetingMessage = i18n.t("constant.good_night");
//   }

//   return `${greetingMessage}, ${userName} `;
// };

const handleFilter = (
  filterArray: ApiFilter[],
  filterTitle: string,
  searchValue: any,
  operator: Operator
): ApiFilter[] => {
  const filterIndex = filterArray.findIndex(
    (filter) => filter.field === filterTitle && filter.operator == operator
  );

  if (searchValue === "" || searchValue === undefined) {
    // If searchValue is empty or undefined, remove the filter
    if (filterIndex !== -1) {
      filterArray.splice(filterIndex, 1);
    }
  } else {
    // Create or update the filter
    const newFilter: ApiFilter = {
      field: filterTitle,
      operator: Operator.Contains,
      value: searchValue,
    };

    if (filterIndex !== -1) {
      // If the filter already exists, replace it
      filterArray[filterIndex] = newFilter;
    } else {
      // If the filter does not exist, push the new filter
      filterArray.push(newFilter);
    }
  }

  return filterArray;
};
/**
 *
 * @param parameterName the name of the query string
 * @returns the value of the query string (if found)
 */
const getUrlParameters = (parameterName: string): string | null => {
  let params = new URL(window.location.href).searchParams;
  let query = params.get(parameterName);
  return query;
};

/**
 * @param text a text that needs to be converted to slug
 * @returns a slug generated based on the input
 */
const slugGenerator = (text: string): string => {
  let striped = "";
  for (let i = 0; i < text.length; i++) {
    if (/^[\u0600-\u06FF\s0-9a-zA-Z-]+$/.test(text[i])) {
      striped += text[i];
    }
  }
  let replaced = striped.replace(/\s/g, "-");

  let doplicatesRemoved = "";
  for (let i = 0; i < replaced.length; i++) {
    if (replaced[i] === "-" && replaced[i - 1] && replaced[i - 1] === "-") {
      //do nothing
    } else {
      doplicatesRemoved += replaced[i];
    }
  }
  return doplicatesRemoved;
};

const retunUrlQueries = (query: string): string | null => {
  const queryString = window.location.search;
  const urlParams = new URLSearchParams(queryString);
  return urlParams.get(query);
};

/**
 *
 * @param params {
 * taostMessage: the message that needs to be shown
 * toastVarient : the variation of the toast (success,info,warning,error)
 * duration of showing the toast(in seconds)
 * }
 */
const showToast = ({
  toastMessage,
  toastVariation,
  duration,
}: {
  toastMessage: string;
  toastVariation: "info" | "success" | "warning" | "error";
  duration?: number;
}): void => {
  let toastMessageArea = document.getElementById("toastMessageArea");
  let toastContainerArea = document.getElementById("toastContainerArea");
  toastContainerArea.classList.remove("hidden");
  toastContainerArea.classList.add("flex");
  toastMessageArea.innerText = toastMessage;
  toastVariation === "info"
    ? (toastMessageArea.className =
        "text-white bg-blue-500 w-full h-[65px] flex items-center px-6 py-4")
    : toastVariation === "success"
    ? (toastMessageArea.className =
        "text-white bg-green-500 w-full h-[65px] flex items-center px-6 py-4")
    : toastVariation === "warning"
    ? (toastMessageArea.className =
        "text-white bg-amber-400 w-full h-[65px] flex items-center px-6 py-4")
    : (toastMessageArea.className =
        "text-white bg-red-500 w-full h-[65px] flex items-center px-6 py-4");
  setTimeout(
    () => {
      toastContainerArea.classList.remove("flex");
      toastContainerArea.classList.add("hidden");
    },
    duration ? duration * 1000 : 3000
  );
};
(<any>window).closeToast = () => {
  let toastContainerArea = document.getElementById("toastContainerArea");
  toastContainerArea.classList.remove("flex");
  toastContainerArea.classList.add("hidden");
};
/**
 *
 * @param sourceCanvas :Rectangle or square Html canvas element
 * @returns round canvas element
 */
const getRoundedCanvas = (
  sourceCanvas: HTMLCanvasElement
): HTMLCanvasElement => {
  const canvas = document.createElement("canvas");
  const context = canvas.getContext("2d");
  const width = sourceCanvas.width;
  const height = sourceCanvas.height;
  canvas.width = width;
  canvas.height = height;
  context.imageSmoothingEnabled = true;
  context.drawImage(sourceCanvas, 0, 0, width, height);
  context.globalCompositeOperation = "destination-in";
  context.beginPath();
  context.arc(
    width / 2,
    height / 2,
    Math.min(width, height) / 2,
    0,
    2 * Math.PI,
    true
  );
  context.fill();
  return canvas;
};
/**
 *
 * @param file get a file
 * @returns file size with a suffix
 */
const getFileSize = (file: File): string => {
  return ` ${
    file.size < 1000
      ? file.size
      : file.size >= 1000 && file.size <= 999999
      ? (file.size / 1000).toFixed(2)
      : (file.size / 1000000).toFixed(2)
  } ${
    file.size < 1000
      ? "Byte"
      : file.size >= 1000 && file.size <= 999999
      ? "KB"
      : "MB"
  }`;
};

const jalaliToGregorian = (
  jy: string,
  jm: string,
  jd: string
): [number, number, number] => {
  let year = parseInt(jy);
  let month: MonthType = parseInt(jm) as MonthType;
  let day: DayType = parseInt(jd) as DayType;
  return shamsi.jalaliToGregorian(year, month, day);
};
export {
  duplicateRemover,
  replaceTexts,
  getJalaaliPresenterDate,
  base64ToFile,
  jalaliToMiladi,
  langDirection,
  serverLang,
  // greetUser,
  handleFilter,
  getUrlParameters,
  slugGenerator,
  // getApiLang
  retunUrlQueries,
  showToast,
  getRoundedCanvas,
  jalaliToGregorian,
};

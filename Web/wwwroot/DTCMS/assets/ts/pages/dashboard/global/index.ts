import { Languages, Operator } from "../../../Enums";
import { CategoryResponse } from "../../../interfaces/panel/category";
import CategoryService from "../../../services/categoryService";
import { slugGenerator } from "../../../utils";
import $ from "jquery";
import validate from "jquery-validation";
$.validate = validate;
let catService: CategoryService;
(<any>window).setGenderValue = (gender: string, btn: HTMLButtonElement) =>
  setGenderValue(gender, btn);
(<any>window).createCat = (btn: HTMLButtonElement) => createCat(btn);

const setGenderValue = (gender: string, btn: HTMLButtonElement) => {
  (document.getElementById("GenderSelector") as HTMLInputElement).value =
    gender;

  let GenderContainer: HTMLCollection =
    document.getElementById("gender__container").children;
  Array.from(GenderContainer).forEach((item) => {
    item.classList.remove("bg-blue-600");
    item.classList.add("text-gray-500");
    item.classList.add("bg-white");
    item.classList.remove("text-white");
  });
  btn.classList.add("bg-blue-600");
  btn.classList.remove("text-gray-500");
  btn.classList.remove("bg-white");
  btn.classList.add("text-white");
};

/**Render Cats😼 */
const getCatData = async (
  dataRoute: string,
  search?: string
): Promise<CategoryResponse[]> => {
  catService = new CategoryService(dataRoute);
  try {
    let { data } = await catService.paginatedList({
      arg: {
        pageNumber: 1,
        pageSize: 50,
      },
      filters: search
        ? [
            {
              field: "title",
              value: search,
              operator: Operator.Contains,
            },
          ]
        : null,
    });
    return data;
  } catch (ex) {
    return [];
  }
};

const generateCatView = (
  cats: CategoryResponse[],
  parentContainer: HTMLElement,
  titleInput: HTMLInputElement
) => {
  let nothingFound = parentContainer.querySelector(".nothig__found");
  let searchResultContainer = parentContainer.querySelector(".search__result");
  let valueInputSelector = parentContainer.getAttribute("data-cat-value");
  let nothigCreated = parentContainer.querySelector(".nothing__created");
  let valueInput: HTMLInputElement = document.querySelector(
    `.${valueInputSelector}`
  );
  let searcInput: HTMLInputElement =
    parentContainer.querySelector("#search__input");

  if (cats.length === 0) {
    if (searchResultContainer && searcInput.value === "") {
      nothigCreated.classList.remove("hidden");
      searchResultContainer.classList.add("hidden");
      nothingFound.classList.add("hidden");
      searcInput.classList.add("hidden");
      parentContainer.querySelector("span").classList.add("hidden");
    } else {
      nothigCreated.classList.add("hidden");
      searchResultContainer.classList.add("hidden");
      nothingFound.classList.remove("hidden");
      searcInput.classList.remove("hidden");
      parentContainer.querySelector("span").classList.remove("hidden");
    }
  } else {
    nothingFound.classList.add("hidden");
    nothigCreated.classList.add("hidden");
    searcInput.classList.remove("hidden");
    parentContainer.querySelector("span").classList.remove("hidden");
    searchResultContainer.classList.remove("hidden");
    let ulContainer: HTMLUListElement =
      searchResultContainer.querySelector("ul");
    ulContainer.innerHTML = null;
    cats.reverse().forEach((cat) => {
      let li: HTMLLIElement = document.createElement("li");
      let titleContainer: HTMLSpanElement = document.createElement("span");
      let iconSpan: HTMLSpanElement = document.createElement("span");
      li.className =
        "cursor-pointer flex justify-between border-b-[1px] border-solid border-gray-100 pb-3.5 pt-3.5";
      titleContainer.className =
        "text-gray-700 text-sm font-normal leading-tight";
      titleContainer.innerText = cat.title;
      iconSpan.className =
        "icon-Chevron-left text-gray-700 font-bold ltr:rotate-180";
      li.appendChild(titleContainer);
      li.appendChild(iconSpan);

      ulContainer.prepend(li);
      li.addEventListener(
        "click",
        () => selectCat(titleInput, li, valueInput, cat.id.toString()),
        true
      );
    });
  }
};
const selectCat = (
  titleInputContainer: HTMLInputElement,
  li: HTMLLIElement,
  valueInput: HTMLInputElement,
  catId: string
) => {
  let titleInput = titleInputContainer.querySelector("input");
  titleInput.value = li.children[0].innerHTML;

  valueInput.value = catId;
  let span: HTMLSpanElement =
    valueInput.parentElement.parentElement.querySelector("span");
  if (span) span.innerHTML = null;
};

const initiateCats = () => {
  let parentCat: NodeListOf<HTMLElement> = document.querySelectorAll(
    "[data-generate-cat]"
  );
  Array.from(parentCat).forEach(async (item) => {
    let dataRoute = item.getAttribute("data-route");
    let titleInputId = item.getAttribute("data-cat-input");
    let titleInput: HTMLInputElement = document.querySelector(
      `#${titleInputId}`
    );
    let cats = await getCatData(dataRoute);
    generateCatView(cats, item, titleInput);
    let searcBar: HTMLInputElement = item.querySelector(`#search__input`);

    searcBar.addEventListener("input", (e) => {
      handleSearch(dataRoute, item, titleInput);
    });
  });
};
const handleSearch = async (
  dataRoute: string,
  container: HTMLElement,
  titleInput: HTMLInputElement
) => {
  let data = await getCatData(
    dataRoute,
    (event.target as HTMLInputElement).value
  );
  generateCatView(data, container, titleInput);
};

(<any>window).removeCatError = (input: HTMLElement) => {
  let spanError = input.nextElementSibling as HTMLElement;
  spanError.classList.add("hidden");
  input.classList.remove("input-validation-error");
};

const createCat = async (btn: HTMLButtonElement) => {
  let input: HTMLInputElement = btn.parentElement.parentElement.querySelector(
    "input"
  ) as HTMLInputElement;
  try {
    let erroMsg: string = input.getAttribute("data-required");
    let spanError = input.nextElementSibling as HTMLElement;
    if (!input.value.length) {
      erroMsg = input.getAttribute("data-required");
      spanError.classList.remove("hidden");
      spanError.innerText = erroMsg;
      input.classList.add("input-validation-error");
    } else if (input.value.length > 50) {
      erroMsg = input.getAttribute("data-max-length");
      spanError.classList.remove("hidden");
      spanError.innerText = erroMsg;
      input.classList.add("input-validation-error");
    } else {
      let slug = slugGenerator(input.value);
      await catService.create({ id: 0, title: input.value, slug: slug });
      let dismisser = input.previousElementSibling as HTMLButtonElement;
      dismisser.click();
      input.value = null;
      initiateCats();
    }
  } catch (ex) {
    console.log(ex);
  }
};

/**End of Cats 😼 */
(() => {
  let GenderContainer: HTMLElement =
    document.getElementById("gender__container");
  let prevValue = document.getElementById("GenderSelector") as HTMLInputElement;
  if (
    prevValue &&
    prevValue.value &&
    prevValue.value !== "" &&
    prevValue.value !== "0" &&
    GenderContainer &&
    GenderContainer.children
  ) {
    let btn: HTMLButtonElement = document.querySelector(
      `#gender${prevValue.value}`
    );
    setGenderValue(prevValue.value, btn);
  }
  initiateCats();
})();

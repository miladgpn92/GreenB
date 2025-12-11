import { Operator } from "../../../Enums";
import { ApiFilter } from "../../../interfaces/panel/common";

let container: HTMLElement = document.getElementById("searchBarContainer");
let overLay: HTMLElement = document.getElementById("searchOverlay");

(<any>window).showSearchBar = (): void => {
  container.classList.remove("hidden");
  overLay.classList.remove("hidden");
};
(<any>window).hideSearchBar = (): void => {
  container.classList.add("hidden");
  overLay.classList.add("hidden");
};
(<any>window).handleSearch = (): void => {
  let searchInput: HTMLInputElement = container.querySelector("#searchInput");
  let searchFilter: HTMLInputElement =
    container.querySelector("#search__filter");
  if (searchFilter && searchInput) {
    let queries: string = searchFilter.attributes["query"].nodeValue;
    let arrayOfQueries = queries.split(",");
    let queryValue = searchInput.value;
    let queryObject: ApiFilter[] = [];
    arrayOfQueries.forEach((item) => {
      queryObject.push({
        field: item,
        operator: Operator.Contains,
        value: queryValue,
      });
    });
    searchFilter.value = JSON.stringify(queryObject);
  }
};
(() => {
  let checkboxes: HTMLCollection = document.getElementsByClassName("grid__box");
  if (checkboxes) {
    let arryOfBoxes = Array.from(checkboxes);
    let addBtn = document.getElementById("pageAdder");
      let groupRemover = document.getElementById("groupRemover");
      let groupRemoveForm = document.getElementById("delete__group");
    arryOfBoxes.forEach((element: HTMLInputElement) => {
      element.addEventListener("change", () => {
        if (element.checked === true) {
          addBtn.classList.replace("inline-flex", "hidden");
          groupRemover.classList.replace("hidden", "inline-flex");
          let input: HTMLInputElement = document.createElement("input");
          input.setAttribute("value", element.value);
            input.setAttribute("type", "hidden");
            input.setAttribute("name", "id");
          input.setAttribute("id", `item-${element.value}`);
          groupRemoveForm.appendChild(input);
        } else {
          let check = arryOfBoxes.find(
            (item: HTMLInputElement) => item.checked === true
          );
          let input = document.getElementById(`item-${element.value}`);
          input.remove();
          if (!check) {
            addBtn.classList.replace("hidden", "inline-flex");
            groupRemover.classList.replace("inline-flex", "hidden");
          }
        }
      });
    });
  }
})();

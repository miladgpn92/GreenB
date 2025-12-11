import { slugGenerator } from "../../../utils";
let firstLoad = true;
const setText = (input: HTMLInputElement) => {
  let classValue = input.getAttribute("data-target-class");
  let isSlug = input.getAttribute("data-has-slug");
  let targets = document.getElementsByClassName(classValue);
  let targetValue = input.value;
  Array.from(targets).forEach((element) => {
    if (isSlug && isSlug === "true") {
      if (element.tagName === "INPUT") {
        let targetInput = element as HTMLInputElement;
        if (!firstLoad) {
          targetInput.value = slugGenerator(targetValue);
        }
        targetInput.classList.remove("input-validation-error");
        let errMsg: HTMLElement =
          targetInput.parentElement.parentElement.querySelector(
            ".field-validation-error"
          );
        if (errMsg) errMsg.innerText = "";
      } else {
        element.innerHTML = slugGenerator(targetValue);
      }
    } else {
      if (element.tagName === "input") {
        (element as HTMLInputElement).value = targetValue;
      } else {
        element.innerHTML = targetValue;
      }
    }
  });

};
(() => {
  const inputList: NodeListOf<HTMLInputElement> = document.querySelectorAll(
    "[data-target-class]"
  );
  if (inputList) {
    inputList.forEach((input) => {
      setText(input);
      input.addEventListener("input", () => setText(input));
    });
    firstLoad = false;
  }
})();

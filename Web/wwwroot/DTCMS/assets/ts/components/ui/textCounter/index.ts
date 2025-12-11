const setCounter = (input: HTMLInputElement) => {
  let classValue = input.getAttribute("data-counter-target");

  let targets = document.getElementsByClassName(classValue);
  let targetValue = input.value?.length;
  Array.from(targets).forEach((element) => {
    element.innerHTML = targetValue.toString();
  });
};

(() => {
  const inputList: NodeListOf<HTMLInputElement> =
    document.querySelectorAll("[data-has-counter]");
  if (inputList) {
    inputList.forEach((input) => {
      let counterRequired = input.getAttribute("data-has-counter");
      setCounter(input);
      if (counterRequired && counterRequired === "True") {
        input.addEventListener("input", () => setCounter(input));
      }
    });
  }
})();

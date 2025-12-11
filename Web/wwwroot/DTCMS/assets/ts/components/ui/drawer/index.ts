const openSheetControllers: NodeListOf<HTMLInputElement> =
  document.querySelectorAll("[data-sheet-control]");

Array.from(openSheetControllers).forEach((openSheetController) => {
  let bottomsheetId = openSheetController.getAttribute("data-sheet-control");
  let bottomsheetHtml = openSheetController.getAttribute("data-html-append");
  const sheet = document.querySelector(`#${bottomsheetId}`);
  let notDragable: string = sheet.getAttribute("data-not-dragable");
  const sheetContents: HTMLElement = sheet.querySelector(".contents");
  const draggableArea: HTMLElement = sheet.querySelector(".draggable-area");
  const extraContent = sheetContents.querySelector("#extraContent");
  let minHeight = sheet.getAttribute("data-min-height");
  let sheetHeight: number;
  let dragPosition: number;
  if (bottomsheetHtml && extraContent) {
    extraContent.innerHTML = bottomsheetHtml;
  }

  const setSheetHeight = (value: number): void => {
    sheetHeight = Math.max(0, Math.min(100, value));
    sheetContents.style.height = `${sheetHeight}vh`;

    if (sheetHeight === 100) {
      sheetContents.classList.add("fullscreen");
    } else {
      sheetContents.classList.remove("fullscreen");
    }
  };

  const setIsSheetShown = (isShown: boolean): void => {
     sheet.setAttribute("aria-hidden", String(!isShown));
  };

  // Open the sheet when clicking the 'open sheet' button
  openSheetController.addEventListener("click", (): void => {
    if ((event.target as HTMLElement).nodeName !== "INPUT") {
      minHeight = minHeight ? minHeight : "50";
      sheetContents.style.height = `${minHeight}vh`;
      if (minHeight && parseInt(minHeight) === 100) {
        sheetContents.classList.add("fullscreen");
      } else {
        sheetContents.classList.remove("fullscreen");
      }
      setIsSheetShown(true);
    }
  });
  openSheetController.addEventListener("input", (): void => {
    let files = (event.target as HTMLInputElement).files;
    let imageINput: File = Array.from(files).find((file) =>
      file.type.includes("image")
    );
    if (files.length > 0 && imageINput) {
      minHeight = minHeight ? minHeight : "50";
      sheetContents.style.height = `${minHeight}vh`;
      if (minHeight && parseInt(minHeight) === 100) {
        sheetContents.classList.add("fullscreen");
      } else {
        sheetContents.classList.remove("fullscreen");
      }
      setIsSheetShown(true);
    }
  });

  // Hide the sheet when clicking the 'close' button
  let dismissBtns: NodeListOf<Element> = sheet.querySelectorAll(
    "[data-sheet-dismiss]"
  );

  if (dismissBtns) {
    dismissBtns.forEach((element) => {
      element.addEventListener("click", (): void => {
        setIsSheetShown(false);
      });
    });
  }

  // Hide the sheet when clicking the background
  sheet.querySelector(".overlay").addEventListener("click", (): void => {
    setIsSheetShown(false);
  });

  const isFocused: (el: any) => boolean = (element) =>
    document.activeElement === element;

  // Hide the sheet when pressing Escape if the target element
  // is not an input field
  window.addEventListener("keyup", (event): void => {
    const isSheetElementFocused =
      sheet.contains(event.target as HTMLElement) && isFocused(event.target);

    if (event.key === "Escape" && !isSheetElementFocused) {
      setIsSheetShown(false);
    }
  });

  const touchPosition = (event) => (event.touches ? event.touches[0] : event);

  const onDragStart = (event): void => {
    dragPosition = touchPosition(event).pageY;
    sheetContents.classList.add("not-selectable");
    draggableArea.style.cursor = document.body.style.cursor = "grabbing";
  };

  const onDragMove = (event): void => {
    if (dragPosition === undefined) return;

    const y = touchPosition(event).pageY;
    const deltaY = dragPosition - y;
    const deltaHeight = (deltaY / window.innerHeight) * 100;

    setSheetHeight(sheetHeight + deltaHeight);
    dragPosition = y;
  };

  const onDragEnd = () => {
    dragPosition = undefined;
    sheetContents.classList.remove("not-selectable");
    draggableArea.style.cursor = document.body.style.cursor = "";
    if (sheetHeight < 25) {
      setIsSheetShown(false);
    } else if (sheetHeight > 75) {
      setSheetHeight(100);
    } else {
      setSheetHeight(50);
    }
  };
  if (notDragable !== "true") {
    draggableArea.addEventListener("mousedown", onDragStart);
    draggableArea.addEventListener("touchstart", onDragStart);
    window.addEventListener("mousemove", onDragMove);
    window.addEventListener("touchmove", onDragMove);
    window.addEventListener("mouseup", onDragEnd);
    window.addEventListener("touchend", onDragEnd);
  }
});

import validate from "jquery-validation";
import $ from "jquery";
import Editor from "../ui/editor";
import * as Clipboard from "clipboard";
import { Notyf } from "notyf";
$.validate = validate;
(() => {
  let aTags: NodeListOf<HTMLAnchorElement> =
    document.querySelectorAll(".process__link");
  let btnTags: NodeListOf<HTMLButtonElement> =
    document.querySelectorAll("button");
  aTags.forEach((item) => {
    item.addEventListener("click", () => {
      setTimeout(() => {
        item.href = "javascript:void(0)";
        item.innerHTML = `
      
                <svg aria-hidden="true"
                    class="inline w-4 h-4  text-gray-200 animate-spin dark:text-gray-600 fill-blue-600"
                    viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path
                        d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z"
                        fill="currentColor" />
                    <path
                        d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z"
                        fill="currentFill" />
                </svg>
                <span class="sr-only">Loading...</span>
          `;
      }, 0);
    });
  });
  btnTags.forEach((btn) => {
    if (btn.form) {
      btn.addEventListener("click", () => {
        let check = $(btn.form).validate();
        let notSubmit = btn.form.getAttribute("data-not-submit");
        if (check.form() && notSubmit !== "true") {
          btn.form.submit();
          btn.disabled = true;
          btn.innerHTML = `
        <span>
        <div role="status">
            <svg aria-hidden="true"
                class="inline w-4 h-4 mr-2 text-gray-200 animate-spin dark:text-gray-600 fill-blue-600"
                viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path
                    d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z"
                    fill="currentColor" />
                <path
                    d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z"
                    fill="currentFill" />
            </svg>
            <span class="sr-only">Loading...</span>
        </div>
    </span>`;
        } else {
          return event.preventDefault();
        }
      });
    }
  });

  let container: NodeListOf<HTMLDivElement> =
    document.querySelectorAll(".editor__container");

  if (container) {
    Array.from(container).forEach((mainContainer) => {
      let editorContainer: HTMLDivElement =
        mainContainer.querySelector(".editorContainer");
      let editorLanguage: HTMLInputElement =
        mainContainer.querySelector(".editorLanguage");
      let editorValueSetter: HTMLInputElement =
        mainContainer.querySelector(".editorValue");
      let editorValueHTMLSetter: HTMLInputElement =
        mainContainer.querySelector(".editorHtmlValue");
      let defaultValue: HTMLInputElement = mainContainer.querySelector(
        ".editorDefaultValue"
      );
      Editor(
        editorContainer,
        editorLanguage.value,
        editorValueSetter,
        editorValueHTMLSetter,
        defaultValue
      );
    });
  }
})();

/// pagination Page size submit form

(<any>window).submitPageSize = () => {
  let Select = event.target as HTMLInputElement;
  let form = Select.parentElement as HTMLFormElement;
  form.submit();
};

/**
 * ClipboardJS
 */

let clipboard = new Clipboard.default(".clipboard__js");
clipboard.on("success", function (e) {
  /// متن خطا
  let ErrorMsg = e.trigger.attributes["data-clipboard-msg"].value;
  const notyf = new Notyf();
    const notification = notyf.success({
        message: ErrorMsg,
        duration: 2000,
        
    });
 setTimeout(() => {
  notyf.dismiss(notification);
 }, 2000);

  e.clearSelection();
});



document.getElementById("backButton")?.addEventListener("click", () => {
    const bodyElement = document.body;
    const siteUrl: string = bodyElement.getAttribute("base-url");
    const referrer: string = document.referrer;

    if (referrer.startsWith(siteUrl)) {
        // If the user navigated within the site, go back in history
        window.history.back();
    } else {
        // If the user came from an external site or opened directly, go to base URL
        window.location.href = siteUrl;
    }
});;
import initAccordion from "../../ui/accordion/index";
import { initDrawer, showDrawer, hideDrawer } from "../../ui/bottomsheet";
import Cookie from "js-cookie";
let currentPath = window.location.pathname;
const els = document.querySelectorAll("[data-sideroute]");
if (currentPath.toLowerCase().includes("/dtcms")) {

  ((<any>window).selectActiveSidebar = (): void => {
    els.forEach((element) => {

      let address = element.getAttribute("data-sideroute");
      if (address === "/dtcms") {
        if (address === currentPath.toLowerCase()) {
          element.className =
            "block w-100 mb-2 py-2 px-3 rounded-lg text-white text-sm bg-gray-900 hover:bg-gray-900";
        } else {
          element.className =
            "block w-100 mb-2 py-2 px-3 rounded-lg text-white text-sm bg-gray-800 hover:bg-gray-900";
        }
      } else {
        if (currentPath.toLowerCase() === address.toLocaleLowerCase()) {

          element.className =
          "block w-100 mb-2 py-2 px-3 rounded-lg text-white text-sm bg-gray-900 hover:bg-gray-900";
        
          // Find the nearest ancestor with data-accordion="collapse"
          const ancestor = element.closest('[data-accordion="collapse"]');

          if (ancestor) {
            // Find the h2 and div elements within the ancestor
            const h2 = ancestor.querySelector('h2[id^="accordion-collapse-heading-"]');
            const div = ancestor.querySelector('div[id^="accordion-collapse-body-"]');

            if (h2 && div) {
              let accordion = initAccordion([
                {
                  id: h2.id,
                  triggerEl: document.getElementById(h2.id),
                  targetEl: document.getElementById(div.id),
                },
              ]);

              accordion.open(h2.id);

            }
          }


        } else {
          element.className =
            "block w-100 mb-2 py-2 px-3 rounded-lg text-white text-sm bg-gray-800 hover:bg-gray-900";
        }
      }
    });


    let langDrawer = document.getElementById("languageDrawer");
    initDrawer(langDrawer);
  })();
}

(<any>window).toggleSideMenu = (): void => {
  let container: HTMLElement = document.getElementById("sideMenuContainer");
  container.classList.toggle("show__aside__menu");
};

(<any>window).logout = (): void => {
  window.location.href = "/auth/login";
};

(<any>window).showLanguageDrawer = (): void => {
  showDrawer();
};
(<any>window).hideLanguageDrawer = (): void => {
  hideDrawer();
};

/**
 * set language cookie based on the language
 */
(<any>window).changeLanguage = (lang?: string): void => {
  let date = new Date();

  date.setTime(date.getTime() + 365 * 24 * 60 * 60 * 1000); // expires in 1 year
  if (!lang)
    lang = (<HTMLInputElement>document.getElementById("systemLangSelector"))
      .value;
  document.cookie = `.AspNetCore.Culture=c=${lang}|uic=${lang}; expires=${date.toUTCString()}; path=/auth`;
  document.cookie = `.AspNetCore.Culture=c=${lang}|uic=${lang}; expires=${date.toUTCString()}; path=/Auth`;
  document.cookie = `.AspNetCore.Culture=c=${lang}|uic=${lang}; expires=${date.toUTCString()}; path=/AUTH`;
  document.cookie = `.AspNetCore.Culture=c=${lang}|uic=${lang}; expires=${date.toUTCString()}; path=/dtcms`;
  document.cookie = `.AspNetCore.Culture=c=${lang}|uic=${lang}; expires=${date.toUTCString()}; path=/Dtcms`;
  document.cookie = `.AspNetCore.Culture=c=${lang}|uic=${lang}; expires=${date.toUTCString()}; path=/DTCMS`;
  let siteLanguageSelector: HTMLSelectElement =
    document.querySelector("#siteLangSelector");
  Cookie.set("SiteLanguage", siteLanguageSelector.value);
  window.location.reload();
};
(<any>window).changeSiteLanguage = (lang: string): void => { };
(() => {
  if (document.getElementById('systemLangSelector')) {
    let panellangSelector: HTMLSelectElement = document.querySelector(
      "#systemLangSelector"
    );
    let siteLanguageSelector: HTMLSelectElement =
      document.querySelector("#siteLangSelector");
    panellangSelector.value = panellangSelector.getAttribute("data-lang");
    siteLanguageSelector.value = Cookie.get("SiteLanguage")
      ? Cookie.get("SiteLanguage")
      : "fa-IR";

  }


})();

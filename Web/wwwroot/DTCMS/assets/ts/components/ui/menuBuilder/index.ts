import { NestedMenu } from "../../../interfaces/panel/nestedMenu";
import Sortable from "sortablejs/modular/sortable.complete.esm.js";
let instances: Sortable[] = [];
let menuJsonSaverInput: HTMLInputElement = document.querySelector(
  "#nested__menu__value"
);

let editTarget: HTMLElement;
const createSortable = (item: HTMLElement) => {
  instances = [
    ...instances.filter((a) => a.el !== null),
    new Sortable(item, {
      group: "nested",
      animation: 150,
      fallbackOnBody: true,
      invertSwap: true,
      swapThreshold: 0.1,
      onMove: (e) => {
        setTimeout(() => {
          exportList();
        }, 500);
      },
    }),
  ];
};

(<any>window).addNewSortable = () => {
  let parent = document.querySelector("#nestedDemo");
  let prev = parent.innerHTML;

  instances.forEach((instance, index) => {
    instance.destroy();
    instances = instances.splice(index, 1);
  });
  let title: HTMLInputElement = document.querySelector("#menu__title");
  let link: HTMLInputElement = document.querySelector("#menu__link");
  parent.innerHTML = prev;
  parent.innerHTML += `<div class="rounded-sm list-group-item nested-2 p-3 border-solid border-gray-300 border-[1px] bg-white" ><div class="w-full flex flex-row-reverse items-center justify-between mb-1"><div class="flex">
  <svg class="cursor-pointer" onclick="handleRemoveMenuItem(this)" width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
  <path d="M19 7L18.1327 19.1425C18.0579 20.1891 17.187 21 16.1378 21H7.86224C6.81296 21 5.94208 20.1891 5.86732 19.1425L5 7M10 11V17M14 11V17M15 7V4C15 3.44772 14.5523 3 14 3H10C9.44772 3 9 3.44772 9 4V7M4 7H20" stroke="#DC2626" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
  </svg><svg  onclick="prepareEditSortable(this)" class="cursor-pointer mx-2" width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
  <path d="M20.2677 3.73223L20.9748 3.02513V3.02513L20.2677 3.73223ZM6.5 21.0355V22.0355C6.76522 22.0355 7.01957 21.9301 7.20711 21.7426L6.5 21.0355ZM3 21.0355H2C2 21.5878 2.44772 22.0355 3 22.0355V21.0355ZM3 17.4644L2.29289 16.7573C2.10536 16.9448 2 17.1992 2 17.4644H3ZM17.4393 4.43934C18.0251 3.85355 18.9748 3.85355 19.5606 4.43934L20.9748 3.02513C19.608 1.65829 17.3919 1.65829 16.0251 3.02513L17.4393 4.43934ZM19.5606 4.43934C20.1464 5.02513 20.1464 5.97487 19.5606 6.56066L20.9748 7.97487C22.3417 6.60804 22.3417 4.39196 20.9748 3.02513L19.5606 4.43934ZM19.5606 6.56066L5.79289 20.3284L7.20711 21.7426L20.9748 7.97487L19.5606 6.56066ZM6.5 20.0355H3V22.0355H6.5V20.0355ZM16.0251 3.02513L2.29289 16.7573L3.70711 18.1715L17.4393 4.43934L16.0251 3.02513ZM2 17.4644V21.0355H4V17.4644H2ZM14.5251 5.93934L18.0606 9.47487L19.4748 8.06066L15.9393 4.52513L14.5251 5.93934Z" fill="#374151"/>
  </svg></div>
  <div data-link-value="${link.value}" class="data__container text-gray-700">${title.value} </div> </div>
  
  <div class="list-group nested-sortable"></div>
 </div>`;
  title.value = null;
  link.value = null;
  generatePageElements();
};
(<any>window).editSortable = (item: HTMLElement) => {
  let parent = document.querySelector("#nestedDemo");
  let title: HTMLInputElement = document.querySelector("#menu__title__edit");
  let link: HTMLInputElement = document.querySelector("#menu__link__edit");

  let linkContainer: HTMLElement = editTarget.querySelector(".data__container");
  linkContainer.innerText = title.value;
  linkContainer.setAttribute("data-link-value", link.value);
  let prev = parent.innerHTML;
  instances.forEach((instance, index) => {
    instance.destroy();
    instances = instances.splice(index, 1);
  });
  parent.innerHTML = prev;
  title.value = null;
  link.value = null;
  generatePageElements();
};
(<any>window).prepareEditSortable = (menuItem: HTMLElement): void => {
  editTarget = menuItem.parentElement.parentElement.parentElement;
  let anchor: HTMLElement =
    menuItem.parentElement.parentElement.querySelector(".data__container");
  let menuTitle: HTMLInputElement =
    document.querySelector("#menu__title__edit");
  let menuLink: HTMLInputElement = document.querySelector("#menu__link__edit");
  let editBtn: HTMLButtonElement = document.querySelector(
    ".sortable__edit__btn"
  );
  editBtn.click();
  let title = anchor.innerText;
  let href = anchor.getAttribute("data-link-value");
  menuTitle.value = title;
  menuLink.value = href;
};

(<any>window).handleRemoveMenuItem = (menuItem: HTMLElement): void => {
  menuItem.parentElement.parentElement.parentElement.remove();
  exportList();
};
const generatePageElements = (): void => {
  let nestedSortables: HTMLElement[] = [].slice.call(
    document.querySelectorAll(".nested-sortable")
  );
  // Loop through each nested sortable element
  for (let i = 0; i < nestedSortables.length; i++) {
    createSortable(nestedSortables[i]);
  }
  setTimeout(() => {
    exportList();
  }, 500);
};

const exportList = () => {
  let nestedMenus: HTMLElement[] = [].slice.call(
    document.querySelectorAll(".list-group-item")
  );
  let menuJson: NestedMenu[] = [];

  let filteredMenus = nestedMenus.filter((item) =>
    item.querySelector(".data__container")
  );
  filteredMenus.forEach((item, index) => {
    let anchor: HTMLElement = item.querySelector(".data__container");
    let parent = item.parentElement.parentElement;
    let parentId = 0;
    if (parent && parent.classList.contains("list-group-item")) {
      let index = filteredMenus.indexOf(parent);
      parentId = menuJson[index].id;
    }
    menuJson.push({
      id: index + 1,
      text: anchor.innerText,
      link: anchor.getAttribute("data-link-value"),
      parentId: parentId,
    });
  });
  menuJsonSaverInput.value = JSON.stringify(menuJson);
};

const generatePageDefaults = () => {
  if (menuJsonSaverInput && menuJsonSaverInput.value.length) {
    let data: NestedMenu[] = JSON.parse(menuJsonSaverInput.value);
    let parent = document.querySelector("#nestedDemo");
    data.forEach((item) => {
      if (item.parentId === 0) {
        parent.innerHTML =
          parent.innerHTML +
          `
          <div id="menu__${item.id}" class="rounded-sm list-group-item nested-2 p-3 border-solid border-gray-300 border-[1px] bg-white" ><div class="w-full flex flex-row-reverse items-center justify-between mb-1"><div class="flex">
  <svg class="cursor-pointer" onclick="handleRemoveMenuItem(this)" width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
  <path d="M19 7L18.1327 19.1425C18.0579 20.1891 17.187 21 16.1378 21H7.86224C6.81296 21 5.94208 20.1891 5.86732 19.1425L5 7M10 11V17M14 11V17M15 7V4C15 3.44772 14.5523 3 14 3H10C9.44772 3 9 3.44772 9 4V7M4 7H20" stroke="#DC2626" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
  </svg><svg  onclick="prepareEditSortable(this)" class="cursor-pointer mx-2" width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
  <path d="M20.2677 3.73223L20.9748 3.02513V3.02513L20.2677 3.73223ZM6.5 21.0355V22.0355C6.76522 22.0355 7.01957 21.9301 7.20711 21.7426L6.5 21.0355ZM3 21.0355H2C2 21.5878 2.44772 22.0355 3 22.0355V21.0355ZM3 17.4644L2.29289 16.7573C2.10536 16.9448 2 17.1992 2 17.4644H3ZM17.4393 4.43934C18.0251 3.85355 18.9748 3.85355 19.5606 4.43934L20.9748 3.02513C19.608 1.65829 17.3919 1.65829 16.0251 3.02513L17.4393 4.43934ZM19.5606 4.43934C20.1464 5.02513 20.1464 5.97487 19.5606 6.56066L20.9748 7.97487C22.3417 6.60804 22.3417 4.39196 20.9748 3.02513L19.5606 4.43934ZM19.5606 6.56066L5.79289 20.3284L7.20711 21.7426L20.9748 7.97487L19.5606 6.56066ZM6.5 20.0355H3V22.0355H6.5V20.0355ZM16.0251 3.02513L2.29289 16.7573L3.70711 18.1715L17.4393 4.43934L16.0251 3.02513ZM2 17.4644V21.0355H4V17.4644H2ZM14.5251 5.93934L18.0606 9.47487L19.4748 8.06066L15.9393 4.52513L14.5251 5.93934Z" fill="#374151"/>
  </svg></div>
  <div data-link-value="${item.link}" class="data__container text-gray-700">${item.text} </div> </div>
  
  <div class="list-group nested-sortable"></div>
 </div>`;
      } else {
        let parent = document.querySelector(`#menu__${item.parentId}`);
        let socket = parent.querySelector(".nested-sortable");
        socket.innerHTML =
          socket.innerHTML +
          `
        <div id="menu__${item.id}" class="rounded-sm list-group-item nested-2 p-3 border-solid border-gray-300 border-[1px] bg-white" ><div class="w-full flex flex-row-reverse items-center justify-between mb-1"><div class="flex">
<svg class="cursor-pointer" onclick="handleRemoveMenuItem(this)" width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
<path d="M19 7L18.1327 19.1425C18.0579 20.1891 17.187 21 16.1378 21H7.86224C6.81296 21 5.94208 20.1891 5.86732 19.1425L5 7M10 11V17M14 11V17M15 7V4C15 3.44772 14.5523 3 14 3H10C9.44772 3 9 3.44772 9 4V7M4 7H20" stroke="#DC2626" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
</svg><svg  onclick="prepareEditSortable(this)" class="cursor-pointer mx-2" width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
<path d="M20.2677 3.73223L20.9748 3.02513V3.02513L20.2677 3.73223ZM6.5 21.0355V22.0355C6.76522 22.0355 7.01957 21.9301 7.20711 21.7426L6.5 21.0355ZM3 21.0355H2C2 21.5878 2.44772 22.0355 3 22.0355V21.0355ZM3 17.4644L2.29289 16.7573C2.10536 16.9448 2 17.1992 2 17.4644H3ZM17.4393 4.43934C18.0251 3.85355 18.9748 3.85355 19.5606 4.43934L20.9748 3.02513C19.608 1.65829 17.3919 1.65829 16.0251 3.02513L17.4393 4.43934ZM19.5606 4.43934C20.1464 5.02513 20.1464 5.97487 19.5606 6.56066L20.9748 7.97487C22.3417 6.60804 22.3417 4.39196 20.9748 3.02513L19.5606 4.43934ZM19.5606 6.56066L5.79289 20.3284L7.20711 21.7426L20.9748 7.97487L19.5606 6.56066ZM6.5 20.0355H3V22.0355H6.5V20.0355ZM16.0251 3.02513L2.29289 16.7573L3.70711 18.1715L17.4393 4.43934L16.0251 3.02513ZM2 17.4644V21.0355H4V17.4644H2ZM14.5251 5.93934L18.0606 9.47487L19.4748 8.06066L15.9393 4.52513L14.5251 5.93934Z" fill="#374151"/>
</svg></div>
<div data-link-value="${item.link}" class="data__container text-gray-700">${item.text} </div> </div>

<div class="list-group nested-sortable"></div>
</div>`;
      }
    });
    generatePageElements();
  }
};
(() => {
  generatePageDefaults();
})();

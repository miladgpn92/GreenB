import EditorJS, { OutputData } from "@editorjs/editorjs";
import NestedList from "@editorjs/nested-list";
// import ImageTool from "@editorjs/image";
import ImageTool from "editorjs-image-enhanced";
import { StyleInlineTool } from "editorjs-style";
import editorjsHTML from "editorjs-html";
import Table from "@editorjs/table";
import DragDrop from "editorjs-drag-drop";
import LinkWithTarget from "davia-editorjs-link-with-target";
import AttachesTool from "@editorjs/attaches";
import Alert from "editorjs-alert";
import Underline from "@editorjs/underline";
import Undo from "./undo/index";
import LinkAutocomplete from "@editorjs/link-autocomplete";
import VideoTool from "@weekwood/editorjs-video";
import Audio from "./audio";
import { replaceToolBarIcons, setSearchIcon } from "./icons";
// import Carousel from "@vtchinh/gallery-editorjs/dist/bundle";

import $ from "jquery";
// import validate from "jquery-validation";
import { uploadFile } from "../uploaderService";
// $.validate = validate;

const Header = require("@editorjs/header");
const SimpleImage = require("@editorjs/simple-image");
const Checklist = require("@editorjs/checklist");
const RawTool = require("@editorjs/raw");
const Quote = require("@editorjs/quote");
const ColorPlugin = require("editorjs-text-color-plugin");
const Paragraph = require("editorjs-paragraph-with-alignment");
const Iframe = require("@hammaadhrasheedh/editorjs-iframe");
const FontSize = require("editorjs-inline-font-size-tool");

let parsed: OutputData;

// Parse this block in editorjs-html
function customParser(block) {
  if (block.type === "checklist") {
    let parsedHtml = "";
    block.data.items.forEach((element) => {
      parsedHtml += `<label><input type="checkbox" checked="${element.checked}" /> ${element.text} </label>`;
    });
    return parsedHtml;
  }
  if (block.type === "simpleImage") {
    let parsedHtml = "";
    parsedHtml += `
     <img src="${block.data.url}" class="editor__simple__image" alt="${block.data.caption}"  />
     <div class="simple__image__caption">${block.data.caption}</div>
    `;
    return parsedHtml;
  }
  if (block.type === "raw") {
    let parsedHtml = "";

    parsedHtml += block.data.html;
    return parsedHtml;
  }
  if (block.type === "paragraph") {
    let parsedHtml = "";
    parsedHtml = `
     <div data-text-alignment="${block.data.alignment}">
       ${block.data.text as string}
     </div>`;
    return parsedHtml;
  }
  if (block.type === "table") {
    let parsedHtml = "<table>";
    block.data.content.forEach((element) => {
      let row = "<tr>";
      element.forEach((data) => {
        row += `<td>${data}</td>`;
      });
      row += "</tr>";
      parsedHtml += row;
    });
    parsedHtml += "</table>";
    return parsedHtml;
  }
  if (block.type === "code") {
    return "<h1>hi</h1>";
  }
  if (block.type === "audio") {
    return `
    <audio controls>
     <source src='${block.data.url}'>
      Your browser does not support the audio tag.
     </audio>`;
  }
  if (block.type === "video") {
    return `
    <video   controls>
     <source src='${block.data.file.url}'>
     Your browser does not support the video tag.
    </video>`;
  }
  if (block.type === "attaches") {
    return `<a class="editor__file__attachment" href="${block.data.file.url}">${block.data.title}</a>`;
  }
}

const Editor = (
  holoderClass: HTMLElement,
  lang: string,
  valueSetter: HTMLInputElement,
  htmlValueSetter: HTMLInputElement,
  defaultValue?: HTMLInputElement
): EditorJS => {
  const instance: EditorJS = new EditorJS({
    holder: holoderClass,
    tools: {
      iframe: Iframe,
      fontSize: FontSize,

      image: {
        class: ImageTool,
        config: {
          uploader: {
            async uploadByFile(file: File) {
              let res = await uploadFile(
                [file],
                "/media",
                () => {},
                () => {}
              );
              return {
                success: 1,
                file: {
                  url: res.data.data[0] as string,
                  // ... and any additional fields you want to store, such as width, height, color, extension, etc
                },
              };
            },
          },
        },
      },
      // carousel: {
      //   class: Carousel,
      //   config: {
      //     uploader: {
      //       async uploadByFile(file: File) {
      //         let res = await uploadFile(
      //           [file],
      //           "/media",
      //           () => {},
      //           () => {}
      //         );
      //         return {
      //           success: 1,
      //           file: {
      //             url: (res.data.data[0] as string).replace("7279", "4071"),
      //             // ... and any additional fields you want to store, such as width, height, color, extension, etc
      //           },
      //         };
      //       },
      //     },
      //   },
      // },

      video: {
        class: VideoTool,
        config: {
          uploader: {
            async uploadByFile(file: File) {
              return uploadFile(
                [file],
                "/media",
                () => {},
                () => {}
              ).then((res) => {
                return {
                  success: 1,
                  file: {
                    url: res.data.data[0] as string,
                    // ... and any additional fields you want to store, such as width, height, color, extension, etc
                  },
                };
              });
            },
          },
          player: {
            controls: true,
            autoplay: false,
          },
        },
      },
      attaches: {
        class: AttachesTool,
        config: {
          uploader: {
            async uploadByFile(file: File) {
              let res = await uploadFile(
                [file],
                "/media",
                () => {},
                () => {}
              );
              return {
                success: 1,
                file: {
                  url: res.data.data[0],
                  // ... and any additional fields you want to store, such as width, height, color, extension, etc
                },
              };
            },
          },
        },
      },
      audio: {
        class: Audio,
        config: {
          saveServer: async (file: File) => {
            let res = await uploadFile(
              [file],
              "/media",
              () => {},
              () => {}
            );
            return {
              data: {
                url: res.data.data[0] as string,
                name: file.name,
                id: "123",
              },
            };
          },
        },
      },
      table: {
        class: Table,
        inlineToolbar: true,
      },
      raw: RawTool,
      list: {
        class: NestedList,
        inlineToolbar: true,
        config: {
          defaultStyle: "unordered",
        },
      },
      quote: {
        class: Quote,
        inlineToolbar: true,
        shortcut: "CMD+SHIFT+O",
        config: {
          quotePlaceholder:
            lang === "fa-IR" ? "یک جمله نقل قول وارد کنید" : "Enter a quote",
          captionPlaceholder:
            lang === "fa-IR" ? "گوینده نقل قول" : "Quote's author",
        },
      },
      header: Header,

      link: {
        class: LinkAutocomplete,
        config: {
          endpoint: "http://localhost:4071/",
          queryParam: "search",
        },
      },
      underline: Underline,

      style: StyleInlineTool,
      paragraph: {
        class: Paragraph,
        inlineToolbar: true,
        config: {
          defaultAlignment: lang === "fa-IR" ? "right" : "left",
        },
      },

      Color: {
        class: ColorPlugin, // if load from CDN, please try: window.ColorPlugin
        config: {
          colorCollections: [
            "#EC7878",
            "#9C27B0",
            "#673AB7",
            "#3F51B5",
            "#0070FF",
            "#03A9F4",
            "#00BCD4",
            "#4CAF50",
            "#8BC34A",
            "#CDDC39",
            "#FFF",
          ],
          defaultColor: "#FF1300",
          type: "text",
          customPicker: true, // add a button to allow selecting any colour
        },
      },
      Marker: {
        class: ColorPlugin, // if load from CDN, please try: window.ColorPlugin
        config: {
          defaultColor: "#FFBF00",
          type: "marker",
          icon: `<svg fill="#000000" height="200px" width="200px" version="1.1" id="Icons" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 32 32" xml:space="preserve"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <g> <path d="M17.6,6L6.9,16.7c-0.2,0.2-0.3,0.4-0.3,0.6L6,23.9c0,0.3,0.1,0.6,0.3,0.8C6.5,24.9,6.7,25,7,25c0,0,0.1,0,0.1,0l6.6-0.6 c0.2,0,0.5-0.1,0.6-0.3L25,13.4L17.6,6z"></path> <path d="M26.4,12l1.4-1.4c1.2-1.2,1.1-3.1-0.1-4.3l-3-3c-0.6-0.6-1.3-0.9-2.2-0.9c-0.8,0-1.6,0.3-2.2,0.9L19,4.6L26.4,12z"></path> </g> <g> <path d="M28,29H4c-0.6,0-1-0.4-1-1s0.4-1,1-1h24c0.6,0,1,0.4,1,1S28.6,29,28,29z"></path> </g> </g></svg>`,
        },
      },

      simpleImage: SimpleImage,

      checklist: {
        class: Checklist,
        inlineToolbar: true,
      },
    },

    onReady: () => {
      new DragDrop({ configuration: instance });
      new Undo({
        editor: instance,
        maxLength: 300,
        onUpdate: () => {},
      });
      setSearchIcon();
      replaceToolBarIcons();
      $(valueSetter.form).each(function () {
        if ($(this).data("validator"))
          $(this).data("validator").settings.ignore =
            ".editorContainer * , .codex-editor *";
      });
      $(valueSetter).on("change", () => {
        $(valueSetter.form).valid();
      });
      instance.save().then(() => {
        if (defaultValue && defaultValue.value && defaultValue.value !== "")
          parsed = JSON.parse(defaultValue.value);
      });
    },
    onChange: function () {
      setSearchIcon();
      replaceToolBarIcons();

      instance.save().then((data) => {
        const edjsParser = editorjsHTML({
          table: customParser,
          raw: customParser,
          checklist: customParser,
          audio: customParser,
          video: customParser,
          paragraph: customParser,
          attaches: customParser,
          alert: customParser,
          simpleImage: customParser,
        });
        let html = edjsParser.parse(data);
        valueSetter.value = JSON.stringify(data);
        $(valueSetter).trigger("change");
        htmlValueSetter.value = "";
        html.forEach((htmlBlock) => {
          htmlValueSetter.value += htmlBlock;
        });
      });
    },
    i18n:
      lang === "fa-IR"
        ? {
            messages: {
              /**
               * Other below: translation of different UI components of the editor.js core
               */
              ui: {
                blockTunes: {
                  toggler: {
                    "Click to tune": "برای تنظیم کلیک کنید",
                    "or drag to move": "или перетащите",
                  },
                },
                inlineToolbar: {
                  converter: {
                    "Convert to": "تبدیل به ...",
                  },
                },
                toolbar: {
                  toolbox: {
                    Add: "اضافه کردن",
                  },
                },
                popover: {
                  Filter: "جستجو",
                  "Nothing found": "چیزی پیدا نشد",
                },
              },

              /**
               * Section for translation Tool Names: both block and inline tools
               */
              toolNames: {
                Text: "متن",
                link: {
                  class: LinkWithTarget,
                },
                Heading: "عنوان",
                List: "لیست",
                Warning: "هشدار",
                Checklist: "چک لیست",
                Quote: "نقل قول",
                Code: "کُد",
                "Raw HTML": "خام HTML",
                Table: "جدول",
                Marker: "مارکر",
                "Link Autocomplete": "لینک",
                "Font Size": "سایز فونت",
                Color: "رنگ",
                Bold: "بُلد",
                Italic: "ایتالیک",
                Image: "عکس",
                Alert: "هشدار",
                Attachment: "ضمیمه",
                Video: "ویدئو",
                Audio: "صدا",
                Style: "استایل دهی",
                Underline: "خط زیر نوشته",
              },

              /**
               * Section for passing translations to the external tools classes
               */
              tools: {
                /**
                 * Each subsection is the i18n dictionary that will be passed to the corresponded plugin
                 * The name of a plugin should be equal the name you specify in the 'tool' section for that plugin
                 */
                warning: {
                  // <-- 'Warning' tool will accept this dictionary section
                  Title: "عنوان",
                  Message: "پیام",
                },

                /**
                 * Link is the internal Inline Tool
                 */
                link: {
                  "Add a link": "یک لینک اضافه کنید",
                },
                /**
                 * The "stub" is an internal block tool, used to fit blocks that does not have the corresponded plugin
                 */
                stub: {
                  "The block can not be displayed correctly.":
                    "بلاک نمیتواند به درستی نمایش داده شود",
                },
                image: {
                  Caption: "عنوان",
                  "Select an Image": "انتخاب عکس",
                  "Embed an Image": "یارگزاری عکس با لینک",

                  "With border": "مرز بندی عکس",
                  "Stretch image": "کشیده شدن عکس",
                  "With link": "اضافه کردن لینک",
                  "With background": "اضافه کردن پس زمینه",
                  Link: "لینک",
                  Alt: "متن جایگزین",
                  ALT: "متن جایگزین",
                  CAPTION: "عنوان",
                },

                table: {
                  "Add column to left": "اضافه کردن ستون از سمت چپ",
                  "Add column to right": "اضافه کردن ستون از سمت راست",
                  "Delete column": "حذف ستون",
                  "Add row above": "اضافه کردن سطر از بالا",
                  "Add row below": "اضافه کردن سطر از پایین",
                  "Delete row": "حذف سطر",
                  "With headings": "با سربرگ",
                  "Without headings": "بدون سربرگ",
                  Heading: "سربرک",
                },
                video: {
                  "Select an Video": "یک ویدئو انتخاب کنید",
                },
                attaches: {
                  " Select file to upload": "یک فایل برای بارگزاری انتخاب کنید",
                  "File title": "عنوان فایل",
                },
                code: {
                  "Enter a code": "کد را وارد کنید",
                  "Enter HTML code": "کد را وارد کنید",
                },
                raw: {
                  "Enter HTML code": "کد را وارد کنید",
                },
                audio: {
                  "Select file": "انتخاب فایل",
                  "Playback speed": "سرعت پخش",
                },

                header: {
                  Header: "عنوان",
                  "Heading 1": "عنوان 1",
                  "Heading 2": "عنوان 2",
                  "Heading 3": "عنوان 3",
                  "Heading 4": "عنوان 4",
                  "Heading 5": "عنوان 5",
                  "Heading 6": "عنوان 6",
                },

                paragraph: {
                  "Enter something": "متن مورد نظر",
                },
                list: {
                  Ordered: "مرتب",
                  Unordered: "بدون ترتیب",
                },
                alert: {
                  Primary: "اولیه",
                  Secondary: "ثانویه",
                  Info: "اطلاع",
                  Success: "موفقیت",
                  Warning: "هشدار",
                  Danger: "خطر",
                  Light: "روشن",
                  Dark: "تاریک",
                  Right: "راست چین",
                  Left: "چپ چین",
                  Center: "وسط چین",
                },
              },

              /**
               * Section allows to translate Block Tunes
               */
              blockTunes: {
                /**
                 * Each subsection is the i18n dictionary that will be passed to the corresponded Block Tune plugin
                 * The name of a plugin should be equal the name you specify in the 'tunes' section for that plugin
                 *
                 * Also, there are few internal block tunes: "delete", "moveUp" and "moveDown"
                 */

                delete: {
                  Delete: "حذف",
                  "Click to delete": "تایید حذف",
                },
                moveUp: {
                  "Move up": "انتقال به بالا",
                },
                moveDown: {
                  "Move down": "انتقال به پایین",
                },
              },
            },
          }
        : null,
    data:
      defaultValue.value && defaultValue.value !== ""
        ? JSON.parse(defaultValue.value)
        : null,
  });
  return instance;
};

export default Editor;

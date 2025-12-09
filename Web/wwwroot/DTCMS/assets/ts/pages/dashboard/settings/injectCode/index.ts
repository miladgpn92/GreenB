import { basicSetup, EditorView } from "codemirror";
import { EditorState, Compartment } from "@codemirror/state";
import { html } from "@codemirror/lang-html";
import { dracula } from "thememirror";

let instances: NodeListOf<HTMLElement> =
  document.querySelectorAll(".code__container");

Array.from(instances).forEach((item) => {
  let language = new Compartment(),
    tabSize = new Compartment();
  let inputId = item.getAttribute("data-codinput-target");
  let input: HTMLInputElement = document.querySelector(`#${inputId}`);
  let state = EditorState.create({
    doc: input.value,
    extensions: [
      dracula,
      basicSetup,
      language.of(html()),
      tabSize.of(EditorState.tabSize.of(8)),
      EditorView.updateListener.of((update) => {
        if (update.docChanged) {
          input.value = update.state.doc.toString();
         }
      }),
    ],
  });

  new EditorView({
    state,
    parent: item,
  });
});

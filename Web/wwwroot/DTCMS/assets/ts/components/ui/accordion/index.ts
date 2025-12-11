import { Accordion } from "flowbite";
import type {
  AccordionOptions,
  AccordionItem,
  AccordionInterface,
} from "flowbite";
const options: AccordionOptions = {
  alwaysOpen: false,

  onOpen: (item) => {},
  onClose: (item) => {},
  onToggle: (item) => {},
};
const initAccordion = (accordionItems: AccordionItem[]): AccordionInterface => {
  const accordion: AccordionInterface = new Accordion(accordionItems, options);
  return accordion;
};

export default initAccordion;

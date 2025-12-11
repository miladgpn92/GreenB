import $ from "jquery";
(<any>window).updateSeoObjects = (
  field: "SeoTitle" | "SeoDesc" | "SeoImage",
  id: number,
  targetHtml?: HTMLInputElement
) => {
  let seoValues: HTMLInputElement =
    document.querySelector("#seo__list__values");
    
  let parsed: {
    id: number;
    path: string;
    SeoTitle: string;
    SeoDesc: string;
    SeoImage: string;
  }[] = JSON.parse(seoValues.value);
  let target = parsed.find((a) => a.id === id);
  if (targetHtml) {
    target[field] = targetHtml.value;
  } else {
    target[field] = (
      event.target as HTMLInputElement | HTMLTextAreaElement
    ).value;
  }
  parsed = parsed.filter((a) => a.id !== id);
  parsed.push(target);
  seoValues.value = JSON.stringify(parsed);
};

(<any>window).checkSeoImage = (className: string) => {
  let target = document.querySelector(`.${className}`) as HTMLInputElement;
  if (target && target.value) {
    $(target).trigger("change");
  }
};

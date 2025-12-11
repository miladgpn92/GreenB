import Swiper from "swiper";
import { Pagination, Autoplay, Thumbs, Navigation } from "swiper/modules";
(() => {
  let sliderContainer: NodeListOf<HTMLElement> =
    document.querySelectorAll(".slider__container");
  Array.from(sliderContainer).forEach((item) => {
    let slidesPerview = item.getAttribute("data-slide-per-view");
    let breakpoints: string = item.getAttribute("data-break-point");
    let thumbsAtt = item.getAttribute("data-thumb-target");
    let autoPlay = item.getAttribute("data-auto-play");
    let navigation = item.getAttribute("data-navigation");
    let vertical = item.getAttribute("data-vertical");
    let thumbs = thumbsAtt
      ? new Swiper(`.${thumbsAtt}`, {
          spaceBetween: 10,
          slidesPerView: 6,
          freeMode: true,
          watchSlidesProgress: true,
        })
      : null;
    new Swiper(item, {
      // configure Swiper to use modules
      slidesPerView: slidesPerview ? parseFloat(slidesPerview) : 1,
      modules: [Pagination, Autoplay, Thumbs, Navigation],
      pagination: {
        el: ".swiper-pagination",
        clickable: true,
        dynamicBullets: true,
      },
      
      navigation: navigation
        ? {
            nextEl: ".swiper-button-next",
            prevEl: ".swiper-button-prev",
          }
        : false,
      loop: true,
      thumbs: thumbs
        ? {
            swiper: thumbs,
          }
        : null,
      autoplay: autoPlay
        ? {
            delay: parseInt(autoPlay),
            disableOnInteraction: false,
          }
            : false,
        direction: vertical ? 'vertical' : 'horizontal',
      breakpoints: breakpoints ? JSON.parse(breakpoints) : null,
    });
  });
})();

import {
    Swiper,

} from 'swiper';

import { Navigation, Pagination, Scrollbar, A11y, Autoplay } from 'swiper/modules';
Swiper.use([Navigation, Pagination, Scrollbar, A11y, Autoplay]);

import * as L from "leaflet";
import lightGallery from 'lightgallery';
import lgThumbnail from 'lightgallery/plugins/thumbnail';
import lgZoom from 'lightgallery/plugins/zoom';
import lgVideo from 'lightgallery/plugins/video';

import {
    Player,

} from 'shikwasa';

document.addEventListener('DOMContentLoaded', function () {
    const body = document.body;
    const lightModeButton = document.querySelector('button[data-mode="light-mode"]');
    const darkModeButton = document.querySelector('button[data-mode="dark-mode"]');
    const themedElements = document.querySelectorAll('[data-theme="light"], [data-theme="dark"]');
    const mobileMenuToggle = document.querySelector('[data-mobile-menu-toggle]');
    const mobileMenu = document.querySelector('[data-mobile-menu]');
    const mobileMenuLinks = mobileMenu ? mobileMenu.querySelectorAll('a') : [];

    function closeMobileMenu() {
        if (!mobileMenu || !mobileMenuToggle) {
            return;
        }

        mobileMenu.classList.add('hidden');
        mobileMenuToggle.setAttribute('aria-expanded', 'false');
    }

    if (mobileMenuToggle && mobileMenu) {
        const syncMenuWithViewport = () => {
            if (window.innerWidth >= 1024) {
                mobileMenu.classList.remove('hidden');
                mobileMenuToggle.setAttribute('aria-expanded', 'false');
            } else {
                mobileMenu.classList.add('hidden');
                mobileMenuToggle.setAttribute('aria-expanded', 'false');
            }
        };

        mobileMenuToggle.addEventListener('click', () => {
            const isHidden = mobileMenu.classList.contains('hidden');

            if (isHidden) {
                mobileMenu.classList.remove('hidden');
                mobileMenuToggle.setAttribute('aria-expanded', 'true');
            } else {
                mobileMenu.classList.add('hidden');
                mobileMenuToggle.setAttribute('aria-expanded', 'false');
            }
        });

        mobileMenuLinks.forEach((link) => {
            link.addEventListener('click', closeMobileMenu);
        });

        window.addEventListener('resize', syncMenuWithViewport);
        syncMenuWithViewport();
    }

    // Function to update theme attributes
    function updateTheme(theme) {
        if (theme === 'dark') {
            body.classList.add('dark');
            themedElements.forEach(el => {
                el.setAttribute('data-theme', 'dark');
                el.classList.add('dark');
            });
        } else {
            body.classList.remove('dark');
            themedElements.forEach(el => {
                el.setAttribute('data-theme', 'light');
                el.classList.remove('dark');
            });
        }
    }

    // Check localStorage for theme preference
    const savedTheme = localStorage.getItem('theme') || 'light';
    updateTheme(savedTheme);

    if (savedTheme === 'dark') {
        darkModeButton.classList.add('d-none');
        lightModeButton.classList.remove('d-none');
    } else {
        lightModeButton.classList.add('d-none');
        darkModeButton.classList.remove('d-none');
    }

    // Event listener for light mode button
    lightModeButton.addEventListener('click', function () {
        localStorage.setItem('theme', 'light');
        updateTheme('light');
        lightModeButton.classList.add('d-none');
        darkModeButton.classList.remove('d-none');
    });

    // Event listener for dark mode button
    darkModeButton.addEventListener('click', function () {
        localStorage.setItem('theme', 'dark');
        updateTheme('dark');
        darkModeButton.classList.add('d-none');
        lightModeButton.classList.remove('d-none');
    });
});

// services slider 
if ('.services-slider-active') {
    var services_slider_active = new Swiper(".services-slider-active", {
        slidesPerView: 1,
        loop: true,
        autoplay: true,
        spaceBetween: 10,
        speed: 5000,
        autoplay: {
            delay: 0,
            disableOnInteraction: false,
            pauseOnMouseEnter: true,
        },
        breakpoints: {
            640: {
              slidesPerView: 2,
             },
            768: {
              slidesPerView: 3,
             },
            1024: {
              slidesPerView: 4,
             },
          },
    });
}

// client slider 
if ('.client-slider-active') {
    var client_slider_active = new Swiper(".client-slider-active", {
        slidesPerView: 'auto',
        loop: true,
        autoplay: true,
        spaceBetween: 130,
        speed: 3000,
        autoplay: {
            delay: 1,
        },
    });
}

// work slider 
if ('.work-slider-active') {
    var work_slider_active = new Swiper(".work-slider-active", {
        slidesPerView: 'auto',
        loop: true,
        autoplay: true,
        spaceBetween: 10,
        speed: 5000,
        autoplay: {
            delay: 0,
            disableOnInteraction: false,
            pauseOnMouseEnter: true,
        },
    });

  

}

// testimonial slider 
if ('.testimonial-slider-active') {
    var testimonial_slider_active = new Swiper(".testimonial-slider-active", {
        effect: "cards",
        grabCursor: true,
        perSlideOffset: 50,
        rotate: false,
        perSlideRotate: 10,
        navigation: {
            prevEl: ".testimonial-button-prev",
            nextEl: ".testimonial-button-next",
        },
    });
}

// testimonial slider
if (('.testimonial-slider').length) {
    var testimonial_slider = new Swiper(".testimonial-slider", {
        effect: "cards",
        grabCursor: true,
        perSlideOffset: 50,
        rotate: false,
        perSlideRotate: 10,
        navigation: {
            prevEl: ".testimonial-button-prev",
            nextEl: ".testimonial-button-next",
        },
    });
}


document.addEventListener("DOMContentLoaded", function () {
    const title = document.getElementById("article__cat__title");
    const articleList = document.getElementById("article-list");
    const plusIcon = document.getElementById("plus-icon");
    const minusIcon = document.getElementById("minus-icon");
    if (title) {

        title.addEventListener("click", function () {
            if (window.innerWidth < 992) {
                const isHidden = articleList.style.display === "none" || articleList.style.display === "";
                articleList.style.display = isHidden ? "block" : "none";
                plusIcon.style.display = isHidden ? "none" : "inline";
                minusIcon.style.display = isHidden ? "inline" : "none";
            }
        });
    }

});

// Wait for the DOM to load
document.addEventListener("DOMContentLoaded", () => {
    // Select the container element
    const mapContainer = document.getElementById("map-container");

    // Ensure the container exists and has data attributes
    if (mapContainer && mapContainer.dataset.lat && mapContainer.dataset.lng) {
        const lat = parseFloat(mapContainer.dataset.lat);
        const lng = parseFloat(mapContainer.dataset.lng);
        const title = mapContainer.dataset.title;

        // Check if lat and lng are valid numbers
        if (!isNaN(lat) && !isNaN(lng)) {
            // Initialize the map
            const map = L.map(mapContainer).setView([lat, lng], 18);

            // Add a tile layer (OpenStreetMap in this case)
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            }).addTo(map);

            // Create a custom icon
            const customIcon = L.icon({
                iconUrl: '/template/assets/imgs/icon/marker.svg', // Replace with your icon path
                iconSize: [32, 32], // Size of the icon
                iconAnchor: [16, 32], // Point of the icon which corresponds to marker's location
                popupAnchor: [0, -32], // Point from which the popup should open relative to the iconAnchor
            });

            // Add a marker with the custom icon
            L.marker([lat, lng], { icon: customIcon }).addTo(map)
                .bindPopup(`<b>${title}</b>`)
                .openPopup();
        } else {
            console.error("Invalid latitude or longitude values in data attributes.");
        }
    }
});

//Gallery
// Initialize gallery
const galleryContainer = document.getElementById('gallery');

if (galleryContainer) {
    const lightGalleryInstance = lightGallery(galleryContainer, {
        plugins: [lgThumbnail, lgZoom, lgVideo],
        selector: 'a',
        thumbnail: true,
        zoom: false,
        videoMaxWidth: '1000px',
        // Optional: autoplay video
        videoAutoplay: false,
        // Optional: mute video by default
        videoMute: true
    });




    document.addEventListener('DOMContentLoaded', () => {
        // Find all elements with data-video attribute
        const videoElements = document.querySelectorAll('[data-video]');

        videoElements.forEach(element => {
            // Find the image with class video__img within this element
            const videoImg = element.querySelector('.video__img');

            if (videoImg) {
                // Get the video source
                let videoSrc;

                // Check if data-video is a JSON string
                try {
                    const videoData = JSON.parse(element.getAttribute('data-video'));
                    // If it's a complex object with sources
                    if (videoData.source && videoData.source[0] && videoData.source[0].src) {
                        videoSrc = videoData.source[0].src;
                    }
                } catch {
                    // If it's a simple string URL
                    videoSrc = element.getAttribute('data-video');
                }

                // If we have a video source
                if (videoSrc) {
                    // Create a video element to extract the thumbnail
                    const video = document.createElement('video');
                    video.src = videoSrc;

                    video.addEventListener('loadedmetadata', () => {
                        // Choose the specific second (default to 2 seconds)
                        const specificSecond = 3; // You can change this value

                        // Seek to the specific time
                        video.currentTime = specificSecond;

                        video.addEventListener('seeked', () => {
                            // Create canvas to capture frame
                            const canvas = document.createElement('canvas');
                            canvas.width = video.videoWidth;
                            canvas.height = video.videoHeight;

                            const ctx = canvas.getContext('2d');
                            ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

                            // Set the image source to the specific frame
                            videoImg.src = canvas.toDataURL('image/jpeg');
                        }, { once: true });
                    });

                    // Preload the video
                    video.load();
                }
            }
        });
    });

}


document.querySelectorAll('.audio__player').forEach(container => {
    new Player({
        container: () => container,
        audio: {
            title: container.dataset.title,
            artist: container.dataset.artist,
            cover: container.dataset.cover,
            src: container.dataset.src,
        },
        themeColor:'#F19100'
    });
});

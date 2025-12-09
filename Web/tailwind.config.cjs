const colors = require("tailwindcss/colors");

module.exports = {
  content: [
    "./Pages/**/*.{js,cshtml,ts}",
    "./TagHelpers/**/*.{js,cshtml,ts,cs}",
    "./wwwroot/DTCMS/assets/ts/**/*.ts",
    "./wwwroot/Admin/assets/ts/**/*.ts",
    "./node_modules/flowbite/**/*.js",
  ],
  theme: {
    extend: {
      screens: {
        xd: "992px",
      },
      gradientColorStops: {
        "image-upload-gradient":
          "background: linear-gradient(180deg, rgba(31, 41, 55, 0) 0%, rgba(24, 32, 47, 0.526042) 52.6%, rgba(21, 28, 43, 0.88) 74.48%, #111827 100%);",
      },
     
    },
    backgroundPosition: {
      'right-center':"background-position: right 5px top 50%;"
    },
    colors: {
      ...colors,
    },
  },
  plugins: [require("@tailwindcss/forms"), require("flowbite/plugin")],
};

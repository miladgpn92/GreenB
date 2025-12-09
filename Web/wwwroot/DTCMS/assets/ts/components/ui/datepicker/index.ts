import { jalaliToGregorian, jalaliToMiladi } from "../../../utils";
import $ from "jquery";
import daterangepicker from "./lib/persian";
let moment = require("moment-jalaali");

$.daterangepicker = daterangepicker;
(() => {
    if ($(".data-date-picker")) {
        Array.from($(".data-date-picker") as NodeListOf<HTMLInputElement>).forEach(
            (item) => {
                let hiddenInputId = item.getAttribute("data-hidden-input");
                let hiddenInput: HTMLInputElement = document.querySelector(
                    `#${hiddenInputId}`
                );
                console.log(hiddenInput.value);
                if (
                    hiddenInput.value
                ) {
                  
                    const parsedDate = moment(hiddenInput.value, "YYYY/M/D H:mm:ss");
                    item.value = parsedDate.format("YYYY/M/D");
                   
                }
                $(item).daterangepicker({
                    alwaysShowCalendars: true,
                    singleDatePicker: true,
                    showCustomRangeLabel: true,
                    showDropdowns: true,
                    minYear: 1901,
                    // minDate: new Date(),
                    persian: {
                        enable: true,
                        persianDigits: true,
                    },
                    locale: {
                        direction: "rtl",
                        firstDay: 0,
                        format: "jYYYY-jMM-jDD",
                        applyLabel: "اعمال",
                        cancelLabel: "لغو",
                        monthNames: [
                            "فروردین",
                            "اردیبهشت",
                            "خرداد",
                            "تیر",
                            "مرداد",
                            "شهریور",
                            "مهر",
                            "آبان",
                            "آذر",
                            "دی",
                            "بهمن",
                            "اسفند",
                        ],
                        daysOfWeek: ["ش", "ی", "د", "س", "چ", "پ", "ج"],
                    },
                });
                $(item).on("apply.daterangepicker", function (ev, picker) {
                    let date = $(item).val().split("-");

                    hiddenInput.value = [date[0], date[1], date[2]].join("/");
                });
            }
        );
    }

  
})();
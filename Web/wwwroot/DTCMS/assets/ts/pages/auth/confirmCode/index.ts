import { sendCode, verifyUser } from "../../../services/authService";
import { getUrlParameters } from "../../../utils";
import { showToast } from "../../../utils";
let timerContainer = document.getElementById("timeCounterContainer");
let resendButton = document.getElementById("resendCodeButton");
const code = <HTMLInputElement>(
  document.getElementById("model_VerificationCode")
);
let userName = getUrlParameters("username");
const createCounter = (): void => {
  timerContainer.classList.remove("hidden");
  resendButton.classList.add("hidden");
  // Set the date we're counting down to
  let countDownDate = new Date().getTime() + 90000;

  // Update the count down every 1 second
  let timerInterval = setInterval(() => {
    // Get today's date and time
    let now = new Date().getTime();

    // Find the distance between now and the count down date
    let distance = countDownDate - now;

    // Time calculations for days, hours, minutes and seconds
    let minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    let seconds = Math.floor((distance % (1000 * 60)) / 1000);
    seconds = seconds >= 0 ? seconds : 0;
    minutes = minutes >= 0 ? minutes : 0;

    // Display the result in the element with id="demo"
    timerContainer.innerHTML = minutes + ":" + seconds;

    // If the count down is finished, write some text
    if (distance <= 0) {
      clearInterval(timerInterval);
      timerContainer.classList.add("hidden");
      resendButton.classList.remove("hidden");
    }
  }, 1000);
};
//(<any>window).verifyPhoneForgotPassword = async (
//  btn: HTMLButtonElement
//): Promise<void> => {
//  event.preventDefault();
//  if (code.value.length === 4) {
//    try {
//      await verifyUser({
//        userName: userName,
//        verificationCode: code.value,
//      });
//      btn.form.submit();
//    } catch (ex) {
//      if (ex.response && ex.response.data && ex.response.data.message) {
//        showToast({
//          toastMessage: ex.response.data.message,
//          toastVariation: "error",
//          duration: 5,
//        });
//      }
//    }
//  }
//};

(<any>window).resendResetCode = async (): Promise<void> => {
  try {
    await sendCode({ userName: userName, isForgotpass: true });
    createCounter();
  } catch (ex) {
    if (ex.response && ex.response.data && ex.response.data.message) {
      showToast({
        toastMessage: ex.response.data.message,
        toastVariation: "error",
        duration: 5,
      });
    }
  }
};

if (timerContainer) {
  if (!userName) window.location.href = "/auth/forgotpassword";

  (() => createCounter())();
}

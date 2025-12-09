
const clockElement = document.getElementById('clock__container');
if (clockElement) {
    function updateClock() {
        const now = new Date();
        const hours = now.getHours().toString().padStart(2, '0');
        const minutes = now.getMinutes().toString().padStart(2, '0');
        const seconds = now.getSeconds().toString().padStart(2, '0');


        if (clockElement) {
            clockElement.innerHTML = `${hours}:${minutes}:${seconds}`;
        }
    }

    // Update the clock every second
    setInterval(updateClock, 1000);

    // Initial update
    updateClock();
}

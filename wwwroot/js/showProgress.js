document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("progress").forEach(bar => {

        const id = bar.id.split("-").pop();
        const runtime = Number(bar.dataset.runtime);

        if (!runtime) {
            bar.style.visibility = "hidden";
            return;
        }

        const serverProgress = Number(bar.dataset.progress) || 0;
        const localProgress = Number(localStorage.getItem(`${id}_timestamp`)) || 0;

        // Pick the most recent progress
        const progress = Math.max(serverProgress, localProgress);

        if (progress <= 0) {
            bar.style.visibility = "hidden";
            return;
        }

        // Save best progress locally
        localStorage.setItem(`${id}_timestamp`, progress);

        // Convert to %
        const percent = Math.floor((progress / 60 / runtime) * 100);

        if (percent > 0) {
            bar.value = percent;
            bar.style.visibility = "visible";
        } else {
            bar.style.visibility = "hidden";
        }
    });
});
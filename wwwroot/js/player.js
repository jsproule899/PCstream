const subtitles = document.getElementById("subtitles");
const video = document.getElementById("video");
const playerContainer = document.getElementById("player-container");
const playerControls = document.getElementById("video-controls");
const progess = document.getElementById("progress-bar");
const progessSlider = document.getElementById("progress-slider");
const restart = document.getElementById("restart");
const backButton = document.getElementById("back");
const fullscreen = document.getElementById("fullscreen");
const fullscreenIcon = document.getElementById("fullscreen-icon");
const currentTime = document.getElementById("current-time");
const duration = document.getElementById("duration");
const playPauseIcon = document.getElementById("play-pause-icon");
const playPauseButton = document.getElementById("play-pause-btn");
const rewind = document.getElementById("rewind-btn");
const fastforward = document.getElementById("forward-btn");
const nextEpisode = document.getElementById("next-episode");
const nextEpisodeTimer = document.getElementById("next-episode-timer");
const watchCreditsBtn = document.querySelector(".watch-credits");
const captionTracks = document.querySelectorAll("#subtitles")
const captionsBtn = document.getElementById("captions-btn");
const captionsIcon = document.getElementById("captions-icon");
const captionsList = document.getElementById("captions-list");
const debug = document.getElementById("debug");
let autoplayStarted = false;
let autoplayCancelled = false;
let nextEpisodeTimeout = null;
let nextEpisodeTimerInterval = null;
let currentTabIndex = captionTracks.length + 1;
captionsList.firstChild.tabIndex = currentTabIndex;
captionTracks.forEach(track => {
    let node = document.createElement("li");
    node.classList.add("caption-li");
    node.classList.add("playerNav");
    currentTabIndex--;
    node.tabIndex = currentTabIndex;
    node.role = "button";
    let textnode = document.createTextNode(track.label);
    node.appendChild(textnode);
    captionsList.appendChild(node);
});

const captionsListItems = document.querySelectorAll(".caption-li")
let currentSubs = "English";

captionsListItems.forEach(li => {
    li.addEventListener("click", () => {
        changeSub(li);
        captionsList.style.visibility = "hidden";
    })
})

function changeSub(li) {
    videoTracks = video.textTracks
    currentSubs = li.innerHTML
    for (i = 0; i < videoTracks.length; i++) {
        videoTracks[i].mode = "disabled"
        if (videoTracks[i].label == li.innerHTML) {
            videoTracks[i].mode = "showing"
        }
    }
    highlightSub()
}

function highlightSub() {
    if (currentSubs == "Off") {
        captionsIcon.classList.remove("fa-solid")
        captionsIcon.classList.add("fa-regular")
    } else {
        captionsIcon.classList.add("fa-solid")
        captionsIcon.classList.remove("fa-regular")
    }

    captionsListItems.forEach(li => {
        if (li.innerHTML == currentSubs) {
            li.style.fontWeight = "900"
        } else {
            li.style.fontWeight = "500"
        }
    })
}

highlightSub()

let scrubbing = false;

const navElements = document.querySelectorAll(".playerNav");

const DispatchTab = (direction) => {
    // Only visible & focusable elements
    const visibleElements = Array.from(navElements).filter(el => {
        const style = window.getComputedStyle(el);
        return style.visibility !== "hidden" && style.display !== "none" && !el.disabled && typeof el.focus === "function";
    });

    if (visibleElements.length === 0) {
        console.warn("No visible focusable nav elements.");
        return;
    }

    // Find currently focused element in visible list
    let currentIndex = visibleElements.indexOf(document.activeElement);
    if (currentIndex === -1) currentIndex = 0;

    // Calculate next index with wrap-around
    const nextIndex = direction === "+"
        ? (currentIndex + 1) % visibleElements.length
        : (currentIndex - 1 + visibleElements.length) % visibleElements.length;

    // Focus safely
    visibleElements[nextIndex].focus({ preventScroll: true });
};




video.addEventListener("loadeddata", loadTimestamp);
video.addEventListener("progress", () => {

    saveTimestamp();

});

video.addEventListener("timeupdate", (event) => {
    let time = video.currentTime;
    let minutes = Math.floor(time / 60);
    let seconds = Math.floor(time - minutes * 60);
    let timeString = `${padTime(minutes)}:${padTime(seconds)}`;
    if (minutes > 60) {
        let hours = Math.floor(minutes / 60);
        minutes = Math.floor(minutes - hours * 60);
        timeString = `${hours}:${padTime(minutes)}:${padTime(seconds)}`;
    }


    currentTime.innerText = timeString
    if (!scrubbing) {
        progessSlider.max = video.duration
        progessSlider.value = video.currentTime
    }
    progess.value = (video.currentTime / video.duration) * 100
    let timeRemaining = video.duration - video.currentTime;
    if (
        !autoplayStarted &&
        !autoplayCancelled &&
        timeRemaining < 120
    ) {
        autoplayStarted = true;
        toggleNextEpisodeControls();
        autoPlayNextEpisode(timeRemaining > 30 ? timeRemaining - 30 : timeRemaining);
    }

})
progessSlider.addEventListener("change", e => {
    video.currentTime = progessSlider.value
    progess.value = (progessSlider.value / progessSlider.max) * 100

})

progessSlider.addEventListener("mousedown", () => {
    scrubbing = true;
})
progessSlider.addEventListener("mouseup", () => {
    scrubbing = false;
})

// video.addEventListener("ended", clearTimestamp);

fullscreen.addEventListener("click", () => {
    toggleFullscreen()
})



document.addEventListener("keydown", (event) => {
    switch (event.key) {
        case "MediaBack":
        case "Back":
        case "Backspace":
            event.preventDefault();
            backButton.click();
            break;
        case "F": toggleFullscreen();
            break;
        case "MediaRewind":
            skip("-", 30);
            break;
        case "MediaFastForward":
            skip("+", 30);
            break;
        case "MediaPlay":
        case "MediaPause":
        case "MediaPlayPause":
        case " ":
            playPause();
            break;
        case "ArrowLeft":
        case "ArrowUp": {
            event.preventDefault();
            DispatchTab("-")
        }
            break;
        case "ArrowRight":
        case "ArrowDown": {
            event.preventDefault();
            DispatchTab("+")
        }
            break;
    }
});



playerContainer.addEventListener("dblclick", (event) => {
    toggleFullscreen()
})

rewind.addEventListener("click", () => skip("-", 30))

fastforward.addEventListener("click", () => skip("+", 30))

playPauseButton.addEventListener("click", playPause)

function padTime(time) {
    return time < 10 ? "0" + time : time
}

function playPause() {
    if (video.paused) {
        playPauseIcon.classList.remove("fa-play")
        playPauseIcon.classList.add("fa-pause")
        video.play()
    } else {
        playPauseIcon.classList.remove("fa-pause")
        playPauseIcon.classList.add("fa-play")
        video.pause()
        watchCredits();
    }
}

function skip(modifier, seconds) {

    if (modifier == "+") {
        video.currentTime += seconds
    } else if (modifier == "-") {
        video.currentTime -= seconds
    }
}

var resetDelay, inactivityTimeout;

resetDelay = function () {
    clearTimeout(inactivityTimeout);
    inactivityTimeout = setTimeout(function () {
        video.paused
            ? playerControls.dataset.state = "shown"
            : playerControls.dataset.state = "hidden"
    }, 2000);
};
resetDelay()

playerContainer.addEventListener('mousemove', function () {
    playerControls.dataset.state = "shown"
    resetDelay()
});

document.addEventListener('keydown', function () {
    playerControls.dataset.state = "shown"
    resetDelay()
});

document.addEventListener('touchstart', function () {
    playerControls.dataset.state = "shown"
    resetDelay()
});

captionsBtn.addEventListener("click", () => {
    captionsList.style.visibility = (captionsList.style.visibility == "hidden") ? "visible" : "hidden";
    captionsList.lastChild.focus();
});

captionsList.addEventListener("blur", (e) => {
    if (e.relatedTarget === null) {
        setTimeout(() => {
            captionsList.style.visibility = "hidden";
        }, 500)
    }
});

restart.addEventListener("click", () => {
    clearTimestamp();
    watchCredits();
    autoplayCancelled = false;
    autoplayStarted = false;
    if (nextEpisodeControls.style.visibility == "visible") {
        toggleNextEpisodeControls();
    }
});
let indexOfId = video.src.lastIndexOf('/');
const video_id = video.src.substring(indexOfId + 1);


function toggleFullscreen() {
    // Check if we're in fullscreen mode
    if (document.fullscreenElement) {
        document.exitFullscreen();
        fullscreenIcon.classList.remove("fa-compress")
        fullscreenIcon.classList.add("fa-expand")
        return;
    }
    // Otherwise enter fullscreen mode
    if (playerContainer.requestFullscreen) {
        playerContainer.requestFullscreen().catch((err) => {
            console.error(`Error enabling fullscreen: ${err.message}`);
        });
        fullscreenIcon.classList.add("fa-compress")
        fullscreenIcon.classList.remove("fa-expand")

    } else if (playerContainer.msRequestFullscreen) {
        playerContainer.msRequestFullscreen().catch((err) => {
            console.error(`Error enabling fullscreen: ${err.message}`);
        });
        fullscreenIcon.classList.add("fa-compress")
        fullscreenIcon.classList.remove("fa-expand")

    } else if (playerContainer.mozRequestFullScreen) {
        playerContainer.mozRequestFullScreen().catch((err) => {
            console.error(`Error enabling fullscreen: ${err.message}`);
        });
        fullscreenIcon.classList.add("fa-compress")
        fullscreenIcon.classList.remove("fa-expand")

    } else if (playerContainer.webkitRequestFullscreen) {
        playerContainer.webkitRequestFullscreen().catch((err) => {
            console.error(`Error enabling fullscreen: ${err.message}`);
        });
        fullscreenIcon.classList.add("fa-compress")
        fullscreenIcon.classList.remove("fa-expand")
    }
}

function saveTimestamp() {
    if (video.currentTime == 0) return;
    localStorage.setItem(video_id + "_timestamp", video.currentTime);

}

function loadTimestamp() {

    let time = video.duration;
    let minutes = Math.floor(time / 60);
    let seconds = Math.floor(time - minutes * 60);
    seconds < 10 ? seconds = "0" + seconds : seconds
    let timeString = `${minutes}:${seconds}`;
    if (minutes > 60) {
        let hours = Math.floor(minutes / 60);
        minutes = Math.floor(minutes - hours * 60);
        minutes < 10 ? minutes = "0" + minutes : minutes
        timeString = `${hours}:${minutes}:${seconds}`;
    }
    duration.innerHTML = timeString;

    let timestamp = localStorage.getItem(video_id + "_timestamp");
    if (!timestamp) return;
    video.currentTime = timestamp;
}

function clearTimestamp() {
    localStorage.removeItem(video_id + "_timestamp")
    video.currentTime = 0
}

function toggleNextEpisodeControls() {
    if (nextEpisode) {
        nextEpisodeControls = document.querySelector(".next-episode-controls");
        let nextEpisodeVisible = nextEpisodeControls.style.visibility == "visible"
        nextEpisodeControls.style.visibility = nextEpisodeVisible ? "hidden" : "visible";
    }
}

function autoPlayNextEpisode(delayInSeconds = 10) {
    if (nextEpisode) {
        nextEpisode.focus();
        nextEpisodeTimer.max = delayInSeconds;
        nextEpisodeTimeout = setTimeout(() => {
            nextEpisode.click();
        }, delayInSeconds * 1000);


        let timeLeft = delayInSeconds;
        nextEpisodeTimerInterval = setInterval(() => {
            timeLeft--;
            nextEpisodeTimer.value = delayInSeconds - timeLeft;
            if (timeLeft <= 0) {
                clearInterval(nextEpisodeTimerInterval);
            }
        }, 1000);
    }
}

function watchCredits() {
    autoplayCancelled = true;
    autoplayStarted = false;

    clearTimeout(nextEpisodeTimeout);
    clearInterval(nextEpisodeTimerInterval);

    nextEpisodeTimeout = null;
    nextEpisodeTimerInterval = null;

    nextEpisodeTimer.value = 0;
}
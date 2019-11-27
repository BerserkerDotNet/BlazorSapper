BlazorSounds = {};

BlazorSounds.play = function (audioFileName) {
    var audio = new Audio("audio/" + audioFileName + ".wav");
    audio.play();
}
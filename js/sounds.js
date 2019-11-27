BlazorSounds = {};

BlazorSounds.play = function (audioFileName) {
    var audio = new Audio("sounds/" + audioFileName + ".wav");
    audio.play();
}
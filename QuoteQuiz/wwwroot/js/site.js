// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function SaveMode(mode) {
    sessionStorage.setItem('mode', mode);
}

$("#quotes input:radio").click(() => {
    $(".hidden").removeClass("hidden");
});

$(document).ready(function () {
    var url = window.location.href.toLocaleLowerCase();
    var session = sessionStorage.getItem("mode")
    if (url.includes("settings")) {
        if (session == "binary") {
            document.getElementById("radioBinary").checked = true;
        }
        else if (session == "multiple") {
            document.getElementById("radioMultiple").checked = true;
        }
    }
    else if (url.includes("quiz")) {
        if (session == "multiple") {
            document.getElementById("binary").style.display = "none";
            document.getElementById("multiple").style.display = "block";
        }
        else {
            document.getElementById("binary").style.display = "block";
            document.getElementById("multiple").style.display = "none";
        }
    }
});


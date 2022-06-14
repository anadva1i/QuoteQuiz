// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function SaveMode(mode) {
    sessionStorage.setItem('mode', mode);
    $("#alert").addClass("show");
}

function alert(answer, author) {
    if (answer) {
        $("#alertSuccess").html("Correct! The right answer is: <b>" + author + "</b>")
        $("#alertSuccess").addClass("show");
    }
    else {
        $("#alertDanger").html("Sorry,you are wrong! The right answer is: <b>" + author + "</b>")
        $("#alertDanger").addClass("show");
    }
}

$("#quotes input:radio").click(function() {
    var author = $("#author").val();
    var label = $("label[for='" + $(this).attr('id') + "']").text();
    console.log(label)
    var spanAuthor = $("#spanAuthor").text().replace("- ", "").replace("?", "");
    console.log(spanAuthor)
    $("#quotes input:radio").remove();
    var userAnswer = label;
    $("#quotes label").remove();
    if (label == "Yes")
        userAnswer = spanAuthor
    else if (label == "No")
        userAnswer = author;
    console.log(userAnswer, author)
    if (userAnswer == author)
        alert(true, author)
    else alert(false, author);
    $(".span-author").text("- " + author)
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


// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function actionFailure(args) {

    var text = null;

    try {
        text = args[0].error.responseText;
    }
    catch { }

    try {
        text = args.error[0].error.responseText;
    }
    catch { }

    if (text == null) {
        text = JSON.stringify(args);
        console.log(args);
    }

    var span = document.createElement('span');
    this.element.parentNode.insertBefore(span, this.element);
    span.style.color = '#FF0000'
    span.innerHTML = "Error: " + text;
    span.className = "Error";

    setTimeout(function () { $('.Error').hide(); }, 4000);
}
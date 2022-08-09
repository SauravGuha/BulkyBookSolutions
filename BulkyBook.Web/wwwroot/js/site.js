// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.




var mytoaster = {
    displayToaster: function (header, contentMessage) {
        var toaster = document.getElementById("toaster");
        var toasterBody = toaster.getElementsByClassName("toast-body")[0];
        toasterBody.innerHTML = contentMessage;
        var header = toaster.getElementsByClassName("me-auto")[0];
        header.innerHTML = header;
        toaster.hidden = false;
    }
}

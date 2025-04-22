// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


document.addEventListener('DOMContentLoaded', function () {
    var toast = new bootstrap.Toast(document.getElementById('successToast'));
    toast.show();
});

document.addEventListener('DOMContentLoaded', function () {
    var toast = new bootstrap.Toast(document.getElementById('errorToast'));
    toast.show();
});


document.getElementById("assignTenantBtn").addEventListener("click", function () {
    // Show spinner and disable button when "Assign Tenant" is clicked
    document.getElementById("spinner").classList.remove("d-none");
    this.setAttribute("disabled", "true");
});

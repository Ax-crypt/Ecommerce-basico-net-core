// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Función para desvanecer mensaje 
$(document).ready(function () {
    // Ocultar las alertas después de 5 segundos
    setTimeout(function () {
        $(".alert").alert('close');
    }, 5000);
});


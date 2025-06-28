// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// BESTELLINGEN PAGINA - DataTable configuratie voor de bestellingen tabel
function initializeOrdersTable() {
    var table = $('#bestellingenTable').DataTable({
        pageLength: 5,
        lengthChange: false,
        info: false,
        ordering: true,
        language: {
            search: "Zoeken:",
            paginate: {
                previous: "Vorige",
                next: "Volgende"
            },
            zeroRecords: "Geen bestellingen gevonden",
            infoEmpty: "Geen bestellingen beschikbaar",
            infoFiltered: "(gefilterd uit _MAX_ bestellingen)"
        },
        dom:
            "<'row mb-3'<'col-sm-12 col-md-6 d-flex align-items-center'<'custom-length-info me-3'>>" +
            "<'col-sm-12 col-md-6'f>>" +
            "<'row'<'col-sm-12'tr>>"
    });

    // Voegt een custom dropdown toe voor het aantal rijen per pagina
    $('.custom-length-info').html(`
        <span class="me-2">Toont</span>
        <select id="pageLengthSelect" class="form-select form-select-sm" style="width: auto; display: inline-block;">
            <option value="5" selected>5</option>
            <option value="10">10</option>
            <option value="25">25</option>
            <option value="50">50</option>
        </select>
        <span class="ms-2">bestellingen.</span>
    `);

    // Luistert naar veranderingen in de dropdown en past het aantal rijen aan
    $('#pageLengthSelect').on('change', function () {
        var val = parseInt($(this).val());
        table.page.len(val).draw();
    });
}

// CART PAGINA - Toont toast berichten voor succes en foutmeldingen
function showCartNotifications() {
    var successToastEl = document.getElementById('successToast');
    if (successToastEl && successToastEl.textContent.trim().length > 0) {
        successToastEl.style.display = 'flex';
        var successToast = new bootstrap.Toast(successToastEl, { delay: 3000 });
        successToast.show();
    }

    var errorToastEl = document.getElementById('errorToast');
    if (errorToastEl && errorToastEl.textContent.trim().length > 0) {
        errorToastEl.style.display = 'flex';
        var errorToast = new bootstrap.Toast(errorToastEl, { delay: 3000 });
        errorToast.show();
    }
}

// PRODUCTS PAGINA - Voegt event listeners toe aan alle Toevoegen knoppen
function initializeAddToCartButtons() {
    document.querySelectorAll('.btn-add-to-cart').forEach(button => {
        button.addEventListener('click', async (e) => {
            e.preventDefault();
            const form = e.target.closest('form');
            const formData = new FormData(form);

            // Haalt het antiforgery token op voor veiligheid
            const tokenInput = form.querySelector('input[name="__RequestVerificationToken"]');
            const token = tokenInput ? tokenInput.value : '';

            try {
                // Stuurt een AJAX verzoek om het product aan de winkelwagen toe te voegen
                const response = await fetch(form.action, {
                    method: 'POST',
                    body: formData,
                    headers: {
                        'RequestVerificationToken': token
                    }
                });

                if (response.ok) {
                    // Toont een succesbericht als het product is toegevoegd
                    const toastEl = document.getElementById('cartToast');
                    const toast = new bootstrap.Toast(toastEl);
                    toast.show();
                } else {
                    alert('Er is iets misgegaan bij het toevoegen aan de winkelmand.');
                }
            } catch (error) {
                console.error('Fout bij fetch:', error);
                alert('Er is een fout opgetreden bij het verwerken van je verzoek.');
            }
        });
    });
}

// PAGINA INITIALISATIE - Wacht tot de pagina is geladen en start de juiste functionaliteit
document.addEventListener('DOMContentLoaded', function() {
    // Bestellingen pagina
    if (document.getElementById('bestellingenTable')) {
        initializeOrdersTable();
    }
    
    // Cart pagina
    if (document.getElementById('successToast') || document.getElementById('errorToast')) {
        showCartNotifications();
    }
    
    // Products pagina
    if (document.querySelector('.btn-add-to-cart')) {
        initializeAddToCartButtons();
    }
});

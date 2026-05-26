let crudModal;

document.addEventListener('DOMContentLoaded', () => {

    const modalElement = document.getElementById('crudModal');

    if (!modalElement) {
        console.error('crudModal element not found.');
        return;
    }

    crudModal = new bootstrap.Modal(modalElement);
});

async function openCrudModal(url) {

    const response = await fetch(url);

    if (!response.ok) {
        alert('Failed to load modal.');
        return;
    }

    const html = await response.text();

    document.getElementById('crudModalContent').innerHTML = html;

    // IMPORTANT
    initializeValidation();

    bindCrudForm();

    crudModal.show();
}

function initializeValidation() {

    const form = $('#crudModalContent form');

    form.removeData('validator');
    form.removeData('unobtrusiveValidation');

    $.validator.unobtrusive.parse(form);
}

function bindCrudForm() {

    const form = document.querySelector('#crudModalContent form');

    if (!form)
        return;

    form.addEventListener('submit', async function (e) {

        e.preventDefault();

        // VALIDATION CHECK
        if (!$(form).valid()) {
            return;
        }

        const formData = new FormData(form);

        const response = await fetch(form.action, {
            method: form.method,
            body: formData
        });

        const html = await response.text();

        // If validation failed server-side
        if (!response.redirected) {

            document.getElementById('crudModalContent').innerHTML = html;

            initializeValidation();

            bindCrudForm();

            return;
        }

        // Success
        window.location.href = response.url;
    });
}
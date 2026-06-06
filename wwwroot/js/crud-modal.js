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

    const response = await fetch(url, {
        headers: {'X-Requested-With': 'XMLHttpRequest'}
    });
    if (response.status === 401) {

        window.location.href = '/Account/Login';
        return;
    }

    if (response.status === 403) {
        notify.error('Access denied.');
        return;
    }

    if (!response.ok) {
        notify.error('Failed to load modal.');
        return;
    }

    const html = await response.text();

    document.getElementById('crudModalContent').innerHTML = html;

    initializeTomSelect(
        document.getElementById('crudModalContent')
    );

    initializePasswordToggle(
        document.getElementById('crudModalContent')
    );

    initializeValidation();

    bindCrudForm();

    crudModal.show();
}

function initializeTomSelect(container = document) {

    container
        .querySelectorAll('select[data-tom-select]')
        .forEach(select => {

            if (!select.tomselect) {

                new TomSelect(select, {
                    create: false,
                    placeholder:
                        select.dataset.placeholder || 'Search...'
                });

            }

        });
}

function initializePasswordToggle(container = document) {

    container
        .querySelectorAll('.toggle-password')
        .forEach(button => {

            button.addEventListener('click', function () {

                const input =
                    this.closest('.input-group')
                        .querySelector('.password-input');

                const icon =
                    this.querySelector('i');

                if (!input)
                    return;

                if (input.type === 'password') {

                    input.type = 'text';

                    icon.classList.remove('bi-eye');
                    icon.classList.add('bi-eye-slash');

                }
                else {

                    input.type = 'password';

                    icon.classList.remove('bi-eye-slash');
                    icon.classList.add('bi-eye');
                }

            });

        });
}

function initializeValidation() {

    const form = $('#crudModalContent form');

    if (!form.length)
        return;

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

        if (!$(form).valid()) {
            return;
        }

        try {

            const formData = new FormData(form);
            const response = await fetch(form.action, {method: form.method, body: formData});

            if (response.redirected) {
                window.location.href = response.url;
                return;
            }

            const html =await response.text();

            document.getElementById('crudModalContent').innerHTML = html;

            initializeTomSelect(document.getElementById('crudModalContent')
            );

            initializePasswordToggle(document.getElementById('crudModalContent')
            );

            initializeValidation();

            bindCrudForm();
        }
        catch (error) {

            console.error(error);

            notify.error('An unexpected error occurred.');
        }
    });
}
window.notify = {

    success(message) {
        showToast('Success', message);
    },

    warning(message) {
        showToast('Warning', message);
    },

    error(message) {
        showToast('Error', message);
    },

    info(message) {
        showToast('Information', message);
    }
};

function showToast(title, message) {

    const toastElement =
        document.getElementById('appToast');

    if (!toastElement)
        return;

    document.getElementById('toastTitle').textContent = title;
    document.getElementById('toastBody').textContent = message;

    const toast =
        bootstrap.Toast.getOrCreateInstance(
            toastElement);

    toast.show();
}
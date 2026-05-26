// this function to return the user to the previous page by browser history
function goBack() {
    if (document.referrer) {
        history.back();
    } else {
        window.location.href = '/';
    }
}
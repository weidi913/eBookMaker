document.addEventListener('DOMContentLoaded', function () {
    var hash = window.location.hash;
    if (hash) {
        console.log(hash);
        var targetElement = document.querySelector(hash);
        if (targetElement) {
            targetElement.scrollIntoView({ behavior: 'smooth' });
        }
    }
});
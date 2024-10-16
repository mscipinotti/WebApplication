function errorsAccountsDtoCallback(accounts) {
    let accountsDto = JSON.parse(accounts.responseText);
    let message = "";
    jQuery.each(accountsDto.errors, function (i, val) {
        i = parseInt(i + 1);
        message = message + i + ". " + val + "<br>";
    });
    $(".warningMessage").html(message);
}

function successAccountsDtoCallback() {
    $(".warningMessage").html('');
}

window.addEventListener('DOMContentLoaded', event => {
    // Toggle the side navigation
    const sidebarToggle = document.body.querySelector('#sidebarToggle');
    if (sidebarToggle) {
        // To persist sidebar toggle between refreshes
        if (localStorage.getItem('sb|sidebar-toggle') === 'true') {
            document.body.classList.toggle('sb-sidenav-toggled');
        }
        sidebarToggle.addEventListener('click', event => {
            event.preventDefault();
            document.body.classList.toggle('sb-sidenav-toggled');
            localStorage.setItem('sb|sidebar-toggle', document.body.classList.contains('sb-sidenav-toggled'));
        });
    }
})

function token(login, password, profile, requestVerificationToken, cookie, jwtToken) {
    this.login = login;
    this.password = password;
    this.profile = profile;
    this.requestVerificationToken = requestVerificationToken;
    this.cookie = cookie;
    this.jwtToken = jwtToken;
}
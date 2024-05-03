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
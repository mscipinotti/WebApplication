﻿let clear = function ($parent, $item) {
    let $feedback = $parent.find("span[name='feedback." + $item.attr("name") + "']");
    $feedback.html("");
    $feedback.hide();
}

let setInvalid = function ($parent, $item, message) {
    let $feedback = $parent.find("span[name='feedback." + $item.attr("name") + "']");
    $feedback.html(message);
    $feedback.show();
    return false;
}

let validate = function ($parent) {
    let isValid = true;

    $parent.find("input[required]:visible:not(:disabled)").each(function (index, item) {
        clear($parent, $(item));
    });

    $parent.find("input[required]:visible:not(:disabled)").each(function (index, item) {
        let value = $(item).val().trim();
        if (value == '') {
            setInvalid($parent, $(item), "Campo obbligatorio");
            isValid = false;
        }
    });

    $parent.find("textarea[required]:visible:not(:disabled)").each(function (index, item) {
        let value = $(item).val().trim();
        if (value == '') {
            setInvalid($parent, $(item), "Campo obbligatorio");
            isValid = false;
        }
    });

    return isValid;
}

$(":submit").on("click", function (event) {
    let $form = $(this).closest("form");
    let validation = validate($form);
    if (validation) {
        let customValidation = $(this).attr("data-custom-validation");
        if (customValidation) validation = eval(customValidation);
    }
    if (!validation) event.preventDefault();
});

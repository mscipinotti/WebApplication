let clearValidate = function ($parent) {
    let $feedback = $parent.find("span[name^='feedback.']");
    $feedback.html("");
    $feedback.hide('slow');
}

let setInvalid = function ($parent, $item, message) {
    let $feedback = $parent.find("span[name='feedback." + $item.attr("name") + "']");
    $feedback.html(message);
    $feedback.show();
    return false;
}

let validate = function ($parent) {
    let isValid = true;

    //$parent.find("input[required]:visible:not(:disabled)").each(function (index, item) {
    //    clear($parent, $(item));
    //});

    $parent.find("input[required]:visible:not(:disabled)").each(function (index, item) {
        let value = $(item).val().trim();
        if (value == '') {
            setInvalid($parent, $(item), $('input[name="MandatoryField"]').val());
            isValid = false;
        }
    });

    $parent.find("textarea[required]:visible:not(:disabled)").each(function (index, item) {
        let value = $(item).val().trim();
        if (value == '') {
            setInvalid($parent, $(item), $('input[name="MandatoryField"]').val());
            isValid = false;
        }
    });

    return isValid;
}

function runCode(obj) {
    return Function("customValidate", `"use strict";return (${obj});`)(customValidate);
}

$(":submit").on("click", function (event) {
    // article e non form per impedire che l'evento scatti quando si clicca sui bottoni submit logoff etc.
    let $form = $(this).closest("article");
    let validation = validate($form);
    if (validation) {
        let customValidation = $(this).attr("data-custom-validation");
        if (customValidation) validation = runCode(customValidation);
    }
    if (!validation) event.preventDefault();
});


/*
 * Ajax Library
 */

function ajaxTemplate(ajaxUrl) {

    // The jqXHR.success(), jqXHR.error(), and jqXHR.complete() callbacks are removed as of jQuery 3.0. 
    // You can use jqXHR.done(), jqXHR.fail(), and jqXHR.always() instead.

    var jqxhr = $.ajax({
        url: ajaxUrl
    });

    jqxhr.done(function (data, textStatus, jqXHR) {
        console.log("done");
        console.log(data);
        console.log(textStatus);
        console.log(jqXHR);
    });

    jqxhr.fail(function (jqXHR, textStatus, errorThrown) {
        console.log("fail");
        console.log(jqXHR);
        console.log(textStatus);
        console.log(errorThrown);
    });

    // In response to a successful request, the function's arguments are the same as those of .done(): data, textStatus, and the jqXHR object. 
    // For failed requests the arguments are the same as those of.fail(): the jqXHR object, textStatus, and errorThrown.
    jqxhr.always(function (data_jqXHR, textStatus, jqXHR_errorThrown) {
        console.log("always");
        console.log(data_jqXHR);
        console.log(textStatus);
        console.log(jqXHR_errorThrown);
    });

    // Incorporates the functionality of the .done() and .fail() methods
    jqxhr.then(function (data, textStatus, jqXHR) {
        console.log("then1");
        console.log(data);
        console.log(textStatus);
        console.log(jqXHR);
    }, function (jqXHR, textStatus, errorThrown) {
        console.log("then2");
        console.log(jqXHR);
        console.log(textStatus);
        console.log(errorThrown);
    });
}

function ajaxSimple(ajaxUrl) {
    $.ajax({
        url: ajaxUrl
    }).done(function () {
        console.log("done");
    }).fail(function () {
        console.log("fail");
    }).always(function () {
        console.log("always");
    });
}

function ajaxSettings(ajaxUrl) {
    $.ajax({
        method: "GET",
        url: ajaxUrl,
        data: { name: "John", location: "Boston" },
        cache: false,
        processData: true,
        dataType: "json",
        statusCode: {
            404: function () {
                alert("ajax url not found");
            }
        }
    }).done(function (data) {
        console.log("done");
        console.log(data);
    }).fail(function () {
        console.log("fail");
    }).always(function () {
        console.log("always");
    });
}
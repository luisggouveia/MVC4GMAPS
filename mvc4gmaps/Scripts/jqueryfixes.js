
$.validator.methods.number = function (value, element) {
    return this.optional(element) || !isNaN(Globalize.parseFloat(value));
}

$(function () {
    Globalize.culture('pt-PT');
});


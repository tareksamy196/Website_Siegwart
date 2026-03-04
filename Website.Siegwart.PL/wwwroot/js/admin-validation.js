(function ($) {
    if (!$.validator || !$.validator.unobtrusive) return;

    var AR_RE = /^[\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF\uFB50-\uFDFF\uFE70-\uFEFF\s0-9\-\(\)\[\]\.,'"/]+$/;
    var EN_RE = /^[A-Za-z0-9\s\-\(\)\[\]\.,'"/]+$/;

    $.validator.addMethod("arabiconly", function (value, element) {
        if (this.optional(element)) return true;
        return AR_RE.test(value);
    }, "اكتب حروف عربية فقط");

    $.validator.addMethod("englishonly", function (value, element) {
        if (this.optional(element)) return true;
        return EN_RE.test(value);
    }, "Type English letters only");

    $.validator.unobtrusive.adapters.add("arabiconly", [], function (options) {
        options.rules["arabiconly"] = true;
        options.messages["arabiconly"] = options.message;
    });

    $.validator.unobtrusive.adapters.add("englishonly", [], function (options) {
        options.rules["englishonly"] = true;
        options.messages["englishonly"] = options.message;
    });

    $(function () {
        $.validator.unobtrusive.parse(document);
    });

})(jQuery);
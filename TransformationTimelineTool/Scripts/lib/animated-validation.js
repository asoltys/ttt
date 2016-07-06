(function($) {
    let checkjQuery = function() {
        return typeof $ !== 'undefined';
    };

    let main = (function() {
        let errorDiv;
        if (checkjQuery()) {
            errorDiv = $('.validation-error');
            errorDiv.fadeIn('slow');
        }
    })();
})(jQuery);
(function ($) {
    $(document).ready(function () {

        var toggleCheck = function (checkbox) {
            return function () {
                if ($(this).prop("checked")) {
                    $(checkbox).prop("checked", false);
                } else {
                    $(checkbox).prop("checked", true);
                }
            };
        };

        $('#selectAllRegions').click(toggleCheck('input[name="selectedRegions"]'));
        $('#selectAllBranches').click(toggleCheck('input[name="selectedBranches"]'));
        $('#select_all_initiatives').click(toggleCheck('input[name="selectedInitiatives"]'));
    });
})(jQuery);
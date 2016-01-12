$(document).ready(function () {
    $('#selectAllRegions').click(function () {
        if ($(this).prop("checked")) {
            $('input[name="selectedRegions"]').prop("checked", false);
        } else {
            $('input[name="selectedRegions"]').prop("checked", true);
        }
    });

    $('#selectAllBranches').click(function () {
        if ($(this).prop("checked")) {
            $('input[name="selectedBranches"]').prop("checked", false);
        } else {
            $('input[name="selectedBranches"]').prop("checked", true);
        }
    });
});
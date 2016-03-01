if (!('console' in window)) {
    var names = ['log', 'debug', 'info', 'warn', 'error', 'assert', 'dir', 'dirxml', 'group', 'groupEnd', 'time', 'timeEnd', 'count', 'trace', 'profile', 'profileEnd'];
    window.console = {};
    for (var i = 0; i < names.length; ++i) window.console[names[i]] = function () { };
} else {
    /*if it exists but doesn't contain all the same methods....silly ie*/
    var names = ['log', 'debug', 'info', 'warn', 'error', 'assert', 'dir', 'dirxml', 'group', 'groupEnd', 'time', 'timeEnd', 'count', 'trace', 'profile', 'profileEnd'];
    for (var i = 0; i < names.length; ++i) if (!window.console[names[i]]) window.console[names[i]] = function () { };
};

/*
 *  module: page
 *  run-on: script load
 *  purpose: it determines the current controller, culture and URL of the current page
 *  warning: function extractInformation should always be tested with IE in mind
 */
var page = (function () {
    var controller = "";
    var culture = "";
    var location = "";
    var parser = null;
    var pathname = "";

    var readLocation = function () {
        location = window.location.href;
    }

    var setupParser = function () {
        parser = document.createElement('a');
        parser.href = location;
        pathname = parser.pathname.split('/');
    }

    var extractInformation = function () {
        if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
            controller = pathname[1];
        } else {
            controller = pathname[0];
        }
        if (window.location.href.indexOf('?') > -1) {
            var query = window.location.href.split('?')[1];
            var pattern = /(lang=)([A-z]{3})/g;
            var match = pattern.exec(query);
            culture = match[2];
        } else {
            culture = "eng";
        }
    }

    var errorCheck = function () {
        if (controller == "" || culture == "" || location == "") {
            controller = "Error";
            culture = "Error";
            location = "Error";
        }
    }

    var analyzeURL = (function () {
        readLocation();
        setupParser();
        extractInformation();
        errorCheck();
    })();

    return {
        controller: controller,
        culture: culture,
        location: location
    }
})();

/*
 *  module: messages
 *  run-on: script load
 *  purpose: it is a resources module that returns culture specific error messages with a given key
 *  example: messages.getErrorMessage("editorValidationError") will return
 *           "The detailed description must be provided in both English and French."
 */
var messages = (function () {
    var errorMap = {
        editorValidationError: {
            eng: "The detailed description must be provided in both English and French.",
            fra: "La description détaillée doit être fourni en français et en anglais."
        },
        branchCheckboxValidationError: {
            eng: "At least one branch must be selected.",
            fra: "Selectionner au moins une direction générale."
        },
        regionCheckboxValidationError: {
            eng: "At least one region must be selected.",
            fra: "Selectionner au moins une région."
        },
        roleCheckboxValidationError: {
            eng: "Select the required user role.",
            fra: "Sélectionner le rôle utilisateur requis."
        },
        approverValidationError: {
            eng: "An Approver must be assigned to create an editor.",
            fra: "Un approbateur est requis pour créer un éditeur."
        }
    };

    var getCultureSpecificErrorMessage = function (key) {
        if (!errorMap.hasOwnProperty(key))
            return "Message key does not exist!";
        return errorMap[key][page.culture];
    }

    return {
        errorMessages: errorMap,
        getErrorMessage: function (key) {
            return getCultureSpecificErrorMessage(key);
        }
    }
})();

var roleCheckboxValidator = (function () {
    var initialized = false;
    var rolesInputName = "selectedRoles";
    var $rolesCheckbox;
    var checkboxExists = false;

    function validateAll() {
        deleteError($rolesCheckbox);
        return isValid($rolesCheckbox);
    }

    function isValid($obj) {
        if (!checkboxExists) return true;
        if (countChecked($obj) == 0) {
            showError($obj);
            return false;
        } else if (countChecked($obj) > 0) {
            deleteError($obj);
        }
        return true;
    }

    function showError($obj) {
        var $headingDiv = $obj.parentsUntil('.form-group', '[class*="col-"]').siblings();
        $headingDiv.append(
            $('<span/>', {
                'class': 'text-danger',
                'style': 'margin: 5px'
            }).append(
                $('<span/>', {
                    'text': messages.getErrorMessage("roleCheckboxValidationError")
                })
            )
        )
    }

    function deleteError($obj) {
        var $headingDiv = $obj.parentsUntil('.form-group', '[class*="col-"]').siblings();
        if ($headingDiv.find('.text-danger').length) {
            $headingDiv.find('.text-danger').remove();
        }
    }

    function countChecked($obj) {
        var checked = 0;
        $.each($obj, function () {
            if ($(this).is(':checked'))
                checked++;
        });
        return checked;
    }

    function registerEvents() {
        $rolesCheckbox.click(function () {
            isValid($rolesCheckbox);
        });
    }

    return {
        init: function () {
            if (initialized) return true;
            initialized = true;
            $rolesCheckbox = $('[name=' + rolesInputName + ']');
            if ($rolesCheckbox.size() > 0) {
                checkboxExists = true;
                registerEvents();
            }
            return true;
        },
        validate: function () {
            return validateAll();
        },
        toString: function () {
            return "roleCheckboxValidator";
        }
    }
})();

var editorApproverValidator = (function () {
    var initialized = false;
    var rolesInputName = "selectedRoles";
    var approverSelectBoxId = "User_ApproverID";
    var $approverSelectBox;

    function validateAll() {
        deleteError($approverSelectBox);
        return isValid($approverSelectBox);
    }

    function isValid($obj) {
        var $selectedRoles = $(".roleContainer > input:checked");
        var isEditorChecked = false;
        $.each($selectedRoles, function () {
            if ($(this).val().toLowerCase() == "editor") {
                isEditorChecked = true;
            }
        });
        if (isEditorChecked) {
            if ($obj.val() != "") {
                deleteError($obj);
                return true;
            } else {
                showError($obj);
                return false;
            }
        } else {
            deleteError($obj);
            return true;
        }
        return false;
    }

    function showError($obj) {
        var $headingDiv = $obj.prev();
        $headingDiv.append(
            $('<span/>', {
                'class': 'text-danger',
                'style': 'margin: 5px'
            }).append(
                $('<span/>', {
                    'text': messages.getErrorMessage("approverValidationError")
                })
            )
        )
    }

    function deleteError($obj) {
        var $headingDiv = $obj.prev();
        if ($headingDiv.find('.text-danger').length) {
            $headingDiv.find('.text-danger').remove();
        }
    }

    function registerEvents() {
        $approverSelectBox.on("change", function () {
            isValid($approverSelectBox);
        });
    }

    return {
        init: function () {
            if (initialized) return true;
            initialized = true;
            $approverSelectBox = $('#' + approverSelectBoxId);
            registerEvents();
            return true;
        },
        validate: function () {
            return validateAll();
        },
        toString: function () {
            return "editorApproverValidator";
        }
    }
})();

var editorValidator = (function () {
    var initialized = false;
    var editorObjects;
    var parityError = "There are odd number of CKEDITORS present";

    function checkParity() {
        if (editorObjects != null)
            return objectLength(editorObjects) % 2 === 0;
        console.log(parityError);
        return false;
    }

    function objectLength(object) {
        var length = 0;
        for (key in object) {
            if (object.hasOwnProperty(key))
                ++length;
        }
        return length;
    }

    function isValid() {
        var contentExists = false;
        deleteError();
        for (key in editorObjects) {
            var currentEditor = CKEDITOR.instances[key];
            currentEditor.updateElement();
            if (!contentExists)
                contentExists = $("#" + key).val().length > 0 ? true : false;
        }
        if (contentExists) {
            for (key in editorObjects) {
                if ($("#" + key).val().length > 0)
                    continue;
                showError();
                return false;
            }
        }
        deleteError();
        return true;
    }

    function showError() {
        var $div = $(".ckeditor").first().parent().parent();
        $div.children("label").after(
            $('<span/>', {
                'class': 'text-danger',
                'style': 'display: block; margin-bottom: 10px;'
            }).append(
                $('<span/>', {
                    'text': messages.getErrorMessage("editorValidationError")
                })
            )
        )
    }

    function deleteError() {
        var $div = $(".ckeditor").first().parent().parent();
        if ($div.find('.text-danger').length) {
            $div.find('.text-danger').remove();
        }
    }

    return {
        init: function () {
            if (initialized) return true;
            initialized = true;
            editorObjects = CKEDITOR != null ? CKEDITOR.instances : null;
            return checkParity();
        },
        validate: function () {
            return isValid();
        },
        toString: function () {
            return "editorValidator";
        }
    }
})();

var branchRegionCheckboxValidator = (function () {
    var initialized = false;
    var branchesInputName = "selectedBranches";
    var regionsInputName = "selectedRegions";
    var $branchesCheckbox;
    var $regionsCheckbox;
    var checkboxExists = false;

    function validateAll() {
        deleteError($branchesCheckbox);
        deleteError($regionsCheckbox);
        var valid = [isValid($branchesCheckbox), isValid($regionsCheckbox)];
        return valid.every(function (val) { return val == true });
    }

    function isValid($obj) {
        if (!checkboxExists) return true;
        if (countChecked($obj) == 0) {
            showError($obj);
            return false;
        } else if (countChecked($obj) > 0) {
            deleteError($obj);
        }
        return true;
    }

    function showError($obj) {
        var $headingDiv = $obj.parent().children(':first-child');
        var isBranchContainer = $obj.parent().attr("id") == "branch_checkbox_container";
        var errorMessage = isBranchContainer ?
            messages.getErrorMessage("branchCheckboxValidationError") : messages.getErrorMessage("regionCheckboxValidationError");
        $headingDiv.after(
            $('<span/>', {
                'class': 'text-danger',
                'style': 'margin: 5px'
            }).append(
                $('<span/>', {
                    'text': errorMessage
                })
            )
        )
    }

    function deleteError($obj) {
        var $headingDiv = $obj.parent();
        if ($headingDiv.find('.text-danger').length) {
            $headingDiv.find('.text-danger').remove();
        }
    }

    function countChecked($obj) {
        var checked = 0;
        $.each($obj, function () {
            if ($(this).is(':checked'))
                checked++;
        });
        return checked;
    }

    function registerEvents() {
        $branchesCheckbox.click(function () {
            isValid($branchesCheckbox);
        });
        $regionsCheckbox.click(function () {
            isValid($regionsCheckbox);
        });
    }

    return {
        init: function () {
            if (initialized) return true;
            initialized = true;
            $branchesCheckbox = $('[name=' + branchesInputName + ']');
            $regionsCheckbox = $('[name=' + regionsInputName + ']');
            if ($branchesCheckbox.size() > 0 && $regionsCheckbox.size() > 0) {
                checkboxExists = true;
                registerEvents();
            }
            return true;
        },
        validate: function () {
            return validateAll();
        },
        toString: function () {
            return "branchRegionCheckboxValidator";
        }
    }
})();

$(document).ready(function () {
    /*
     *  module: pageAdapter
     *  run-on: document load
     *  purpose: it determines which validator object should be used for the current culture and page
     */
    var pageAdapter = (function () {
        var validators = [];
        var determineValidators = (function () {
            switch (page.controller) {
                case "Utilisateurs-Users":
                    validators.push(roleCheckboxValidator);
                    validators.push(editorApproverValidator);
                    break;
                case "Activites-Activities":
                    validators.push(editorValidator);
                    validators.push(branchRegionCheckboxValidator);
                    break;
                case "Repercussions-Impacts":
                    validators.push(branchRegionCheckboxValidator);
                    break;
                default:
                    return;
            }
        })();

        function validateAll() {
            var valid = [];
            for (var i = 0; i < validators.length; i++) {
                validators[i].init();
                valid.push(validators[i].validate());
            }
            return valid.every(function (val) { return val == true });
        }

        return {
            validators: validators,
            validate: function () {
                return validateAll();
            }
        }
    })();
    $("form").on("submit", function (event) {
        if (!pageAdapter.validate()) {
            console.log("Custom validation failed...");
            event.preventDefault();
        }
    });
});
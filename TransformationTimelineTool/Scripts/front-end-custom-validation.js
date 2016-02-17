if (!('console' in window)) {
    var names = ['log', 'debug', 'info', 'warn', 'error', 'assert', 'dir', 'dirxml', 'group', 'groupEnd', 'time', 'timeEnd', 'count', 'trace', 'profile', 'profileEnd'];
    window.console = {};
    for (var i = 0; i < names.length; ++i) window.console[names[i]] = function () { };
} else {
    /*if it exists but doesn't contain all the same methods....silly ie*/
    var names = ['log', 'debug', 'info', 'warn', 'error', 'assert', 'dir', 'dirxml', 'group', 'groupEnd', 'time', 'timeEnd', 'count', 'trace', 'profile', 'profileEnd'];
    for (var i = 0; i < names.length; ++i) if (!window.console[names[i]]) window.console[names[i]] = function () { };
};

$(document).ready(function () {
    var editorValidationError = {
        eng: "English Text and French Text fields both need to be filled out.",
        fra: "English Text and French Text fields both need to be filled out."
    };
    var checkboxValidationError = {
        eng: "At least one option needs to be selected.",
        fra: "At least one option needs to be selected."
    };
    var approverValidationError = {
        eng: "Approver needs to be assigned for an editor.",
        fra: "Approver needs to be assigned for an editor."
    };

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
                        'text': editorValidationError[pageAdapter.getCulture()]
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
            $headingDiv.after(
                $('<span/>', {
                    'class': 'text-danger',
                    'style': 'margin: 5px'
                }).append(
                    $('<span/>', {
                        'text': checkboxValidationError[pageAdapter.getCulture()]
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
            console.log($headingDiv);
            $headingDiv.append(
                $('<span/>', {
                    'class': 'text-danger',
                    'style': 'margin: 5px'
                }).append(
                    $('<span/>', {
                        'text': approverValidationError[pageAdapter.getCulture()]
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
                        'text': checkboxValidationError[pageAdapter.getCulture()]
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

    var pageAdapter = (function () {
        var initialized = false;
        var validatorArray = [];
        var culture;
        var controller;

        function analyzeCurrentURL() {
            var parser = document.createElement('a');
            parser.href = window.location.href;
            var pathname = parser.pathname.split('/');
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

        function mapValidators() {
            validatorArray = [];
            switch (controller) {
                case 'Utilisateurs-Users':
                    validatorArray.push(roleCheckboxValidator);
                    validatorArray.push(editorApproverValidator);
                    break;
                case 'Activites-Activities':
                    validatorArray.push(editorValidator);
                    validatorArray.push(branchRegionCheckboxValidator);
                    break;
                case 'Repercussions-Impacts':
                    validatorArray.push(branchRegionCheckboxValidator);
                    break;
                default:
                    return;
            }
        }

        function validateAll() {
            var valid = [];
            for (var i = 0; i < validatorArray.length; i++) {
                validatorArray[i].init();
                valid.push(validatorArray[i].validate());
            }
            console.log("pageAdapter: Iterated all validator objects...");
            return valid.every(function (val) { return val == true });
        }

        return {
            init: function () {
                if (initialized) return true;
                console.log("pageAdapter: Initializing...");
                analyzeCurrentURL();
                if (controller != undefined) {
                    mapValidators();
                }
                console.log("pageAdapter: Initialized...");
                console.log("pageAdapter: Validators - " + validatorArray.join(", "));
                return true;
            },
            getCulture: function() {
                return culture;
            },
            validators: validatorArray,
            validate: function () {
                return validateAll();
            }
        }
    })();

    $("form").on("submit", function (event) {
        pageAdapter.init();
        if (!pageAdapter.validate()) {
            console.log("Custom validation failed...");
            event.preventDefault();
        } else {
            console.log("Custom validation passed...");
        }
    });
});
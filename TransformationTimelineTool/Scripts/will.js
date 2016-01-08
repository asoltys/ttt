/* start params */
// set default lang to english (this is a catch all)
var lang = 'e';

// if url has -fra in it change it to french
if (window.location.href.indexOf("-fra") > 0) {
    var lang = 'f'
};

timeLine = {
    about: timeLine.utility.translate("about"),
    legend: timeLine.utility.translate("legend"),
    utility: timeLine.utility,
    render: '',
    initiatives: '',
    initiativesURL : '/en/initiatives/data',
    branches: '',
    branchesURL : '/en/branches/data',
    regions: '',
    regionsURL : '/en/regions/data',
    startMonth: 1,
    startYear: 2012,
    endMonth: 12,
    endYear: 2020,
    start: function () { return (this.startYear * 12) + (this.startMonth - 1); },
    end: function () { return (this.endYear * 12) + (this.endMonth); },
    totalMonth: function () { return this.end() - this.start(); },
    countTimeLine: 0,
    timeLineHeight: 50,
    widthMonth: 100,
    heightQuarter: 18,
    heightMonth: 18,
    heightGroup: 20,
    content: function () {
        $.ajax({
            type: "GET",
            url: timeLine.initiativesURL,
            dataType: "json",
            timeout: 6000,
            success: function (initiatives) {
                timeLine.initiatives = initiatives;
                $("#about").append(timeLine.about);
                $("#legend").append(timeLine.legend);
                $("#leftNav").append(timeLine.leftNav());
                timeLine.areaNav();
                timeLine.branchNav();
                $("#rightNav").append(timeLine.rightNav());
                timeLine.render += timeLine.projectContainer();
                timeLine.render += timeLine.dragContainer(initiatives);
                $("#render").append(timeLine.render);
                $("#dragContainer").draggable({ axis: "x" });
                timeLine.css();
            }
        });
    },
    leftNav: function () {
        var html = '';
        html = "<img src='img/arrow_left.png' style='width:30px; height:30px;' class='scrollButton' id='leftButton' />";
        return html;
    },
    areaNav: function () {
        $.ajax({
            type: "GET",
            url: timeLine.regionsURL,
            dataType: "json",
            timeout: 6000,
            success: function (regions) {
                var html = '';
                html += "<select id='areaSelect'><option value=''>" + timeLine.utility.translate("area") + "</option>";
                timeLine.regions = regions.sort(function (a, b) { return a.NameE.localeCompare(b.NameE); });
                $.each(timeLine.regions, function (key, value) {
                    if (timeLine.regions[key].NameE == 'All') {
                        html += "<option value='" + timeLine.regions[key].ID + "'>" + timeLine.regions[key].NameE + "</option>"
                    };
                });
                $.each(timeLine.regions, function (key, value) {
                    if (timeLine.regions[key].NameE != 'All') {
                        html += "<option value='" + timeLine.regions[key].ID + "'>" + timeLine.regions[key].NameE + "</option>"
                    };
                });
                html += "</select>";
                $("#areaNav").append(html);
            }
        });
    },
    branchNav: function () {
        $.ajax({
            type: "GET",
            url: timeLine.branchesURL,
            dataType: "json",
            timeout: 6000,
            success: function (branches) {
                var html = '';
                html += "<select id='branchSelect'><option value=''>" + timeLine.utility.translate("branch") + "</option>";
                //html += "<option value=''>All</option>"
                timeLine.branches = branches.sort(function (a, b) { return a.NameE.localeCompare(b.NameE); });
                $.each(timeLine.branches, function (key, value) {
                    if (timeLine.branches[key].NameE == 'All') {
                        html += "<option value='" + timeLine.branches[key].ID + "'>" + timeLine.branches[key].NameE + "</option>"
                    };
                });
                $.each(timeLine.branches, function (key, value) {
                    if (timeLine.branches[key].NameE != 'All') {
                        html += "<option value='" + timeLine.branches[key].ID + "'>" + timeLine.branches[key].NameE + "</option>"
                    };
                });
                html += "</select>";
                $("#branchNav").append(html);
            }
        });
    },
    rightNav: function () {
        var html = '';
        html += "<img src='img/arrow_right.png' style='width:30px; height:30px;' class='scrollButton' id='rightButton' />";
        return html;
    },
    projectContainer: function () {
        var html = '';
        html += "<div id='projectContainer'>";
        html += "<div class='projectSpace'></div>";
        html += "<div class='projectGroupRow' id='pg3'>There will be significant changes to the way that an employee does their work. <a href='#' style='color:#ffffff;'>Hide</a></div>";
        html += "<div class='projectGroupRow' id='pg2'>There will be some changes to the way that an employee does their work. <a href='#' style='color:#000000;'>Hide</a></div>";
        html += "<div class='projectGroupRow' id='pg1'>There will be minimal changes to the way that an employee does their work. <a href='#' style='color:#ffffff;'>Hide</a></div>";
        html += "<div class='projectGroupRow' id='pg0'>There will be no changes to the way that an employee does their work. <a href='#' style='color:#ffffff;'>Hide</a></div>";
        $.each(timeLine.initiatives, function (key, value) {
            timeLine.countTimeLine = timeLine.countTimeLine + 1;
            html += "<div class='projectRow' id='p" + timeLine.initiatives[key].ID + "'>" + eval("timeLine.initiatives[key].Name".concat(lang.toUpperCase())) + "</div>";
            //html += "<img src='img/white.gif' class='projectRowBackground' />";
        });
        html += "</div>";
        return html;
    },
    dragContainer: function () {
        var html = '';
        html += "<div id='dragContainer'>";
        html += timeLine.today(timeLine.initiatives);
        html += timeLine.quarterMonthContainer();
        html += timeLine.timeLineContainer(timeLine.initiatives);
        html += "</div>";
        return html;
    },
    quarterMonthContainer: function () {
        var html = '';
        html += "<div id='quarterMonthContainer' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'>";
        html += timeLine.quarterContainer();
        html += timeLine.monthContainer();
        html += "</div>";
        return html;
    },
    quarterContainer: function () {
        var html = '';
        var quarter = '';
        var month = timeLine.startMonth;
        html += "<div id='quarterContainer' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'>";
        for (i = 1; i < timeLine.totalMonth() + 1; i++) {
            if (month == 13) {
                month = 1;
            };
            if (month == '4' || month == '7' || month == '10' || month == '1') {
                if (month == '4') {
                    quarter = "| Q1";
                } else if (month == '7') {
                    quarter = "| Q2";
                } else if (month == '10') {
                    quarter = "| Q3";
                } else if (month == '1') {
                    quarter = "| Q4";
                };
            } else {
                quarter = "";
            };
            html += "<div class='quarter month" + month + "' style='width:" + timeLine.widthMonth + "px; height:" + timeLine.heightQuarter + "px;'>";
            html += quarter;
            html += "</div>";
            month = month + 1;
        };
        html += "</div>";
        return html;
    },
    monthContainer: function () {
        var html = '';
        var month = timeLine.startMonth;
        var year = timeLine.startYear;
        html += "<div id='monthContainer' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'>";
        for (i = 1; i < timeLine.totalMonth() + 1; i++) {
            if (month == 13) {
                year = year + 1;
                month = 1;
            };
            html += "<div class='month month" + month + "' style='width:" + timeLine.widthMonth + "px; height:" + timeLine.heightMonth + "px;'>"
            html += "|" + timeLine.utility.translate("m" + month);
            if (month == '4' || month == '7' || month == '10' || month == '1') {
                html += " ";
                html += year;
            };
            html += "</div>";
            month = month + 1;
        };
        html += "</div>";
        return html;
    },
    today: function () {
        var now = new Date();
        var today = Number(now.getMonth() + 1) + "/" + now.getDate() + "/" + now.getFullYear();
        return "<img src='img/red.gif' id='today' style='margin-left:" + timeLine.getLeft(today) + "px' />";
    },
    timeLineContainer: function () {
        var html = '';
        var barStartDate = '';
        html += "<div id='timeLineContainer'>";
        html += "<div class='timeLineGroupRow' id='tg3' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'></div>";
        html += "<div class='timeLineGroupRow' id='tg2' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'></div>";
        html += "<div class='timeLineGroupRow' id='tg1' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'></div>";
        html += "<div class='timeLineGroupRow' id='tg0' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'></div>";
        $(timeLine.initiatives).each(function (key, value) {
            barStartDate = value.StartDate;
            html += "<div class='timeLineRow' id = 't" + value.ID + "' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'>";
            html += "<div class='timeLineBar' style='margin-left:" + timeLine.getLeft(barStartDate) + "px; width:" + timeLine.getRight(barStartDate, value.EndDate) + "px;'>"
            $.each(value.Events, function (key, value) {
                var image = ''
                if (value.Type == 'Milestone') {
                    image = 'circle.png'
                } else if (value.Type == 'Training') {
                    image = 'book.png'
                };
                html += "<img title='" + value.HoverE + "' onClick='timeLine.dialog(\"" + value.ID + "\")' src='img/" + image + "' class='event' style='width:24px; height:32px; margin-left:" + timeLine.getEvent(value.Date, barStartDate) + "px; position:absolute;' />"
            });
            html += "</div></div>";
        });
        html += "</div>";
        return html;
    },
    dialog: function (x) {
        $("#dialog").dialog("open");
        $(timeLine.initiatives).each(function (key, value) {
            $.each(value.Events, function (key, value) {
                if (x == value.ID) {
                    $("#dialog").html(value.TextE);
                };
            });
        });
    },
    getLeft: function (date) {
        var date = date.split("/");
        var month = ((Number(date[2]) * 12) + Number(date[0]));
        var day = ((Number(date[1]) / 30) * timeLine.widthMonth);
        var startMonth = (timeLine.startYear * 12) + timeLine.startMonth;
        var z = ((month - startMonth) * timeLine.widthMonth) + day;
        return z;
    },
    getEvent: function (date, date2) {
        var date = date.split("/");
        var startDate = date2.split("/");
        var month = ((Number(date[2]) * 12) + Number(date[0]));
        var day = ((Number(date[1]) / 30) * timeLine.widthMonth);
        var startMonth = (Number(startDate[2]) * 12) + Number(startDate[0]);
        var z = ((month - startMonth) * timeLine.widthMonth) + day;
        var z = z - 14;
        return z;
    },
    getRight: function (s, e) {
        var start = s.split("/");
        var end = e.split("/");
        var offsetDay = ((Number(start[1]) / 30) * timeLine.widthMonth) - 1;
        var addDay = ((Number(end[1]) / 30) * timeLine.widthMonth);
        var x = ((((end[2] - start[2]) * 12) + (end[0] - start[0])) * timeLine.widthMonth) - offsetDay + addDay;
        return x;
    },
    css: function () {
        var top = timeLine.heightQuarter + timeLine.heightMonth;
        $(".projectRow").css("height", timeLine.timeLineHeight - 1 - 10);
        $(".projectRowBackground").css("height", timeLine.timeLineHeight);
        $(".timeLineRow").css("height", timeLine.timeLineHeight - 1);
        $(".projectSpace").css("height", top - 1);
        $("#today").css("height", timeLine.countTimeLine * timeLine.timeLineHeight + 80);
        $("#dragContainer").css("top", ((timeLine.countTimeLine * timeLine.timeLineHeight) + top) * -1 - 80);
        var now = new Date();
        var today = Number(now.getMonth() + 1) + "/" + now.getDate() + "/" + now.getFullYear();
        $("#dragContainer").css("left", timeLine.getLeft(today) * -1 + 575);
    },
    goLeft: function () {
        $("#dragContainer").css('left', "+=" + timeLine.widthMonth * 2);
    },
    goRight: function () {
        $("#dragContainer").css('left', "-=" + timeLine.widthMonth * 2);
    },
    areaSelect: function () {
        timeLine.filter();
    },
    branchSelect: function () {
        timeLine.filter();
    },
    filter: function () {
        $.ajax({
            type: "GET",
            url: timeLine.branchesURL,
            dataType: "json",
            timeout: 6000,
            success: function (initiatives) {
                var temp = '';
                var id = '';
                var temp_level = '';
                var rSelected = $('#areaSelect').val();
                var bSelected = $('#branchSelect').val();
                var branches = '';
                var regions = '';
                $.each(timeLine.initiatives, function (key, value) {
                    var id = timeLine.initiatives[key].ID;
                    $("#p" + id).insertAfter("#pg0");
                    $("#t" + id).insertAfter("#tg0");
                });
                $.each(timeLine.initiatives, function (key, value) {
                    var id = timeLine.initiatives[key].ID;
                    $.each(value.Impacts, function (key, value) {
                        temp_level = value.Level;
                        regions = value.Regions;
                        branches = value.Branches;
                        //alert("regions: " + regions + " selected: " + rSelected);
                        //alert("branch: " + branches + " selected: " + bSelected);
                        if ($.inArray(parseInt(bSelected), branches) >= 0 && $.inArray(parseInt(rSelected), regions) >= 0) {
                            $("#pg" + temp_level).css("display", "inline");
                            $("#tg" + temp_level).css("display", "inline");
                            $("#p" + id).insertAfter("#pg1");
                            $("#t" + id).insertAfter("#tg1");
                            //alert("id " + id + " is level " + temp_level);
                        };
                    });
                });
            }
        });

    }
}

$(document).ready(function () {
    timeLine.content();
    $('body').on("click", "#leftButton", timeLine.goLeft);
    $('body').on("click", "#rightButton", timeLine.goRight);
    $('body').on("change", "#areaSelect", timeLine.areaSelect);
    $('body').on("change", "#branchSelect", timeLine.branchSelect);
    $("#dialog").dialog({ autoOpen: false });
    $(document).tooltip();
});


// set default lang to english (this is a catch all)
var lang = 'e';

// if url has -fra in it change it to french
if (window.location.href.indexOf("fr") > 0) {
    var lang = 'f'
};

timeLine = {
    // params
    about: timeLine.utility.translate("about"),
    legend: timeLine.utility.translate("legend"),
    utility: timeLine.utility,
    render: '',
    initiatives: '',
    initiativesURLe: '/initiatives/data?lang=eng',
    initiativesURLf: '/initiatives/data?lang=fra',
    //initiativesURLe: '/data/initiatives-eng',
    //initiativesURLf: '/data/initiatives-fra',
    branchesURL: '/directions-generales-branches/data?lang=eng',
    regionsURL: '/regions/data?lang=eng',
    branches: '',
    regions: '',
    startMonth: 1,
    startYear: 2012,
    endMonth: 12,
    endYear: 2020,
    start: function () { return (this.startYear * 12) + (this.startMonth - 1); },
    end: function () { return (this.endYear * 12) + (this.endMonth); },
    totalMonth: function () { return this.end() - this.start(); },
    hide0: 0,
    hide1: 0,
    hide2: 0,
    hide3: 0,
    hide4: 0,
    runOnce: 0,
    countTimeLine: 0,
    timeLineHeight: 50,
    widthMonth: 100,
    heightQuarter: 18,
    heightMonth: 18,
    heightGroup: 20,
    // populate with content
    content: function () {
        $.ajax({
            type: "GET",
            url: eval("timeLine.initiativesURL".concat(lang)),
            dataType: "json",
            timeout: 20000,
            success: function (initiatives) {
                timeLine.initiatives = initiatives.sort(function (a, b) { return b.Timeline.localeCompare(a.Timeline); });
                $("#about").append(timeLine.about);
                $("#legend").append(timeLine.legend);
                $("#leftNav").append(timeLine.leftNav());
                timeLine.viewNav();
                timeLine.areaNav();
                timeLine.branchNav();
                $("#rightNav").append(timeLine.rightNav());
                $("#dateNav").append(timeLine.quarterMonthContainer());
                $("#navHeaderSpace").height($('#navHeader').height());
                timeLine.render += timeLine.projectContainer();
                timeLine.render += timeLine.dragContainer(initiatives);
                timeLine.render += timeLine.footer();
                $("#render").append(timeLine.render);
                $("#dragContainer").draggable({
                    axis: "x",
                    alsoDrag: "#quarterMonthContainer"
                });
                timeLine.toggleIcons();
                timeLine.css();
                timeLine.orderRows();
                setFixedHeader();
            }
        });
    },
    // set left arrow
    leftNav: function () {
        var html = '';
        html = "<img src='/timeline/img/arrow_left.png' style='width:30px; height:30px;' class='scrollButton' id='leftButton' />";
        return html;
    },
    // set region dropdown
    viewNav: function () {
        var html = '';
        html += "<select id='viewSelect'>";
        html += "<option value=''>" + timeLine.utility.translate("viewAll") + "</option>";
        html += "<option value='BP2020'>" + timeLine.utility.translate("viewBP") + "</option>";
        html += "<option value='TransformationTimeline'>" +timeLine.utility.translate("viewTransformation") + "</option>";
        html += "</select>";
        $("#viewNav").append(html);
    },
    // set region dropdown
    areaNav: function () {
        $.ajax({
            type: "GET",
            url: timeLine.regionsURL,
            dataType: "json",
            timeout: 6000,
            success: function (regions) {
                var html = '';
                html += "<select id='areaSelect'><option value=''>" + timeLine.utility.translate("area") + "</option>";
                if (lang == 'e') {
                    timeLine.regions = regions.sort(function (a, b) { return a.NameE.localeCompare(b.NameE); });
                } else {
                    timeLine.regions = regions.sort(function (a, b) { return a.NameF.localeCompare(b.NameF); });
                };
                $.each(timeLine.regions, function (key, value) {
                    if (timeLine.regions[key].ID == 1) {
                        if (lang == 'e') {
                            //html += "<option value='" + timeLine.regions[key].ID + "'>" + timeLine.regions[key].NameE + "</option>"
                        } else {
                            //html += "<option value='" + timeLine.regions[key].ID + "'>" + timeLine.regions[key].NameF + "</option>"
                        };
                    };
                    if (timeLine.regions[key].ID == 5) {
                        if (lang == 'e') {
                            html += "<option value='" + timeLine.regions[key].ID + "'>" + timeLine.regions[key].NameE + "</option>";
                        } else {
                            html += "<option value='" + timeLine.regions[key].ID + "'>" + timeLine.regions[key].NameF + "</option>";
                        };
                    };
                });
                $.each(timeLine.regions, function (key, value) {
                    if (timeLine.regions[key].ID != 1 && timeLine.regions[key].ID != 5) {
                        if (lang == 'e') {
                            html += "<option value='" + timeLine.regions[key].ID + "'>" + timeLine.regions[key].NameE + "</option>";
                        } else {
                            html += "<option value='" + timeLine.regions[key].ID + "'>" + timeLine.regions[key].NameF + "</option>";
                        };
                    };
                });
                html += "</select>";
                $("#areaNav").append(html);
            }
        });
    },
    // set branch dropdown
    branchNav: function () {
        $.ajax({
            type: "GET",
            url: timeLine.branchesURL,
            dataType: "json",
            timeout: 6000,
            success: function (branches) {
                var html = '';
                html += "<select id='branchSelect' disabled><option value=''>" + timeLine.utility.translate("branch") + "</option>";
                timeLine.branches = branches.sort(function (a, b) { return a.NameE.localeCompare(b.NameE); });
                $.each(timeLine.branches, function (key, value) {
                    if (timeLine.branches[key].ID == 1) {
                        if (lang == 'e') {
                            //html += "<option value='" + timeLine.branches[key].ID + "'>" + timeLine.branches[key].NameE + "</option>"
                        } else {
                            //html += "<option value='" + timeLine.branches[key].ID + "'>" + timeLine.branches[key].NameF + "</option>"
                        };
                    };
                });
                $.each(timeLine.branches, function (key, value) {
                    if (timeLine.branches[key].ID != 1) {
                        if (lang == 'e') {
                            var tempName = timeLine.branches[key].NameE.substring(0, 44);
                            var tempTrim = "";
                            if (timeLine.branches[key].NameE.length > 44) {
                                tempTrim = "...";
                            };
                            html += "<option value='" + timeLine.branches[key].ID + "'>" + tempName + tempTrim + "</option>"
                        } else {
                            var tempName = timeLine.branches[key].NameF.substring(0, 44);
                            var tempTrim = "";
                            if (timeLine.branches[key].NameF.length > 44) {
                                tempTrim = "...";
                        };
                            html += "<option value='" + timeLine.branches[key].ID + "'>" + tempName + tempTrim + "</option>"
                    };
                    };
                });
                html += "</select>";
                $("#branchNav").append(html);
            }
        });
    },
    // set right arrow
    rightNav: function () {
        var html = '';
        html += "<img src='/timeline/img/arrow_right.png' style='width:30px; height:30px;' class='scrollButton' id='rightButton' />";
        return html;
    },
    // set left side initiative list
    projectContainer: function () {
        var html = '';
        html += "<div id='projectContainer'>";
        var pat = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
        html += "<div class='projectGroupRow' id='pg3'>" + timeLine.utility.translate("highImDesc") + pat + " <a href='#' style='color:#ffffff;' id='hide3' class='hide'>" + timeLine.utility.translate("hide") + "</a>";
        html += "<a href='#' style='color:#ffffff;' id='show3' class='show'>" + timeLine.utility.translate("show") + "</a></div>";
        html += "<div class='projectGroupRow' id='pg2'>" + timeLine.utility.translate("medImDesc") + pat + " <a href='#' style='color:#000000;' id='hide2' class='hide'>" + timeLine.utility.translate("hide") + "</a>"
        html += "<a href='#' style='color:#000000;' id='show2' class='show'>" + timeLine.utility.translate("show") + "</a></div>";
        html += "<div class='projectGroupRow' id='pg1'>" + timeLine.utility.translate("lowImDesc") + pat + " <a href='#' style='color:#ffffff;' id='hide1' class='hide'>" + timeLine.utility.translate("hide") + "</a>"
        html += "<a href='#' style='color:#ffffff;' id='show1' class='show'>" + timeLine.utility.translate("show") + "</a></div>";
        html += "<div class='projectGroupRow' id='pg0'>" + timeLine.utility.translate("noImDesc") + pat + " <a href='#' style='color:#ffffff;' id='hide0' class='hide'>" + timeLine.utility.translate("hide") + "</a>"
        html += "<a href='#' style='color:#ffffff;' id='show0' class='show'>" + timeLine.utility.translate("show") + "</a></div>";
        html += "<div class='projectGroupRow' id='pg4'>" + timeLine.utility.translate("bpImDesc") + pat + " <a href='#' style='color:#000000;' id='hide4' class='hide'>" + timeLine.utility.translate("hide") + "</a>";
        html += "<a href='#' style='color:#000000;' id='show4' class='show'>" + timeLine.utility.translate("show") + "</a></div>";
        $.each(timeLine.initiatives, function (key, value) {
            timeLine.countTimeLine = timeLine.countTimeLine + 1;
            html += "<div class='projectRow p" + timeLine.initiatives[key].Timeline + "' id='p";
            html += timeLine.initiatives[key].ID;
            html += "'><a href='#' onClick='timeLine.dialogCustom(\"";
            html += timeLine.cleanSpecialCharacters(timeLine.initiatives[key].Description);
            html += "\")'>";
            html += timeLine.initiatives[key].Name;
            html += "</a></div>";
            //html += "<img src='img/white.gif' class='projectRowBackground' />";
        });
        html += "</div>";
        return html;
    },
    // set everything draggable
    dragContainer: function () {
        var html = '';
        html += "<div id='dragContainer'>";
        html += timeLine.today(timeLine.initiatives);
        html += "<div class='projectSpace'></div>";
        html += timeLine.timeLineContainer(timeLine.initiatives);
        html += "</div>";
        return html;
    },
    // set quarter/month container
    quarterMonthContainer: function () {
        var html = '';
        html += "<div id='quarterMonthContainer' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'>";
        html += timeLine.quarterContainer();
        html += timeLine.monthContainer();
        html += "</div>";
        return html;
    },
    // set quarters
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
    // set months
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
    // set red today indicator
    today: function () {
        var now = new Date();
        var today = Number(now.getMonth() + 1) + "/" + now.getDate() + "/" + now.getFullYear();
        return "<img src='/timeline/img/red.gif' id='today' style='margin-left:" + timeLine.getLeft(today) + "px' />";
    },
    // set timeline container
    timeLineContainer: function () {
        var html = '';
        var barStartDate = '';
        var mergeDates = [];
        var rowID = '';
        html += "<div id='timeLineContainer'>";
        html += "<div class='timeLineGroupRow' id='tg3' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'></div>";
        html += "<div class='timeLineGroupRow' id='tg2' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'></div>";
        html += "<div class='timeLineGroupRow' id='tg1' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'></div>";
        html += "<div class='timeLineGroupRow' id='tg0' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'></div>";
        html += "<div class='timeLineGroupRow' id='tg4' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'></div>";
        $(timeLine.initiatives).each(function (key, value) {
            mergeDates = [];
            barStartDate = value.StartDate;
            rowID = value.ID;
            html += "<div class='timeLineRow t" + timeLine.initiatives[key].Timeline + "' id = 't" + rowID + "' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'>";
            html += "<div class='timeLineBar' style='margin-left:" + timeLine.getLeft(barStartDate) + "px; width:" + timeLine.getRight(barStartDate, value.EndDate) + "px;'>"
            $.each(value.Events, function (key, value) {
                var image = '';
                var date = '';
                var id = '';
                if (value.Show == true) {
                    if (value.Type == 'Milestone') {
                        image = 'circle.png'
                    } else if (value.Type == 'Training') {
                        image = 'book.png'
                    };
                    date = value.Date;
                    id = value.ID;
                } else {
                    return true;
                }
                idString = toString(id);
                html += "<img id='icon" + id + "' title='" + timeLine.hover(id) + "' onClick='timeLine.dialog(\"" + id + "," + rowID + "\")' src='/timeline/img/" + image + "' class='event' style='width:24px; height:32px; margin-left:" + timeLine.getEvent(date, barStartDate) + "px; position:absolute;' />";
            });
            html += "</div></div>";
        });
        html += "</div>";
        return html;
    },
    // hover script
    hover: function (x) {
        var hover = '';
        $(timeLine.initiatives).each(function (key, value) {
            $.each(value.Events, function (key, value) {
                if (x == value.ID) {
                    hover = value.Hover;
                }
            });
        });
        if (hover != null) {
            hover = timeLine.cleanSpecialCharacters(hover);
        } else {
            return '';
        };
        return hover;
    },
    // popup script for icons
    dialog: function (x) {
        var x = x.split(",");
        var OGID = x[0];
        var rowID = x[1];
        var text = '';
        var title = '';
        var OGDate = '';
        var firstRunNull = 1;
        $(timeLine.initiatives).each(function (key, value) {
            $.each(value.Events, function (key, value) {
                if (OGID == value.ID) {
                    OGDate = value.Date;
                    if (value.Text != null && timeLine.iconFilter(value.Branches, value.Regions) == 1) {
                        text = text + value.Text;
                        title = title + value.Hover;
                        firstRunNull = 0;
                    };
                };
            });
        });
        $(timeLine.initiatives).each(function (key, value) {
            if (value.ID == rowID) {
                $.each(value.Events, function (key, value) {
                    if (timeLine.fuzzyDate(OGDate, value.Date) == 1) {
                        if (value.Show == true && OGID != value.ID && value.Text != null && timeLine.iconFilter(value.Branches, value.Regions) == 1) {
                            if (firstRunNull != 1) {
                                text = text + "<hr />";
                            };
                            text = text + value.Text;
                            if (firstRunNull != 1) {
                                title = title + " - ";
                                firstRunNull = 0;
                            };
                            title = title + value.Hover;
                        };
                    };
                });
            }
        });
        if (text != '') {
            $("#dialog").dialog("open");
            $("#dialog").html(text);
            $("#ui-id-1").html(title);
        }
    },
    // popup script for initiatives
    dialogCustom: function (x) {
        $("#dialog").dialog("open");
        $("#dialog").html(x);
        $("#ui-id-1").html("&nbsp;");
    },
    // currently does nothing. will use this later if we wanted to have fuzzy dates (ex: jan5 opens jan4 and jan6 because they're overlapping)
    fuzzyDate: function (x, y) {
        if (x == y) {
            return 1;
        };
    },
    // function to push timeline bar based on date
    getLeft: function (date) {
        var date = date.split("/");
        var month = ((Number(date[2]) * 12) + Number(date[0]));
        var day = ((Number(date[1]) / 30) * timeLine.widthMonth);
        var startMonth = (timeLine.startYear * 12) + timeLine.startMonth;
        var z = ((month - startMonth) * timeLine.widthMonth) + day;
        return z;
    },
    // function to push icon based on date and bar start date
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
    // function to get width of bar
    getRight: function (s, e) {
        var start = s.split("/");
        var end = e.split("/");
        var offsetDay = ((Number(start[1]) / 30) * timeLine.widthMonth) - 1;
        var addDay = ((Number(end[1]) / 30) * timeLine.widthMonth);
        var x = ((((end[2] - start[2]) * 12) + (end[0] - start[0])) * timeLine.widthMonth) - offsetDay + addDay;
        return x;
    },
    // moves timeline left
    goLeft: function () {
        $("#dragContainer").css('left', "+=" + timeLine.widthMonth * 2);
        $("#quarterMonthContainer").css('left', "+=" + timeLine.widthMonth * 2);
    },
    // moves timeline right
    goRight: function () {
        $("#dragContainer").css('left', "-=" + timeLine.widthMonth * 2);
        $("#quarterMonthContainer").css('left', "-=" + timeLine.widthMonth * 2);
    },
    // runs filter when dropdown changes
    viewSelect: function () {
        timeLine.filter();
    },
    // runs filter when dropdown changes
    areaSelect: function () {
        timeLine.filter();
    },
    // runs filter when dropdown changes
    branchSelect: function () {
        timeLine.filter();
    },
    orderRows: function () {
        var getAll = [];
        var getBP2020 = [];
        var getTransformationTimeline = [];
        $.each(timeLine.initiatives, function (key, value) {
            getAll.push({ id: timeLine.initiatives[key].ID, name: timeLine.initiatives[key].Name, view: timeLine.initiatives[key].Timeline, impact: timeLine.initiatives[key].Impacts });
            $("#p" + timeLine.initiatives[key].ID).css("display", "inline");
            $("#t" + timeLine.initiatives[key].ID).css("display", "inline");
        });
        getTransformationTimeline = $.grep(getAll, function (elem, idx) {
            return elem.view == "TransformationTimeline";
        });
        getBP2020 = $.grep(getAll, function (elem, idx) {
            return elem.view == "BP2020";
        });
        getTransformationTimeline = getTransformationTimeline.sort(function (a, b) { return b.name.localeCompare(a.name); });
        getBP2020 = getBP2020.sort(function (a, b) { return b.name.localeCompare(a.name); });
        for (var i = 0; i < getBP2020.length; i++) {
            $("#p" + getBP2020[i].id).insertAfter("#pg0");
            $("#t" + getBP2020[i].id).insertAfter("#tg0");
        }
        for (var i = 0; i < getTransformationTimeline.length; i++) {
            $("#p" + getTransformationTimeline[i].id).insertAfter("#pg0");
            $("#t" + getTransformationTimeline[i].id).insertAfter("#tg0");
        }
    },
    // this is where the magic happens
    filter: function () {
                var rSelected = $('#areaSelect').val();
                var bSelected = $('#branchSelect').val();
        var vSelected = $('#viewSelect').val();
        var getAll = [];
        var color = ["#dbdbdb", "#f0caeb", "#ebf2b1", "#abdbcf", "#D1E8FF"];
        var rowsActive = timeLine.countTimeLine;
        var levelCount = [];
        // first run popup
        if (timeLine.runOnce == 0) {
            //timeLine.dialogCustom(timeLine.utility.translate("splash"));
            timeLine.runOnce = 1;
        };
        // store all initiatives in object
        $.each(timeLine.initiatives, function (key, value) {
            getAll.push({ id: timeLine.initiatives[key].ID, name: timeLine.initiatives[key].Name, view: timeLine.initiatives[key].Timeline, impact: timeLine.initiatives[key].Impacts });
            $("#p" + timeLine.initiatives[key].ID).css("display", "inline");
            $("#t" + timeLine.initiatives[key].ID).css("display", "inline");
        });
        getTransformationTimeline = $.grep(getAll, function (elem, idx) {
            return elem.view == "TransformationTimeline";
        });
        getBP2020 = $.grep(getAll, function (elem, idx) {
            return elem.view == "BP2020";
        });
        // if view is selected hide all and display view
        if (vSelected != "") {
            for (var i = 0; i < getAll.length; i++) {
                $("#p" + getAll[i].id).css("display", "none");
                $("#t" + getAll[i].id).css("display", "none");
            }
            getAll = eval("get".concat(vSelected));
            rowsActive = 0;
            for (var i = 0; i < getAll.length; i++) {
                $("#p" + getAll[i].id).css("display", "inline");
                $("#t" + getAll[i].id).css("display", "inline");
                rowsActive = rowsActive + 1;
            }
        }
        // if region is selected, unlock branches
                if (rSelected != "") {
                    $("#branchSelect").prop("disabled", false);
                } else {
                    $("#branchSelect").prop("disabled", true);
                    $("#branchSelect").val('');
                };
        //reset
        $(".projectGroupRow").css("display", "none");
        $(".timeLineGroupRow").css("display", "none");
        // if region/branch is selected, filter
                if (rSelected != "" && bSelected != "") {
            getAll = getAll.sort(function (a, b) { return b.name.localeCompare(a.name); });
            for (var i = 0; i < getAll.length; i++) {
                var level = 0;
                var impact = getAll[i].impact;
                for (var j in impact) {
                    var branches = impact[j].Branches;
                    var regions = impact[j].Regions;
                    // reset to zero
                    $("#p" + getAll[i].id).insertAfter("#pg0");
                    $("#t" + getAll[i].id).insertAfter("#tg0");
                            if ($.inArray(parseInt(bSelected), branches) > -1 && $.inArray(parseInt(rSelected), regions) > -1) {
                        level = impact[j].Level;
                        levelCount.push(level);
                                };
                }
                $("#pg" + level).css("display", "inline");
                $("#tg" + level).css("display", "inline");
                        if (eval("timeLine.hide".concat(level)) == 1) {
                            $("#hide" + level).css("display", "none");
                            $("#show" + level).css("display", "inline");
                    $("#p" + getAll[i].id).css("display", "none");
                    $("#t" + getAll[i].id).css("display", "none");
                    rowsActive = rowsActive - 1;
                        } else {
                            $("#hide" + level).css("display", "inline");
                            $("#show" + level).css("display", "none");
                    $("#p" + getAll[i].id).css("display", "inline");
                    $("#t" + getAll[i].id).css("display", "inline");
                }
                $("#p" + getAll[i].id).insertAfter("#pg" + level);
                $("#t" + getAll[i].id).insertAfter("#tg" + level);
                $("#p" + getAll[i].id).css("background-color", color[level]);
                $("#t" + getAll[i].id).css("background-color", color[level]);
                };
        }
                timeLine.toggleIcons();
                var top = timeLine.heightQuarter + timeLine.heightMonth;
        var uniqueLevelCount = [];
        $.each(levelCount, function (i, el) {
            if ($.inArray(el, uniqueLevelCount) === -1) uniqueLevelCount.push(el);
        });
        var levelSum = uniqueLevelCount.length;
        $("#today").css("height", (rowsActive) * timeLine.timeLineHeight + (timeLine.heightGroup * levelSum));
        $("#dragContainer").css("margin-top", (((rowsActive) * timeLine.timeLineHeight) + top) * -1 - (timeLine.heightGroup * levelSum));
    },
    // stuff that goes under the timeline
    footer: function () {
        var html = '';
        html += "<div id='timeLineFooter'>"
        html += "<div id='iAmLegend'>"
        html += "<div class='box' style='background-color:#226655'></div> " + timeLine.utility.translate("highIm") + "<br />"
        html += "<div class='box' style='background-color:#CCDD44'></div> " + timeLine.utility.translate("medIm") + "<br />"
        html += "<div class='box' style='background-color:#551155'></div> " + timeLine.utility.translate("lowIm") + "<br />"
        html += "<div class='box' style='background-color:#666666'></div> " + timeLine.utility.translate("noIm") + "<br />"
        html += "</div>"
        html += "<div id='clear'><a href='' id='clearResults'>" + timeLine.utility.translate("clear") + "</a></div>"
        html += "</div>"
        return html;
    },
    // resets dropdowns
    clearResults: function () {
        $("#viewSelect").val('');
        $("#areaSelect").val('');
        $("#branchSelect").val('');
        timeLine.reset();
        timeLine.orderRows();
        timeLine.filter();
    },
    // resets filter
    reset: function () {
        $(".projectRow").css("display", "inline");
        $(".timeLineRow").css("display", "inline");
        $(".timeLineBar").css("background-color", "#eeeeee");
        $(".projectRow").css("background-color", "#ffffff");
        $(".timeLineRow").css("background-color", "#ffffff");
        $(".tBP2020").css("background-color", "#D1E8FF");
        $(".pBP2020").css("background-color", "#D1E8FF");
        var top = timeLine.heightQuarter + timeLine.heightMonth;
        $("#today").css("height", timeLine.countTimeLine * timeLine.timeLineHeight);
        $("#dragContainer").css("margin-top", ((timeLine.countTimeLine * timeLine.timeLineHeight) + top) * -1);
        timeLine.hide0 = 0;
        timeLine.hide1 = 0;
        timeLine.hide2 = 0;
        timeLine.hide3 = 0;
        timeLine.hide4 = 0;
    },
    // display or hide icon based on dropdown selections
    toggleIcons: function () {
        $(timeLine.initiatives).each(function (key, value) {
            $.each(value.Events, function (key, value) {
                if (value.Show == true) {
                    if (timeLine.iconFilter(value.Branches, value.Regions) == 1) {
                        $("#icon" + value.ID).css("display", "inline");
                    } else {
                        $("#icon" + value.ID).css("display", "none");
                    };
                };
            });
        });
    },
    // sub function of toggleicons seperated so we can re-use it
    iconFilter: function (branches, regions) {
        var display = 0;
        var b = parseInt($('#branchSelect').val());
        var r = parseInt($('#areaSelect').val());
        if (isNaN(b)) { b = 1; };
        if (isNaN(r)) { r = 1; };
        display = 0;
        if (r == 1 && b == 1) {
            display = 1;
        };
        if (r == 1 && $.inArray(b, branches) > -1) {
            display = 1;
        };
        if (r == 1 && $.inArray(1, branches) > -1) {
            display = 1;
        };
        if (b == 1 && $.inArray(r, regions) > -1) {
            display = 1;
        };
        if (b == 1 && $.inArray(1, regions) > -1) {
            display = 1;
        };
        if ($.inArray(b, branches) > -1 && $.inArray(r, regions) > -1) {
            display = 1;
        };
        if (branches == 1) {
            display = 1
        };
        if (regions == 1) {
            display = 1
        };
        return display;
    },
    // removes funky chars @#$%
    cleanSpecialCharacters: function (x) {
        x = x.replace(/(\r\n|\n|\r)/gm, "");
        var y = document.createElement("div");
        y.innerText = y.textContent = x;
        x = y.innerHTML;
        function escapeRegExp(str) {
            return str.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
        }
        function replaceAll(str, find, replace) {
            return str.replace(new RegExp(escapeRegExp(find), 'g'), replace);
        }
        x = replaceAll(x, "\"", "'");
        x = replaceAll(x, "'", "&#39;");
        x = replaceAll(x, "\"", "&#34;");
        return x;
    },
    // css stuff
    css: function () {
        var top = timeLine.heightQuarter + timeLine.heightMonth;
        $(".projectRow").css("height", timeLine.timeLineHeight - 1 - 10);
        $(".projectRowBackground").css("height", timeLine.timeLineHeight);
        $(".timeLineRow").css("height", timeLine.timeLineHeight - 1);
        $(".projectSpace").css("height", top - 1);
        $("#today").css("height", timeLine.countTimeLine * timeLine.timeLineHeight);
        $("#dragContainer").css("margin-top", ((timeLine.countTimeLine * timeLine.timeLineHeight) + top) * -1);
        var now = new Date();
        var today = Number(now.getMonth() + 1) + "/" + now.getDate() + "/" + now.getFullYear();
        $("#dragContainer").css("left", timeLine.getLeft(today) * -1 + 575);
        $("#quarterMonthContainer").css("left", timeLine.getLeft(today) * -1 + 575);
    }
}

// skeleton
$(document).ready(function () {
    $('#navHeader').css({
        'background-color': '#ffffff',
        'z-index': '100',
        'display': 'inline-block',
        'position': 'absolute'
    });
    timeLine.content();
    $('body').on("click", "#leftButton", timeLine.goLeft);
    $('body').on("click", "#rightButton", timeLine.goRight);
    $('body').on("change", "#viewSelect", timeLine.viewSelect);
    $('body').on("change", "#areaSelect", timeLine.areaSelect);
    $('body').on("change", "#branchSelect", timeLine.branchSelect);
    $('body').on("click", "#hide0", function () { timeLine.hide0 = 1; timeLine.filter(); });
    $('body').on("click", "#hide1", function () { timeLine.hide1 = 1; timeLine.filter(); });
    $('body').on("click", "#hide2", function () { timeLine.hide2 = 1; timeLine.filter(); });
    $('body').on("click", "#hide3", function () { timeLine.hide3 = 1; timeLine.filter(); });
    $('body').on("click", "#hide4", function () { timeLine.hide4 = 1; timeLine.filter(); });
    $('body').on("click", "#show0", function () { timeLine.hide0 = 0; timeLine.filter(); });
    $('body').on("click", "#show1", function () { timeLine.hide1 = 0; timeLine.filter(); });
    $('body').on("click", "#show2", function () { timeLine.hide2 = 0; timeLine.filter(); });
    $('body').on("click", "#show3", function () { timeLine.hide3 = 0; timeLine.filter(); });
    $('body').on("click", "#show4", function () { timeLine.hide4 = 0; timeLine.filter(); });
    $('body').on("click", "#clearResults", timeLine.clearResults);
    $("#dialog").dialog({ autoOpen: false, width: "50%", maxWidth: "768px" });
    $(document).tooltip({ items: ':not(.ui-button)' });
    $("body").bind("click", function (e) {
        if ($("#dialog").dialog("isOpen")
            && !$(e.target).is("img, a")
            && !$(e.target).closest('.ui-dialog').length) {
            $("#dialog").dialog("close");
        }
    });
});

var setFixedHeader = function() {
    var $window = $(window);
    var $mainContentArea = $('#wb-main-in');
    var $navHeader = $('#navHeader');
    var $dragContainer = $('#dragContainer');
    var initialNavHeaderOffset = $navHeader.offset().top;
    var returnNavHeaderPosition = initialNavHeaderOffset - $mainContentArea.offset().top;
    var isFixed = false;
    
    $window.on('scroll', function() {
        var currentScroll = $window.scrollTop();
        var maxBottomScroll = $dragContainer.offset().top + $dragContainer.height() - $navHeader.height();
        if (isFixed === false && currentScroll > initialNavHeaderOffset) {
            if (currentScroll < maxBottomScroll) {
                isFixed = true;
                $navHeader.css({
                    top: 0,
                    position: 'fixed'
                });   
            }
        } else if (isFixed === true && 
            (currentScroll <= initialNavHeaderOffset || currentScroll >= maxBottomScroll)) {
            isFixed = false;
            $navHeader.css({
                top: returnNavHeaderPosition,
                position: 'absolute'
            });
    }
    });
};

$.ui.plugin.add("draggable", "alsoDrag", {
    start: function() {
        var that = $(this).data("ui-draggable"),
            o = that.options,
            _store = function (exp) {
                $(exp).each(function() {
                    var el = $(this);
                    el.data("ui-draggable-alsoDrag", {
                        top: parseInt(el.css("top"), 10),
                        left: parseInt(el.css("left"), 10)
                    });
    });
            };

        if (typeof(o.alsoDrag) === "object" && !o.alsoDrag.parentNode) {
            if (o.alsoDrag.length) { o.alsoDrag = o.alsoDrag[0]; _store(o.alsoDrag); }
            else { $.each(o.alsoDrag, function (exp) { _store(exp); }); }
        }else{
            _store(o.alsoDrag);
}
    },
    drag: function () {
        var that = $(this).data("ui-draggable"),
            o = that.options,
            os = that.originalSize,
            op = that.originalPosition,
            delta = {
                top: (that.position.top - op.top) || 0, 
                left: (that.position.left - op.left) || 0
            },

            _alsoDrag = function (exp, c) {
                $(exp).each(function() {
                    var el = $(this), start = $(this).data("ui-draggable-alsoDrag"), style = {},
                        css = ["top", "left"];

                    $.each(css, function (i, prop) {
                        var sum = (start[prop]||0) + (delta[prop]||0);
                        style[prop] = sum || null;
                    });

                    el.css(style);
                });
            };

        if (typeof(o.alsoDrag) === "object" && !o.alsoDrag.nodeType) {
            $.each(o.alsoDrag, function (exp, c) { _alsoDrag(exp, c); });
        }else{
            _alsoDrag(o.alsoDrag);
        }
    },
    stop: function() {
        $(this).removeData("draggable-alsoDrag");
}
});

$.extend($.ui.dialog.prototype.options, {
    modal: true
});
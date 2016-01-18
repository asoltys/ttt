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
    //initiativesURL : 'json/initiatives.json',
    branches: '',
    branchesURL : '/en/branches/data',
    //branchesURL : 'json/branches.json',
    regions: '',
    regionsURL : '/en/regions/data',
    //regionsURL : 'json/regions.json',
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
    runOnce: 0,
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
                timeLine.render += timeLine.footer();
				$("#render").append(timeLine.render);
                $("#dragContainer").draggable({ axis: "x" });
				timeLine.toggleIcons();
				timeLine.css();
            }
        });
    },
    leftNav: function () {
        var html = '';
        html = "<img src='/timeline/img/arrow_left.png' style='width:30px; height:30px;' class='scrollButton' id='leftButton' />";
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
                if(lang == 'e'){
					timeLine.regions = regions.sort(function (a, b) { return a.NameE.localeCompare(b.NameE); });
				} else {
					timeLine.regions = regions.sort(function (a, b) { return a.NameF.localeCompare(b.NameF); });
				};
                $.each(timeLine.regions, function (key, value) {
                    if (timeLine.regions[key].ID == 1) {
                        if(lang == 'e'){
							html += "<option value='" + timeLine.regions[key].ID + "'>" + timeLine.regions[key].NameE + "</option>"
						} else {
							html += "<option value='" + timeLine.regions[key].ID + "'>" + timeLine.regions[key].NameF + "</option>"
						};
                    };
                });
                $.each(timeLine.regions, function (key, value) {
                    if (timeLine.regions[key].ID != 1) {
						if(lang == 'e'){
							html += "<option value='" + timeLine.regions[key].ID + "'>" + timeLine.regions[key].NameE + "</option>"
						} else {
							html += "<option value='" + timeLine.regions[key].ID + "'>" + timeLine.regions[key].NameF + "</option>"
						};
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
                timeLine.branches = branches.sort(function (a, b) { return a.NameE.localeCompare(b.NameE); });
                $.each(timeLine.branches, function (key, value) {
                    if (timeLine.branches[key].ID == 1) {
						if(lang == 'e'){
							html += "<option value='" + timeLine.branches[key].ID + "'>" + timeLine.branches[key].NameE + "</option>"
						} else {
							html += "<option value='" + timeLine.branches[key].ID + "'>" + timeLine.branches[key].NameF + "</option>"
						};
                    };
                });
                $.each(timeLine.branches, function (key, value) {
                    if (timeLine.branches[key].ID != 1) {
						if(lang == 'e'){
							html += "<option value='" + timeLine.branches[key].ID + "'>" + timeLine.branches[key].NameE + "</option>"
						} else {
							html += "<option value='" + timeLine.branches[key].ID + "'>" + timeLine.branches[key].NameF + "</option>"
						};
                    };
                });
                html += "</select>";
                $("#branchNav").append(html);
            }
        });
    },
    rightNav: function () {
        var html = '';
        html += "<img src='/timeline/img/arrow_right.png' style='width:30px; height:30px;' class='scrollButton' id='rightButton' />";
        return html;
    },
    projectContainer: function () {
        var html = '';
        html += "<div id='projectContainer'>";
        html += "<div class='projectSpace'></div>";
        html += "<div class='projectGroupRow' id='pg3'>" + timeLine.utility.translate("highImDesc") + " <a href='#' style='color:#ffffff;' id='hide3' class='hide'>" + timeLine.utility.translate("hide") + "</a><a href='#' style='color:#ffffff;' id='show3' class='show'>" + timeLine.utility.translate("show") + "</a></div>";
        html += "<div class='projectGroupRow' id='pg2'>" + timeLine.utility.translate("medImDesc") + " <a href='#' style='color:#000000;' id='hide2' class='hide'>" + timeLine.utility.translate("hide") + "</a><a href='#' style='color:#000000;' id='show2' class='show'>" + timeLine.utility.translate("show") + "</a></div>";
        html += "<div class='projectGroupRow' id='pg1'>" + timeLine.utility.translate("lowImDesc") + " <a href='#' style='color:#ffffff;' id='hide1' class='hide'>" + timeLine.utility.translate("hide") + "</a><a href='#' style='color:#ffffff;' id='show1' class='show'>" + timeLine.utility.translate("show") + "</a></div>";
        html += "<div class='projectGroupRow' id='pg0'>" + timeLine.utility.translate("noImDesc") + " <a href='#' style='color:#ffffff;' id='hide0' class='hide'>" + timeLine.utility.translate("hide") + "</a><a href='#' style='color:#ffffff;' id='show0' class='show'>" + timeLine.utility.translate("show") + "</a></div>";
        $.each(timeLine.initiatives, function (key, value) {
            timeLine.countTimeLine = timeLine.countTimeLine + 1;			
            html += "<div class='projectRow' id='p" + timeLine.initiatives[key].ID + "'><a href='#' onClick='timeLine.dialogCustom(\"" + eval("timeLine.initiatives[key].Description".concat(lang.toUpperCase())) + "\")'>" + eval("timeLine.initiatives[key].Name".concat(lang.toUpperCase())) + "</a></div>";
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
        return "<img src='/timeline/img/red.gif' id='today' style='margin-left:" + timeLine.getLeft(today) + "px' />";
    },
    timeLineContainer: function () {
        var html = '';
        var barStartDate = '';
		var mergeDates = [];
        html += "<div id='timeLineContainer'>";
        html += "<div class='timeLineGroupRow' id='tg3' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'></div>";
        html += "<div class='timeLineGroupRow' id='tg2' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'></div>";
        html += "<div class='timeLineGroupRow' id='tg1' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'></div>";
        html += "<div class='timeLineGroupRow' id='tg0' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'></div>";
        $(timeLine.initiatives).each(function (key, value) {
			mergeDates = [];
            barStartDate = value.StartDate;
            html += "<div class='timeLineRow' id = 't" + value.ID + "' style='width:" + timeLine.widthMonth * timeLine.totalMonth() + "px;'>";
            html += "<div class='timeLineBar' style='margin-left:" + timeLine.getLeft(barStartDate) + "px; width:" + timeLine.getRight(barStartDate, value.EndDate) + "px;'>"
			$.each(value.Events, function (key, value) {
				if(value.Show){
					mergeDates.push(value.Date);
				};
            });
            mergeDates = jQuery.unique(mergeDates);
            $.each(mergeDates, function(index, dates) {
				var image = '';
                var mergedDate = '';
                var id = [];
				$.each(value.Events, function (key, value) {
                if (value.Show && dates == value.Date){
					if (value.Type == 'Milestone') {
						image = 'circle.png'
					} else if (value.Type == 'Training') {
						image = 'book.png'
					};
					mergedDate = dates;
					id.push(value.ID);
				}
				});
				idString = toString(id);
				html += "<img id='icon" + id[0] + "' title='" + timeLine.hover(id) + "' onClick='timeLine.dialog(\"" + id + "\")' src='/timeline/img/" + image + "' class='event' style='width:24px; height:32px; margin-left:" + timeLine.getEvent(mergedDate, barStartDate) + "px; position:absolute;' />";
			});
			html += "</div></div>";
		});
		html += "</div>";
		return html;
    },
	hover: function(x){
		var hover = '';
		var check = '';
		var firstRun = 0;
		$.each(x, function(index,value) {
			check = value;
			$(timeLine.initiatives).each(function (key, value) {
				$.each(value.Events, function (key, value) {
					if (check == value.ID) {
						if (firstRun == 0){
							firstRun = 1;
						} else {
							hover = hover + " - ";
						};
						hover = hover + eval("value.Hover".concat(lang.toUpperCase()));
					};
				});
			});
		});
		hover = hover.replace("'", "&#39;");
		hover = hover.replace("\"", "&#34;");
		return hover;
	},
    dialog: function (x) {
		if (x.length > 0) {
			x = x.split(",");
		};
		var text = '';
		var title = '';
		var check = '';
		var firstRun = 0;
		$.each(x, function(index,value) {
			check = value;
			$(timeLine.initiatives).each(function (key, value) {
				$.each(value.Events, function (key, value) {
					if (check == value.ID) {
						if (firstRun == 0){
							firstRun = 1;
						} else {
							text = text + "<hr />";
							title = title + " - ";
						};
						text = text + eval("value.Text".concat(lang.toUpperCase()));
						title = title + eval("value.Hover".concat(lang.toUpperCase()));
					};
				});
			});
		});
		$("#dialog").dialog("open");
		$("#dialog").html(text);
		$("#ui-id-1").html(title);
    },
	dialogCustom: function (x) {
        $("#dialog").dialog("open");
        $("#dialog").html(x);
		$("#ui-id-1").html("&nbsp;");
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
                var id = '';
                var currentLevel = '';
                var rSelected = $('#areaSelect').val();
                var bSelected = $('#branchSelect').val();
                var branches = '';
                var regions = '';
                var setLevel0 = 0;
                var setLevel1 = 0;
                var setLevel2 = 0;
                var setLevel3 = 0;
                var currentLevel0 = 0;
                var currentLevel1 = 0;
                var currentLevel2 = 0;
                var currentLevel3 = 0;
                var rowsHidden = 0;
				$("#pg0").css("display", "none");
                $("#tg0").css("display", "none");
				$("#pg1").css("display", "none");
                $("#tg1").css("display", "none");
				$("#pg2").css("display", "none");
                $("#tg2").css("display", "none");
				$("#pg3").css("display", "none");
                $("#tg3").css("display", "none");
					if(rSelected != "" && bSelected != "" ){
					if(timeLine.runOnce == 0){
						timeLine.dialogCustom(timeLine.utility.translate("splash"));
						timeLine.runOnce = 1;
					};
					$.each(timeLine.initiatives, function (key, value) {
						var id = timeLine.initiatives[key].ID;
						currentLevel0 = 1;
						$.each(value.Impacts, function (key, value) {
							currentLevel = value.Level;
							regions = value.Regions;
							branches = value.Branches;
							if ($.inArray(parseInt(bSelected), branches) > -1 && $.inArray(parseInt(rSelected), regions) > -1) {
								currentLevel0 = 0;
								if(currentLevel == 0){
									if(timeLine.hide0 == 1){
										$("#hide" + currentLevel).css("display", "none");
										$("#show" + currentLevel).css("display", "inline");
										$("#p" + id).css("display", "none");
										$("#t" + id).css("display","none");
										rowsHidden = rowsHidden + 1;
									} else {
										$("#hide" + currentLevel).css("display", "inline");
										$("#show" + currentLevel).css("display", "none");
										$("#p" + id).css("display", "inline");
										$("#t" + id).css("display","inline");
									};
									setLevel0 = 1;
									$("#p" + id).css("background-color", "#dbdbdb");
									$("#t" + id).css("background-color","#dbdbdb");
								};
								if(currentLevel == 1){
									if(timeLine.hide1 == 1){
										$("#hide" + currentLevel).css("display", "none");
										$("#show" + currentLevel).css("display", "inline");
										$("#p" + id).css("display", "none");
										$("#t" + id).css("display","none");
										rowsHidden = rowsHidden + 1;
									} else {
										$("#hide" + currentLevel).css("display", "inline");
										$("#show" + currentLevel).css("display", "none");
										$("#p" + id).css("display", "inline");
										$("#t" + id).css("display","inline");
									};
									setLevel1 = 1;
									$("#p" + id).css("background-color", "#f0caeb");
									$("#t" + id).css("background-color","#f0caeb");
								};
								if(currentLevel == 2){
									if(timeLine.hide2 == 1){
										$("#hide" + currentLevel).css("display", "none");
										$("#show" + currentLevel).css("display", "inline");
										$("#p" + id).css("display", "none");
										$("#t" + id).css("display","none");
										rowsHidden = rowsHidden + 1;
									} else {
										$("#hide" + currentLevel).css("display", "inline");
										$("#show" + currentLevel).css("display", "none");
										$("#p" + id).css("display", "inline");
										$("#t" + id).css("display","inline");
									};
									setLevel2 = 1;
									$("#p" + id).css("background-color", "#ebf2b1");
									$("#t" + id).css("background-color","#ebf2b1");
								};
								if(currentLevel == 3){
									if(timeLine.hide3 == 1){
										$("#hide" + currentLevel).css("display", "none");
										$("#show" + currentLevel).css("display", "inline");
										$("#p" + id).css("display", "none");
										$("#t" + id).css("display","none");
										rowsHidden = rowsHidden + 1;
									} else {
										$("#hide" + currentLevel).css("display", "inline");
										$("#show" + currentLevel).css("display", "none");
										$("#p" + id).css("display", "inline");
										$("#t" + id).css("display","inline");
									};
									setLevel3 = 1;
									$("#p" + id).css("background-color", "#abdbcf");
									$("#t" + id).css("background-color","#abdbcf");
								};
								$("#pg" + currentLevel).css("display", "inline");
								$("#tg" + currentLevel).css("display", "inline");
								$("#p" + id).insertAfter("#pg" + currentLevel);
								$("#t" + id).insertAfter("#tg" + currentLevel);
							};
						});
					if(currentLevel0 == 1){
						if(timeLine.hide0 == 1){
							$("#hide0").css("display", "none");
							$("#show0").css("display", "inline");
							$("#p" + id).css("display", "none");
							$("#t" + id).css("display","none");
							rowsHidden = rowsHidden + 1;
						} else {
							$("#hide0").css("display", "inline");
							$("#show0").css("display", "none");
							$("#p" + id).css("display", "inline");
							$("#t" + id).css("display","inline");
						};
						setLevel0 = 1;
						$("#pg0").css("display", "inline");
						$("#tg0").css("display", "inline");
						$("#p" + id).insertAfter("#pg0");
						$("#t" + id).insertAfter("#tg0");
						$("#p" + id).css("background-color", "#dbdbdb");
						$("#t" + id).css("background-color","#dbdbdb");
						$(".timeLineBar").css("background-color","#ffffff");
					}
					});
				} else {
					timeLine.reset();
				};
			timeLine.toggleIcons();
			var top = timeLine.heightQuarter + timeLine.heightMonth;
			var levelSum = setLevel0 + setLevel1 + setLevel2 + setLevel3;
			$("#today").css("height", (timeLine.countTimeLine - rowsHidden) * timeLine.timeLineHeight + (timeLine.heightGroup * levelSum));
			$("#dragContainer").css("margin-top", (((timeLine.countTimeLine - rowsHidden) * timeLine.timeLineHeight) + top) * -1 - (timeLine.heightGroup * levelSum));
            }
        });
    },
	reset: function () {
		$(".projectRow").css("display", "inline");
		$(".timeLineRow").css("display","inline");
		$(".timeLineBar").css("background-color","#eeeeee");
		$(".projectRow").css("background-color", "#ffffff");
		$(".timeLineRow").css("background-color","#ffffff");
		var top = timeLine.heightQuarter + timeLine.heightMonth;
		$("#today").css("height", timeLine.countTimeLine * timeLine.timeLineHeight);
		$("#dragContainer").css("margin-top", ((timeLine.countTimeLine * timeLine.timeLineHeight) + top) * -1);
		timeLine.hide0 = 0;
		timeLine.hide1 = 0;
		timeLine.hide2 = 0;
		timeLine.hide3 = 0;
	},
	footer: function () {
		 var html = '';
		 html += "<div id='timeLineFooter'><a href='' id='clearResults'>" + timeLine.utility.translate("clear") + "</a></div>"
		 return html;
	},
	clearResults: function () {
		$("#areaSelect").val('');
		$("#branchSelect").val('');
		timeLine.filter();
	},
	toggleIcons: function (x,y){
		var rSelected = $('#areaSelect').val();
        var bSelected = $('#branchSelect').val();
		var mergeDates = [];
        $(timeLine.initiatives).each(function (key, value) {
			mergeDates = [];
            $.each(value.Events, function (key, value) {
				if(value.Show){
					mergeDates.push(value.Date);
				};
            });
            mergeDates = jQuery.unique(mergeDates);
            $.each(mergeDates, function(index, dates) {
                var mergedDate = '';
                var id = [];
				$.each(value.Events, function (key, value) {
					if (value.Show && dates == value.Date){
						mergedDate = dates;
						id.push(value.ID);
						var b = parseInt($('#branchSelect').val());
						var r = parseInt($('#areaSelect').val());
						if(isNaN(b)){
							var b = 1;
						};
						if(isNaN(r)){
							var r = 1;
						};
						if($.inArray(b, value.Branches) > -1 && $.inArray(r, value.Regions) > -1){
							$("#icon"+id[0]).css("display", "inline");
						} else {
							$("#icon"+id[0]).css("display", "none");
						};
					}
				});
			});
        });
	},
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
    }
}

$(document).ready(function () {
    timeLine.content();
    $('body').on("click", "#leftButton", timeLine.goLeft);
    $('body').on("click", "#rightButton", timeLine.goRight);
    $('body').on("change", "#areaSelect", timeLine.areaSelect);
    $('body').on("change", "#branchSelect", timeLine.branchSelect);
	$('body').on("click", "#hide0", function(){timeLine.hide0 = 1; timeLine.filter()});
	$('body').on("click", "#hide1", function(){timeLine.hide1 = 1; timeLine.filter()});
	$('body').on("click", "#hide2", function(){timeLine.hide2 = 1; timeLine.filter()});
	$('body').on("click", "#hide3", function(){timeLine.hide3 = 1; timeLine.filter()});
	$('body').on("click", "#show0", function(){timeLine.hide0 = 0; timeLine.filter()});
	$('body').on("click", "#show1", function(){timeLine.hide1 = 0; timeLine.filter()});
	$('body').on("click", "#show2", function(){timeLine.hide2 = 0; timeLine.filter()});
	$('body').on("click", "#show3", function(){timeLine.hide3 = 0; timeLine.filter()});
	$('body').on("click", "#clearResults", timeLine.clearResults);
    $("#dialog").dialog({ autoOpen: false,width: "50%", maxWidth: "768px" });
    $(document).tooltip({ items: ':not(.ui-button)' });
});


// jQuery custom extensions
$.extend($.ui.dialog.prototype.options, { modal: true });
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
	stop: function(event, ui) {
		$(this).removeData("draggable-alsoDrag");
	}
});

var helpers = (function($) { // $ => jQuery
	var _ajax = function (method, url, callback, jsonData) {
	    if (arguments.length == 3) 
	        return $.ajax({
	            type: method,
	            url: url,
	            cache: false,
	            success: function (data) { callback(JSON.stringify(data)); },
	            error: function (jqXHR, textStatus, errorThrown) { console.log(JSON.stringify(jqXHR) + ", " + textStatus); }
	        });
	    else 
	        return $.ajax({
	            type: method,
	            contentType: "application/json",
	            url: url,
	            cache: false,
	            data: JSON.stringify(jsonData),
	            success: function (data) { callback(JSON.stringify(data)); },
	            error: function (jqXHR, textStatus, errorThrown) { console.log(JSON.stringify(jqXHR) + ", " + textStatus); }
	        });
	}
	var _jsonCopy = function(jsonObj) { return JSON.parse(JSON.stringify(jsonObj)) }
	var _htmlDecode = function(value) { return $('<div/>').html(value).text(); }
	var _htmlEncode = function(value) { return $('<div/>').text(value).html(); }
	var _sortComparator = function (a, b) {
	    if (a > b) return 1;
	    if (a < b) return -1;
	    return 0;
	}
	var _truncate = function(string, limit) {
		return string.length >= limit ? string.substring(0, limit) + '...' : string;
	}
	return {
		ajax: _ajax,
		jsonCopy: _jsonCopy,
		htmlDecode: _htmlDecode,
		htmlEncode: _htmlEncode,
		sortComparator: _sortComparator,
		truncate: _truncate
	}
})($);

var timeline = (function(r, $, m, h) { // r => resources, $ => jQuery, m => moment, h => helpers
	var CULTURE = window.location.href.indexOf('lang=fra') > -1 ? 'fr-ca' : 'en-ca',
		CULTURE_APPEND = CULTURE == 'en-ca' ? 'E' : 'F',
		API_DATE_FORMAT = 'MM/DD/YYYY',
		GET = 'get', POST = 'post';
	m.locale(CULTURE);

	var dataManager = (function($, h) { // $ => jQuery, h => helpers, ui => ui
		var TIMELINES_URL = '/data/all', 
			REGIONS_URL = '/data/regions',
			BRANCHES_URL = '/data/branches';
		var TIMELINE_BUFFER = 3,
			TIMELINE_BUFFER_UNIT = 'month';
		var TIMELINES, REGIONS, BRANCHES, TIMESPAN;
		var timelines, regions, branches;
		var loaded = false;

		var _prepareData = function() {
			_prepareRegions();
			_prepareBranches();
			_findTimespan();
			_deepCopyData();
			return $.Deferred().resolve().promise();
		}

		var _prepareRegions = function() {
			REGIONS = _sortByName(REGIONS);
			var nca;
			REGIONS = REGIONS.filter(function (object) {
			    if (object.NameShort == "nca")
			        nca = object;
			    return object.NameShort != "nca"
			});
			REGIONS.unshift(nca);
		}

		var _prepareBranches = function() {
			BRANCHES = _sortByName(BRANCHES);
		}

		var _findTimespan = function() {
			var earliestDate, latestDate, startDates = [], endDates = [];
			TIMELINES.forEach(function(timeline, index, array) {
				timeline.Data.forEach(function(initiative, index, array) {
					startDates.push(moment(initiative.StartDate, API_DATE_FORMAT));
					endDates.push(moment(initiative.EndDate, API_DATE_FORMAT));
				});
				earliestDate = startDates.reduce(function(a, b) { return a < b ? a : b; });
				latestDate = endDates.reduce(function(a, b) { return a > b ? a : b; });
			});
			earliestDate.subtract(TIMELINE_BUFFER, TIMELINE_BUFFER_UNIT);
			latestDate.add(TIMELINE_BUFFER, TIMELINE_BUFFER_UNIT);
			var duration = moment.duration(latestDate.diff(earliestDate));
			TIMESPAN = { Start: earliestDate, End: latestDate,
				Duration: {
					Days: Math.ceil(duration.asDays()),
					Months: Math.ceil(duration.asMonths()),
					Years: Math.ceil(duration.asYears())
				}
			};
		}

		var _deepCopyData = function() {
			timelines = h.jsonCopy(TIMELINES);
			regions = h.jsonCopy(REGIONS);
			branches = h.jsonCopy(BRANCHES);
		}

		var _sortByName = function(data) {
		    return data.sort(function(a, b) {
		        return h.sortComparator(a['Name' + CULTURE_APPEND], b['Name' + CULTURE_APPEND]);
		    });
		}

		var _setLoadState = function() { loaded = true; }
		var _getLoadState = function() { return loaded; }
		var _unlockScreen = function() { _setLoadState(); ui.unlock(); }
		var _getTimelines = function() { return timelines; }
		var _getRegions   = function() { return regions; }
		var _getBranches  = function() { return branches; }
		var _getTimespan  = function() { return TIMESPAN; }

		var _initialize = (function() {
			h.ajax(POST, TIMELINES_URL, function(data) {
				TIMELINES = JSON.parse(data);
			}, CULTURE)
			.then(h.ajax(POST, REGIONS_URL, function(data) {
				REGIONS = JSON.parse(data);
			}))
			.then(h.ajax(POST, BRANCHES_URL, function(data) {
				BRANCHES = JSON.parse(data);
			}))
			.then(_prepareData)
			.then(_unlockScreen);
		})();

		var _filterTimeline = function(value) {
			if (value == 'all') {
				timelines = h.jsonCopy(TIMELINES);
				return;
			}
			timelines = TIMELINES.filter(function(timeline) {
				return timeline['Name' + CULTURE_APPEND] == value;
			})
		}

		return {
			loaded: _getLoadState,
			timelines: _getTimelines,
			regions: _getRegions,
			branches: _getBranches,
			timespan: _getTimespan,
			filterTimeline: _filterTimeline
		}
	})($, h)

	var ui = (function(r, $, h, dm) { // r => resources, $ => jQuery, h => helpers, dm => dataManager
		var MONTH_WIDTH = 100;

		var _unlock = function() {
			controllers();
			navigation();
			eventManager();
			ui.renderer.draw(dm.timelines());
			_showTool();
		}

		var _showTool = function() {
			$('#timeline-tool-loading-icon').addClass('hide');
			$('#timeline-tool').removeClass('hide');
		}

		// navigation, controllers, events are private submodules of ui - INACCESSIBLE FROM CONSOLE
		// please keep it this way
		var navigation = function() {
			var $navContainer = $('#nav-calendar'),
				$calendarContainer = $('#nav-calendar-container'),
				$quarterContainer = $('#nav-calendar-quarter'),
				$monthContainer = $('#nav-calendar-month'),
				$dynamicContentContainer = $('#dynamic-content-container');
			var timespan = dm.timespan(); 	// be careful in handling moment object
											// day & year base 1, but month base 0
			var _populateCalendar = function() {
				_setupCalendarContainer();
				_populateQuarterLine();
				_populateMonthLine();
				_attachDraggableModule();
			}

			var _attachDraggableModule = function() {
				var xContainment = $calendarContainer.width() * -1 + $navContainer.width();
				var marginOffset = 42;
				var dragWith = '#nav-calendar-container';
				$dynamicContentContainer.draggable({
				    axis: 'x',
				    alsoDrag: dragWith,
				    stop: function(event, ui) {
				    	if (ui.position.left > 0) {
				    		$(this).animate({'left': '0px'}, 300);
				    		$(dragWith).animate({'left': '0px'}, 300);
				    	} else if (ui.position.left < xContainment) {
				    		$(this).animate({'left': xContainment}, 300);
				    		$(dragWith).animate({'left': xContainment}, 300);
				    	}
				    }
				});
			}

			var _setupCalendarContainer = function() {
				$calendarContainer.css('width', timespan.Duration.Months * MONTH_WIDTH);
				$dynamicContentContainer.css('width', timespan.Duration.Months * MONTH_WIDTH);
			}

			var _populateQuarterLine = function() {
				var currentMonth = timespan.Start.clone();
				for (var i = 0; i < timespan.Duration.Months; i++, currentMonth.add(1, 'month')) {
					$quarterContainer.append(_generateQuarterBox(currentMonth.month()));
				}
				$quarterContainer.append(_generateClearBox());
			}

			var _populateMonthLine = function() {
				var currentMonth = timespan.Start.clone();
				for (var i = 0; i < timespan.Duration.Months; i++, currentMonth.add(1, 'month')) {
					$monthContainer.append(_generateMonthBox(currentMonth));
				}
				$monthContainer.append(_generateClearBox());
			}

			var _generateQuarterBox = function(month) {
				var content = _generateQuarterString(month);
				var classToUse = _determineColorClass(month);
				var html = "<div class='" + classToUse + "' style='width:" + MONTH_WIDTH + "px;'>";
				html += content;
				html += "</div>";
				return html;
			}

			var _generateMonthBox = function(momentObject) {
				var content = _generateMonthString(momentObject);
				var classToUse = _determineColorClass(momentObject.month());
				var html = "<div class='" + classToUse + "' style='width:" + MONTH_WIDTH + "px;'>";
				html += content;
				html += "</div>";
				return html;
			}

			var _generateQuarterString = function(month) {
				month++;
				var quarterString = '| ' + r.ReportQuarterPrefix;
				switch(month) {
					case 4: quarterString += '1'; break; case 7: quarterString += '2'; break;
					case 10: quarterString += '3'; break; case 1: quarterString += '4'; break;
					default: quarterString = '&nbsp;';
				}
				return quarterString;
			}

			var _generateMonthString = function(momentObject) {
				var month = momentObject.month() + 1;
				var monthString = momentObject.format('MMM');
				var year = momentObject.year();
				switch(month) {
					case 4: case 7: case 10: case 1: monthString = '| ' + monthString + ' ' + year; break;
					default: monthString = '| ' + monthString;
				}
				return monthString;
			}

			var _determineColorClass = function(month) {
				month++;
				switch(month) {
					case 1: case 2: case 3: return 'fourth-quarter';
					case 4: case 5: case 6: return 'first-quarter';
					case 7: case 8: case 9: return 'second-quarter';
					case 10: case 11: case 12: return 'third-quarter';
					default: return 'first-quarter';
				}
			}

			var _generateClearBox = function() {
				return "<div class='clear'></div>"
			}

			var _initialize = (function() {
				_populateCalendar();
			})();

			var _getMonthWidth = function() { return MONTH_WIDTH; }

			return {
				monthWidth: _getMonthWidth
			}
		}

		var controllers = function() {
			var $left = $('#nav-go-left-button'),
				$right = $('#nav-go-right-button'),
				$timeline = $('#nav-controller-timeline'),
				$region = $('#nav-controller-region'),
				$branch = $('#nav-controller-branch');

			var _addOption = function(jQueryObject, optionObject) {
				var $select = jQueryObject,
					option = document.createElement('option');
				option.text = optionObject.text
				option.value = optionObject.value;
				$select.append(option);
			}

			var _populateTimeline = function() {
				dm.timelines().forEach(function(element, index, array) {
					var timeline = element['Name' + CULTURE_APPEND];
					var translation = r[timeline];
					_addOption($timeline, { text: translation, value: timeline });
				});
			}

			var _populateRegionsAndBranches = function() {
				dm.regions().forEach(function(element, index, array) {
					var region = element['Name' + CULTURE_APPEND];
					_addOption($region, { text: region, value: element.ID });
				});
				dm.branches().forEach(function(element, index, array) {
					var branch = element['Name' + CULTURE_APPEND];
					branch = h.truncate(branch, 45);
					_addOption($branch, { text: branch, value: element.ID });
				});
			}

			var _initialize = (function() {
				_populateTimeline();
				_populateRegionsAndBranches();
			})();
		}

		var eventManager = function() {
			var $left = $('#nav-go-left-button'),
				$right = $('#nav-go-right-button'),
				$timeline = $('#nav-controller-timeline'),
				$region = $('#nav-controller-region'),
				$branch = $('#nav-controller-branch'),
				$navContainer = $('#nav-calendar'),
				$calendar = $('#nav-calendar-container'),
				$dynamic = $('#dynamic-content-container');

			var _handleLeftClick = function() {
				var currentPosition = parseInt($calendar.css('left'), 10);
				var amountToMove = MONTH_WIDTH * 2;
				amountToMove = currentPosition + amountToMove >= 0 ?
							   0 : currentPosition + amountToMove;
				_setDragPosition(amountToMove);
			}

			var _handleRightClick = function() {
				var maxRightPosition = $calendar.width() * -1 + $navContainer.width();
				var currentPosition = parseInt($calendar.css('left'), 10);
				var amountToMove = MONTH_WIDTH * 2;
				amountToMove = currentPosition - amountToMove > maxRightPosition ? 
							   (currentPosition - amountToMove) : maxRightPosition;
				_setDragPosition(amountToMove);
			}

			var _setDragPosition = function(leftPosition) {
				$calendar.css('left', leftPosition + 'px');
				$dynamic.css('left', leftPosition + 'px');
			}

			var _handleTimeline = function() {
				dm.filterTimeline(this.value);
				ui.renderer.draw(dm.timelines());
			}

			var _handleRegion = function() {
				console.log("Region changed");
			}

			var _handleBranch = function() {
				console.log("branch changed");
			}

			var _keyMap = function(e) {
				// 33 == page up, 34 == page down, 37 == left arrow, 39 == right arrow
				switch (e.keyCode) {
					case 37: _handleLeftClick(); return;
					case 39: _handleRightClick(); return;
					default: return;
				}
			}

			var _registerEvents = (function() {
				$left.on('click', _handleLeftClick);
				$right.on('click', _handleRightClick);
				$timeline.on('change', _handleTimeline);
				$region.on('change', _handleRegion);
				$branch.on('change', _handleBranch);
				$('#dialog').dialog({ autoOpen: false, width: "50%", maxWidth: "768px" });
				$(document).tooltip({ items: ':not(.ui-button)' });
				document.addEventListener('keydown', _keyMap, false);
				$('body').bind('click', function (e) {
					if ($('#dialog').dialog('isOpen') && !$(e.target).is('img, a') && !$(e.target).closest('.ui-dialog').length) {
						$('#dialog').dialog('close');
					}
				});
				$('#dynamic-content-container').on('click', '.icon', function() { 
					ui.renderer.openDialog($(this));
				});
			})();
		}

		return {
			unlock: _unlock
		}
	})(r, $, h, dataManager);

	ui.renderer = (function(dm) {
		var $contentContainer = $('#dynamic-content-container'),
			$calendar = $('#nav-calendar-container');
		var MONTH_WIDTH = 100, 
			ROW_WIDTH,
			ROW_IN_MILLISECONDS;
		var timespan;

		var _momentize = function(date) {
			return moment(date, API_DATE_FORMAT);
		}

		var _calculateLeftMargin = function(momentObject1, momentObject2) {
			var leftStartInMS = momentObject1.diff(timespan.Start);
			if (arguments.length == 2)
				leftStartInMS = momentObject1.diff(momentObject2);
			return Math.floor(leftStartInMS / ROW_IN_MILLISECONDS * ROW_WIDTH);
		}

		var _calculateBarWidth = function(initiative) {
			var initiativeStartDate = _momentize(initiative.StartDate);
			var initiativeEndDate = _momentize(initiative.EndDate);
			return Math.floor((initiativeEndDate.diff(initiativeStartDate)) / ROW_IN_MILLISECONDS * ROW_WIDTH);
		}

		var _addTimelineRow = function(initiative) {
			var barLeftMargin = _calculateLeftMargin(_momentize(initiative.StartDate));
			var barWidth = _calculateBarWidth(initiative);
			var html = "";
			html += "<div class='timeline-row' style='width:" + ROW_WIDTH + "px'>";
			html += "<div class='timeline-bar align-middle' style='width: " + barWidth + "px; margin-left: " 
					+ barLeftMargin + "px;'>";
			html += _addIcons(initiative);
			html += "</div>"
			html += "</div>";
			return html;
		}

		var _addIcons = function(initiative) {
			var html = "";
			var iconWidth = 24;
			initiative.Events.forEach(function(event) {
				var eventLeftMargin = _calculateLeftMargin(_momentize(event.Date), _momentize(initiative.StartDate));
				eventLeftMargin -= iconWidth / 2; // subtract half of icon width
				var type = event.Type == 'Milestone' ? 'circle.png' : 'book.png';
				var hover = event['Hover' + CULTURE_APPEND];
				var dialogText = event['Text' + CULTURE_APPEND];
				html += "<img src='/timeline/img/" + type + "' class='icon' title='" + hover + 
				"' style='width:24px; height:32px; margin-left:" + eventLeftMargin + "px; position:absolute;'" +
				" data-title='" + hover + "' data-description='" + dialogText + "'/>";
			});
			return html;
		}

		var _addTodayLine = function() {
			return "<img src='/timeline/img/red.gif' id='today' style='margin-left:" + _calculateLeftMargin(moment()) + "px' />";
		}

		var _setDragPosition = function(leftPosition) {
			$calendar.css('left', leftPosition + 'px');
			$contentContainer.css('left', leftPosition + 'px');
		}

		var _showToday = function() {
			var viewableWidth = $contentContainer.parent().width();
			_setDragPosition(_calculateLeftMargin(moment()) * -1 + Math.floor(viewableWidth / 2));
		}

		var _reset = function() {
			$contentContainer.empty();
			timespan = dm.timespan();
			ROW_WIDTH = timespan.Duration.Months * MONTH_WIDTH;
			ROW_IN_MILLISECONDS = timespan.End.diff(timespan.Start);
		}

		var _draw = function(timelines) {
			if (timelines == null) return 'no argument was passed in';
			_reset();
			$contentContainer.append(_addTodayLine());
			timelines.forEach(function(timeline) {
				timeline.Data.forEach(function(initiative) {
					$contentContainer.append(_addTimelineRow(initiative));
				});
			});
			_showToday();
		}

		var _dialog = function($icon) {
			var title = '';
			var text = '';
			title = $icon.attr('data-title');
			text = $icon.attr('data-description');
			if (text != '' && text != 'null') {
				$("#dialog").dialog("open");
				$("#dialog").html(text);
				$("#ui-id-1").html(title);
			}
		}

		return {
			draw: _draw,
			openDialog: _dialog
		}
	})(dataManager);

	return {
		dataManager: dataManager,
		ui: ui
	}
})(resources, jQuery, moment, helpers);

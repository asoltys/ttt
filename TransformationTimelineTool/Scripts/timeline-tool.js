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
	var _keyExists = function(key, object, returnValue) {
		return key in object ? returnValue == true ? object[key] : true : false;
	}
	return {
		ajax: _ajax,
		jsonCopy: _jsonCopy,
		htmlDecode: _htmlDecode,
		htmlEncode: _htmlEncode,
		sortComparator: _sortComparator,
		truncate: _truncate,
		keyExists: _keyExists
	}
})($);

var timeline = (function(r, $, m, h) { // r => resources, $ => jQuery, m => moment, h => helpers
	var CULTURE = window.location.href.indexOf('lang=fra') > -1 ? 'fr-ca' : 'en-ca',
		CULTURE_APPEND = CULTURE == 'en-ca' ? 'E' : 'F',
		NAME_CULTURE = 'Name' + CULTURE_APPEND,
		TEXT_CULTURE = 'Text' + CULTURE_APPEND,
		DESC_CULTURE = 'Description' + CULTURE_APPEND,
		HOVER_CULTURE = 'Hover' + CULTURE_APPEND,
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
		var timeline = 'all', region = 'all', branch = 'all';
		var momentObject1 = moment(), momentObject2 = moment();

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

		var _sortByImpact = function(data) {
			return data.sort(function(a, b) {
				if (b.Weight == 4) return false;
				return h.sortComparator(b.Weight, a.Weight);
			})
		}

		// there are only 2 moment objects in data manager
		// object index needs to be defined to return the proper object
		var _momentize = function(date, objectIndex) {
			date = date == '' ? moment().format('MM/DD/YY').split('/') : date.split('/');
			if (objectIndex == 1) {
				momentObject1.set('month', (date[0] - 1));
				momentObject1.set('date', date[1]);
				momentObject1.set('year', date[2]);
				return momentObject1;
			} else {
				momentObject2.set('month', (date[0] - 1));
				momentObject2.set('date', date[1]);
				momentObject2.set('year', date[2]);
				return momentObject2;
			}
		}

		var _momentDiff = function() {
			return momentObject2.diff(momentObject1);
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
			timelines = h.jsonCopy(TIMELINES);
			if (timeline == 'all') {
				timelines.forEach(function(timeline) {
					_sortByName(timeline.Data);
				});
				return;
			}
			timelines.forEach(function(timelineParam) {
				timelineParam.Skip = timelineParam[NAME_CULTURE] == timeline ? false : true;
			});
		}

		var _filterAll = function() {
			_filterTimeline(timeline);
			if (region == 'all' || branch == 'all') return;
			timelines.forEach(function(timeline) {
				timeline.Data.forEach(function(initiative) {
					// save weight to data
					var weight = h.keyExists(_getControl(), initiative.Impacts, true);
					initiative.Weight = weight == false ? 0 : weight;
					// save event skip data
					initiative.Events.forEach(function(event) {
						event.Skip = !h.keyExists(_getControl(), event.Control, false);
					});
				});
				// sort by weight then name
				timeline.Data.sort(function(a, b) {
					return (h.sortComparator(b.Weight, a.Weight) ||
						h.sortComparator(a[NAME_CULTURE], b[NAME_CULTURE]));
				});
			});
		}

		var _setTimeline = function(key) { timeline = key; }
		var _setRegion = function(key) { region = key; }
		var _setBranch = function(key) { branch = key; }
		var _getControl = function() { return region + ',' + branch };
		var _getControlObject = function() { return { region: region, branch: branch } };

		return {
			loaded: _getLoadState,
			timelines: _getTimelines,
			regions: _getRegions,
			branches: _getBranches,
			timespan: _getTimespan,
			filter: _filterAll,
			setTimeline: _setTimeline,
			setRegion: _setRegion,
			setBranch: _setBranch,
			getControl: _getControl,
			getControlObject: _getControlObject,
			momentize: _momentize,
			momentDiff: _momentDiff
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
			$('#timeline-tool-nav-space').height($('#timeline-tool-nav-container').height());
		}

		var _showTool = function() {
			$('#timeline-tool-loading-icon').addClass('hide');
			$('#timeline-tool').removeClass('hide');
		}

		// navigation, controllers, events are private submodules of ui - INACCESSIBLE FROM CONSOLE
		// please keep it this way
		var navigation = function() {
			var $calendarContainer = $('#nav-calendar'),
				$calendar = $('#nav-calendar-container'),
				$quarterContainer = $('#nav-calendar-quarter'),
				$monthContainer = $('#nav-calendar-month'),
				$dynamicContentContainer = $('#dynamic-content-container'),
				$navContainer = $('#timeline-tool-nav-container'),
				$navSpace = $('#timeline-tool-nav-space');
			var timespan = dm.timespan(); 	// be careful in handling moment object
											// day & year base 1, but month base 0
			var _populateCalendar = function() {
				_setupCalendarContainer();
				_populateQuarterLine();
				_populateMonthLine();
				_attachDraggableModule();
			}

			var _attachDraggableModule = function() {
				var xContainment = $calendar.width() * -1 + $calendarContainer.width();
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
				$calendar.css('width', timespan.Duration.Months * MONTH_WIDTH);
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
					branch = h.truncate(branch, 40);
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
				$navContainer = $('#timeline-tool-nav-container'),
				$calendarContainer = $('#nav-calendar'),
				$calendar = $('#nav-calendar-container'),
				$navSpace = $('#timeline-tool-nav-space'),
				$descriptionContainer = $('#dynamic-description-container'),
				$contentContainer = $('#dynamic-content-container'),
				$startOver = $('#timeline-tool-start-over'),
				$dialog = $('#dialog'),
				$main = $('#wb-main-in'),
				$body = $('body'),
				$document = $(document),
				$window = $(window),
				splashRan = false;

			var INITIAL_NAV_OFFSET, NAV_RETURN_POSITION;
			var navFixed;

			var _handleLeftClick = function() {
				var currentPosition = parseInt($calendar.css('left'), 10);
				var amountToMove = MONTH_WIDTH * 2;
				amountToMove = currentPosition + amountToMove >= 0 ?
							   0 : currentPosition + amountToMove;
				_setDragPosition(amountToMove);
			}

			var _handleRightClick = function() {
				var maxRightPosition = $calendar.width() * -1 + $calendarContainer.width();
				var currentPosition = parseInt($calendar.css('left'), 10);
				var amountToMove = MONTH_WIDTH * 2;
				amountToMove = currentPosition - amountToMove > maxRightPosition ? 
							   (currentPosition - amountToMove) : maxRightPosition;
				_setDragPosition(amountToMove);
			}

			var _setDragPosition = function(leftPosition) {
				$calendar.css('left', leftPosition + 'px');
				$contentContainer.css('left', leftPosition + 'px');
			}

			var _handleTimeline = function() {
				dm.setTimeline(this.value);
				dm.filter();
				ui.renderer.sort(dm.timelines());
			}

			var _handleRegion = function() {
				dm.setRegion(this.value);
				dm.filter();
				ui.renderer.sort(dm.timelines());
			}

			var _handleBranch = function() {
				dm.setBranch(this.value);
				dm.filter();
				ui.renderer.sort(dm.timelines());
				if (this.value != 'all' && !splashRan) {
					splashRan = true;
					setTimeout(function() {
						ui.renderer.openDialog({ 
							title: '&nbsp;', text: h.htmlDecode(r.InitialFilterSplash) 
						}, false);
					}, 1);
				}
			}

			var _handleOutsideDialogClick = function(e) {
				if ($dialog.dialog('isOpen') && !$(e.target).is('img, a, p') && !$(e.target).closest('.ui-dialog').length) {
					$dialog.dialog('close');
				}
			}

			var _handleHideButton = function() {
				var weightToHide = $(this).attr('data-weight');
				var $domToHide = $('.description-box[data-weight=' + weightToHide + '],.bar[data-weight=' + weightToHide + ']');
				$domToHide.addClass('hide');
				_toggleShowHide($(this));
			}

			var _handleShowButton = function() {
				var weightToShow = $(this).attr('data-weight');
				var $domToShow = $('.description-box[data-weight=' + weightToShow + '],.bar[data-weight=' + weightToShow + ']');
				$domToShow.removeClass('hide');
				_toggleShowHide($(this));
			}

			var _keyMap = function(e) {
				// 33 == page up, 34 == page down, 37 == left arrow, 39 == right arrow
				switch (e.keyCode) {
					case 37: _handleLeftClick(); return;
					case 39: _handleRightClick(); return;
					default: return;
				}
			}

			var _toggleShowHide = function($obj) {
				if ($obj.hasClass('hide-button')) {
					$obj.removeClass('hide-button');
					$obj.addClass('show-button');
					$obj.text(r.Show);
				} else {
					$obj.removeClass('show-button');
					$obj.addClass('hide-button');
					$obj.text(r.Hide);
				}
			}

			var _handleScroll = function() {
				if (INITIAL_NAV_OFFSET == null) {
					INITIAL_NAV_OFFSET = $navContainer.offset().top;
					NAV_RETURN_POSITION = INITIAL_NAV_OFFSET - $main.offset().top;
					navFixed = false;
				}
				var currentScroll = $window.scrollTop();
				var maxBottomScroll = $contentContainer.offset().top + $contentContainer.height() - $navContainer.height();
				if (navFixed == false && currentScroll > INITIAL_NAV_OFFSET) {
					if (currentScroll < maxBottomScroll) {
						navFixed = true;
						$navContainer.css({
							top: 0,
							position: 'fixed'
						});
					}
				} else if (navFixed == true && (currentScroll <= INITIAL_NAV_OFFSET || currentScroll >= maxBottomScroll)) {
					navFixed = false;
					$navContainer.css({
						top: NAV_RETURN_POSITION,
						position: 'absolute'
					});
				}
			}

			var _handleStartOver = function() {
				$timeline.val('all');
				$region.val('all');
				$branch.val('all');
				$timeline.trigger('change');
				$region.trigger('change');
				$branch.trigger('change');
				splashRan = false;
			}

			var _registerEvents = (function() {
				// tooltip & dialog initialize
				$document.tooltip({ items: ':not(.ui-button)' });
				$dialog.dialog({ autoOpen: false, width: "50%", maxWidth: "768px" });

				// timeline tool controllers
				$left.on('click', _handleLeftClick);
				$right.on('click', _handleRightClick);
				$timeline.on('change', _handleTimeline);
				$region.on('change', _handleRegion);
				$branch.on('change', _handleBranch);
				
				// left and right keyboard navigation
				$document.on('keydown', _keyMap);

				// when outside of dialog is clicked, close dialog
				$body.bind('click', _handleOutsideDialogClick);

				// delegate click event to each class
				$descriptionContainer.on('click', '.description', function() { ui.renderer.openDialog($(this), true); });
				$contentContainer.on('click', '.icon', function() { ui.renderer.openDialog($(this), true); });

				// hide and show buttons in timeline tool
				$descriptionContainer.on('click', '.hide-button', _handleHideButton);
				$descriptionContainer.on('click', '.show-button', _handleShowButton);

				// fix navigation bar
				$window.on('scroll', _handleScroll)

				// start over
				$startOver.on('click', _handleStartOver)
			})();
		}

		return {
			unlock: _unlock
		}
	})(r, $, h, dataManager);

	ui.renderer = (function(dm) {
		var descriptionContainer = document.querySelector('#dynamic-description-container'),
			$descriptionContainer = $('#dynamic-description-container'),
			contentContainer = document.querySelector('#dynamic-content-container'),
			$contentContainer = $('#dynamic-content-container'),
			calendar = document.querySelector('#nav-calendar-container');
		var MONTH_WIDTH = 100, 
			ROW_WIDTH,
			ROW_IN_MILLISECONDS,
			TODAY = moment().format('MM/DD/YY'),
			TODAY_MARGIN;
		var sortableTimelines;

		var _getLeftMargin = function(date1, date2) {
			dm.momentize(date1, 1);
			dm.momentize(date2, 2);
			return Math.floor((dm.momentDiff()) / ROW_IN_MILLISECONDS * ROW_WIDTH);
		}

		var _getBarWidth = function(initiative) {
			dm.momentize(initiative.StartDate, 1);
			dm.momentize(initiative.EndDate, 2);
			return Math.floor((dm.momentDiff()) / ROW_IN_MILLISECONDS * ROW_WIDTH);
		}

		var _makeIcons = function(initiative, dom) {
			var icon,
				iconWidth = 24,
				halfIconWidth = 12,
				eventLeftMargin,
				type, hover, dialogText;
			initiative.Events.forEach(function(event, index) {
				eventLeftMargin = _getLeftMargin(initiative.StartDate, event.Date) - halfIconWidth;
				type = event.Type == 'Milestone' ? 'circle.png' : 'book.png';
				hover = event[HOVER_CULTURE];
				dialogText = event[TEXT_CULTURE];
				icon = document.createElement('img');
				icon.id = 'event-' + event.ID;
				icon.src = '/timeline/img/' + type,
				icon.className = dialogText != null ? 'icon icon-click' : 'icon',
				icon.title = hover,
				icon.style.marginLeft = eventLeftMargin + 'px',
				icon.setAttribute('data-title', hover),
				icon.setAttribute('data-description', dialogText);
				dom.appendChild(icon);
			});
			return dom;
		}

		var _drawBar = function(initiative, timelineName) {
			var bar = document.createElement('div'),
				durationBar = document.createElement('div'),
				barWidth = _getBarWidth(initiative),
				barLeftMargin = _getLeftMargin(timespan.Start, initiative.StartDate);
			bar.id = 'initiative-' + initiative.ID;
			bar.className = 'bar ' + timelineName;
			durationBar.className = 'duration';
			durationBar.style.width = barWidth + 'px';
			durationBar.style.marginLeft = barLeftMargin + 'px';
			durationBar = _makeIcons(initiative, durationBar);
			bar.appendChild(durationBar);
			return bar;
		}

		var _drawDescription = function(initiative, timelineName) {
			var initiativeName = initiative[NAME_CULTURE];
			var dialogText = initiative[DESC_CULTURE];
			var box = document.createElement('div'),
				content = document.createElement('p');
			box.id = 'initiative-description-' + initiative.ID;
			box.className = 'description-box ' + timelineName;
			content.className = 'description';
			content.setAttribute('data-title', initiativeName);
			content.setAttribute('data-description', dialogText);
			content.innerHTML = h.truncate(initiativeName, 55);
			box.appendChild(content);
			return box;
		}

		var _drawRow = function(initiative) {
			descriptionContainer.appendChild(_drawDescription(initiative, this));
			contentContainer.appendChild(_drawBar(initiative, this));
		}

		var _drawTimeline = function(timeline) {
			var name = timeline[NAME_CULTURE];
			timeline.Data.forEach(_drawRow, name);
		}

		var _emptyContinaers = function() {
			descriptionContainer.innerHTML = '';
			contentContainer.innerHTML = '';
			timespan = dm.timespan();
			if (ROW_WIDTH == undefined || ROW_IN_MILLISECONDS == undefined) {
				ROW_WIDTH = timespan.Duration.Months * MONTH_WIDTH;
				ROW_IN_MILLISECONDS = timespan.End.diff(timespan.Start);
				timespan.Start = timespan.Start.format('MM/DD/YY');
				timespan.End = timespan.End.format('MM/DD/YY');
			}
		}

		var _createImpactHeadingElement = function(weight, impactClass) {
			var container, statement, button, rowSpace;
			container = document.createElement('div');
			container.className = 'hide impact-statement ' + impactClass;
			container.setAttribute('data-weight', weight);
			statement = document.createElement('p');
			statement.innerHTML = _getImpactStatment(weight);
			button = document.createElement('a');
			button.innerHTML = r.Hide;
			button.className = 'hide-button';
			button.setAttribute('data-weight', weight)
			container.appendChild(statement);
			container.appendChild(button);
			rowSpace = document.createElement('div'); 
			rowSpace.className = 'hide impact-statement-space ' + impactClass; 
			rowSpace.setAttribute('data-weight', weight);
			return [container, rowSpace];
		}

		var _addImpactHeadingElements = function() {
			var weight, impactClass, element, weightOrder;
			weightOrder = [3, 2, 1, 0, 4];
			for (var i = 0, cond = weightOrder.length; i < cond; i++) {
				weight = weightOrder[i], impactClass = _impactCSSClass(weightOrder[i], false);
				element = _createImpactHeadingElement(weight, impactClass);
				descriptionContainer.appendChild(element[0]);
				contentContainer.appendChild(element[1]);
			}
		}

		var _showToday = function() {
			var viewableWidth = $contentContainer.parent().width();
			if (TODAY_MARGIN == undefined) {
				TODAY_MARGIN = _getLeftMargin(timespan.Start, TODAY) * -1 + Math.floor(viewableWidth / 2);
			}
			requestAnimationFrame(function() {
				_addTodayLine();
				calendar.style.left = TODAY_MARGIN + 'px';
				contentContainer.style.left = TODAY_MARGIN + 'px';
			});
		}

		var _addTodayLine = function() {
			var img = "<img src='/timeline/img/red.gif' id='today' style='margin-left:" + _getLeftMargin(timespan.Start, TODAY) + "px' />";
			var $children = $contentContainer.children('.impact-statement-space');
			var impactStatementSpaceExists = $children.length > 0;
			if (impactStatementSpaceExists) {
				$(img).insertAfter($children.first());
			} else {
				$contentContainer.prepend(img);
			}
		}

		var _draw = function(timelines) {
			if (timelines == null) return 'no argument was passed in';
			_emptyContinaers();
			_addImpactHeadingElements();
			timelines.forEach(_drawTimeline);
			_showToday();
		}

		var _removeImpactClass = function(index, css) {
			return (css.match(/\S+color-content($|\s)/g) || []).join(' ');
		}

		var _resetSort = function() {
			$descriptionContainer.find('.description-box').attr({
				'style': 'order: 0',
				'data-order': '0'
			}).removeClass(_removeImpactClass);
			$contentContainer.find('.bar').attr({
				'style': 'order: 0',
				'data-order': '0'
			}).removeClass(_removeImpactClass);
			$contentContainer.find('.icon').removeClass('hide');

			$showButtons = $('.show-button');
			$showButtons.each(function() {
				$(this).removeClass('show-button').addClass('hide-button').text(r.Hide);
			});
		}

		var _filterTimelines = function() {
			sortableTimelines.forEach(function(timeline) {
				var timelineSelector = '.' + timeline[NAME_CULTURE];
				if (timeline.Skip == true) $(timelineSelector).addClass('hide');
				else $(timelineSelector).removeClass('hide');
			})
		}

		var _sortInitiatives = function() {
			sortableTimelines.forEach(function(timeline) {
				timeline.Data.forEach(function(initiative) {
					// check if Weight is defined; if undefined, region & branch not selected
					if (initiative.Weight == undefined) return false;
					var order = initiative.Weight == undefined ? 0 : _getFlexOrder(initiative.Weight);
					var impactClass = _impactCSSClass(initiative.Weight, true);

					// re-order flex layout by setting order
					var idSuffix = initiative.ID;
					var descriptionSelector = '#initiative-description-' + idSuffix;
					var barSelector = '#initiative-' + idSuffix;
					var weight = initiative.Weight.toString();
					$(descriptionSelector).attr({
						'style': ('order: ' + order),
						'data-order': order,
						'data-weight': weight
					}).addClass(impactClass);
					$(barSelector).attr({
						'style': ('order: ' + order),
						'data-order': order,
						'data-weight': weight
					}).addClass(impactClass);

					// hide icons if needs be
					initiative.Events.forEach(function(event) {
						if (event.Skip == undefined) return false;
						var eventSelector = '#event-' + event.ID;
						if (event.Skip == true) $(eventSelector).addClass('hide');
						else $(eventSelector).removeClass('hide');
					});
				});
			});
		}

		var _categorize = function() {
			var $box, $statement, $space, weight, order, weightOrder = [3, 2, 1, 0, 4];
			for (var i = 0, cond = weightOrder.length; i < cond; i++) {
				$box = $(".description-box[data-weight='" + weightOrder[i] + "']:not(.hide)").first();
				$statement = $(".impact-statement[data-weight='" + weightOrder[i] + "']").first();
				$space = $(".impact-statement-space[data-weight='" + weightOrder[i] + "']").first();
				order =  $box.attr('data-order');
				if ($box.length > 0 && order != undefined && order > 0) {
					$statement.attr({
						'style': ('order: ' + order)
					}).removeClass('hide');
					$space.attr({
						'style': ('order: ' + order)
					}).removeClass('hide');
				} else {
					$statement.addClass('hide');
					$space.addClass('hide');
				}
			}
		}

		var _getImpactStatment = function(level) {
			switch (level) {
				case 0: return r.NoImpact; case 1: return r.LowImpact; case 2: return r.MediumImpact;
				case 3: return r.HighImpact; case 4: return r.BlueprintImpact; default: return r.NoImpact;
			}
		}

		var _impactCSSClass = function(level, lighter) {
			switch (level) {
				case 0: return lighter == true ? 'no-impact-color-content' : 'no-impact-color';
				case 1: return lighter == true ? 'low-impact-color-content' : 'low-impact-color';
				case 2: return lighter == true ? 'medium-impact-color-content' : 'medium-impact-color';
				case 3: return lighter == true ? 'high-impact-color-content' : 'high-impact-color';
				case 4: return lighter == true ? 'blueprint-impact-color-content' : 'blueprint-impact-color';
				default: return lighter == true ? 'no-impact-color-content' : 'no-impact-color';
			}
		}

		var _getFlexOrder = function(weight) {
			switch (weight) {
				// just a helper function to switch flex orders around
				case 0: return 4; case 1: return 3; case 2: return 2;
				case 3: return 1; case 4: return 5; default: return 4;
			}
		}

		var _sort = function(timelines) {
			sortableTimelines = timelines;
			_resetSort();
			_filterTimelines();
			_sortInitiatives();
			_categorize();
		}

		var _dialog = function($obj, dom) {
			var title = '';
			var text = '';
			if (!dom) {
				title = $obj.title;
				text = $obj.text;
			} else {
				title = $obj.attr('data-title');
				text = $obj.attr('data-description');
			}
			if (text != '' && text != 'null') {
				$("#dialog").html(text);
				$("#ui-id-1").html(title);
				$("#dialog").dialog('open');
			}
		}

		return {
			draw: _draw,
			sort: _sort,
			openDialog: _dialog
		}
	})(dataManager);

	return {
		dataManager: dataManager,
		ui: ui
	}
})(resources, jQuery, moment, helpers);

require(['jquery-private', 'moment', 'helper', 'data-manager', 'jquery-ui'],
function($, moment, helper, dataManager) {
	'use strict';
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
	
	// constants
	var API_DATE_FORMAT = 'MM/DD/YYYY';
	var CURRENT_URL = window.location.href,
		CULTURE = CURRENT_URL.indexOf('lang=fra') > -1 ? 'fr-ca' : 'en-ca',
		CULTURE_APPEND = CULTURE == 'en-ca' ? 'E' : 'F',
		DESC_CULTURE = 'Description' + CULTURE_APPEND,
		HOVER_CULTURE = 'Hover' + CULTURE_APPEND,
		NAME_CULTURE = 'Name' + CULTURE_APPEND,
		TEXT_CULTURE = 'Text' + CULTURE_APPEND;
	var MONTH_WIDTH = 100;
	
	// debug variables
    var DEBUG = CURRENT_URL.indexOf('on-dev') > -1;
    DEBUG = DEBUG ? DEBUG : CURRENT_URL.indexOf(':1803') > -1; 
    var CONSOLE_PREFIX = 'TimelineTool: ';
	
	// other namespaces & config setup
	var r = resources;
	moment.locale(CULTURE);
	var h = helper;
	var dm = dataManager;
		
	var ui = (function() {
		// controllers, navigation, events are private submodules
	    var controllers = function () {
	        var $left = $('#nav-go-left-button'),
				$right = $('#nav-go-right-button'),
				$timeline = $('#nav-controller-timeline'),
				$region = $('#nav-controller-region'),
				$branch = $('#nav-controller-branch');

	        var _addOption = function (jQueryObject, optionObject) {
	            var $select = jQueryObject;
		    var option = document.createElement('option');
	            option.text = optionObject.text;
	            option.value = optionObject.value;
                    option.dataset.regions = optionObject.relatedRegions;
	            $select.append(option);
	        };

	        var _populateTimeline = function () {
	            dm.timelines().forEach(function (element, index, array) {
	                var timeline = element['Name' + CULTURE_APPEND];
	                var translation = r[timeline];
	                _addOption($timeline, { text: translation, value: timeline });
	            });
	        };

	        var _populateRegionsAndBranches = function () {
	            dm.regions().forEach(function (element, index, array) {
	                var region = element['Name' + CULTURE_APPEND];
	                _addOption($region, { text: region, value: element.ID });
	            });
	            dm.branches().forEach(function (element, index, array) {
	                var branch = element['Name' + CULTURE_APPEND];
	                branch = h.truncate(branch, 40);
	                _addOption($branch, { text: branch, value: element.ID, relatedRegions: element.relatedRegions });
	            });
	        };

	        var _initialize = (function () {
	            $branch.attr('disabled', 'true');
	            _populateTimeline();
	            _populateRegionsAndBranches();
	        })();
	    };
		
	    var navigation = function () {
	        // hoist all jQuery objects and cache them inside the submodule
	        // this results in faster interactions with document object
	        var $calendar = $('#nav-calendar-container'),
				$calendarContainer = $('#nav-calendar'),
				$dynamicContentContainer = $('#dynamic-content-container'),
				$monthContainer = $('#nav-calendar-month'),
				$navContainer = $('#timeline-tool-nav-container'),
				$navSpace = $('#timeline-tool-nav-space'),
				$quarterContainer = $('#nav-calendar-quarter');

	        var timespan = dm.timespan(); // day&year[base 1], month[base 0]

	        var _attachDraggableModule = function () {
	            var xContainment = helper.negate($calendar.width()) +
                    $calendarContainer.width();
	            var marginOffset = 42;
	            var dragWith = '#nav-calendar-container';
	            $dynamicContentContainer.draggable({
	                axis: 'x',
	                alsoDrag: dragWith,
	                stop: function (event, ui) {
	                    if (ui.position.left > 0) {
	                        $(this).animate({ 'left': '0px' }, 300);
	                        $(dragWith).animate({ 'left': '0px' }, 300);
	                    } else if (ui.position.left < xContainment) {
	                        $(this).animate({ 'left': xContainment }, 300);
	                        $(dragWith).animate({ 'left': xContainment }, 300);
	                    }
	                }
	            });
	        };

	        var _generateClearBox = function () {
	            return "<div class='clear'></div>";
	        };

	        var _generateMonthBox = function (momentObject) {
	            var content = _generateMonthString(momentObject);
	            var classToUse = _getCSSClass(momentObject.month());
	            var html = "<div class='" + classToUse + "' style='width:" + MONTH_WIDTH + "px;'>";
	            html += content;
	            html += "</div>";
	            return html;
	        };

	        var _generateMonthString = function (momentObject) {
	            var month = momentObject.month() + 1;
	            var monthString = momentObject.format('MMM');
	            var year = momentObject.year();
	            switch (month) {
	                case 4: case 7: case 10: case 1: monthString = '| ' + monthString + ' ' + year; break;
	                default: monthString = '| ' + monthString;
	            }
	            return monthString;
	        };

	        var _generateQuarterBox = function (month) {
	            var content = _generateQuarterString(month);
	            var classToUse = _getCSSClass(month);
	            var html = "<div class='" + classToUse + "' style='width:" + MONTH_WIDTH + "px;'>";
	            html += content;
	            html += "</div>";
	            return html;
	        };

	        var _generateQuarterString = function (month) {
	            month++;
	            var quarterString = '| ' + r.ReportQuarterPrefix;
	            switch (month) {
	                case 4: quarterString += '1'; break; case 7: quarterString += '2'; break;
	                case 10: quarterString += '3'; break; case 1: quarterString += '4'; break;
	                default: quarterString = '&nbsp;';
	            }
	            return quarterString;
	        };

	        var _getCSSClass = function (month) {
	            month++;
	            switch (month) {
	                case 1: case 2: case 3: return 'fourth-quarter';
	                case 4: case 5: case 6: return 'first-quarter';
	                case 7: case 8: case 9: return 'second-quarter';
	                case 10: case 11: case 12: return 'third-quarter';
	                default: return 'first-quarter';
	            }
	        };

	        var _setupCalendarContainer = function () {
	            $calendar.css('width', timespan.Duration.Months * MONTH_WIDTH);
	            $dynamicContentContainer.css(
					'width', timespan.Duration.Months * MONTH_WIDTH
				);
	        };

	        var _populateMonthLine = function () {
	            var current = timespan.Start.clone();
	            var months = timespan.Duration.Months;
	            for (var i = 0; i < months; i++, current.add(1, 'month')) {
	                $monthContainer.append(_generateMonthBox(current));
	            }
	            $monthContainer.append(_generateClearBox());
	        };

	        var _populateQuarterLine = function () {
	            var current = timespan.Start.clone();
	            var months = timespan.Duration.Months;
	            for (var i = 0; i < months; i++, current.add(1, 'month')) {
	                $quarterContainer.append(
						_generateQuarterBox(current.month())
					);
	            }
	            $quarterContainer.append(_generateClearBox());
	        };

	        var _populateCalendar = (function () {
	            _setupCalendarContainer();
	            _populateMonthLine();
	            _populateQuarterLine();
	            _attachDraggableModule();
	        })();
	    };

	    var eventManager = function () {
	        // every other jQuery object other than controller objects
	        var $body = $('body'),
				$document = $(document),
				$calendar = $('#nav-calendar-container'),
				$calendarContainer = $('#nav-calendar'),
				$contentContainer = $('#dynamic-content-container'),
				$descriptionContainer = $('#dynamic-description-container'),
				$dialog = $('#dialog'),
				$main = $('#wb-main-in'),
				$navContainer = $('#timeline-tool-nav-container'),
				$navSpace = $('#timeline-tool-nav-space'),
				$startOver = $('#timeline-tool-start-over'),
				$window = $(window);

	        // controller objects
	        var $left = $('#nav-go-left-button'),
				$right = $('#nav-go-right-button'),
				$timeline = $('#nav-controller-timeline'),
				$region = $('#nav-controller-region'),
				$branch = $('#nav-controller-branch');

	        // for first run hover instruction display
	        var splashRan = false;

	        // constants & a variable for fixed nav header
	        var INITIAL_NAV_OFFSET, NAV_RETURN_POSITION;
	        var navFixed;

	        // event callback handlers
	        var _handleBranch = function () {
	            dm.setBranch(this.value);
	            dm.filter();
	            ui.content.sort(dm.timelines());
	            if (this.value != 'all' && !splashRan) {
	                splashRan = true;
	                ui.content.openDialog({
	                    title: '&nbsp;',
	                    text: h.htmlDecode(r.InitialFilterSplash)
	                }, false);
	            }
	        };

	        var _handleHideButton = function () {
	            var weightToHide = $(this).attr('data-weight');
	            var $domToHide = $('.description-box[data-weight=' +
					weightToHide + '],.bar[data-weight=' + weightToHide + ']');
	            $domToHide.addClass('hide');
	            _toggleShowHide($(this));
	        };

	        var _handleOutsideDialogClick = function (e) {
	            if ($dialog.dialog('isOpen') && !$(e.target).is('img, a, p') &&
					!$(e.target).closest('.ui-dialog').length) {
	                $dialog.dialog('close');
	            }
	        };

	        var _handleRegion = function () {
                    var selected = this.value;
                    _enableAllBranches();
	            if (selected.indexOf('all') === -1) {
	                $branch.removeAttr('disabled');
	            } else {
	                $branch.attr('disabled', 'disabled');
	            }
                    _disableUnrelatedBranches(selected);
	            dm.setRegion(selected);
	            dm.filter();
	            ui.content.sort(dm.timelines());
	        };

                var _enableAllBranches = function() {
                    var $options = $branch.children().not("[value='all']");
                    $options.each(function (index) {
                        this.removeAttribute('disabled');
                    });
                };

                var _disableUnrelatedBranches = function(selectedRegion) {
                    var $options = $branch.children().not("[value='all']");
                    $options.each(function (index) {
                        var regions = this.dataset.regions.split(',');
                        if (regions.indexOf(selectedRegion) === -1) {
                            this.disabled = true;
                        }
                    });
                };

	        var _handleScroll = function () {
	            if (typeof INITIAL_NAV_OFFSET === 'undefined') {
	                INITIAL_NAV_OFFSET = $navContainer.offset().top;
	                NAV_RETURN_POSITION = INITIAL_NAV_OFFSET -
						$main.offset().top;
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
	            } else if (navFixed == true &&
					(currentScroll <= INITIAL_NAV_OFFSET || currentScroll >= maxBottomScroll)) {
	                navFixed = false;
	                $navContainer.css({
	                    top: NAV_RETURN_POSITION,
	                    position: 'absolute'
	                });
	            }
	        };

	        var _handleShowButton = function () {
	            var weightToShow = $(this).attr('data-weight');
	            var $domToShow = $('.description-box[data-weight=' +
					weightToShow + '],.bar[data-weight=' + weightToShow + ']');
	            $domToShow.removeClass('hide');
	            _toggleShowHide($(this));
	        };

	        var _handleStartOver = function () {
	            $timeline.val('all');
	            $region.val('all');
	            $branch.val('all');
	            $timeline.trigger('change');
	            $region.trigger('change');
	            $branch.trigger('change');
	            splashRan = false;
				$('html, body').animate({
					scrollTop: NAV_RETURN_POSITION
				}, 'slow');
				ui.content.showToday();
	        };

	        var _handleTimeline = function () {
	            dm.setTimeline(this.value);
	            dm.filter();
	            ui.content.sort(dm.timelines());
	        };

	        var _keyMap = function (e) {
	            // 33 == page up, 34 == page down, 37 == left arrow, 39 == right arrow
	            switch (e.keyCode) {
	                case 37: _moveTimelineLeft(); return;
	                case 39: _moveTimelineRight(); return;
	                default: return;
	            }
	        };

	        var _moveTimelineLeft = function () {
	            var currentPosition = parseInt($calendar.css('left'), 10);
	            var amountToMove = MONTH_WIDTH * 2;
	            amountToMove = currentPosition + amountToMove >= 0 ?
							   0 : currentPosition + amountToMove;
	            _setDragPosition(amountToMove);
	        };

	        var _moveTimelineRight = function () {
	            var maxRightPosition = $calendar.width() * -1 + $calendarContainer.width();
	            var currentPosition = parseInt($calendar.css('left'), 10);
	            var amountToMove = MONTH_WIDTH * 2;
	            amountToMove = currentPosition - amountToMove > maxRightPosition ?
							   (currentPosition - amountToMove) : maxRightPosition;
	            _setDragPosition(amountToMove);
	        };

	        var _setDragPosition = function (leftPosition) {
	            $calendar.css('left', leftPosition + 'px');
	            $contentContainer.css('left', leftPosition + 'px');
	        };

	        var _toggleShowHide = function ($obj) {
	            if ($obj.hasClass('hide-button')) {
	                $obj.removeClass('hide-button');
	                $obj.addClass('show-button');
	                $obj.text(r.Show);
	            } else {
	                $obj.removeClass('show-button');
	                $obj.addClass('hide-button');
	                $obj.text(r.Hide);
	            }
	        };

	        var _registerEvents = (function () {
	            // tooltip & dialog initialize
	            $document.tooltip({ items: ':not(.ui-button)' });
	            $dialog.dialog({
	                autoOpen: false, width: "50%", maxWidth: "768px"
	            });

	            // timeline tool controllers
	            $left.on('click', _moveTimelineLeft);
	            $right.on('click', _moveTimelineRight);
	            $timeline.on('change', _handleTimeline);
	            $region.on('change', _handleRegion);
	            $branch.on('change', _handleBranch);

	            // left and right keyboard navigation
	            $document.on('keydown', _keyMap);

	            // when outside of dialog is clicked, close dialog
	            $body.bind('click', _handleOutsideDialogClick);

	            // delegate click event to each class
	            $descriptionContainer.on('click', '.description', function () {
	                ui.content.openDialog($(this), true);
	            });
	            $contentContainer.on('click', '.icon-click', function () {
	                ui.content.openDialog($(this), true);
	            });

	            // hide and show buttons in timeline tool
	            $descriptionContainer.on(
					'click', '.hide-button', _handleHideButton
				);
	            $descriptionContainer.on(
					'click', '.show-button', _handleShowButton
				);

	            // fix navigation bar
	            $window.on('scroll', _handleScroll);

	            // start over
	            $startOver.on('click', _handleStartOver);
	        })();
	    };

		var _generate = function() {
			controllers();
			navigation();
			eventManager();
			ui.content.draw(dm.timelines());
			_showTool();
			$('#timeline-tool-nav-space').height(
				$('#timeline-tool-nav-container').height()
			);
		};

		var _showTool = function () {
		    $('#timeline-tool-loading-icon').addClass('hide');
		    $('#timeline-tool').removeClass('hide');
		};
		
		return {
		    generate: _generate
		};
	})();

	ui.content = (function() {
		// constants
		var ROW_IN_MILLISECONDS, ROW_WIDTH, TODAY_LINE_ADDED;
		var TODAY = moment().format('MM/DD/YY'), TODAY_MARGIN;
		
		// selector variables
		var s = document.querySelector.bind(document);
		var calendar = s('#nav-calendar-container'),
			descriptionContainer = s('#dynamic-description-container'),
			contentContainer = s('#dynamic-content-container');
		var $descriptionContainer = $('#dynamic-description-container'),
			$contentContainer = $('#dynamic-content-container');
		
		// private variables used for content generation
		var sortableTimelines, timespan;

		// content drawing functions - alphabetically ordered
		var _addImpactHeadingElements = function () {
		    var weight, impactClass, element, weightOrder;
		    weightOrder = [3, 2, 1, 0, 4];
		    for (var i = 0, cond = weightOrder.length; i < cond; i++) {
		        weight = weightOrder[i];
		        impactClass = _getImpactClass(weightOrder[i], false);
		        element = _createImpactHeadingElement(weight, impactClass);
		        descriptionContainer.appendChild(element[0]);
		        contentContainer.appendChild(element[1]);
		    }
		};

		var _addTodayLine = function() {
			if (TODAY_LINE_ADDED) return;
			var img = "<img src='/timeline/img/red.gif' id='today' style='margin-left:" + _getLeftMargin(timespan.Start, TODAY) + "px' />";
			var $children = $contentContainer.children('.impact-statement-space');
			var impactStatementSpaceExists = $children.length > 0;
			if (impactStatementSpaceExists) {
				$(img).insertAfter($children.first());
			} else {
				$contentContainer.prepend(img);
			}
			TODAY_LINE_ADDED = true;
		};

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
			button.setAttribute('data-weight', weight);
			container.appendChild(statement);
			container.appendChild(button);
			rowSpace = document.createElement('div'); 
			rowSpace.className = 'hide impact-statement-space ' + impactClass; 
			rowSpace.setAttribute('data-weight', weight);
			return [container, rowSpace];
		};

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
		};

		var _drawDescription = function (initiative, timelineName) {
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
		};

		var _drawRow = function(initiative) {
			descriptionContainer.appendChild(_drawDescription(initiative, this));
			contentContainer.appendChild(_drawBar(initiative, this));
		};

		var _drawTimeline = function(timeline) {
			var name = timeline[NAME_CULTURE];
			timeline.Data.forEach(_drawRow, name);
		};
		
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
		};
		
		var _getBarWidth = function(initiative) {
			dm.momentize(initiative.StartDate, 1);
			dm.momentize(initiative.EndDate, 2);
			return Math.floor((dm.momentDiff()) / ROW_IN_MILLISECONDS * ROW_WIDTH);
		};
		
		var _getImpactStatment = function(level) {
			switch (level) {
				case 0: return r.NoImpact; 		case 1: return r.LowImpact;
				case 2: return r.MediumImpact; 	case 3: return r.HighImpact;
				case 4: return r.BlueprintImpact;
				default: return r.NoImpact;
			}
		};
		
		var _getLeftMargin = function(date1, date2) {
			dm.momentize(date1, 1);
			dm.momentize(date2, 2);
			return Math.floor((dm.momentDiff()) / ROW_IN_MILLISECONDS * ROW_WIDTH);
		};

		var _makeIcons = function(initiative, dom) {
			var icon,
				iconWidth = 24,
				halfIconWidth = 12,
				eventLeftMargin,
				type, hover, dialogText;
			initiative.Events.forEach(function(event, index) {
				eventLeftMargin = _getLeftMargin(initiative.StartDate, event.Date) - halfIconWidth;
				type = event.Type == 'Milestone' ? 'fa-dot-circle-o' : 'fa-book';
				hover = event[HOVER_CULTURE];
				dialogText = event[TEXT_CULTURE];
				icon = document.createElement('i');
				icon.id = 'event-' + event.ID;
				icon.className = 'icon fa fa-2x ' + type + ' ' + event.Type.toLowerCase();
				icon.className += dialogText != null ? ' icon-click' : '';
				icon.title = hover;
				icon.style.marginLeft = eventLeftMargin + 'px';
				icon.setAttribute('data-title', hover);
				icon.setAttribute('data-description', dialogText);
				dom.appendChild(icon);
			});
			return dom;
		};
		
		var _showToday = function () {
		    var viewableWidth = $contentContainer.parent().width();
		    if (TODAY_MARGIN == undefined) {
		        TODAY_MARGIN = _getLeftMargin(timespan.Start, TODAY) * -1 +
                    Math.floor(viewableWidth / 2);
		    }
		    requestAnimationFrame(function () {
		        _addTodayLine();
		        calendar.style.left = TODAY_MARGIN + 'px';
		        contentContainer.style.left = TODAY_MARGIN + 'px';
		    });
		};
		
		// content sorting functions - alphabetically ordered
		var _categorize = function () {
		    var $box, $statement, $space, weight, order, weightOrder = [3, 2, 1, 0, 4];
		    for (var i = 0, cond = weightOrder.length; i < cond; i++) {
		        $box = $(".description-box[data-weight='" + weightOrder[i] + "']:not(.hide)").first();
		        $statement = $(".impact-statement[data-weight='" + weightOrder[i] + "']").first();
		        $space = $(".impact-statement-space[data-weight='" + weightOrder[i] + "']").first();
		        order = $box.attr('data-order');
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
		};

		var _filterTimelines = function() {
		    sortableTimelines.forEach(function (timeline) {
		        var timelineSelector = '.' + timeline[NAME_CULTURE];
		        if (timeline.Skip == true) $(timelineSelector).addClass('hide');
		        else $(timelineSelector).removeClass('hide');
		    });
		};
		
		var _getFlexOrder = function (weight) {
		    switch (weight) {
		        // just a helper function to switch flex orders around
		        case 0: return 4; case 1: return 3; case 2: return 2;
		        case 3: return 1; case 4: return 5; default: return 4;
		    }
		};

		var _removeImpactClass = function (index, css) {
		    return (css.match(/\S+color-content($|\s)/g) || []).join(' ');
		};

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

			var $showButtons = $('.show-button');
			$showButtons.each(function() {
				$(this).removeClass('show-button')
				.addClass('hide-button').text(r.Hide);
			});
		};
		
		var _sortInitiatives = function() {
			sortableTimelines.forEach(function(timeline) {
				timeline.Data.forEach(function(initiative) {
					// check if Weight is in initiative object
					// if DNE, region&branch are not selected
                                        if (!("Weight" in initiative)) return false;
					if (initiative.Weight === undefined) {
                                            initiative.Weight = 0;
                                        }
					var order = _getFlexOrder(initiative.Weight);
					var impactClass = _getImpactClass(initiative.Weight, true);

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
		};
		
		// other helper methods
		var _getImpactClass = function(level, lighter) {
			switch (level) {
				case 0: return lighter == true ?
				'no-impact-color-content' : 'no-impact-color';
				case 1: return lighter == true ?
				'low-impact-color-content' : 'low-impact-color';
				case 2: return lighter == true ?
				'medium-impact-color-content' : 'medium-impact-color';
				case 3: return lighter == true ?
				'high-impact-color-content' : 'high-impact-color';
				case 4: return lighter == true ?
				'blueprint-impact-color-content' : 'blueprint-impact-color';
				default: return lighter == true ?
				'no-impact-color-content' : 'no-impact-color';
			}
		};
		
		// public functions that other submodules can use
		var _draw = function(timelines) {
			if (timelines == null) return 'no argument was passed in';
			_emptyContinaers();
			_addImpactHeadingElements();
			timelines.forEach(_drawTimeline);
			_showToday();
		};

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
				setTimeout(function() {
					$("#dialog").dialog('open');
				}, 1);
			}
		};

		var _sort = function(timelines) {
			sortableTimelines = timelines;
			_resetSort();
			_filterTimelines();
			_sortInitiatives();
			_categorize();
		};

		return {
		    draw: _draw,
		    openDialog: _dialog,
		    sort: _sort,
			showToday: _showToday
		};
	})();
	
	// GET data from the server and generate UI
	dm.config({CULTURE: CULTURE});
	dm.load(ui.generate);
	h.log(CONSOLE_PREFIX + 'initialized successfully', DEBUG);
    h.log(CONSOLE_PREFIX + 'jquery v-' + $.fn.jquery, DEBUG);
    h.log(CONSOLE_PREFIX + 'moment v-' + moment.version, DEBUG);
    h.log(CONSOLE_PREFIX + 'helper v-' + h.version, DEBUG);
    h.log(CONSOLE_PREFIX + 'data-manager v-' + dm.version, DEBUG);
});

require(['jquery-private', 'moment', 'helper', 'data-manager'],
function($, moment, helper, dataManager) {
    'use strict';
	// constants
	var API_DATE_FORMAT = 'MM/DD/YYYY';
	var CURRENT_URL = window.location.href,
		CULTURE = CURRENT_URL.indexOf('lang=fra') > -1 ? 'fr-ca' : 'en-ca',
		CULTURE_APPEND = CULTURE == 'en-ca' ? 'E' : 'F',
		DESC_CULTURE = 'Description' + CULTURE_APPEND,
		HOVER_CULTURE = 'Hover' + CULTURE_APPEND,
		NAME_CULTURE = 'Name' + CULTURE_APPEND,
		TEXT_CULTURE = 'Text' + CULTURE_APPEND;
        
    // debug variables
    var DEBUG = CURRENT_URL.indexOf('on-dev') > -1;
    DEBUG = DEBUG ? DEBUG : CURRENT_URL.indexOf(':1803') > -1; 
    var CONSOLE_PREFIX = 'Accessible: ';
    	
	// other namespaces & config setup
	var r = resources;
	moment.locale(CULTURE);
	var h = helper;
	var dm = dataManager;
    
    var ui = (function() {
        // controllers, events are private submodules
        var controllers = function() {
            var $timeline = $('#nav-controller-timeline'),
                $region = $('#nav-controller-region'),
                $branch = $('#nav-controller-branch');

            var _addOption = function(jQueryObject, optionObject) {
                var $select = jQueryObject,
                    option = document.createElement('option');
                option.text = optionObject.text
                option.value = optionObject.value;
                $select.append(option);
            };

            var _populateTimeline = function() {
                dm.timelines().forEach(function(element, index, array) {
                    var timeline = element[NAME_CULTURE];
                    var translation = r[timeline];
                    _addOption($timeline, { text: translation, value: timeline });
                });
            };

            var _populateRegionsAndBranches = function() {
                dm.regions().forEach(function(element, index, array) {
                    var region = element[NAME_CULTURE];
                    _addOption($region, { text: region, value: element.ID });
                });
                dm.branches().forEach(function(element, index, array) {
                    var branch = element[NAME_CULTURE];
                    branch = branch;
                    _addOption($branch, { text: branch, value: element.ID });
                });
            };

            var _initialize = (function() {
                $branch.attr('disabled', 'true');
                _populateTimeline();
                _populateRegionsAndBranches();
            })();
        };
        
        var eventManager = function() {
            // every other jQuery object other than controller objects
            var $accordions = $('#timeline-accordions'),
                $body = $('body'),
                $document = $(document),
                $window = $(window);
            
            // controller objects
            var $timeline = $('#nav-controller-timeline'),
                $region = $('#nav-controller-region'),
                $branch = $('#nav-controller-branch');

            // event callback handlers
            var _handleBranch = function() {
                dm.setBranch(this.value);
                dm.filter();
                ui.content.sort(dm.timelines());
            };

            var _handleRegion = function() {
                if (this.value.indexOf('all') === -1) {
                    $branch.removeAttr('disabled');
                } else {
                    $branch.attr('disabled', 'disabled');
                }
                dm.setRegion(this.value);
                dm.filter();
                ui.content.sort(dm.timelines());
            };

            var _handleTimeline = function() {
                dm.setTimeline(this.value);
                dm.filter();
                ui.content.sort(dm.timelines());
            };
            
            var _handleAccordionClick = function(elem) {
                elem.toggleClass('toggle');
                var contentID = elem.attr("href");
                $(contentID).slideToggle();
            };

            var _registerEvents = (function() {
                // timeline tool controllers
                $timeline.on('change', _handleTimeline);
                $region.on('change', _handleRegion);
                $branch.on('change', _handleBranch);
                
                // delegate click event to each class
                $accordions.on("click", "a.accordion-section-title", function (e) {
                    e.preventDefault();
                    _handleAccordionClick($(this));
                });
            })();
        };
        
        var _generate = function() {
            controllers();
            eventManager();
            ui.content.draw(dm.timelines());
        };
        
        return {
			generate: _generate
		}
    })();
    
    ui.content = (function() {		
		// selector variables
		var s = document.querySelector.bind(document);
        var accordions = s('#timeline-accordions');
		
        // jQuery selector variables
        var $accordions = $('#timeline-accordions');
        
		// private variables used for content generation
		var _accordionCount = 0, sortableTimelines, timespan;

		// content drawing functions - alphabetically ordered
        var _addTimelineHeader = function(title) {
            var header = document.createElement('h2');
            header.className = title;
            header.innerHTML = r[title];
            accordions.appendChild(header);
        };
        
        var _addImpactHeaders = function(container) {
			var weight, weights, element;
			weights = [3, 2, 1, 0];
            if (container.className.indexOf('BP2020') > -1) {
                weights = [4];
            }
            weights.forEach(function(w, index) {
                element = _createImpactHeader(w);
                container.appendChild(element);
            });
            _hideElement('.impact-statement');
		};

		var _createImpactHeader = function(weight) {
			var container, statement, order;
            order = _getFlexOrder(weight);
			container = document.createElement('div');
			container.className = 'impact-statement';
			container.setAttribute('data-weight', weight);
			container.setAttribute('data-order', order);
            container.style.order = order;
			statement = document.createElement('p');
			statement.innerHTML = _getImpactStatment(weight);
			container.appendChild(statement);
			return container;
		};

		var _drawRow = function(initiative) {
			this.appendChild(_generateAccordion(initiative, this.className));
		};

		var _drawTimeline = function(timeline) {
			var name = timeline[NAME_CULTURE];
            _addTimelineHeader(name);
            var timelineContainer = document.createElement('div');
            timelineContainer.className = 'text-timeline ' + name;
            accordions.appendChild(timelineContainer);
            _addImpactHeaders(timelineContainer);
			timeline.Data.forEach(_drawRow, timelineContainer);
		};
		
		var _emptyContinaers = function() {
            accordions.innerHTML = '';
            timespan = dm.timespan();
            timespan.Start = timespan.Start.format('MM/DD/YY');
            timespan.End = timespan.End.format('MM/DD/YY');
		};
        
        var _generateAccordion = function (initiative, timeline) {
            _accordionCount++;
            var initiativeName = initiative[NAME_CULTURE];
            var accordion = document.createElement('div');
            accordion.id = 'initiative-accordion-' + initiative.ID;
            accordion.className = 'accordion-section ' + timeline;
            
            var accordionLink = document.createElement('a');
            accordionLink.className = 'accordion-section-title';
            accordionLink.href = '#accordion-' + _accordionCount;
            accordionLink.innerHTML = initiativeName;
            var accordionStatus = document.createElement('span');
            accordionLink.insertBefore(accordionStatus, accordionLink.firstChild);
            
            var accordionContent = document.createElement('div');
            accordionContent.id = 'accordion-' + _accordionCount;
            accordionContent.className = 'accordion-section-content closed';
            accordionContent.innerHTML = _generateContent(initiative);
            accordion.appendChild(accordionLink);
            accordion.appendChild(accordionContent);
            return accordion;
        };
        
        var _generateContent = function (initiative) {
            var content = [];
            var heading = [r.Description, r.Timespan, r.Milestones, r.Trainings];
            var headingClass = [
                'header-description', 'header-timespan',
                'header-milestones', 'header-trainings'
            ];

            // Create description
            var description = "<p>" + initiative[DESC_CULTURE] + "</p>";

            // Create timespan
            var startDate = moment(initiative.StartDate, API_DATE_FORMAT);
            startDate = _generateQuarterString(startDate.month(), startDate.year());
            var endDate = moment(initiative.EndDate, API_DATE_FORMAT);
            endDate = _generateQuarterString(endDate.month(), endDate.year());
            var timespan = startDate + " " + r.To + " " + endDate;
            timespan = "<p>" + timespan + "</p>";

            // Create milestones & training
            var milestones = "<ul>";
            var training = "<ul>";
            var events = initiative["Events"];
            var milstoneCount = 0;
            var trainingCount = 0;
            if (events.length > 0) {
                events.forEach(function (event, i) {
                    var dateStr = moment(event.Date, API_DATE_FORMAT);
                    dateStr = dateStr.format('LL');
                    var hoverText = event[HOVER_CULTURE] == null ? 
                        "" : event[HOVER_CULTURE];
                    var longText = event[TEXT_CULTURE] == null ? 
                        "" : event[TEXT_CULTURE];
                    var text = longText.length > 0 ? longText : hoverText;
                    if ((/milestone/gi).test(event.Type)) {
                        milestones += "<li id='event-" + event.ID +"' class='list-milestone'>" + dateStr + "<br>" + text + "</li>";
                        milstoneCount++
                    } else {
                        training += "<li id='event-" + event.ID +"' class='list-training'>"  + dateStr + "<br>" + text + "</li>";
                        trainingCount++;
                    }
                });
            }
            milestones += "</ul>";
            training += "</ul>";
            milestones = milstoneCount == 0 ? "" : milestones;
            training = trainingCount == 0 ? "" : training;

            // Create overall content 
            content.push(description, timespan, milestones, training);
            for (var i = 0; i < content.length; i++) {
                if (content[i].length == 0) continue;
                content[i] = "<h3 class='" + headingClass[i] + "'>" + heading[i] + "</h3>" + content[i];
            }
            return content.filter(function (n) { return n != "" }).join("");
        };
        
        var _generateQuarterString = function(month, year) {
            month++;
            var quarterString = r.ReportQuarterPrefix;
            if (month >= 1 && month <= 3) quarterString += ('4 ' + (year-1));
            if (month >= 4 && month <= 6) quarterString += ('1 ' + year);
            if (month >= 7 && month <= 9) quarterString += ('2 ' + year);
            if (month >= 10 && month <= 12) quarterString += ('3 ' + year);
            return quarterString;
        };
        
		// content sorting functions - alphabetically ordered
        var _categorize = function() {
            // decide whether to hide or show impact statement
            for (var i = 1; i < 6; i++) {
                var order = '[data-order='+i+']';
                var impactClass = '.impact-statement';
                var $sameImpactInitiatives = $accordions.find(order).not(impactClass);
                var $statement = $(impactClass + order);
                if ($sameImpactInitiatives.length > 0) {
                    _showElement($statement);
                } else {
                    _hideElement($statement);
                }
            }
        };
        
		var _filterTimelines = function() {
			sortableTimelines.forEach(function(timeline) {
				var timelineSelector = '.' + timeline[NAME_CULTURE];
				if (timeline.Skip == true) {
                    _hideElement(timelineSelector);
                } else {
                    _showElement(timelineSelector);
                }
			});
		};

		var _resetSort = function() {
			$accordions.find('.accordion-section').attr({
				'style': 'order: 0',
				'data-order': '0'
			});
            _showElement('[class*="header-"]');
            _showElement('.list-milestone, .list-training');
		};
		
		var _sortInitiatives = function() {
			sortableTimelines.forEach(function(timeline) {
				timeline.Data.forEach(function(initiative) {
					// check if Weight is defined
					// if undefined, region&branch are not selected
					if (initiative.Weight === undefined) return false;
					var order = _getFlexOrder(initiative.Weight);

					// re-order flex layout by setting order
					var idSuffix = initiative.ID;
					var accordion = '#initiative-accordion-' + idSuffix;
					var weight = initiative.Weight.toString();
					$(accordion).attr({
						'style': ('order: ' + order),
						'data-order': order,
						'data-weight': weight
					});

					// hide events if needs be
					initiative.Events.forEach(function(event, index) {
						if (event.Skip == undefined) return false;
						var eventSelector = '#event-' + event.ID;
						if (event.Skip == true) {
                            _hideElement(eventSelector);
                        } else {
                            _showElement(eventSelector);
                        }
					});
                    
                    // check to see if heading needs to be hidden
                    // these selectors cannot be cached - dynamic content
                    var $parent = $('#initiative-accordion-' + initiative.ID);
                    var $children = [
                        $parent.find('.list-milestone[aria-hidden=false]'),
                        $parent.find('.list-training[aria-hidden=false]')
                    ];
                    $children.forEach(function($childList, index) {
                        // # of visible list === 0 -> hide header
                        if ($childList.length === 0) {
                            // if index === 0 -> milestone
                            var $header = index === 0 ?
                            $parent.find('.header-milestones') : 
                            $parent.find('.header-trainings');
                            if ($header.length > 0) {
                                _hideElement($header);
                            }
                        }
                    });
				});
			});
		};
		
		// other helper methods
		var _getFlexOrder = function(weight) {
			switch (weight) {
				// just a helper function to switch flex orders around
				case 0: return 4; case 1: return 3; case 2: return 2;
				case 3: return 1; case 4: return 5; default: return 4;
			}
		};
        
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
		
		var _getImpactStatment = function(level) {
			switch (level) {
				case 0: return r.NoImpact; 		case 1: return r.LowImpact;
				case 2: return r.MediumImpact; 	case 3: return r.HighImpact;
				case 4: return r.BlueprintImpact;
				default: return r.NoImpact;
			}
		};

        var _hideElement = function(selector) {
            var obj = typeof selector === 'object' ? selector : $(selector);
            obj.attr({
                'aria-hidden': 'true',
                'hidden': 'hidden'
            });
            obj.addClass('hide');
        };
        
        var _showElement = function(selector) {
            var obj = typeof selector === 'object' ? selector : $(selector);
            obj.attr('aria-hidden', 'false').removeAttr('hidden');
            obj.removeClass('hide');
        };
        
		// public functions that other submodules can use
		var _draw = function(timelines) {
			if (timelines == null) return 'no argument was passed in';
			_emptyContinaers();
			timelines.forEach(_drawTimeline);
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
			sort: _sort
		}
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

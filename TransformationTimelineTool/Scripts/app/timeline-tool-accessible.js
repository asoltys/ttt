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
            }

            var _populateTimeline = function() {
                dm.timelines().forEach(function(element, index, array) {
                    var timeline = element[NAME_CULTURE];
                    var translation = r[timeline];
                    _addOption($timeline, { text: translation, value: timeline });
                });
            }

            var _populateRegionsAndBranches = function() {
                dm.regions().forEach(function(element, index, array) {
                    var region = element[NAME_CULTURE];
                    _addOption($region, { text: region, value: element.ID });
                });
                dm.branches().forEach(function(element, index, array) {
                    var branch = element[NAME_CULTURE];
                    branch = h.truncate(branch, 40);
                    _addOption($branch, { text: branch, value: element.ID });
                });
            }

            var _initialize = (function() {
                $branch.attr('disabled', 'true');
                _populateTimeline();
                _populateRegionsAndBranches();
            })();
        }
        
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
            }

            var _handleRegion = function() {
                if (this.value.indexOf('all') === -1) {
                    $branch.removeAttr('disabled');
                } else {
                    $branch.attr('disabled', 'disabled');
                }
                dm.setRegion(this.value);
                dm.filter();
                ui.content.sort(dm.timelines());
            }

            var _handleTimeline = function() {
                dm.setTimeline(this.value);
                dm.filter();
                ui.content.sort(dm.timelines());
            }
            
            var _handleAccordionClick = function(elem) {
                elem.toggleClass('toggle');
                var contentID = elem.attr("href");
                $(contentID).slideToggle();
            }

            var _registerEvents = (function() {
                // timeline tool controllers
                $timeline.on('change', _handleTimeline);
                $region.on('change', _handleRegion);
                $branch.on('change', _handleBranch);
                
                // delegate click event to each class
                $accordions.on("click", "a.accordion-section-title", function () {
                    _handleAccordionClick($(this));
                });
            })();
        }
        
        var _generate = function() {
            controllers();
            eventManager();
            ui.content.draw(dm.timelines());
        }
        
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

		var _drawRow = function(initiative) {
			accordions.appendChild(_generateAccordion(initiative, this));
		};

		var _drawTimeline = function(timeline) {
			var name = timeline[NAME_CULTURE];
            _addTimelineHeader(name);
			timeline.Data.forEach(_drawRow, name);
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
            accordions.appendChild(accordion);
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
        
        // **************************************************** from timeline-tool.js
		var _addImpactHeadingElements = function() {
			var weight, impactClass, element, weightOrder;
			weightOrder = [3, 2, 1, 0, 4];
			for (var i = 0, cond = weightOrder.length; i < cond; i++) {
				weight = weightOrder[i];
				impactClass = _getImpactClass(weightOrder[i], false);
				element = _createImpactHeadingElement(weight, impactClass);
				descriptionContainer.appendChild(element[0]);
				contentContainer.appendChild(element[1]);
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
		
		var _getImpactStatment = function(level) {
			switch (level) {
				case 0: return r.NoImpact; 		case 1: return r.LowImpact;
				case 2: return r.MediumImpact; 	case 3: return r.HighImpact;
				case 4: return r.BlueprintImpact;
				default: return r.NoImpact;
			}
		}
        
		// content sorting functions - alphabetically ordered
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
		};

		var _filterTimelines = function() {
			sortableTimelines.forEach(function(timeline) {
				var timelineSelector = '.' + timeline[NAME_CULTURE];
				if (timeline.Skip == true) $(timelineSelector).addClass('hide');
				else $(timelineSelector).removeClass('hide');
			});
		};
		
		var _getFlexOrder = function(weight) {
			switch (weight) {
				// just a helper function to switch flex orders around
				case 0: return 4; case 1: return 3; case 2: return 2;
				case 3: return 1; case 4: return 5; default: return 4;
			}
		}

		var _removeImpactClass = function(index, css) {
			return (css.match(/\S+color-content($|\s)/g) || []).join(' ');
		}

		var _resetSort = function() {
			$accordions.find('.accordion-section').attr({
				'style': 'order: 0',
				'data-order': '0'
			});
            _showElement('[class*="header-"]');
            _showElement('.list-milestone, .list-training');
		}
		
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
		}
		
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
		}
        
        var _hideElement = function(selector) {
            var obj = typeof selector === 'object' ? selector : $(selector);
            obj.attr({
                'aria-hidden': 'true',
                'hidden': 'hidden'
            });
        }
        
        var _showElement = function(selector) {
            $(selector).attr('aria-hidden', 'false').removeAttr('hidden');
        }
        
		// public functions that other submodules can use
		var _draw = function(timelines) {
			if (timelines == null) return 'no argument was passed in';
			_emptyContinaers();
			timelines.forEach(_drawTimeline);
		}

		var _sort = function(timelines) {
			sortableTimelines = timelines;
			_resetSort();
			_filterTimelines();
			_sortInitiatives();
			// _categorize();
		}

		return {
			draw: _draw,
			sort: _sort
		}
	})();
    
    // GET data from the server and generate UI
	dm.config({CULTURE: CULTURE});
	dm.load(ui.generate);
});


/*
var getQuarterData = function (month, year) {
    if (month >= 1 && month <= 3) return [4,year-1];
    if (month >= 4 && month <= 6) return [1,year];
    if (month >= 7 && month <= 9) return [2,year];
    if (month >= 10 && month <= 12) return [3,year];
    return [0,0];
}

var getQuarter = function (month, year) {
    if (month >= 1 && month <= 3) return 'Q4 ' + (year-1);
    if (month >= 4 && month <= 6) return 'Q1 ' + year;
    if (month >= 7 && month <= 9) return 'Q2 ' + year;
    if (month >= 10 && month <= 12) return 'Q3 ' + year;
    return 'error';
}

function toggleAccordion(elem) {
    elem.toggleClass('toggle');
    var contentID = elem.attr("href");
    $(contentID).slideToggle();
}


// ----------------------------------------------------------------------------

var gui = (function (resources) {
    var _accordionCount = 0;

    var _addHeading = function(title) {
        var heading = "<h2>";
        heading += title;
        heading += "</h2>";
        $('#timeline-accordions').append(heading);
    }

    var _addAccordion = function (title, content, weight) {
        weight = weight !== undefined ? weight : -1;
        _accordionCount++;
        var accordion = "<div class='accordion-section' data-weight=" + weight + ">";
        accordion += "<a class='accordion-section-title' href='#accordion-" + _accordionCount + "'><span></span>&nbsp;" + title + "</a>";
        accordion += "<div id='accordion-" + _accordionCount + "' class='accordion-section-content closed'>";
        accordion += content;
        accordion += "</div></div>";
        $("#timeline-accordions").append(accordion);
    }

    var _render = function(data) {
        $("#timeline-accordions").empty();
        data.forEach(function(block) {
            _addHeading(resources.get(block['NameE']));
            block.Data.forEach(function(initiative, idx, arr) {
                var content = _createContent(initiative);
                var initiativeName = initiative['Name' + cultureDataAppend];
                if (initiative.Weight === undefined) {
                    _addAccordion(initiativeName, content);
                } else {
                    _addAccordion(initiativeName, content, initiative.Weight);
                }
            });
        });
    }

    var _getImpactResourceByWeight = function (weight) {
        switch (weight) {
            case 0: return '@Resources.TimelineNoImpact';
            case 1: return '@Resources.TimelineLowImpact';
            case 2: return '@Resources.TimelineMediumImpact';
            case 3: return '@Resources.TimelineHighImpact';
            case 4: return '@Resources.BP2020';
        }
    }

    var _categorize = function () {
        if (!(controller.regionKey() == 0 || controller.branchKey() == 0)) {
            var $accordions = $(".accordion-section");
            var previousWeight;
            $.each($accordions, function () {
                var weight = $(this).data('weight');
                if (weight == previousWeight)
                    return true; // skip to next iteration
                if (weight != 4) {
                    $("<h4>" + _getImpactResourceByWeight(weight) + "</h4>").insertBefore($(this));
                    previousWeight = weight;
                } else {
                    return true;
                }
            });
        }
    }

    var _createContent = function (initiative) {
        var content = [];
        var heading = ['@Resources.Description', '@Resources.Timespan',
                       '@Resources.Milestones', '@Resources.TrainingPlural'];

        // Create description
        var description = "<p>" + initiative['Description' + cultureDataAppend] + "</p>";

        // Create timespan
        var startDate = moment(initiative.StartDate, apiReturnDateFormat);
        startDate = getQuarter(startDate.month()+1, startDate.year());
        var endDate = moment(initiative.EndDate, apiReturnDateFormat);
        endDate = getQuarter(endDate.month()+1, endDate.year());
        var timespan = startDate + " @Resources.to " + endDate;
        timespan = "<p>" + timespan + "</p>";

        // Create milestones & training
        var milestones = "<ul>";
        var training = "<ul>";
        var events = initiative["Events"];
        var milstoneCount = 0;
        var trainingCount = 0;
        if (events.length > 0) {
            events.forEach(function (event, i) {
                var dateStr = moment(event.Date, apiReturnDateFormat);
                dateStr = dateStr.format('LL');
                var hoverText = event["Hover" + cultureDataAppend] == null ? "" : event["Hover" + cultureDataAppend];
                var longText = event["Text" + cultureDataAppend] == null ? "" : event["Text" + cultureDataAppend];
                var text = longText.length > 0 ? longText : hoverText;
                if ((/milestone/gi).test(event.Type)) {
                    milestones += "<li>" + dateStr + "<br>" + text + "</li>";
                    milstoneCount++
                } else {
                    training += "<li>" + dateStr + "<br>" + text + "</li>";
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
            content[i] = "<h3>" + heading[i] + "</h3>" + content[i];
        }
        return content.filter(function (n) { return n != "" }).join("");
    }

    return {
        draw: function (data) {
            var start = Date.now();
            _render(data);
            _categorize();
            var later = Date.now();
            console.log("Took " + (later - start) + "ms to draw data");
        }
    }
})(resources);


var dataManager = (function() {
    var _protectedInitiatives = null;
    var _filteredInitiatives = null;
    var _timelines = [];
    var _regions = null;
    var _branches = null;
    var _initiativesURL = "/data/accessibility";
    var _regionsURL = "/data/regions";
    var _branchesURL = "/data/branches";
    var _initialize = function(callback) {
        var xhr1 = callPostAPI(_initiativesURL, function(data) {
            _protectedInitiatives = JSON.parse(data);
            _protectedInitiatives.forEach(function(elem, idx, arr) {
                _timelines.push(elem['Name' + cultureDataAppend]);
            });
            _filteredInitiatives = JSON.parse(data);
        }, {culture: culture});
        var xhr2 = callPostAPI(_regionsURL, function(data) {
            data = JSON.parse(data);
            data = _sortByName(data);
            var nca;
            data = data.filter(function (object) {
                if (object.NameShort == "nca")
                    nca = object;
                return object.NameShort != "nca"
            });
            data.unshift(nca);
            _regions = data;
        });
        var xhr3 = callPostAPI(_branchesURL, function(data) {
            data = JSON.parse(data);
            _branches = _sortByName(data);
        });
        $.when(xhr1, xhr2, xhr3).done(callback);
    };

    var _filterTimeline = function() {
        var timelineKey = controller.timelineKey();
        if (timelineKey !== 'All') {
            _filteredInitiatives = _filteredInitiatives.filter(function(elem, idx, arr) {
                return elem['Name' + cultureDataAppend] == timelineKey;
            });
        }
    }

    var _determineWeight = function() {
        _filteredInitiatives.forEach(function(block) {
            if (controller.regionKey() != '0' && controller.branchKey() != '0') {
                block.Data.forEach(function(elem, idx, arr) {
                    if (_keyExists(elem, 'Impacts', controller.controlKey())) {
                        elem.Weight = _getValueByKey(elem, 'Impacts', controller.controlKey());
                    } else {
                        elem.Weight = 0;
                    }
                })
            }
        });
    }

    var _filterEvents = function() {
        _filteredInitiatives.forEach(function(block) {
            if (controller.regionKey() != '0' && controller.branchKey() != '0') {
                block.Data.forEach(function(initiative, idx, arr) {
                    initiative.Events = initiative.Events.filter(function(elem, idx, arr) {
                        return _keyExists(elem, 'Control', controller.controlKey());
                    })
                });
            }
        });
    }

    var _sortOrder = function() {
        _filteredInitiatives.forEach(function(block) {
            block.Data.sort(function(a, b) {
                return (sortComparator(b.Weight, a.Weight) ||
                        sortComparator(a['Name' + cultureDataAppend], b['Name' + cultureDataAppend]));
            });
        });
    }

    var _filterRegionBranch = function() {
        _determineWeight();
        _filterEvents();
        _sortOrder();
    }

    var _filterInitiatives = function() {
        _filteredInitiatives = _jsonDeepCopy(_protectedInitiatives);
        _filterTimeline();
        _filterRegionBranch();
        gui.draw(_filteredInitiatives);
    }

    var _jsonDeepCopy = function(jsonToCopy) {
        return JSON.parse(JSON.stringify(jsonToCopy));
    }

    var _keyExists = function(object, objectAccessor, key) {
        return key in object[objectAccessor];
    }

    var _getValueByKey = function(object, objectAccessor, key) {
        return object[objectAccessor][key];
    }

    var _sortByName = function(data) {
        return data.sort(function(a, b) {
            return sortComparator(a['Name' + cultureDataAppend], b['Name' + cultureDataAppend]);
        });
    }

    var _getProtectedInitiatives = function() { return _protectedInitiatives };
    var _getFilteredInitiatives = function() { return _filteredInitiatives };
    var _getTimelines = function() { return _timelines };
    var _getRegions = function() { return _regions };
    var _getBranches = function() { return _branches };

    return {
        filterDataset: _filterInitiatives,
        getProtectedInitiatives: _getProtectedInitiatives,
        getFilteredInitiatives: _getFilteredInitiatives,
        getTimelines: _getTimelines,
        getRegions: _getRegions,
        getBranches: _getBranches,
        initialize: _initialize
    }
})();

var controller = (function(dataManager, resources) {
    var _timeline = '#select-timeline',
        _timelineKey = document.querySelector(_timeline).value;
    var _region = '#select-region',
        _regionKey = document.querySelector(_region).value;
    var _branch = '#select-branch',
        _branchKey = document.querySelector(_branch).value;
    var _controlKey;

    var _addOption = function(selector, optionObject) {
        var select = document.querySelector(selector);
        var option = document.createElement('option');
        option.text = optionObject.text;
        option.value = optionObject.value;
        select.add(option);
    }

    var _addTimelineOptions = function() {
        dataManager.getTimelines().forEach(function(elem, idx, arr) {
            _addOption(_timeline, {text: resources.get(elem), value: elem});
        });
    }

    var _addBranchOptions = function() {
        dataManager.getBranches().forEach(function(elem, idx, arr) {
            _addOption(_branch, {text: elem['Name' + cultureDataAppend], value: elem['ID']});
        });
    }
    
    var _addRegionOptions = function() {
        dataManager.getRegions().forEach(function(elem, idx, arr) {
            _addOption(_region, {text: elem['Name' + cultureDataAppend], value: elem['ID']});
        });
    }

    var _addEventListener = function(selector, eventType, callback) {
        var elements = document.querySelectorAll(selector);
        [].forEach.call(elements, function(elem) {
            elem.addEventListener(eventType, callback);
        });
    }

    var _lockRegionBranch = function(lock) {
        var region = document.querySelector(_region),
            branch = document.querySelector(_branch);
        if (lock) {
            region.disabled = true;
            branch.disabled = true;
            region.options.item(0).selected = true;
            branch.options.item(0).selected = true;
            _regionKey = 0;
            _branchKey = 0;
        } else {
            region.disabled = false;
            branch.disabled = false;
        }
    }

    var _resetPage = function() {
        _timelineKey = this.value;
        if (_timelineKey == 0) {
            _lockRegionBranch(true);
        } else {
            _lockRegionBranch(false);
            dataManager.filterDataset();
        }
    }

    var _applyFilter = function() {
        _regionKey = document.querySelector(_region).value;
        _branchKey = document.querySelector(_branch).value;
        dataManager.filterDataset();
    }

    var _registerEvents = function() {
        _addEventListener(_timeline, 'change', _resetPage);
        _addEventListener(_region, 'change', _applyFilter);
        _addEventListener(_branch, 'change', _applyFilter);
    }

    var _populateControllers = function() {
        _addTimelineOptions();
        _addRegionOptions();
        _addBranchOptions();
        _registerEvents();
    }

    var _getControlKey = function() {
        _controlKey = _regionKey + ',' + _branchKey;
        return _controlKey;
    }

    var _getRegionKey = function() { return _regionKey }

    var _getBranchKey = function() { return _branchKey }

    var _getControllerState = function() {
        return 0;
    }

    var _getTimelineKey = function() {
        return _timelineKey;
    }

    return {
        generate: _populateControllers,
        controlKey: _getControlKey,
        regionKey: _getRegionKey,
        branchKey: _getBranchKey,
        controlState: _getControllerState,
        timelineKey: _getTimelineKey
    }
})(dataManager, resources);

$(window).on('load', function() {
    dataManager.initialize(controller.generate);
    // Delegated Event listener to bind events to dynamic HTML contents
    $("#timeline-accordions").on("click", "a.accordion-section-title", function () {
        toggleAccordion($(this));
    });
})
*/
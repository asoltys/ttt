var callPostAPI = function (url, successCallback, loaderCallback, jsonData) {
	if (arguments.length == 3) 
		$.ajax({
	        type: "POST",
	        url: url,
	        cache: false,
	        beforeSend: function() {
	        	if (loaderCallback !== undefined)
	        		loaderCallback();
	        },
	        success: function (data) {
	            successCallback(JSON.stringify(data));
	        	if (loaderCallback !== undefined)
	        		loaderCallback();
	        },
	        error: function (jqXHR, textStatus, errorThrown) {
	            console.log(JSON.stringify(jqXHR) + ", " + textStatus);
	        }
	    });
	else 
		$.ajax({
	        type: "POST",
	        contentType: "application/json",
	        url: url,
	        cache: true,
	        data: JSON.stringify(jsonData),
	        beforeSend: function() {
	        	if (loaderCallback !== undefined)
	        		loaderCallback();
	        },
	        success: function (data) {
	        	if (loaderCallback !== undefined)
	        		loaderCallback();
	            successCallback(JSON.stringify(data));
	        },
	        error: function (jqXHR, textStatus, errorThrown) {
	            console.log(JSON.stringify(jqXHR) + ", " + textStatus);
	        }
	    });
}

var culture = window.location.href.indexOf('lang=fra') > -1 ? 'fr-ca':'en-ca';
moment.locale(culture);
// API will always return dates in DD/MM/YYYY from Database
// -> do not forget to parse it in the right way
var apiReturnDateFormat = 'DD/MM/YYYY';

// ----------------------------------------------------------------------------

var sortComparator = function (a, b) {
    if (a > b) return 1;
    if (a < b) return -1;
    return 0;
}


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

var controller = (function() {
	var _initiativeListUrl = '/data/initiatives';
	var _quarterListUrl = '/data/quarters';
	var _regionListUrl = '/data/regions';
	var _branchListUrl = '/data/branches';

	var _addInitiativeOption = function(object) {
		var textToDisplay = culture == 'en-ca' ? object.NameE : object.NameF;
		$('#select_initiative').append($('<option>', {
		    value: object.ID,
		    text: textToDisplay
		}));
	}

	var _addQuarterOption = function(object) {
		var startDate = moment(object.Start, apiReturnDateFormat);
		var endDate = moment(object.End, apiReturnDateFormat);
		var startQuarter = getQuarterData(startDate.month()+1, 
			startDate.year());
		var endQuarter = getQuarterData(endDate.month()+1, 
			endDate.year());
		var textToDisplay = [];
		var valueToHold = [];
		for (var month = startQuarter[0], year = startQuarter[1];
			year <= endQuarter[1];) {
			endMonth = year == endQuarter[1] ? endQuarter[0] : 4;
			for (var i = month; i <= endMonth; i++) {
				textToDisplay.push("Q" + i + "/" + year);
				var value = {quarter: i, year: year, culture: culture};
				valueToHold.push(JSON.stringify(value));
			}
			month = 1;
			year++;
		}
		for (var i = 0; i < textToDisplay.length; i++) {
			var $option = $('<option></option>');
			$option.text(textToDisplay[i]);
			$option.attr('data-value', valueToHold[i]);
			$('#select_quarter').append($option);
		}
	}

	var _addRegionOption = function(object) {
		var textToDisplay = culture == 'en-ca' ? object.NameE : object.NameF;
		$('#select_region').append($('<option>', {
		    value: object.ID,
		    text: textToDisplay
		}));
	}

	var _addBranchOption = function(object) {
		var textToDisplay = culture == 'en-ca' ? object.NameE : object.NameF;
		$('#select_branch').append($('<option>', {
		    value: object.ID,
		    text: textToDisplay
		}));
	}

	var _fillInitiativeController = function(data) {
		data = JSON.parse(data);
		data.forEach(_addInitiativeOption);
	}

	var _fillQuarterController = function(data) {
		data = JSON.parse(data);
		data.forEach(_addQuarterOption);
	}

	var _fillRegionController = function(data) {
		data = JSON.parse(data);
		var propertyCulture = culture == "en-ca" ? "NameE" : "NameF";
		data = data.sort(function (a, b) {
		    return sortComparator(a[propertyCulture], b[propertyCulture]);
		});
		var nca;
		data = data.filter(function (object) {
		    if (object.NameShort == "nca")
		        nca = object;
		    return object.NameShort != "nca"
		});
		data.unshift(nca);
		data.forEach(_addRegionOption);
	}

	var _fillBranchController = function(data) {
		data = JSON.parse(data);
		var propertyCulture = culture == "en-ca" ? "NameE" : "NameF";
		data = data.sort(function (a, b) {
		    return sortComparator(a[propertyCulture], b[propertyCulture]);
		});
		data.forEach(_addBranchOption);
	}

	var _toggleInitiativeLoading = function() {
		$('#icon_initiative_loading').toggleClass('hide');
	}

	var _toggleQuarterLoading = function() {
		$('#icon_quarter_loading').toggleClass('hide');
	}

	var _toggleRegionLoading = function() {
		$('#icon_region_loading').toggleClass('hide');
	}

	var _toggleBranchLoading = function() {
		$('#icon_branch_loading').toggleClass('hide');
	}

	var _registerEvents = function() {
		$('#radio_report_initiative').on('click', function() {
			gui.reset();
			$('#initiative-container').removeClass('hide');
			$('#quarter-container').addClass('hide');
			$('#timeline_container').addClass('hide print-none');
		});
		$('#radio_report_quarterly').on('click', function() {
			gui.reset();
			$('#quarter-container').removeClass('hide');
			$('#initiative-container').addClass('hide');
			$('#timeline_container').removeClass('hide print-none');
		});
	}

	var _initialize = (function() {
		_registerEvents();
		callPostAPI(_initiativeListUrl, _fillInitiativeController,
					_toggleInitiativeLoading);
		callPostAPI(_quarterListUrl, _fillQuarterController, 
					_toggleQuarterLoading);
		callPostAPI(_regionListUrl, _fillRegionController, 
					_toggleRegionLoading);
		callPostAPI(_branchListUrl, _fillBranchController, 
					_toggleBranchLoading);
	})();

	return {
		initiative: $('#select_initiative'),
		quarter: $('#select_quarter'),
		region: $('#select_region'),
		branch: $('#select_branch')
	}
})();

// ----------------------------------------------------------------------------

var quarterlyReport = (function() {
	var _initiativesQuarterlyUrl = '/data/initiative-quarterly'
	var _regionId;
	var _branchId;
	var _initiatives;
	var _sortedInitiatives
	var _cultureDataAppend = culture == 'en-ca' ? 'E' : 'F';

	var _toggleReportLoading = function() {
		$('#icon_report_loading').toggleClass('hide');
	}

	var _createContent = function(initiative) {
		var content = [];
		var heading = ["Description", "Timespan", "Key milestones", "Training"];

		// Create description
		var description = "<p>" + 
			initiative["Description" + _cultureDataAppend] + "</p>";

		// Create timespan
		var startDate = moment(initiative.StartDate, apiReturnDateFormat);
		startDate = getQuarter(startDate.month()+1, startDate.year());
		var endDate = moment(initiative.EndDate, apiReturnDateFormat);
		endDate = getQuarter(endDate.month()+1, endDate.year());
		var timespan = startDate + " ~ " + endDate;
		timespan = "<p>" + timespan + "</p>";

		// Create milestones & training
		var milestones = "<ul>";
		var training = "<ul>";
		var events = initiative.Events;
		var milstoneCount = 0;
		var trainingCount = 0;
		if (events.length > 0) {
		    events.forEach(function (event, i) {
		    	var dateStr = moment(event.Date, apiReturnDateFormat);
		    	dateStr = dateStr.format('LL');
		        var hoverText = event["Hover" + _cultureDataAppend] == null ? "" : event["Hover" + _cultureDataAppend];
		        var longText = event["Text" + _cultureDataAppend] == null ? "" : event["Text" + _cultureDataAppend];
		        if ((/milestone/gi).test(event.Type)) {
		            milestones += "<li>" + dateStr + "<br>" + hoverText + longText + "</li>";
		            milstoneCount++
		        } else {
		            training += "<li>" + dateStr + "<br>" + hoverText + longText + "</li>";
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
		return {Name: initiative["Name" + _cultureDataAppend], 
			htmlContent: content.filter(function (n) { 
				return n != ""
			}).join("")};
	}

	var _processData = function(data) {
		_initiatives = JSON.parse(data);
		_initiatives.forEach(function (initiative, i) {
		    var tempControlObject;
		    initiative.Impacts.forEach(function (impact, j) {
		        tempControlObject = _mergeObjects(tempControlObject, _createControlObject(impact, 'impact'));
		    });
		    initiative.control = tempControlObject;
		    delete initiative.Impacts;

		    initiative.Events.forEach(function (event, j) {
		        initiative.Events[j].control = _createControlObject(event, 'event');
		        delete initiative.Events[j].Regions;
		        delete initiative.Events[j].Branches;
		    });
		});
		_sortedInitiatives = JSON.parse(JSON.stringify(_initiatives));
		_sortedInitiatives.forEach(function(initiative) {
			var content = _createContent(initiative);
			gui.makeBox(content);
		});
		gui.render();
	}

	var _impactWeight = function (level) {
	    switch (level) {
	        case 0: return 10; // None
	        case 1: return 20; // Low
	        case 2: return 30; // Medium
	        case 3: return 40; // High
	        case 4: return 0; // BP2020
	        default: return -1;
	    }
	}

	var _createControlObject = function (object, type) {
	    /*  Control Object has key:value structure
	     *  key => [region,branch]
	     *  value => [impactWeight or -1]
	     */
	    var control = {};
	    object.Regions.forEach(function (region, i) {
	        object.Branches.forEach(function (branch, i) {
	            var hashKey = region + "," + branch;
	            if (type === 'impact') {
	                control[hashKey] = _impactWeight(object.Level);
	            } else {
	                control[hashKey] = -1;
	            }
	        });
	    });
	    return control;
	}

	var _mergeObjects = function (obj1, obj2) {
	    var temp = {};
	    for (var attrname in obj1) { temp[attrname] = obj1[attrname]; }
	    for (var attrname in obj2) { temp[attrname] = obj2[attrname]; }
	    return temp;
	}

	var _getInitiatives = function(callback, quarterData) {
		gui.reset();
		callPostAPI(
			_initiativesQuarterlyUrl,
			callback,
			_toggleReportLoading, 
			quarterData);
	}

	var _registerEvents = function() {
		controller.quarter.on('change', function() {
			gui.reset();
			if ($(this).val() != 0) {
				_quarterData = $(this).find('option:selected').data('value');
				_getInitiatives(_processData, _quarterData);
			}
		});
		controller.region.on('change', function() {
			gui.reset();
			if ($(this).val() != 0) {
				_regionId = $(this).val();
			}
		});
		controller.branch.on('change', function() {
			gui.reset();
			if ($(this).val() != 0) {
				_branchId = $(this).val();
			}
		});
	}

	var _getInitiativesObject = function() {
		return _initiatives;
	}

	var _initialize = (function() {
		_registerEvents();
	})();
	return {
		getInitiatives: _getInitiativesObject
	}
})();

var initiativeReport = (function() {
	var _initiativeIdUrl = '/data/initiative'
	var _currentInitiativeId = 0;
	var _initiative;
	var _cultureDataAppend = culture == 'en-ca' ? 'E' : 'F';

	var _createContent = function(initiative) {
		var content = [];
		var heading = ["Description", "Timespan", "Key milestones", "Training"];

		// Create description
		var description = "<p>" + 
			initiative["Description" + _cultureDataAppend] + "</p>";

		// Create timespan
		var startDate = moment(initiative.StartDate, apiReturnDateFormat);
		startDate = getQuarter(startDate.month()+1, startDate.year());
		var endDate = moment(initiative.EndDate, apiReturnDateFormat);
		endDate = getQuarter(endDate.month()+1, endDate.year());
		var timespan = startDate + " ~ " + endDate;
		timespan = "<p>" + timespan + "</p>";

		// Create milestones & training
		var milestones = "<ul>";
		var training = "<ul>";
		var events = initiative.Events;
		var milstoneCount = 0;
		var trainingCount = 0;
		if (events.length > 0) {
		    events.forEach(function (event, i) {
		    	var dateStr = moment(event.Date, apiReturnDateFormat);
		    	dateStr = dateStr.format('LL');
		        var hoverText = event["Hover" + _cultureDataAppend] == null ? "" : event["Hover" + _cultureDataAppend];
		        var longText = event["Text" + _cultureDataAppend] == null ? "" : event["Text" + _cultureDataAppend];
		        if ((/milestone/gi).test(event.Type)) {
		            milestones += "<li>" + dateStr + "<br>" + hoverText + longText + "</li>";
		            milstoneCount++
		        } else {
		            training += "<li>" + dateStr + "<br>" + hoverText + longText + "</li>";
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
		return {Name: initiative["Name" + _cultureDataAppend], 
			htmlContent: content.filter(function (n) { 
				return n != ""
			}).join("")};
	}

	var _processData = function(data) {
		_initiative = JSON.parse(data); // one element array!
		_initiative.forEach(function(initiative) {
			var content = _createContent(initiative);
			gui.makeBox(content);
		});
		gui.render();
	}

	var _registerEvents = function() {
		controller.initiative.on('change', function() {
			gui.reset();
			if ($(this).val() != 0) {
				_currentInitiativeId = $(this).val();
				_getInitiative(_processData);
			}
		});
	}

	var _toggleReportLoading = function() {
		$('#icon_report_loading').toggleClass('hide');
	}

	var _getInitiative = function(callback) {
		callPostAPI(
			_initiativeIdUrl,
			callback,
			_toggleReportLoading, 
			{Id : parseInt(_currentInitiativeId)});
	}

	var _initialize = (function() {
		_registerEvents();
	})();
})();

// ----------------------------------------------------------------------------

var gui = (function() {
	var boxes = [];

	var _addSummaryBox = function(content) {
		var box = "<div class='module-summary module-simplify span-12'>";
		box += "<h3><span class='color-dark'>" + content.Name + "</span></h3>";
		box += content.htmlContent;
		box += "</div>";
		boxes.push(box);
	}

	var _draw = function() {
		boxes.forEach(function(element) {
			$('#report').append(element);
		});
	}

	var _reset = function() {
		boxes = [];
		$('#report').empty();
	}

	return {
		reset: _reset,
		makeBox: _addSummaryBox,
		render: _draw
	}
})();

// ----------------------------------------------------------------------------

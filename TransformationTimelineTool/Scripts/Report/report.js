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
	        	successCallback(JSON.stringify(data));
	        	if (loaderCallback !== undefined)
	        		loaderCallback();
	        },
	        error: function (jqXHR, textStatus, errorThrown) {
	            console.log(JSON.stringify(jqXHR) + ", " + textStatus);
	        }
	    });
}

var culture = window.location.href.indexOf('lang=fra') > -1 ? 'fr-ca':'en-ca';
moment.locale(culture);
var apiReturnDateFormat = 'MM/DD/YYYY';

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
	var _selectInitiativeTimeline = $('#select-initiative-timeline');
	var _selectInitiative = $('#select-initiative');

	var _addInitiativeOption = function(object) {
		var textToDisplay = culture == 'en-ca' ? object.NameE : object.NameF;
		var $option = $('<option></option>');
		$option.attr('value', object.ID);
		$option.text(textToDisplay);
		$option.attr('data-delete', 'true');
		_selectInitiative.append($option);
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
			$('#select-quarter').append($option);
		}
	}

	var _addOption = function(optionObject, idSuffix) {
		var text = culture == 'en-ca' ? optionObject.NameE : optionObject.NameF;
		var value = optionObject.ID === undefined ? text : optionObject.ID;
		$('#select-' + idSuffix).append($('<option>', {
			value: value,
			text: text
		}));
	}

	var _setupInitiativeReportControllers = function(data) {
		data = JSON.parse(data);
		initiativeReport.setData(data);
		dataFactory.setInitiativeReport(data);
		data.forEach(function(initiativeBlock) {
			_setupTimelineController(initiativeBlock, 'initiative-timeline');
			_setupTimelineController(initiativeBlock, 'quarter-timeline');
		});
	}

	var _setupTimelineController = function(object, idSuffix) {
		_addOption(object, idSuffix);
	}

	var _clearInitiativeController = function() {
		var $options = _selectInitiative.children('option');
		$options.each(function() {
			if ($(this).attr('data-delete') == 'true')
				$(this).remove();
		});
	}

	var _populateInitiativeController = function() {
		_clearInitiativeController();
		var currentBlock = initiativeReport.getData().filter(function(obj) {
			var timeline = _selectInitiativeTimeline.val();
			if (timeline == 'All') return true;
			var propertyCulture = culture == 'en-ca' ? 'NameE' : 'NameF';
			return timeline == obj[propertyCulture];
		});
		currentBlock.forEach(function(block) {
			block.Data.forEach(function(data) {
				_addInitiativeOption(data);
			});
		});
	}

	var _fillQuarterController = function(data) {
		data = JSON.parse(data);
		data.forEach(function(initiativeBlock) {
			_addQuarterOption(initiativeBlock);
		});
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
		data.forEach(function (object) {
			_addOption(object, 'region');
		});
	}

	var _fillBranchController = function(data) {
		data = JSON.parse(data);
		var propertyCulture = culture == "en-ca" ? "NameE" : "NameF";
		data = data.sort(function (a, b) {
		    return sortComparator(a[propertyCulture], b[propertyCulture]);
		});
		data.forEach(function (object) {
			_addOption(object, 'branch');
		});
	}

	var _toggleLoading = function(idSuffix) {
		$('#icon-loading-' + idSuffix).toggleClass('hide');
	}

	var _registerEvents = function() {
		$('#radio-report-initiative').on('click', function() {
			gui.reset();
			$('#initiative-container').removeClass('hide');
			$('#quarter-container').addClass('hide');
			$('#timeline-container').addClass('hide print-none');
		});
		$('#radio-report-quarterly').on('click', function() {
			gui.reset();
			$('#quarter-container').removeClass('hide');
			$('#initiative-container').addClass('hide');
			$('#timeline-container').removeClass('hide print-none');
		});
		$('#select-quarter-timeline').on('change', function() {
			if ($(this).val() != 0) {
				$('#region-branch').removeClass('hide');
			} else {
				$('#region-branch').addClass('hide');
			}
		});
		$('#select-initiative-timeline').on('change', function() {
			if ($(this).val() != 0) {
				$('#initiative').removeClass('hide');
				_selectInitiative.val('0');
				_populateInitiativeController();
			} else {
				$('#initiative').addClass('hide');
			}
		});
	}

	var _initialize = (function() {
		_registerEvents();
		// initiative report API call
		callPostAPI(_initiativeListUrl, _setupInitiativeReportControllers,
					function () { _toggleLoading('initiative') }, {culture: culture});
		// quarterly report API calls
		callPostAPI(_quarterListUrl, _fillQuarterController, 
					function () { _toggleLoading('quarter') });
		callPostAPI(_regionListUrl, _fillRegionController, 
					function () { _toggleLoading('region') });
		callPostAPI(_branchListUrl, _fillBranchController, 
					function () { _toggleLoading('branch') });
	})();

	return {
		initiative: _selectInitiative,
		quarter: $('#select-quarter'),
		region: $('#select-region'),
		branch: $('#select-branch'),
		quarterTimeline: $('#select-quarter-timeline'),
		initiativeTimeline: $('#select-initiative-timeline')
	}
})();

// ----------------------------------------------------------------------------

var dataFactory = (function() {
	var _protectedInitiativeReportDataset;
	var _filteredInitiativeReportDataset;
	var _contentInitiativeReportDataset;
	var _protectedQuarterlyReportDataset;
	var _filteredQuarterlyReportDataset;

	var _getInitiativeReportDataset = function() {
		return _contentInitiativeReportDataset;
	}

	var _setInitiativeReportDataset = function(data) {
		_protectedInitiativeReportDataset = data;
		_filteredInitiativeReportDataset = data;
	}

	var _getQuarterlyReportDataset = function() {
		return _filteredQuarterlyReportDataset;
	}

	var _setQuarterlyReportDataset = function(data) {
		_protectedQuarterlyReportDataset = data;
		_filteredQuarterlyReportDataset = data;
	}

	var _registerEvents = function() {
		controller.initiative.on('change', function() {
			var _filteredInitiativeReportDataset = _protectedInitiativeReportDataset.filter(function(obj) {
				var timeline = controller.initiativeTimeline.val();
				if (timeline == 'All') return true;
				var propertyCulture = culture == 'en-ca' ? 'NameE' : 'NameF';
				return timeline == obj[propertyCulture];
			});
			_contentInitiativeReportDataset = [];
			_filteredInitiativeReportDataset.forEach(function(block) {
				var temp = block.Data.filter(function(obj) {
					var initiative = controller.initiative.val();
					if (initiative == 'All') return true;
					return initiative == obj.ID;
				});
				_contentInitiativeReportDataset = _mergeObjects(_contentInitiativeReportDataset, temp);
			});
		});
	}

	var _mergeObjects = function (obj1, obj2) {
	    var temp = [];
	    obj1.forEach(function(obj) { temp.push(obj); });
	    obj2.forEach(function(obj) { temp.push(obj); });
	    return temp;
	}

	var _initialize = (function() {
		_registerEvents();
	})();

	return {
		getInitiativeReport: _getInitiativeReportDataset,
		setInitiativeReport: _setInitiativeReportDataset,
		getQuarterlyReport: _getQuarterlyReportDataset,
		setQuarterlyReport: _setQuarterlyReportDataset
	}
})();

var quarterlyReport = (function() {
	var _initiativesQuarterlyUrl = '/data/initiative-quarterly'
	var _regionId;
	var _branchId;
	var _initiatives;
	var _sortedInitiatives
	var _cultureDataAppend = culture == 'en-ca' ? 'E' : 'F';

	var _toggleReportLoading = function() {
		$('#icon-report-loading').toggleClass('hide');
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

var contentGenerator = (function() {
	var _cultureDataAppend = culture == 'en-ca' ? 'E' : 'F';

	var _createInitiativeBox = function(initiative) {
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
		console.log(initiative);
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

	return {
		createInitiative: _createInitiativeBox
	}
})();

var initiativeReport = (function(dataFactory) {
	var _currentInitiativeId = 0;
	var _initiative;

	var _processData = function(initiatives) {
		initiatives.forEach(function(initiative) {
			var content = _createContent(initiative);
			gui.makeBox(content);
		});
		gui.render();
	}

	var _registerEvents = function() {
		controller.initiative.on('change', function() {
			gui.reset();
			if ($(this).val() != 0) {
				dataFactory.getInitiativeReport().forEach(function(initiative) {
					gui.makeBox(contentGenerator.createInitiative(initiative));
				});
				gui.render();
			}
		});
	}

	var _getInitiative = function(id) {
		console.log(dataFactory.getInitiativeReport());
		return _dataset.filter(function(obj) {
			if (id == "All") return true;
			return obj.ID == id;
		});
	}

	var _toggleReportLoading = function() {
		$('#icon-report-loading').toggleClass('hide');
	}

	var _setData = function(data) {
		_dataset = data;
	}

	var _getData = function() {
		return _dataset;
	}

	var _initialize = (function() {
		_registerEvents();
	})();
	return {
		setData: _setData,
		getData: _getData
	}
})(dataFactory);

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

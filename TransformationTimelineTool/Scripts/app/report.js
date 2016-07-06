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

var gui = (function(resources) {
	var boxes = [];
	var controllerText = [];
	var $controllerInfo = $('#controller-information');
	var $report = $('#report');

	var _addTimelineBox = function(content) {
		var box = "<div class='span-12 timeline'>";
		box += "<h3><span class='color-dark timeline-title'>" + content.Name + "</span></h3>";
		box += content.htmlContent;
		box += "</div>"
		boxes.push(box);
	}

	var _addSummaryBox = function(content) {
		var box = "<div class='module-summary module-simplify span-12 initiative'>";
		box += "<h3><span class='color-dark initiative-title'>" + content.Name + "</span></h3>";
		box += content.htmlContent;
		box += "</div>";
		boxes.push(box);
	}

	var _draw = function() {
		boxes.forEach(function(element) {
			$('#report').append(element);
		});
		_setControllerInfo();
	}

	var _setControllerInfo = function() {
		var $currentControllers = $('[id$="container"]:not(.hide) [name^="select"] option:selected');
		controllerText = [];
		$currentControllers.each(function() {
			if ($(this).parents('#region-branch').hasClass('hide')) {
				return true;
			}
			controllerText.push('<strong>' + $(this).parent().attr('data-controller-name') + '</strong>: ' + $(this).text());
		});
		$controllerInfo.append(controllerText.join(', '));
	}

	var _empty = function() {
		boxes = [];
		controllerText = [];
		$report.empty();
		$controllerInfo.empty();
	}

	var _hide = function() {
		$report.addClass('hide');
	}

	var _unhide = function() {
		$report.removeClass('hide');
	}

	return {
		reset: _empty,
		hide: _hide,
		unhide: _unhide,
		addTimelineBox: _addTimelineBox,
		makeBox: _addSummaryBox,
		render: _draw
	}
})(resources);

var controller = (function(gui, resources) {
	var _initiativeListUrl = '/data/initiatives';
	var _initiativesQuarterlyUrl = '/data/initiative-quarterly';
	var _quarterListUrl = '/data/quarters';
	var _regionListUrl = '/data/regions';
	var _branchListUrl = '/data/branches';
	var _selectInitiativeTimeline = $('#select-initiative-timeline');
	var _selectQuarterTimeline = $('#select-quarter-timeline');
	var _selectInitiative = $('#select-initiative');
	var _selectRegion = $('#select-region');
	var _selectBranch = $('#select-branch');

	var _addQuarterOption = function(object) {
		var startDate = moment(object.Start, apiReturnDateFormat);
		// add Teresa's quarter bypass option via hard coding
		startDate = moment('04/01/2014', apiReturnDateFormat);
		var endDate = moment(object.End, apiReturnDateFormat);
		var startQuarter = getQuarterData(startDate.month()+1, 
			startDate.year());
		var endQuarter = getQuarterData(endDate.month()+1, 
			endDate.year());
		var textToDisplay = [];
		var valueToHold = [];
		for (var month = startQuarter[0], year = startQuarter[1];
			year <= endQuarter[1]; year++) {
			endMonth = year == endQuarter[1] ? endQuarter[0] : 4;
			for (var i = month; i <= endMonth; i++) {
				textToDisplay.push(resources.get('report-fiscal-year') + 
					year + '-' + (year + 1) + ' ' + resources.get('report-quarter-prefix') + i);
				var value = {quarter: i, year: year, culture: culture};
				valueToHold.push(JSON.stringify(value));
			}
			// set it to 1 because after the first iteration, the quarters should start on a new FY
			month = 1;
		}
		for (var i = 0; i < textToDisplay.length; i++) {
			var $option = $('<option></option>');
			$option.text(textToDisplay[i]);
			$option.attr('data-value', valueToHold[i]);
			$('#select-quarter').append($option);
		}
	}

	var _addOption = function(optionObject, idSuffix, optionalText) {
		var text = culture == 'en-ca' ? optionObject.NameE : optionObject.NameF;
		var value = optionObject.ID === undefined ? text : optionObject.ID;
		text = optionalText == null ? text : optionalText;
		var $option = $('<option></option>');
		$option.attr('value', value);
		$option.text(text);
		$option.attr('data-delete', 'true');
		$('#select-' + idSuffix).append($option);
	}

	var _setupInitiativeReportControllers = function(data) {
		data = JSON.parse(data);
		dataFactory.setInitiativeReport(data);
		data.forEach(function(initiativeBlock) {
			_setupTimelineController(initiativeBlock, 'initiative-timeline');
		});
	}

	var _setupTimelineController = function(object, idSuffix) {
		var translatedTimelineName = resources.get(object['NameE']);
		_addOption(object, idSuffix, translatedTimelineName);
	}

	var _clearControllerOptions = function(idSuffix) {
		var $options = $('#select-' + idSuffix).children('option');
		$options.each(function() {
			if ($(this).attr('data-delete') == 'true')
				$(this).remove();
		});
	}

	var _populateInitiativeController = function() {
		_clearControllerOptions('initiative');
		var currentBlock = dataFactory.getInitiativeReport().filter(function(obj) {
			var timeline = _selectInitiativeTimeline.val();
			if (timeline == 'All') return true;
			var propertyCulture = culture == 'en-ca' ? 'NameE' : 'NameF';
			return timeline == obj[propertyCulture];
		});
		currentBlock.forEach(function(block) {
			block.Data.forEach(function(data) {
				_addOption(data, 'initiative');
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

	var _setRegionBranchDefault = function() {
		$('#select-region').val('0');
		$('#select-branch').val('0');
	}

	var _registerEvents = function() {
		$('#radio-report-initiative').on('click', function() {
			gui.reset();
			_selectInitiative.val('0');
			$('#initiative-container').removeClass('hide');
			$('#quarter-container').addClass('hide');
			$('#report-title').text(resources.get('report-print-initiative-title'));
			$('#report-subtitle').text($('<div/>').html(resources.get('application-name')).text());
		});
		$('#radio-report-quarterly').on('click', function() {
			gui.reset();
			_selectQuarterTimeline.val('0');
			$('#region-branch').addClass('hide');
			_setRegionBranchDefault();
			$('#quarter-container').removeClass('hide');
			$('#initiative-container').addClass('hide');
			$('#report-title').text(resources.get('report-print-quarterly-title'));
			$('#report-subtitle').text($('<div/>').html(resources.get('application-name')).text());
		});
		$('#select-quarter-timeline').on('change', function() {
			gui.reset();
			if ($(this).val() != 0 && $(this).val() != 'BP2020') {
				$('#region-branch').removeClass('hide');
			} else {
				$('#region-branch').addClass('hide');
				_setRegionBranchDefault();
			}
		});
		$('#select-initiative-timeline').on('change', function() {
			gui.reset();
			if ($(this).val() != 0) {
				$('#initiative').removeClass('hide');
				_selectInitiative.val('0');
				_populateInitiativeController();
			} else {
				$('#initiative').addClass('hide');
			}
		});
		$('#select-quarter').on('change', function() {
			gui.reset();
			$('#region-branch').addClass('hide');
			_selectQuarterTimeline.val('0');
			_setRegionBranchDefault();
			_clearControllerOptions('quarter-timeline');
			if ($(this).val() != 0) {
				_quarterData = $(this).find('option:selected').data('value');
				callPostAPI(_initiativesQuarterlyUrl, _setQuarterReportDataset,
					function () { _toggleLoading('quarter-timeline'); }, _quarterData);
			}
		});
		$('#print-button').on('click', function() {
			if (_confirmControllerStates()) {
				window.print();
			} else {
				alert(resources.get('report-error-no-selection'));
			}
		});
	}

	var _confirmControllerStates = function() {
		var states = _controllerStates();
		if (states.reportType[0] === true || states.reportType[1] === true) {
			return states.reportState.every(function(elem, idx, arr) {
				return elem === true;
			});
		}
		return false;
	}

	var _controllerStates = function() {
		var reportTypeState = [
			document.querySelector('#radio-report-quarterly').checked,
			document.querySelector('#radio-report-initiative').checked
		]
		var reportState;		
		if (reportTypeState[0] === true) {
			// Quarterly report is selected
			reportState = [
				document.querySelector('#select-quarter').value != 0,
				document.querySelector('#select-quarter-timeline').value != 0
			]
		} else if (reportTypeState[1] === true) {
			// Initiative report is selected
			reportState = [
				document.querySelector('#select-initiative-timeline').value != 0,
				document.querySelector('#select-initiative').value != 0
			]
		}

		return {
			reportType: reportTypeState,
			reportState: reportState
		}
	}

	var _setQuarterReportDataset = function(data) {
		data = JSON.parse(data);
		dataFactory.setQuarterlyReport(data);
		data.forEach(function(initiativeBlock) {
			_setupTimelineController(initiativeBlock, 'quarter-timeline');
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
		initiative: $('#select-initiative'),
		quarter: $('#select-quarter'),
		region: $('#select-region'),
		branch: $('#select-branch'),
		quarterTimeline: $('#select-quarter-timeline'),
		initiativeTimeline: $('#select-initiative-timeline')
	}
})(gui, resources);

var contentGenerator = (function(resources) {
	var _cultureDataAppend = culture == 'en-ca' ? 'E' : 'F';

	var _removeLink = function(text) {
		return text.replace(/<a.*<\/a>/gi, '');
	}

	var _createInitiativeContent = function(initiative) {
		var content = [];
		var heading = [resources.get('description'), resources.get('timespan'),
						resources.get('milestones'), resources.get('training-plural')];

		// Create description
		var description = "<p>" + 
			initiative["Description" + _cultureDataAppend] + "</p>";
		// description = _removeLink(description);
		// Create timespan
		var startDate = moment(initiative.StartDate, apiReturnDateFormat);
		startDate = getQuarter(startDate.month()+1, startDate.year());
		var endDate = moment(initiative.EndDate, apiReturnDateFormat);
		endDate = getQuarter(endDate.month()+1, endDate.year());
		var timespan = startDate + " " + resources.get('to') + " " + endDate;
		timespan = "<p>" + timespan + "</p>";

		// Create milestones & training
		var milestones = "<ul>";
		var training = "<ul>";
		var events = initiative.Events;
		var milstoneCount = 0;
		var trainingCount = 0;
		if (events.length > 0) {
		    events.forEach(function (event, i) {
				if (event["Hover"+_cultureDataAppend] === null) return;
		    	var dateStr = moment(event.Date, apiReturnDateFormat);
		    	dateStr = dateStr.format('LL');
		        var hoverText = event["Hover" + _cultureDataAppend] == null ? "" : event["Hover" + _cultureDataAppend];
		        var longText = event["Text" + _cultureDataAppend] == null ? "" : event["Text" + _cultureDataAppend];
		        var text = longText.length > 0 ? longText : hoverText;
		        if ((/milestone/gi).test(event.Type)) {
		            milestones += "<li>" + dateStr + "<br>" + text + "</li>";
		            milstoneCount++;
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
		    content[i] = "<h3 class='heading'>" + heading[i] + "</h3>" + content[i];
		}
		return {
			Name: initiative["Name" + _cultureDataAppend], 
			htmlContent: content.filter(function (n) { 
				return n != ""
			}).join("")
		};
	}

	var _createTimelineContent = function(timeline) {
		var content = '';
		var startDate = moment(timeline.StartDate, apiReturnDateFormat);

		// invariant: There will be 3 tables (3 months in 1 quarter)
		for (var i = 0; i < 3; i++, startDate.add(1, 'month')) {
			var tableHeader = _getLocaleMonthAndYear(startDate);
			var correspondingMonthEvents = timeline.Data.filter(function(elem, idx, arr) {
				var eventDate = moment(elem['Date'], apiReturnDateFormat);
				return eventDate.month() == startDate.month();
			});
			if (correspondingMonthEvents.length == 0) continue;

			// dynamically generate table
			content += "<table class='span-12 timeline-table'>";

			// table header
			content += "<thead>";
			content += _generateTableRow(_generateTableHeader(tableHeader, 4));
			content += _generateTableRow(
				_generateTableHeader(resources.get('date')) + 
				_generateTableHeader(resources.get('initiative')) + 
				_generateTableHeader(resources.get('type')) +
				_generateTableHeader(resources.get('description')));
			content += "</thead>"

			// table body
			content += "<tbody>";
			correspondingMonthEvents.forEach(function(elem, idx, arr) {
				var shortDescription = elem['Hover' + _cultureDataAppend] == null ? "" : elem["Hover" + _cultureDataAppend];
				var longDescription = elem['Text' + _cultureDataAppend] == null ? "" : elem["Text" + _cultureDataAppend];;
				var description = longDescription.length > 0 ? longDescription : shortDescription;
				var eventType = elem['Type'].toLowerCase();
				eventType = resources.get(eventType);
				content += _generateTableRow(
					_generateTableCell(moment(elem['Date'], apiReturnDateFormat).format('LL')) +
					_generateTableCell(elem['InitiativeName' + _cultureDataAppend]) +
					_generateTableCell(eventType) +
					_generateTableCell(description))
			});
			content += "</tbody>";

			// end of dynamically generated table
			content += "</table>";
		}

		var unusedInitiatives = [];
		if (timeline.Data.length == 0) {
			unusedInitiatives = timeline.Initiatives;
		} else {
			var temp = [];
			timeline.Data.forEach(function(elem, idx, arr) {
				temp.push(elem.InitiativeID);
			});
			unusedInitiatives = timeline.Initiatives.filter(function(elem, idx, arr) {
				for (var i = 0; i < temp.length; i++) {
					if (temp[i] == elem.ID) return false;
				}
				return true;
			});
		}
		
		if (unusedInitiatives.length > 0) {
			content += "<h4>";
			content += resources.get('unused-initiatives');
			content += "</h4>";

			// dynamically generate table
			content += "<table class='span-12 unused-initiatives'>";

			// table header
			content += "<thead>";
			content += _generateTableRow(_generateTableHeader(resources.get('initiative')));
			content += "</thead>";

			// table body
			content += "<tbody>";
			unusedInitiatives.forEach(function(elem, idx, arr) {
				content += _generateTableRow(_generateTableCell(elem['Name' + _cultureDataAppend]));
			});
			content += "</tbody>";

			// end of dynamically generated table
			content += "</table>";	
		}

		return {
			Name: resources.get(timeline['NameE']),
			htmlContent: content
		};
	}

	var _generateTableHeader = function(content, colspan) {
		colspan = colspan ? colspan : '';
		return '<th colspan=' + colspan + '>' + content + '</th>';
	}

	var _generateTableCell = function(content) {
		return '<td>' + content + '</td>'; 
	}

	var _generateTableRow = function(content) {
		return '<tr>' + content + '</tr>';
	}

	var _getLocaleMonthAndYear = function(momentObject) {
		return _getLocaleMonthName(momentObject) + " " + _getYear(momentObject);
	}

	var _getLocaleMonthName = function(momentObject) {
		return moment.localeData()._months[momentObject.toObject().months];
	}

	var _getYear = function(momentObject) {
		return momentObject.toObject().years;
	}

	return {
		createInitiative: _createInitiativeContent,
		createTimeline: _createTimelineContent
	}
})(resources);

var dataFactory = (function() {
	var _protectedInitiativeReportDataset;
	var _filteredInitiativeReportDataset;
	var _contentInitiativeReportDataset;

	var _protectedQuarterlyReportDataset;
	var _filteredQuarterlyReportDataset;
	var _contentQuarterlyReportDataset;

	var _regionBranchKey = '';
	var _regionBranchDefault = '0,0';
	var _cultureDataAppend = culture == 'en-ca' ? 'E' : 'F';

	var _getInitiativeReportDataset = function() {
		return _protectedInitiativeReportDataset;
	}

	var _getInitiativeReportContent = function() {
		return _contentInitiativeReportDataset;
	}

	var _setInitiativeReportDataset = function(data) {
		_protectedInitiativeReportDataset = data;
		_filteredInitiativeReportDataset = data;
	}

	var _getQuarterlyReportDataset = function() {
		return _protectedQuarterlyReportDataset;
	}

	var _getQuarterlyReportContent = function() {
		return _contentQuarterlyReportDataset;
	}

	var _setQuarterlyReportDataset = function(data) {
		_protectedQuarterlyReportDataset = data;
		_filteredQuarterlyReportDataset = data;
	}

	var _createInitiativeReportContentDataset = function() {
		_filteredInitiativeReportDataset = _protectedInitiativeReportDataset.filter(function(obj) {
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
	}

	var _createQuarterReportContentDataset = function() {
		var temp = _protectedQuarterlyReportDataset.filter(function(obj) {
			var timeline = controller.quarterTimeline.val();
			if (timeline == 'All') return true;
			var propertyCulture = culture == 'en-ca' ? 'NameE' : 'NameF';
			return timeline == obj[propertyCulture];
		});
		_filteredQuarterlyReportDataset = JSON.parse(JSON.stringify(temp));
		_applyQuarterReportFilter();
	}

	var _applyQuarterReportFilter = function() {
		var regionKey = controller.region.val();
		var branchKey = controller.branch.val();
		_regionBranchKey = regionKey + "," + branchKey;
		_contentQuarterlyReportDataset = JSON.parse(JSON.stringify(_filteredQuarterlyReportDataset));
		_contentQuarterlyReportDataset.forEach(function(block) {
			if (regionKey != '0' && branchKey != '0') {
				block.Data = block.Data.filter(function(elem, idx, arr) {
					return _keyExists(elem, 'Control', _regionBranchKey);
				});
			}
		});
	}

	var _sortComparator = function (a, b) {
		if (a > b) return 1;
		if (a < b) return -1;
		return 0;
	}

	var _keyExists = function(object, objectAccessor, key) {
		return key in object[objectAccessor];
	}

	var _getValueByKey = function(object, objectAccessor, key) {
		return object[objectAccessor][key];
	}

	var _getRegionBranchKey = function() {
		return _regionBranchKey;
	}

	var _registerEvents = function() {
		controller.initiative.on('change', _createInitiativeReportContentDataset);
		controller.quarterTimeline.on('change', _createQuarterReportContentDataset);
		controller.region.on('change', _applyQuarterReportFilter);
		controller.branch.on('change', _applyQuarterReportFilter);
	}

	var _mergeObjects = function(obj1, obj2) {
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
		getInitiativeReportContent: _getInitiativeReportContent,
		setInitiativeReport: _setInitiativeReportDataset,
		getQuarterlyReport: _getQuarterlyReportDataset,
		getQuarterlyReportContent: _getQuarterlyReportContent,
		setQuarterlyReport: _setQuarterlyReportDataset,
		getRegionBranchKey: _getRegionBranchKey
	}
})();

// ----------------------------------------------------------------------------

var quarterlyReport = (function(dataFactory, gui) {
	var _drawReport = function() {
		gui.reset();
		dataFactory.getQuarterlyReportContent().forEach(function(timeline) {
			var content = contentGenerator.createTimeline(timeline);
			gui.addTimelineBox(content);
		});
		gui.render();
		pageNavigator.per('.timeline-table');
		_setPrintReportMoreInfo();
	}

	var _setPrintReportMoreInfo = function() {
		$('#report-more-info').html($('#select-quarter').val() + ' ' + resources.get('report-print-overview'));
	}

	var _registerEvents = function() {
		controller.quarterTimeline.on('change', _drawReport);
		controller.region.on('change', _drawReport);
		controller.branch.on('change', _drawReport);
	}

	var _initialize = (function() {
		_registerEvents();
	})();
	return {
	}
})(dataFactory, gui, resources);

var initiativeReport = (function(dataFactory, gui) {
	var _drawReport = function() {
		gui.reset();
		if (controller.initiative.val() != 0) {
			dataFactory.getInitiativeReportContent().forEach(function(initiative) {
				gui.makeBox(contentGenerator.createInitiative(initiative));
			});
			gui.render();
			pageNavigator.per('.initiative');
			_setPrintReportMoreInfo();
		}
	}

	var _setPrintReportMoreInfo = function() {
		var $selectedOption = $('#select-initiative option:selected');
		var text = $selectedOption.val() == 'All' ?
			resources.get('report-print-all-initiatives') : 
			$selectedOption.text();
		$('#report-more-info').text(text);
	}

	var _registerEvents = function() {
		controller.initiative.on('change', _drawReport);
	}

	var _toggleReportLoading = function() {
		$('#icon-report-loading').toggleClass('hide');
	}

	var _initialize = (function() {
		_registerEvents();
	})();
})(dataFactory, gui, resources);

// ----------------------------------------------------------------------------

var pageNavigator = (function() {
	var _arrayOfScrollPosition = [];
	var _arrayRelativeIndex = 0;
	var _domToScroll = null;

	var _currentScrollPosition = function() {
		return document.documentElement.scrollTop || document.body.scrollTop;
	}

	var _similarValue = function(a, b) {
		return Math.abs(a-b) < 10 ? true : false;
	}

	var _jumpToNextScrollPosition = function() {
		if (_similarValue(_arrayOfScrollPosition[_arrayOfScrollPosition.length-1],
		 	_currentScrollPosition())) return false;
		for (var i = 0; i < _arrayOfScrollPosition.length; i++) {
			if (_arrayOfScrollPosition[i] > _currentScrollPosition()) {
				if (_similarValue(_arrayOfScrollPosition[i], _currentScrollPosition())) {
					continue;
				}
				window.scrollTo(0, _arrayOfScrollPosition[i]);
				break;
			}
		}
		return true;
	}

	var _jumpToPrevScrollPosition = function() {
		if (_similarValue(_arrayOfScrollPosition[0], _currentScrollPosition())) return false;
		for (var i = _arrayOfScrollPosition.length; i >= 0; i--) {
			if (_arrayOfScrollPosition[i] < _currentScrollPosition()) {
				if (_similarValue(_arrayOfScrollPosition[i], _currentScrollPosition())) {
					continue;
				}
				window.scrollTo(0, _arrayOfScrollPosition[i]);
				break;
			}
		}
		return true;
	}

	var _relativePositionTo = function(elem, d) {
		var acceptedArguments = ['top', 'bottom'];
		if (acceptedArguments.indexOf(d) > -1)
			return elem.getBoundingClientRect()[d] + window.pageYOffset;
		return 0;
	}

	var _populateScrollArray = function() {
		_arrayOfScrollPosition = Array.prototype.map.call(_domToScroll, function(elem) {
			return _relativePositionTo(elem, 'top');
		});
	}

	var _reset = function() {
		_arrayOfScrollPosition = [];
		_domToScroll = null;
		_arrayRelativeIndex = 0;
	}

	var _registerKeys = function() {
		document.addEventListener('keydown', _keyMap, false);
	}

	var _keyMap = function(e) {
		// 33 == page up
		// 34 == page down
		switch (e.keyCode) {
			case 33:
				if (_jumpToPrevScrollPosition())
					e.preventDefault();	
				break;
			case 34:
				if (_jumpToNextScrollPosition())
					e.preventDefault();
			default:
				return;
		}
	}

	var _initialize = function(selectorToScroll) {
		_reset();
		_domToScroll = document.querySelectorAll(selectorToScroll);
		if (_domToScroll.length == 0)
			return 'There are no elements that have "' + selectorToScroll + '" selector.';
		else {
			_populateScrollArray();
			_registerKeys();
			return _arrayOfScrollPosition;
		}
	}
	return {
		per: _initialize,
		next: _jumpToNextScrollPosition,
		prev: _jumpToPrevScrollPosition
	}
})();
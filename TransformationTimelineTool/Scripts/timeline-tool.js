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
	return {
		ajax: _ajax,
		jsonCopy: _jsonCopy,
		htmlDecode: _htmlDecode,
		htmlEncode: _htmlEncode,
		sortComparator: _sortComparator
	}
})($);

var timeline = (function(r, $, m, h) { // r => resources, $ => jQuery, m => moment, h => helpers
	var CULTURE = window.location.href.indexOf('lang=fra') > -1 ? 'fr-ca' : 'en-ca',
		CULTURE_APPEND = CULTURE == 'en-ca' ? 'E' : 'F',
		API_DATE_FORMAT = 'MM/DD/YYYY',
		GET = 'get', POST = 'post';
	m.locale(CULTURE);

	var dataManager = (function($, h, ui) { // $ => jQuery, h => helpers, ui => ui
		var TIMELINES_URL = '/data/all', 
			REGIONS_URL = '/data/regions',
			BRANCHES_URL = '/data/branches';
		var TIMELINES, REGIONS, BRANCHES;
		var timelines, regions, branches;
		var loaded = false;

		var _prepareData = function() {
			_prepareRegions();
			_prepareBranches();
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

		var _state = function(v) {
			switch (v) {
				case 0: return timelines;
				case 1: return regions;
				case 2: return branches;
				default: return timelines;
			}
		}

		var _setLoadState = function() { loaded = true; }
		var _getLoadState = function() { return loaded; }

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
			.then(_setLoadState);
		})();

		return {
			state: _state,
			loaded: _getLoadState
		}
	})($, h, ui)

	var ui = (function(r, h) { // r => resources, h => helpers
		var intro = (function() {
			$container = $('#timeline-tool-introduction-container');
			$introduction = $('#timeline-tool-introduction');
			$iconLegend = $('#timeline-tool-legend');

			var _initialize = (function() {
				$introduction.html(h.htmlDecode(r.About));
				$iconLegend.html(h.htmlDecode(r.IconLegend));
			})();
		})();
	})(r, h);

	return {
		dataManager: dataManager,
		ui: ui
	}
})(resources, jQuery, moment, helpers);

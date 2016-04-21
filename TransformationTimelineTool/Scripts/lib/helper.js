define('helper', ['jquery-private'], function($) {
	'use strict';
		
	// constants
	var VERSION = '1.0.0';
	
	// helper functions
	var _ajax = function (type, url, callback, jsonData) {
	    if (arguments.length == 3) {
	        return $.ajax({
	            type: type,
	            url: url,
	            cache: false,
	            success: function (data) {
					callback(JSON.stringify(data));
				},
	            error: function (jqXHR, textStatus, errorThrown) {
					console.log(JSON.stringify(jqXHR) + ': ' + textStatus);
				}
	        });
		} else if (arguments.length == 4) {
			return $.ajax({
	            type: type,
	            contentType: 'application/json',
	            url: url,
	            cache: false,
	            data: JSON.stringify(jsonData),
	            success: function (data) {
					callback(JSON.stringify(data));
				},
	            error: function (jqXHR, textStatus, errorThrown) {
					console.log(JSON.stringify(jqXHR) + ': ' + textStatus);
				}
	        });
		} else {
			console.log('helper.ajax: expects 3 or 4 arguments');
			console.log('arguments length is ' + arguments.length);
		}
	};
	
	var _getValueByKey = function(key, object) {
		if (_keyExists(key, object)) {
			return object[key];
		}
	};
	
	var _getVersion = (function() {
		return VERSION;
	})();
	
	var _htmlDecode = function(value) {
		return $('<div/>').html(value).text();
	};
	
	var _htmlEncode = function(value) {
		return $('<div/>').text(value).html();
	};
	
	var _jsonCopy = function(jsonObj) {
		return JSON.parse(JSON.stringify(jsonObj));
	};
	
	var _log = function(message, output) {
		if (!output) return;
		if (typeof message === 'obj') {
			message = JSON.parse(JSON.stringify(message));	
		}
		console.log(message);
	};
	
	var _keyExists = function(key, object) {
		return key in object;
	};
	
	var _negate = function(number) {
		return -Math.abs(number);
	};
	
	var _sortComparator = function (a, b) {
	    if (a > b) return 1;
	    if (a < b) return -1;
	    return 0;
	};
	
	var _truncate = function(str, limit) {
		return str.length >= limit ? str.substring(0, limit) + '...' : str;
	};
	
	return {
		ajax: _ajax,
		getValueByKey: _getValueByKey,
		htmlDecode: _htmlDecode,
		htmlEncode: _htmlEncode,
		jsonCopy: _jsonCopy,
		log: _log,
		keyExists: _keyExists,
		negate: _negate,
		sortComparator: _sortComparator,
		truncate: _truncate,
		version: _getVersion
	}
}, function(err) {
    console.log(err.requireModules);
});

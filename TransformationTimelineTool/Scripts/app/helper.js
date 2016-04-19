"use strict";

var helper = (function($) {
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
	var loadJavaScript = function(dir, callback) {
		var head = document.getElementByTagName('head')[0];
		var script = document.createElement('script');
		script.type = 'text/javascript';
		script.src = url;
		script.onreadystatechange = callback;
		script.onload = callback;
		head.appendChild(script);
	}

	return {
		ajax: _ajax,
		jsonCopy: _jsonCopy,
		htmlDecode: _htmlDecode,
		htmlEncode: _htmlEncode,
		sortComparator: _sortComparator,
		truncate: _truncate,
		keyExists: _keyExists,
		loadJavaScript: _loadJavaScript
	}
})(jQuery);
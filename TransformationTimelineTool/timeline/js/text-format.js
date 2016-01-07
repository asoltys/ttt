function xmlProcessor(URL) {
	this.url = URL;
	this.xmlData = null;
	this.html = "";

	this.fetchXML = function() {
		var that = this;
		return $.ajax({
			url: this.url,
			async: false,
			type: 'GET',
			dataType: 'xml',
			error: function() {
				console.log("XML data not available.");
			},
			success: function(data) {
				that.xmlData = $(data);
				that.parseXML();
			}
		});
	};

	this.parseXML = function() {
		var header = ["Initiative", "Milestone/Class", "ID", "Details"]
		this.html += "<table><thead class='header-style'><tr>";
		for (var i = 0; i < header.length; i++) {
			this.html += this.wrapATag(header[i], "th");
		}
		this.html += "</tr></thead><tbody>";
		var that = this;
		this.xmlData.findEach('initiative ',function() {
			var name_e = $(this).findElementText('name_e');
			var milestone_count = 0;
			var class_count = 0;
			var events = $(this).find('events').children();
			$(events).each(function() {
				var id = $(this).findElementText('id');
				var eventType = $(this).prop("tagName");
				var date = $(this).findElementText('date');
				date = sqlToJsDate(date);
				date = date.getFullYear() + "/" + date.getMonth() + "/" + date.getDate();
				var text_e = $(this).findElementText('text_e');
				text_e = text_e.replace(/(”|“)/g, '"');

				if (eventType.indexOf("class") > -1) {
					that.html += "<tr class='class-style'>";
				} else {
					that.html += "<tr class='milestone-style'>";
				}
				that.html += that.wrapATag(name_e, "td");
				that.html += that.wrapATag(eventType, "td");
				that.html += that.wrapATag(id, "td");
				that.html += that.wrapATag(text_e, "td");
				that.html += "</tr>";
			});
		});
		that.html += "</tbody></table>";
	};

	this.appendHTML = function() {
		$("body").append(this.html);
	};

	this.wrapATag = function(str, tag) {
		return "<" + tag + ">" + str + "</" + tag + ">";
	};
}


function sqlToJsDate(sqlDate){
    //sqlDate in SQL DATETIME format ("yyyy-mm-dd hh:mm:ss.ms")
    var sqlDateArr1 = sqlDate.split("-");
    //format of sqlDateArr1[] = ['yyyy','mm','dd hh:mm:ms']
    var sYear = sqlDateArr1[0];
    var sMonth = (Number(sqlDateArr1[1]) - 1).toString();
    var sqlDateArr2 = sqlDateArr1[2].split(" ");
    //format of sqlDateArr2[] = ['dd', 'hh:mm:ss.ms']
    var sDay = sqlDateArr2[0];
    var sqlDateArr3 = sqlDateArr2[1].split(":");
    //format of sqlDateArr3[] = ['hh','mm','ss.ms']
    var sHour = sqlDateArr3[0];
    var sMinute = sqlDateArr3[1];
    var sqlDateArr4 = sqlDateArr3[2].split(".");
    //format of sqlDateArr4[] = ['ss','ms']
    var sSecond = sqlDateArr4[0];
    var sMillisecond = sqlDateArr4[1];
    return new Date(sYear,sMonth,sDay,sHour,sMinute,sSecond,sMillisecond);
};

(function($){
  $.fn.findElementText = function(elementName){
    return this.find(elementName).text();
  }
  $.fn.findEach = function(element, callBack){
    return this.find(element).each(callBack);
  }
})(jQuery)


$(document).ready(function() {
	var URL = "production_data.xml";
	var processor = new xmlProcessor(URL);
	processor.fetchXML();
	processor.appendHTML();
});
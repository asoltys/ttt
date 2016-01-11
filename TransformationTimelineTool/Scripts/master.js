// ############################################################################
// Debug area at the top setting
// ############################################################################
var debug = false;
var textAreaHeight = 300;

if (debug) {
	$("#wb-body").css("margin-top", textAreaHeight);
	createTextAreaContainers(2);
}

function createTextAreaContainers(debugTextAreaCount) {
	for (var i = 0; i < debugTextAreaCount; i++) {
		var domContainerID = "textarea_container_" + i;
		var domContainer = "<div id='" + domContainerID + "'></div>";
		if (i > 1) {
			var previousDOMContainerID = "textarea_container_" + (i - 1);
			$("#" + previousDOMContainerID).after(domContainer);
		} else {
			$("body").prepend(domContainer);
		}

		var width = Math.round(100 / debugTextAreaCount);

		var div = $("#" + domContainerID);
		div.css("position", "absolute");
		div.css("width", width + "%");
		div.css("left", width * i + "%");
		div.css("height", textAreaHeight);
		$("body").css("top", textAreaHeight);
		div.css("border", "1px solid black");

		var textarea = "<textarea id=textarea_" + i + " readonly></textarea>";
		div.prepend(textarea);

		textarea = $("#textarea_" + i);
		textarea.css("padding", 0);
		textarea.css("width", "100%");
		textarea.css("height", "100%");
		textarea.css("overflow-y", "scroll");
		textarea.css("resize", "none");
	}
}

function log(DOMid, str) {
	if (!debug) {
		if (typeof(DOMid) === 'object'
		 || !!document.getElementById(DOMid)) console.log(DOMid);
		else console.log(str);
		return false
	};
	if (document.getElementById(DOMid)) var dom = $("#" + DOMid);
	else return false;
	var text = dom.text();
	var newLineCount = text.match(/\n/gi) !== null ?
		text.match(/\n/gi).length : 0;
	str = typeof(str) === 'object' ? JSON.stringify(str) : str;
	if (text.length > 0) text += "\n" + (newLineCount + 1) + " → " + str;
	else text = newLineCount + " → " + str;
	dom.text(text);
}

// ############################################################################
// Main area
// ############################################################################

// debug area variables
var cons1 = "textarea_0";
var cons2 = "textarea_1";

// data URL local vs remote
var local = true;

// data URLs
var initiativesURL  = local ? "/en/initiatives/data" : "http://win-atl-v001.ncr.pwgsc.gc.ca:8200/en/initiatives/Data";
var regionsURL 		= local ? "/en/regions/data" 	: "http://win-atl-v001.ncr.pwgsc.gc.ca:8200/en/regions/Data"; 
var branchesURL		= local ? "/en/branches/data" 	: "http://win-atl-v001.ncr.pwgsc.gc.ca:8200/en/branches/Data";

// data variables
var initiatives;
var regionKey = 0;
var branchKey = 0;
var showAllRegionKey;
var showAllBranchKey;

// HTML variables
var regionController;
var branchController;

// Lock JS using Deferred object
var lock;

function getJSON(url, successCallback) {
	if (local) {
		$.ajax({
			type: "GET",
			url: url,
			success: function(data) {
				successCallback(JSON.stringify(data));
			},
			error: function(jqXHR, textStatus, errorThrown) {
				log(cons2, JSON.stringify(jqXHR) + ", " + textStatus);
			}
		});
	} else {
		$.ajax({
			type: "GET",
			url: url,
			dataType: "json",
			timeout: 5000,
			success: function(data) {
				successCallback(JSON.stringify(data));
			},
			error: function(jqXHR, textStatus, errorThrown) {
				log(cons2, JSON.stringify(jqXHR) + ", " + textStatus);
			}
		});
	}
}

function populateControllers() {
	regionController = $("#region_controller");
	branchController = $("#branch_controller");
	if (regionController.length == 0 || branchController.length == 0)
		throw { name: "NoDOM", message: "Controllers are missing" };

	getJSON(regionsURL, function(data) {
		regions = JSON.parse(data);
		for (var k in regions) {
			if (regions[k].NameE == "All") showAllRegionKey = regions[k].ID;
			var option = "<option id=" + regions[k].ID + ">" + regions[k].NameE + "</option>";
			regionController.append(option);
		}
	});
	
	getJSON(branchesURL, function(data) {
		branches = JSON.parse(data);
		for (var k in branches) {
			if (branches[k].NameE == "All") showAllBranchKey = branches[k].ID;
			var option = "<option id=" + branches[k].ID + ">" + branches[k].NameE + "</option>";
			branchController.append(option);
		}
	})
}

function getInitiatives() {
	lock = new $.Deferred();
	getJSON(initiativesURL, function(data) {
		initiatives = JSON.parse(data);
		lock.resolve();
	});
	return lock;
}

var accordionCount = 0;
function addAccordion(title, content) {
	accordionCount++;
	var accordion = "<div class='accordion-section'>";
	accordion 	 += "<a class='accordion-section-title' href='#accordion-"+accordionCount+"'>&#9654;&nbsp;" + title + "</a>";
	accordion 	 += "<div id='accordion-"+accordionCount+"' class='accordion-section-content closed'>";
	accordion 	 += content;
	accordion 	 += "</div></div>";
	$("#timeline_accordion").append(accordion);
}

function createShowControl(index, initiativeParam) {
	var level 		 = 0;
	var impacts 	 = initiativeParam.Impacts;
	var events		 = initiativeParam.Events;

	// Serialize the IDs for each initiative
	var controlArray = [];
	$.each(impacts, function(idx, impact) {
		$.each(impact.Regions, function(idx, region) {
			$.each(impact.Branches, function(idx, branch) {
				controlArray.push(region + "," + branch);
			});
		});
	});
	controlArray = controlArray.length > 0 ? controlArray : ["0,0"];
	initiatives[index]["showControl"] = controlArray;

	// Serialize the IDs for each event
	$.each(events, function(idx, eventParam) {
		controlArray = [];
		if (eventParam.Regions.length == 1) {
			if (eventParam.Regions[0] == showAllRegionKey) {
				for (var i = 1; i <= showAllRegionKey; i++) {
					eventParam.Regions[i-1] = i;
				}
			}
		}
		if (eventParam.Branches.length == 1) {
			if (eventParam.Branches[0] == showAllBranchKey) {
				for (var i = 1; i <= showAllBranchKey; i++) {
					eventParam.Branches[i-1] = i;
				}
			}
		}
		$.each(eventParam.Regions, function(idx, region) {
			$.each(eventParam.Branches, function(idx, branch) {
				controlArray.push(region + "," + branch);
			});
		});
		controlArray = controlArray.length > 0 ? controlArray : ["0,0"];
		initiatives[index]["Events"][idx]["showControl"] = controlArray;
	});
}

function setupInitiatives() {
	$.each(initiatives, function(idx, initiativeParam) {
		createShowControl(idx, initiativeParam);
	});
}

function getControlKey(key1, key2) {
	return (key1 + "," + key2);
}

function formatDate(format, date) {
	try {
		return $.datepicker.formatDate(format, new Date(date));
	} catch (e) {
		throw {
			name: "Dateformat Error",
			message: "Date passed could not be formatted"
		}
	}
}

function isDate(date) {
	return !(/NaN/g).test(date);
}

function createContent(initiativeParam) {
	var content = [];
	var heading = ["Timespan", "Key milestones", "Training", "Description"];

	// Create timespan
	var startDate 	= initiativeParam["StartDate"];
	var endDate 	= initiativeParam["EndDate"];
	if (startDate.length == 0 || endDate.length == 0) {
		throw {
			name: "JSON Parse Error",
			message: "Dates could not be parsed correctly"
		}
	}
	var timespan = formatDate('MM, yy', startDate) + " - " + formatDate('MM, yy', endDate);
	
	// Create milestones & training
	var milestones 		= "<ul>";
	var training 		= "<ul>";
	var events 			= initiativeParam["Events"];
	var milstoneCount	= 0;
	var trainingCount	= 0;
	if (events.length > 0) {
		$.each(events, function(idx, eventParam) {
			var control = eventParam["showControl"];
			for (var i = 0; i < control.length; i++) {
				if (control[i] == getControlKey(regionKey, branchKey)) {
					var dateStr	  = formatDate('MM dd, yy', eventParam["Date"]);
					var eventTime = isDate(dateStr) ? dateStr : eventParam["Date"];
					if ((/milestone/gi).test(eventParam["Type"])) {
						var text 	= eventParam["TextE"] == null ? "" : eventParam["TextE"];
						milestones += "<li>" + eventTime + ": " + text + "</li>";
						milstoneCount++
					} else if (false) {
					} else {
						var text 	= eventParam["TextE"] == null ? "" : eventParam["TextE"];
						training += "<li>" + eventTime + ": " + text + "</li>";
						trainingCount++;
					}
					break;
				}
			}
		})
	}
	milestones 	+= "</ul>";
	training 	+= "</ul>";
	milestones 	 = milstoneCount == 0 ? "" : milestones;
	training 	 = trainingCount == 0 ? "" : training;

	// Create description
	var description = initiativeParam["DescriptionE"];

	// Create last minute format changes and overall content 
	timespan = "<p>" + timespan + "</p>";
	content.push(timespan, milestones, training, description);
	for (var i = 0; i < content.length; i++) {
		if (content[i].length == 0) continue;
		content[i] = "<h3>" + heading[i] + "</h3>" + content[i];
	}
	return content.filter(function(n){ return n != ""}).join("");
}

function filterData() {
    if (initiatives) {
        try {
            $.each(initiatives, function (idx, initiativeParam) {
                var control = initiativeParam["showControl"];
                for (var i = 0; i < control.length; i++) {
                    if (control[i] == getControlKey(regionKey, branchKey)) {
                        var content = createContent(initiativeParam);
                        addAccordion(initiativeParam["NameE"], content);
                        break;
                    }
                }
            });
        } catch (e) {
            log(cons1, e.name + ": " + e.message);
        }
    }
}

function toggleAccordion(elem) {
	var contentID = elem.attr("href");
	$(contentID).slideToggle();
}

$(window).on("load", function() {
	function main() {
		if (debug) $("body").scrollTop(textAreaHeight);
		try {
			// Initial setup
			populateControllers();
			getInitiatives();
			$.when(lock).done(function () {
				setupInitiatives();
				filterData();
			});

			// Event listeners
			$("#branch_controller, #region_controller").on("change", function() {
				$("#timeline_accordion").empty();
				var selected = $(this).find("option:selected").attr("id");
				regionKey = $(this).attr("id") == "region_controller" ? selected : regionKey;
				branchKey = $(this).attr("id") == "branch_controller" ? selected : branchKey;
				filterData();
			});

			// Delegated Event listener to bind events to dynamic HTML contents
			$("#timeline_accordion").on("click", "a.accordion-section-title", function() {
				toggleAccordion($(this));
			});
		} catch (e) {
			log(cons1, e.name + ": " + e.message);
		}
	}
	main();
});
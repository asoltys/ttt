// data variables
var initiatives;
var regionKey = 0;
var branchKey = 0;
var showAllRegionKey;
var showAllBranchKey;
var regionName;
var branchName;
var currentCulture = window.location.href.indexOf("fr") > -1 ? "fr" : "en";

// data URLs
var initiativesURL = currentCulture == "en" ? "/en/initiatives/datauni" : "/fr/initiatives/datauni";
var regionsURL = "/en/regions/data";
var branchesURL = "/en/branches/data";

// HTML variables
var regionController;
var branchController;

// Lock JS using Deferred object
var lock;

function getJSON(url, successCallback) {
	$.ajax({
		type: "GET",
		url: url,
		success: function(data) {
			successCallback(JSON.stringify(data));
			initCenterSpan();
		},
		error: function(jqXHR, textStatus, errorThrown) {
			console.log(JSON.stringify(jqXHR) + ", " + textStatus);
		}
	});
}

function populateControllers() {
	regionController = $("#region_controller");
	branchController = $("#branch_controller");
	if (regionController.length == 0 || branchController.length == 0)
		throw { name: "NoDOM", message: "Controllers are missing" };

	getJSON(regionsURL, function(data) {
		regions = JSON.parse(data);
		for (var k in regions) {
		    if (regions[k].NameShort == "all") showAllRegionKey = regions[k].ID;
		    regionName = currentCulture == "en" ? regions[k].NameE : regions[k].NameF;
		    var option = "<option id=" + regions[k].ID + ">" + regionName + "</option>";
			regionController.append(option);
		}
	});
	
	getJSON(branchesURL, function (data) {
	    branches = JSON.parse(data);
	    for (var k in branches) {
	        if (branches[k].NameShort == "all") showAllBranchKey = branches[k].ID;
	        branchName = currentCulture == "en" ? branches[k].NameE : branches[k].NameF;
	        var option = "<option id=" + branches[k].ID + ">" + branchName + "</option>";
	        branchController.append(option);
	    }
	});
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
        var monthObj = {
            1: { en: "January", fr: "janvier" },
            2: { en: "February", fr: "février" },
            3: { en: "March", fr: "mars" },
            4: { en: "April", fr: "avril" },
            5: { en: "May", fr: "mai" },
            6: { en: "June", fr: "juin" },
            7: { en: "July", fr: "juillet" },
            8: { en: "August", fr: "août" },
            9: { en: "September", fr: "septembre" },
            10: { en: "October", fr: "octobre" },
            11: { en: "November", fr: "novembre" },
            12: { en: "December", fr: "décembre" }
        };
        var date = date.split("/");
        var day = currentCulture == "en" ? date[1] : date[0];
        var month = currentCulture == "en" ? date[0] : date[1];;
        var year = date[2];
        return monthObj[parseInt(month)][currentCulture] + " " + day + ", " + year;
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
	console.log(endDate);
	
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
						var text 	= eventParam["Text"] == null ? "" : eventParam["Text"];
						milestones += "<li>" + eventTime + ": " + text + "</li>";
						milstoneCount++
					} else if (false) {
					} else {
						var text 	= eventParam["Text"] == null ? "" : eventParam["Text"];
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
	var description = "<p>" + initiativeParam["Description"] + "</p>";

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
                        addAccordion(initiativeParam["Name"], content);
                        break;
                    }
                }
            });
        } catch (e) {
            console.log(e.name + ": " + e.message);
        }
    }
}

function toggleAccordion(elem) {
	var contentID = elem.attr("href");
	$(contentID).slideToggle();
}

$(window).on("load", function() {
	function main() {
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
			console.log(e.name + ": " + e.message);
		}
	}
	main();
});


// Initialize center-span class (calculate children width and set span width)
function initCenterSpan() {
    var elems = $(".center-span");
    if (elems.length > 0) {
        elems.each(function () {
            var children = $(this).children();
            if (children.length > 0) {
                var childrenWidth = 0;
                children.each(function () {
                    childrenWidth += $(this).outerWidth(true);
                });
            }
            $(this).outerWidth(Math.ceil(childrenWidth / 100) * 100);
        });
    }
}
var Initiative = function(data){
  this.data = data;
  this.id = data.ID;
  this.startDate = data.StartDate;
  this.endDate = data.EndDate;
  this.description = data.DescriptionE;
  if (lang == "e"){
		this.name = data.NameE;
		this.description = data.DescriptionE;
  } else if (lang == "f") {
		this.name = data.NameF;
		this.description = data.DescriptionF;
  }
  this.impacts = this.getImpacts(data);
  //this.events = this.$data.findElementText('events');
  //this.impacts = data.Impacts;
  this.events = data.Events;
};

Initiative.prototype.getImpacts = function(){
  var obj = {};

	$(this.data.Impacts).each(function(index,value){
		obj[value.Branch]= value.Level
	});

  return obj;
};

Initiative.prototype.getEvents = function(){
  var obj = {};
  var events = $(this.data).find('events').children();
  var eventArray = [];
  var text, hover;

  if (lang == "e"){
    text = "text_e";
    hover = "hover_e";
  } else if (lang == "f") {
    text = "text_f";
    hover = "hover_f";
  }


  $(events).each(function() {
    var $this;
    $this = $(this);
    obj = {
      "text" : $this.findElementText(text),
      "hover" : $this.findElementText(hover),
      "date" : $this.findElementText('date'),
      "id" : $this.findElementText('id'),
      "branch" : $this.findElementText('branch'),
      "region" : $this.findElementText('region'),
      "type" : $this.prop("tagName")
    }
    eventArray.push(obj);
  });
  return eventArray;
}

Initiative.prototype.generateProjectInfoHtml = function(){
  var html;

  html = "<img class='projectBackGround " +
                "region_" + this.regionNameShort + " " +
                "branch_" + this.branchNameShort + "' " +
              "id='pBG" + this.id + "' " +
              "src='img/white.gif' />"
  html += "<div class='project " +
                  "region_" + this.regionNameShort + " " +
                  "branch_" + this.branchNameShort + "' " +
                "id='p" + this.id + "'>";
  html += "<div class='projectText'>";

  html += "<a href='#'>" + this.name + "</a>";

  $.each(this.impacts, function(branch, impactLevel){
    html += "<span class='impact " + branch + "' style='display: none;'> (" + impactLevel + ")</span>";
  });
  html += "</div></div>";
  return html;
};


Initiative.prototype.generateBar = function(){
  var html;
	var startDateSplit = this.startDate.split("-");
	var endDateSplit = this.endDate.split("-");
  var left = timeLine.getLeft(startDateSplit[2], startDateSplit[1], startDateSplit[0]);
  var right = timeLine.getLeft(endDateSplit[2], endDateSplit[1], endDateSplit[0]) +17; //hack to hold icon
  var branchDropDown = $("#branchDropdown option:selected").val();
  var regionDropDown = $("#regionDropdown option:selected").val();
  var classString;

  html = "<div class='timeline " +
                "region_" + this.regionNameShort + " " +
                "branch_" + this.branchNameShort +"' " +
              "id='t" + this.id + "'>";

  right = right - left;
  html += "<img src='img/white.gif' " +
                "class='bar' " +
                "style='width:" + right + "px; margin-left: " + left + "px;' " +
                "id='b" + this.id + "' />";
  $(this.events).each(function(){
    var date, icon;

		if (lang == "e"){
			this.text = this.TextE;
			this.hover = this.HoverE;
		} else if (lang == "f") {
			this.text = this.TextF;
			this.hover = this.HoverF;
		}

    if(((this.Branch == "all") && (this.Region == "all")) ||
        (this.Region == regionDropDown) ||
        (this.Branch == branchDropDown)){
      switch (this.Type){
        case "Milestone":
          icon = "circle";
          break;
        case "Class":
          icon = "book";
          break;
      }

      date = timeLine.utility.sqlToJsDate(this.Date);
      var dateLocation = timeLine.getLeft(date.getDate(), date.getMonth() + 1, date.getFullYear()) - 5;
      var dateFormated = date.getDate() + "-" + monthFormat[date.getMonth()] + "-" + date.getFullYear();

      if(this.text){
        classString = 'milestoneTarget eventDialog';
      } else {
        classString = 'milestoneTarget';
      }
      html += "<div class='" + classString + "'" +
                  "style='left:" + dateLocation + "px'" +
                  "title='" + this.hover + "'>" +
                "<img src='img/" + icon + ".png'>" +
              "</div>";
      if(this.text != ''){
        html += "<div id='event_" + this.id + "'" +
                    "style='display:none'" +
                    "title='Activity Info'>" + this.text +"</div>";
      }
    }
  });
  html += "</div>";
  return html;
};

Initiative.prototype.removeBar = function(){
  $("#t" + this.id).remove()
};


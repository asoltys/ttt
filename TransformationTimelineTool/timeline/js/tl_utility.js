var timeLine = timeLine || {};

timeLine.utility = timeLine.utility || {};

timeLine.utility = {
  sqlToJsDate : function (sqlDate){

		split_date = sqlDate.split("-");
    return new Date(split_date[0], split_date[1]-1, split_date[2]);
  },
  resetAccordion : function (){
    $('.accordion').text(timeLine.utility.translate("hide"));
  },
  translate : function (entry_code){
    if(lang=="e"){
      return dictionary[entry_code].e;
    } else if(lang=="f"){
      return dictionary[entry_code].f;
    } else {
      return entry_code
    };
  },
  toggleAccordion : function(event){
    var $this = $(this);

    if ($this.text() == timeLine.utility.translate("hide")){
      $this.text(timeLine.utility.translate("show"));
    } else if ($this.text() == timeLine.utility.translate("show")){
      $this.text(timeLine.utility.translate("hide"));
    }

    timeLine.filter('accordion');
  },
  cleanArray : function (array){
    var unique_array = [];

    array = array.sort();
    $.each(array, function(i, el){
      if($.inArray(el, unique_array) === -1) unique_array.push(el);
    });

    return unique_array;
  },
  goLeft : function () {
    $("#timelineContainer").css('marginLeft', "+=" + timeLine.monthWidth);
  },
  goRight : function () {
    $("#timelineContainer").css('marginLeft', "-=" + timeLine.monthWidth);
  }
}

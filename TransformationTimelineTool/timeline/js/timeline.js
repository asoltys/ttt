var curXPos = 0,
	curDown = false;

if(navigator.userAgent.search("MSIE") >= 0){ //check if using IE
	$(document).ready(function(){
		$('#rowOfMonths').css({
			'top': ($(this).scrollTop()+15)
		});
		$('#rowOfMonths').css({
			'left': (211)
		});
	});
	
	$(window).scroll(function(){
		$('#projectColumn').css({
			'left': $(this).scrollLeft()
		});
		$('#rowOfMonths').css({
			'top': ($(this).scrollTop()+15)
		});
	});
	
}else {
	$(window).scroll(function(){
		$('#projectColumn').css({
			'left': $(this).scrollLeft()
		});
		$('#rowOfMonths').css({
			'top': ($(this).scrollTop()+8)
		});
	});	
}	

$(function() {
    $( "#entries" ).draggable({ axis: "x" });
  });

/*$(document).mousedown(function(event) {
	switch(event.which) {
		case 1:
			event.preventDefault();
			curDown = true; 
			curXPos = event.pageX;
			document.body.style.cursor = "all-scroll";
			break;
	}
});
$(document).mouseup(function(event) {
	event.preventDefault();
	curDown = false;
	document.body.style.cursor = "default";
});
$(document).mousemove(function(event) {
	if(curDown == true){
		event.preventDefault();
		window.scrollTo(document.body.scrollLeft + (curXPos - event.pageX), document.body.scrollTop);
	}
});*/
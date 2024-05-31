jQuery(document).ready(function($){
	var timelines = $('.cd-horizontal-timeline'),
		eventsMinDistance = 60;

	(timelines.length > 0) && initTimeline(timelines);

	function initTimeline(timelines) {
		timelines.each(function(){
			var timeline = $(this),
				timelinebook_cwponents = {};
			//cache timeline components 
			timelinebook_cwponents['timelineWrapper'] = timeline.find('.events-wrapper');
			timelinebook_cwponents['eventsWrapper'] = timelinebook_cwponents['timelineWrapper'].children('.events');
			timelinebook_cwponents['fillingLine'] = timelinebook_cwponents['eventsWrapper'].children('.filling-line');
			timelinebook_cwponents['timelineEvents'] = timelinebook_cwponents['eventsWrapper'].find('a');
			timelinebook_cwponents['timelineDates'] = parseDate(timelinebook_cwponents['timelineEvents']);
			timelinebook_cwponents['eventsMinLapse'] = minLapse(timelinebook_cwponents['timelineDates']);
			timelinebook_cwponents['timelineNavigation'] = timeline.find('.cd-timeline-navigation');
			timelinebook_cwponents['eventsContent'] = timeline.children('.events-content');

			//assign a left postion to the single events along the timeline
			setDatePosition(timelinebook_cwponents, eventsMinDistance);
			//assign a width to the timeline
			var timelineTotWidth = setTimelineWidth(timelinebook_cwponents, eventsMinDistance);
			//the timeline has been initialize - show it
			timeline.addClass('loaded');

			//detect click on the next arrow
			timelinebook_cwponents['timelineNavigation'].on('click', '.next', function(event){
				event.preventDefault();
				updateSlide(timelinebook_cwponents, timelineTotWidth, 'next');
			});
			//detect click on the prev arrow
			timelinebook_cwponents['timelineNavigation'].on('click', '.prev', function(event){
				event.preventDefault();
				updateSlide(timelinebook_cwponents, timelineTotWidth, 'prev');
			});
			//detect click on the a single event - show new event content
			timelinebook_cwponents['eventsWrapper'].on('click', 'a', function(event){
				event.preventDefault();
				timelinebook_cwponents['timelineEvents'].removeClass('selected');
				$(this).addClass('selected');
				updateOlderEvents($(this));
				updateFilling($(this), timelinebook_cwponents['fillingLine'], timelineTotWidth);
				updateVisibleContent($(this), timelinebook_cwponents['eventsContent']);
			});

			//on swipe, show next/prev event content
			timelinebook_cwponents['eventsContent'].on('swipeleft', function(){
				var mq = checkMQ();
				( mq == 'mobile' ) && showNewContent(timelinebook_cwponents, timelineTotWidth, 'next');
			});
			timelinebook_cwponents['eventsContent'].on('swiperight', function(){
				var mq = checkMQ();
				( mq == 'mobile' ) && showNewContent(timelinebook_cwponents, timelineTotWidth, 'prev');
			});

			//keyboard navigation
			$(document).keyup(function(event){
				if(event.which=='37' && elementInViewport(timeline.get(0)) ) {
					showNewContent(timelinebook_cwponents, timelineTotWidth, 'prev');
				} else if( event.which=='39' && elementInViewport(timeline.get(0))) {
					showNewContent(timelinebook_cwponents, timelineTotWidth, 'next');
				}
			});
		});
	}

	function updateSlide(timelinebook_cwponents, timelineTotWidth, string) {
		//retrieve translateX value of timelinebook_cwponents['eventsWrapper']
		var translateValue = getTranslateValue(timelinebook_cwponents['eventsWrapper']),
			wrapperWidth = Number(timelinebook_cwponents['timelineWrapper'].css('width').replace('px', ''));
		//translate the timeline to the left('next')/right('prev') 
		(string == 'next') 
			? translateTimeline(timelinebook_cwponents, translateValue - wrapperWidth + eventsMinDistance, wrapperWidth - timelineTotWidth)
			: translateTimeline(timelinebook_cwponents, translateValue + wrapperWidth - eventsMinDistance);
	}

	function showNewContent(timelinebook_cwponents, timelineTotWidth, string) {
		//go from one event to the next/previous one
		var visibleContent =  timelinebook_cwponents['eventsContent'].find('.selected'),
			newContent = ( string == 'next' ) ? visibleContent.next() : visibleContent.prev();

		if ( newContent.length > 0 ) { //if there's a next/prev event - show it
			var selectedDate = timelinebook_cwponents['eventsWrapper'].find('.selected'),
				newEvent = ( string == 'next' ) ? selectedDate.parent('li').next('li').children('a') : selectedDate.parent('li').prev('li').children('a');
			
			updateFilling(newEvent, timelinebook_cwponents['fillingLine'], timelineTotWidth);
			updateVisibleContent(newEvent, timelinebook_cwponents['eventsContent']);
			newEvent.addClass('selected');
			selectedDate.removeClass('selected');
			updateOlderEvents(newEvent);
			updateTimelinePosition(string, newEvent, timelinebook_cwponents);
		}
	}

	function updateTimelinePosition(string, event, timelinebook_cwponents) {
		//translate timeline to the left/right according to the position of the selected event
		var eventStyle = window.getComputedStyle(event.get(0), null),
			eventLeft = Number(eventStyle.getPropertyValue("left").replace('px', '')),
			timelineWidth = Number(timelinebook_cwponents['timelineWrapper'].css('width').replace('px', '')),
			timelineTotWidth = Number(timelinebook_cwponents['eventsWrapper'].css('width').replace('px', ''));
		var timelineTranslate = getTranslateValue(timelinebook_cwponents['eventsWrapper']);

        if( (string == 'next' && eventLeft > timelineWidth - timelineTranslate) || (string == 'prev' && eventLeft < - timelineTranslate) ) {
        	translateTimeline(timelinebook_cwponents, - eventLeft + timelineWidth/2, timelineWidth - timelineTotWidth);
        }
	}

	function translateTimeline(timelinebook_cwponents, value, totWidth) {
		var eventsWrapper = timelinebook_cwponents['eventsWrapper'].get(0);
		value = (value > 0) ? 0 : value; //only negative translate value
		value = ( !(typeof totWidth === 'undefined') &&  value < totWidth ) ? totWidth : value; //do not translate more than timeline width
		setTransformValue(eventsWrapper, 'translateX', value+'px');
		//update navigation arrows visibility
		(value == 0 ) ? timelinebook_cwponents['timelineNavigation'].find('.prev').addClass('inactive') : timelinebook_cwponents['timelineNavigation'].find('.prev').removeClass('inactive');
		(value == totWidth ) ? timelinebook_cwponents['timelineNavigation'].find('.next').addClass('inactive') : timelinebook_cwponents['timelineNavigation'].find('.next').removeClass('inactive');
	}

	function updateFilling(selectedEvent, filling, totWidth) {
		//change .filling-line length according to the selected event
		var eventStyle = window.getComputedStyle(selectedEvent.get(0), null),
			eventLeft = eventStyle.getPropertyValue("left"),
			eventWidth = eventStyle.getPropertyValue("width");
		eventLeft = Number(eventLeft.replace('px', '')) + Number(eventWidth.replace('px', ''))/2;
		var scaleValue = eventLeft/totWidth;
		setTransformValue(filling.get(0), 'scaleX', scaleValue);
	}

	function setDatePosition(timelinebook_cwponents, min) {
		for (i = 0; i < timelinebook_cwponents['timelineDates'].length; i++) { 
		    var distance = daydiff(timelinebook_cwponents['timelineDates'][0], timelinebook_cwponents['timelineDates'][i]),
		    	distanceNorm = Math.round(distance/timelinebook_cwponents['eventsMinLapse']) + 2;
		    timelinebook_cwponents['timelineEvents'].eq(i).css('left', distanceNorm*min+'px');
		}
	}

	function setTimelineWidth(timelinebook_cwponents, width) {
		var timeSpan = daydiff(timelinebook_cwponents['timelineDates'][0], timelinebook_cwponents['timelineDates'][timelinebook_cwponents['timelineDates'].length-1]),
			timeSpanNorm = timeSpan/timelinebook_cwponents['eventsMinLapse'],
			timeSpanNorm = Math.round(timeSpanNorm) + 4,
			totalWidth = timeSpanNorm*width;
		timelinebook_cwponents['eventsWrapper'].css('width', totalWidth+'px');
		updateFilling(timelinebook_cwponents['eventsWrapper'].find('a.selected'), timelinebook_cwponents['fillingLine'], totalWidth);
		updateTimelinePosition('next', timelinebook_cwponents['eventsWrapper'].find('a.selected'), timelinebook_cwponents);
	
		return totalWidth;
	}

	function updateVisibleContent(event, eventsContent) {
		var eventDate = event.data('date'),
			visibleContent = eventsContent.find('.selected'),
			selectedContent = eventsContent.find('[data-date="'+ eventDate +'"]'),
			selectedContentHeight = selectedContent.height();

		if (selectedContent.index() > visibleContent.index()) {
			var classEnetering = 'selected enter-right',
				classLeaving = 'leave-left';
		} else {
			var classEnetering = 'selected enter-left',
				classLeaving = 'leave-right';
		}

		selectedContent.attr('class', classEnetering);
		visibleContent.attr('class', classLeaving).one('webkitAnimationEnd oanimationend msAnimationEnd animationend', function(){
			visibleContent.removeClass('leave-right leave-left');
			selectedContent.removeClass('enter-left enter-right');
		});
		eventsContent.css('height', selectedContentHeight+'px');
	}

	function updateOlderEvents(event) {
		event.parent('li').prevAll('li').children('a').addClass('older-event').end().end().nextAll('li').children('a').removeClass('older-event');
	}

	function getTranslateValue(timeline) {
		var timelineStyle = window.getComputedStyle(timeline.get(0), null),
			timelineTranslate = timelineStyle.getPropertyValue("-webkit-transform") ||
         		timelineStyle.getPropertyValue("-moz-transform") ||
         		timelineStyle.getPropertyValue("-ms-transform") ||
         		timelineStyle.getPropertyValue("-o-transform") ||
         		timelineStyle.getPropertyValue("transform");

        if( timelineTranslate.indexOf('(') >=0 ) {
        	var timelineTranslate = timelineTranslate.split('(')[1];
    		timelineTranslate = timelineTranslate.split(')')[0];
    		timelineTranslate = timelineTranslate.split(',');
    		var translateValue = timelineTranslate[4];
        } else {
        	var translateValue = 0;
        }

        return Number(translateValue);
	}

	function setTransformValue(element, property, value) {
		element.style["-webkit-transform"] = property+"("+value+")";
		element.style["-moz-transform"] = property+"("+value+")";
		element.style["-ms-transform"] = property+"("+value+")";
		element.style["-o-transform"] = property+"("+value+")";
		element.style["transform"] = property+"("+value+")";
	}

	//based on http://stackoverflow.com/questions/542938/how-do-i-get-the-number-of-days-between-two-dates-in-javascript
	function parseDate(events) {
		var dateArrays = [];
		events.each(function(){
			var singleDate = $(this),
				datebook_cwp = singleDate.data('date').split('T');
			if( datebook_cwp.length > 1 ) { //both DD/MM/YEAR and time are provided
				var dayComp = datebook_cwp[0].split('/'),
					timebook_cwp = datebook_cwp[1].split(':');
			} else if( datebook_cwp[0].indexOf(':') >=0 ) { //only time is provide
				var dayComp = ["2000", "0", "0"],
					timebook_cwp = datebook_cwp[0].split(':');
			} else { //only DD/MM/YEAR
				var dayComp = datebook_cwp[0].split('/'),
					timebook_cwp = ["0", "0"];
			}
			var	newDate = new Date(dayComp[2], dayComp[1]-1, dayComp[0], timebook_cwp[0], timebook_cwp[1]);
			dateArrays.push(newDate);
		});
	    return dateArrays;
	}

	function daydiff(first, second) {
	    return Math.round((second-first));
	}

	function minLapse(dates) {
		//determine the minimum distance among events
		var dateDistances = [];
		for (i = 1; i < dates.length; i++) { 
		    var distance = daydiff(dates[i-1], dates[i]);
		    dateDistances.push(distance);
		}
		return Math.min.apply(null, dateDistances);
	}

	/*
		How to tell if a DOM element is visible in the current viewport?
		http://stackoverflow.com/questions/123999/how-to-tell-if-a-dom-element-is-visible-in-the-current-viewport
	*/
	function elementInViewport(el) {
		var top = el.offsetTop;
		var left = el.offsetLeft;
		var width = el.offsetWidth;
		var height = el.offsetHeight;

		while(el.offsetParent) {
		    el = el.offsetParent;
		    top += el.offsetTop;
		    left += el.offsetLeft;
		}

		return (
		    top < (window.pageYOffset + window.innerHeight) &&
		    left < (window.pageXOffset + window.innerWidth) &&
		    (top + height) > window.pageYOffset &&
		    (left + width) > window.pageXOffset
		);
	}

	function checkMQ() {
		//check if mobile or desktop device
		return window.getComputedStyle(document.querySelector('.cd-horizontal-timeline'), '::before').getPropertyValue('content').replace(/'/g, "").replace(/"/g, "");
	}
});

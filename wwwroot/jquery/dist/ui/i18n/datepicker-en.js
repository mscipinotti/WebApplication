/* English/UK initialisation for the jQuery UI date picker plugin. */
/* Written by Stuart. */
(function (factory) {
	// A self-invoking expression is invoked automatically, without being called. We can’t call a self-invoking function. We have to add parentheses around the function to indicate that it is a function expression. Self-Invoking function do not contain any name.
	"use strict";

	if (typeof define === "function" && define.amd) {

		// AMD. Register as an anonymous module.
		define(["./widgets/datepicker"], factory);
	} else {

		// Browser globals
		factory(jQuery.datepicker);
	}
})(function (datepicker) {
	"use strict";

	datepicker.regional.en = {
		closeText: "Done",
		prevText: "Prev",
		nextText: "Next",
		currentText: "Today",
		monthNames: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
		monthNamesShort: ["Jan", "Feb", "Mar", "Apr", "May", "Jun",	"Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
		dayNames: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
		dayNamesShort: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
		dayNamesMin: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"],
		weekHeader: "Wk",
		dateFormat: "dd/mm/yy",
		firstDay: 1,
		isRTL: false,
		showMonthAfterYear: false,
		yearSuffix: ""
	};
	datepicker.setDefaults(datepicker.regional.en);

	return datepicker.regional.en;

});
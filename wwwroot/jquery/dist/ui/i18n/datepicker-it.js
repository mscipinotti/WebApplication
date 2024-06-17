/* Italian initialisation for the jQuery UI date picker plugin. */
/* Written by Antonello Pasella (antonello.pasella@gmail.com). */
// https://github.com/jquery/jquery-ui/blob/main/ui/i18n/datepicker-it.js
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

	datepicker.regional.it = {
		closeText: "Chiudi",
		prevText: "Prec",
		nextText: "Succ",
		currentText: "Oggi",
		monthNames: ["Gennaio", "Febbraio", "Marzo", "Aprile", "Maggio", "Giugno", "Luglio", "Agosto", "Settembre", "Ottobre", "Novembre", "Dicembre"],
		monthNamesShort: ["Gen", "Feb", "Mar", "Apr", "Mag", "Giu",	"Lug", "Ago", "Set", "Ott", "Nov", "Dic"],
		dayNames: ["Domenica", "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì", "Sabato"],
		dayNamesShort: ["Dom", "Lun", "Mar", "Mer", "Gio", "Ven", "Sab"],
		dayNamesMin: ["Do", "Lu", "Ma", "Me", "Gi", "Ve", "Sa"],
		weekHeader: "Sm",
		dateFormat: "dd/mm/yy",
		firstDay: 1,
		isRTL: false,
		showMonthAfterYear: false,
		yearSuffix: ""
	};
	datepicker.setDefaults(datepicker.regional.it);

	return datepicker.regional.it;

});
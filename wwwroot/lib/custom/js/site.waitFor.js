// https://bootsnipp.com/snippets/rga0j
// La chiamata alla modale è modificata per essere adattata alla versione 5.3.3 di jquery

let waitingDialog = (function ($) {
	'use strict';
	
	return {
		show: function (id, options) {
			if (typeof options === 'undefined') {
				options = {};
			}
			const modal = new bootstrap.Modal('#' + id, options);
			modal.show();
			return modal;
		},
		hide: function (modal) {
			modal.hide();
		}
	};

})(jQuery);
// See https://stackoverflow.com/questions/2937227/what-does-function-jquery-mean for an explanation
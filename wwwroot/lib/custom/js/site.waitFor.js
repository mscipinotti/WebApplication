let waitingDialog = (function ($) {
	'use strict';

	let $dialog = $(
		'<div class="modal fade" data-backdrop="static" data-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true" style="padding-top:15%; overflow-y:visible;">' +
			'<div class="modal-dialog modal-m">' +
				'<div class="spinner-border" role="status">' +
					'<span class="sr-only"></span>' +
				'</div>' +
			'</div>' +
		'</div>');

	return {
		show: function (options) {
			if (typeof options === 'undefined') {
				options = {};
			}
			let settings = $.extend({
				dialogSize: 'm',
				progressType: '',
				onHide: null // This callback runs after the dialog was hidden
			}, options);

			$dialog.find('.modal-dialog').attr('class', 'modal-dialog').addClass('modal-' + settings.dialogSize);
			// Adding callbacks
			if (typeof settings.onHide === 'function') {
				$dialog.off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
					settings.onHide.call($dialog);
				});
			}
			$dialog.modal();
		},
		hide: function () {
			$dialog.modal('hide');
		}
	};

})(jQuery);
// See https://stackoverflow.com/questions/2937227/what-does-function-jquery-mean for an explanation
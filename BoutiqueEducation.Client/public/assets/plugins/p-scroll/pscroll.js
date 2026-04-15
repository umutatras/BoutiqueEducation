(function($) {
	"use strict";

	var el = document.querySelector('.app-sidebar');
	if (el) {
		const ps = new PerfectScrollbar(el, {
		  useBothWheelAxes: true,
		  suppressScrollX: true,
		});
	}

})(jQuery);
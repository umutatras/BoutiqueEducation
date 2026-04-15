(function($) {
	"use strict";

	var el = document.querySelector('.sidebar-right');
	if (el) {
		const ps11 = new PerfectScrollbar(el, {
		  useBothWheelAxes: true,
		  suppressScrollX: true,
		});
	}

})(jQuery);
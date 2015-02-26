var textareaIsActive;
var countId;


var DropDown = {
	init: function (buttonList) {
		return "<div class=\"btn-group\"> <button type=\"button\" class=\"btn btn-xs btn-default dropdown-toggle\" data-toggle=\"dropdown\"> Actions <span class=\"caret\"></span> </button> <ul class=\"dropdown-menu\">" + buttonList + "</ul></div>";
	},
	button: function (title, href, onclick, iconClass, dataTargetClass) {
		return "<li><a onclickEval='true' eval=\"" + onclick + "\" data-toggle=\"modal\" data-target=\"" + dataTargetClass + "\" href=\"" + href + "\"><i class=\"fa " + iconClass + "\"></i> " + title + "</a></li>";
	}
}


function closeModal(modalSelector) {
	$("" + modalSelector + "").modal('hide');
}

function cookieSet(c_name, value, exdays) {
	var exdate = new Date();
	exdate.setDate(exdate.getDate() + exdays);
	var c_value = escape(value) + ((exdays == null) ? "" : "; path=/; expires=" + exdate.toUTCString());
	document.cookie = c_name + "=" + c_value;
}

function cookieGet(c_name) {
	var i, x, y, arrCookies = document.cookie.split(";");
	for (i = 0; i < arrCookies.length; i++) {
		x = arrCookies[i].substr(0, arrCookies[i].indexOf("="));
		y = arrCookies[i].substr(arrCookies[i].indexOf("=") + 1);
		x = x.replace(/^\s+|\s+$/g, "");
		if (x == c_name) {
			return unescape(y);
		}
	}
}

function SubmitFormAction(elem) {
	try {
		var dataText;
		$.ajax({
			url: $(elem).attr("action"),
			data: $(elem).serialize() + "&action=" + $('[name=action]').val() + "",
			async: false,
			dataType: "text",
			type: "POST",
			success: function (data) {
				if (data == null)
					return null;
				dataText = data;
			}
		});
		return JSON.parse(dataText);
	} catch (err) {
		return null;
	}
}

function openModalWithAjax(modalSelector, modalTitle, ajaxUrl, postData, completeEval) {
	$("" + modalSelector + " .modal_title").html(modalTitle);
	$("" + modalSelector + " .modal_content").html("");
	$.ajax({
		"type": "GET",
		"url": ajaxUrl,
		"data": postData,
		"async": false,
		"success": function (response) {
			$("" + modalSelector + " .modal_content").html(response);
		},
		"complete": function (response) {
			if (completeEval != null)
				eval(completeEval);
		},
		"error": function () {
			alert("Baðlantý Açýlamadý: " + ajaxUrl + "");
		}
	});
}



///// PAGE ONLOAD
$(function () {
	$('#side-menu').metisMenu();
	if ($(".numeric")[0]) {
		$(".numeric").ForceNumericOnly();
	}
	if ($(".date-picker").attr("name")) {
		$(".date-picker").datepicker({
			changeMonth: true,
			changeYear: true,
			minDate: "-120y",
			numberOfMonths: 1,
			autoSize: true,
			dateFormat: "yy-mm-dd",
			maxDate: "30"
		});
	}
	$(window).bind("load resize", function () {
		width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
		if (width < 768) {
			$('div.sidebar-collapse').addClass('collapse')
		} else {
			$('div.sidebar-collapse').removeClass('collapse')
		}
		$("#page-wrapper").css("min-height", "" + ($(window).height() - 50) + "px");
		$("#leftMenu").css("height", "" + ($(window).height() - 55) + "px");
	});
	
});
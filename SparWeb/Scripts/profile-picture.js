Dropzone.autoDiscover = false;
$(function () {

	//initializing DropZone
	initializeDropZone();

	//making sure we text passes click events to dropzone div
	$(".profileUploadPicAction").click(function () {
		$(this).parent().parent().trigger("click");
	});
	$(".profileUploadPicHeader").click(function () {
		$(this).parent().trigger("click");
	});
});

function initializeDropZone() {
	$(".profilePictureDefault").each(function () {
		var userId = $(this).data('userid');
		$(this).dropzone({
			url: "/Account/UploadProfilePicture?userId=" + userId,
			thumbnailWidth: 150,
			thumbnailHeight: 150,
			init: function () {
				this.on("complete", function (data) {
					var res = eval('(' + data.xhr.responseText + ')');
					if (res.Message.indexOf("Error:") != 0) {
						$('.profilePictureDefault').hide();
						$('.dz-preview').hide();

						showProfilePicture(res.Message, userId);
					}
					else {
						alert(res.Message);
					}
				});
			}
		});
	});
}

function showProfilePicture(pictureFileName, userId) {
	var parentDiv = $('.profilePictureDefault[data-userid="' + userId + '"]').parent();
	parentDiv.find('.cancelProfilePictureUpdate').addClass('hidden');
	parentDiv.find('.profilePictureDefault').addClass('hidden');
	parentDiv.find('.profilePicture').find('img').attr("src", pictureFileName);
	parentDiv.find('.profilePicture').removeClass('hidden');
}

function updateProfilePicture() {
	$('.profilePicture').addClass('hidden');
	$('.profilePictureDefault').removeClass('hidden');
	$('.cancelProfilePictureUpdate').removeClass('hidden');
}

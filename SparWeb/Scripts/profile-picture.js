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
	$('[class^="profilePictureDefault"]').each(function () {
		var userId = $(this).data('userid');
		var thumbnailsize = $(this).data('thumbnailsize');
		$(this).dropzone({
			url: "/Account/UploadProfilePicture?userId=" + userId + "&thumbnailSize=" + thumbnailsize,
			thumbnailWidth: 150,
			thumbnailHeight: 150,
			init: function () {
				this.on("complete", function (data) {
					var res = eval('(' + data.xhr.responseText + ')');
					if (res.Message.indexOf("Error:") != 0) {
						var parentDiv = $('[class^="profilePictureDefault"][data-userid="' + userId + '"]').parent();
						parentDiv.find('[class^="profilePictureDefault"]').hide();
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
	var parentDiv = $('[class^="profilePictureDefault"][data-userid="' + userId + '"]').parent();
	parentDiv.find('.cancelProfilePictureUpdate').addClass('hidden');
	parentDiv.find('[class^="profilePictureDefault"]').addClass('hidden');
	parentDiv.find('.profilePicture').find('img').attr("src", pictureFileName);
	parentDiv.find('.profilePicture').removeClass('hidden');
}

function updateProfilePicture(userId) {
	var parentDiv = $('[class^="profilePictureDefault"][data-userid="' + userId + '"]').parent();
	parentDiv.find('.profilePicture').addClass('hidden');
	parentDiv.find('[class^="profilePictureDefault"]').removeClass('hidden');
	parentDiv.find('.cancelProfilePictureUpdate').removeClass('hidden');
}

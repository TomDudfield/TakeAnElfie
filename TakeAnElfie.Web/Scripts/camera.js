
(function(){
	var setupCamera = function(){
		var deviceMedia = navigator.getUserMedia();
		console.log(deviceMedia);
	}();

	var takePhoto = function(){
		alert("got yo picture")
		debugger;
	};

	var btn = $('.takePhoto').on('click.takePhoto', function(){
		takePhoto();
	})

})();


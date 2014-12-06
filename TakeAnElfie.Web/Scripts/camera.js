
(function(){
	var gotMedia = function(stream){
		var video = document.getElementById("v");
		var canvas = document.getElementById("c");
		var button = document.getElementById("b");

		video.src = stream;
		button.disabled = false;
		button.onclick = function() {
			canvas.getContext("2d").drawImage(video, 0, 0, 300, 300, 0, 0, 300, 300);
			var img = canvas.toDataURL("image/png");
			debugger;
		};
	};

	var noMedia = function(err){
		console.log("There was an error - " + err)
	};

	var takePhoto = function(){
		alert("got yo picture")
		debugger;
	};

	var setup = function(){
		navigator.getUserMedia = navigator.webkitGetUserMedia || navigator.getUserMedia;
		if (navigator.getUserMedia) {
		   navigator.getUserMedia({video: true},gotMedia,noMedia);
		} else {
		   console.log("getUserMedia not supported");
		}
	}();
})();


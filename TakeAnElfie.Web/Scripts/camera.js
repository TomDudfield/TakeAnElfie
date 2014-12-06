
(function(){
	//setup dom refs

	var video = document.getElementById("v");
	var canvas = document.getElementById("c");
	var button = document.getElementById("b");
	var captured = document.getElementById("img");

	// Setup video stream
	navigator.getUserMedia = navigator.webkitGetUserMedia || navigator.getUserMedia;
	if (navigator.getUserMedia) {
	   navigator.getUserMedia({video: true},function(stream) {
			if(window.URL){
				video.src = window.URL.createObjectURL(stream);
			} else{
				video.src = stream;
			}
			video.play();
		}, function(err) { alert("there was an error " + err)});
	} else {
	   console.log("getUserMedia not supported");
	};

	var getImage = function(){
		canvas.getContext("2d").drawImage(video, 0, 0, 300, 300, 0, 0, 300, 300);
		var img = canvas.toDataURL("image/png");
		captured.src = img;
		return img;
	}

	var elfie = $.connection.ElfieHub;

	$.connection.hub.start().done(function() {
	    elfie.server.connectCamera();
	});

	elfie.client.takeImage = function(userId) {
		var userImg = getImage();
		debugger;
	    elfie.server.processImage(userId, userImg);
	};

})();


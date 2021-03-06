﻿
(function(){
	//setup dom refs

	var video = document.getElementById("v");
	var canvas = document.getElementById("c");

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
		canvas.getContext("2d").drawImage(video, 0, 0, video.videoWidth, video.videoHeight, 0, 0, video.videoWidth, video.videoHeight);
		var img = canvas.toDataURL("image/png");

		return img;
	}

	var elfie = $.connection.ElfieHub;

	$.connection.hub.start().done(function() {
	    elfie.server.connectCamera();
	});

	elfie.client.takeImage = function(userId) {
		var userImg = getImage();
	    elfie.server.processImage(userId, userImg);
	};

})();


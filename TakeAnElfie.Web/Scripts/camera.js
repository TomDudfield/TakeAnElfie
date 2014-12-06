﻿
(function(){


	var setup = function(){
		navigator.getUserMedia = navigator.webkitGetUserMedia || navigator.getUserMedia;
		if (navigator.getUserMedia) {
		   navigator.getUserMedia({video: true},function(stream) {
				var video = document.getElementById("v");
				var canvas = document.getElementById("c");
				var button = document.getElementById("b");
				var captured = document.getElementById("i");

				if(window.URL){
					video.src = window.URL.createObjectURL(stream);
				} else{
					video.src = stream;
				}
				video.play();


				
				button.disabled = false;
				button.onclick = function() {
					canvas.getContext("2d").drawImage(video, 0, 0, 300, 300, 0, 0, 300, 300);
					var img = canvas.toDataURL("image/png");
					captured.src = img;
					debugger;	
				};
				}, function(err) { alert("there was an error " + err)});
		} else {
		   console.log("getUserMedia not supported");
		}
	}();
})();


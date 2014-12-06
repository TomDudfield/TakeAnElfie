
(function(){
	var setup = function(){
		navigator.getUserMedia = navigator.webkitGetUserMedia || navigator.getUserMedia;
		if (navigator.getUserMedia) {
		   navigator.getUserMedia (

		      // constraints
		      {
		         video: true,
		         audio: false
		      },

		      // successCallback
		      function(localMediaStream) {
		         console.log(localMediaStream);
		         // Do something with the video here, e.g. video.play()
		      },

		      // errorCallback
		      function(err) {
		         console.log("The following error occured: " + err);
		      }
		   );
		} else {
		   console.log("getUserMedia not supported");
		}
	}();

	var takePhoto = function(){
		alert("got yo picture")
		debugger;
	};

	var btn = $('.takePhoto').on('click.takePhoto', function(){
		takePhoto();
	})

})();


// What connection string do i need
var elfie = $.connection.ElfieHub;

$.connection.hub.start().done(function() {
    console.log('Connection started');
})

$('.take-elfie').click(function() {
    elfie.server.takeImage();
});

$('.approve').click(function() {
    elfie.server.approveImage();
});

elfie.client.reviewImage = function(image) {
    console.log(image);

    $('.selfie').attr('src', image);
    $('.review-image').show();
};

elfie.client.showTweet = function(tweet) {
    console.log(tweet);
};


$(document).ready(function() {
    $('.review-image').hide();
});

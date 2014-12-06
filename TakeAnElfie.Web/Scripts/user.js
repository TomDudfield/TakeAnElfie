// What connection string do i need
var elfie = $.connection.ElfieHub;

$.connection.hub.start().done(function() {
    console.log('Connection started');
})

$(document).ready(function() {
    $('.take-elfie').on('click', function() {
        elfie.server.send('takeElfie');
    });
});

elfie.client.debug = function() {
    console.log(arguments.join(', '));
};

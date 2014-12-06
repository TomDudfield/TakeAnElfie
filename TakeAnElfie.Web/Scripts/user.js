// What connection string do i need
var connection = $.connection.ElfieHub;

connection.hub.start().done(function() {
    console.log('Connection started');
})

$(document).ready(function() {
    $('.take-elfie').on('click', function() {
        connection.server.send('takeElfie');
    });
});

connection.client.debug = function() {
    console.log(arguments.join(', '));
};

$(document).ready(function () {
    Modernizr.load({
        test: Modernizr.svg,
        yep: 'js/network.js',
        nope: ['js/jquery-1.9.1.min.js', 'js/fallback.js']
    });
    loadNetwork(2008);
    $('#yearInTitle').html(2008);
    $('#yearSelector button').click(function () {
        $(this).addClass('active').siblings().removeClass('active');
        
        $('#yearInTitle').html($(this).html());
        $('#networkChart').html('');
        loadNetwork($(this).html());
    });
});
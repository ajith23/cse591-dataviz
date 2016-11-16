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

function getRequestParam() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}
function loadWordChart1(tagName)
{
    $.ajax({
        type: 'GET',
        url: 'https://d81a451e.ngrok.io/topic_model?tag=' + tagName,
        contentType: 'text/plain',
        async: true,
        success(data) {
            loadList(data);
            renderWordChart();
        },
        error(error) {
           
        }
    });
}

function loadWordChart(tagName) {
    var s = {
        "a": {
            "(or": 0.051,
            "all": 0.036,
            "angularjs": 0.051,
            "apply": 0.037,
            "array": 0.286,
            "between": 0.088,
            "call": 0.037,
            "check": 0.17,
            "clipboard": 0.097,
            "clone": 0.037,
            "creating": 0.041,
            "current": 0.118,
            "date": 0.059,
            "detect": 0.079,
            "difference": 0.088,
            "do": 0.546,
            "element": 0.13,
            "empty": 0.051,
            "equivalent": 0.097,
            "file": 0.059,
            "function": 0.171,
            "get": 0.187,
            "hidden": 0.019,
            "if": 0.196,
            "javascript": 1.607,
            "jquery": 0.523,
            "json": 0.037,
            "keyword": 0.107,
            "loop": 0.075,
            "node.js": 0.109,
            "object": 0.301,
            "objects": 0.056,
            "or": 0.066,
            "page": 0.141,
            "property": 0.066,
            "remove": 0.051,
            "return": 0.082,
            "selected": 0.079,
            "through": 0.075,
            "url": 0.112,
            "use": 0.185,
            "using": 0.041,
            "value": 0.069,
            "var": 0.04,
            "vs": 0.106,
            "without": 0.11,
            "work": 0.097,
            "you": 0.126,
            "{}": 0.076
        }
    }
    loadList(s);
    renderWordChart();
}

function loadList(data)
{
    for (var name in data.a)
        $("#weightTags").append('<li><a href="#" data-weight="' + parseInt(data.a[name]) + '">' + name + '</a></li>');
}

function renderWordChart() {
    TagCanvas.interval = 20;
    TagCanvas.textFont = 'sans-serif';
    TagCanvas.textColour = '#d0743c';
    TagCanvas.textHeight = 18;
    TagCanvas.outlineColour = '#d0743c';
    TagCanvas.outlineThickness = 1;
    TagCanvas.maxSpeed = 0.04;
    TagCanvas.minBrightness = 0.1;
    TagCanvas.depth = 0.92;
    TagCanvas.pulsateTo = 0.2;
    TagCanvas.pulsateTime = 0.75;
    TagCanvas.initial = [0.1, -0.1];
    TagCanvas.decel = 0.98;
    TagCanvas.reverse = true;
    TagCanvas.hideTags = false;
    TagCanvas.shadow = '#ccf';
    TagCanvas.shadowBlur = 3;
    //TagCanvas.weight = true;
    TagCanvas.weightFrom = 'data-weight';
    TagCanvas.fadeIn = 800;
    try {
        TagCanvas.Start('tagcanvas', 'weightTags');
    } catch (e) {
    }
    var i, j, g, gc = document.getElementById('gradient').getContext('2d');
    g = gc.createLinearGradient(0, 0, 0, gc.canvas.height);
    for (i in TagCanvas.weightGradient)
        g.addColorStop(i, TagCanvas.weightGradient[i]);
    gc.fillStyle = g;
    gc.fillRect(0, 0, gc.canvas.width, gc.canvas.height);
    for (i = 0; i < gexamples.length; ++i) {
        gc = document.getElementById('example' + i).getContext('2d');
        g = gc.createLinearGradient(0, 0, gc.canvas.width, 0);
        for (j in gexamples[i])
            g.addColorStop(j, gexamples[i][j]);
        gc.fillStyle = g;
        gc.fillRect(0, 0, gc.canvas.width, gc.canvas.height);
    }
};
var g1 = {
    0: 'red',
    0.5: 'orange',
    1: 'rgba(0,0,0,0.1)'
}, wOpts = {
    none: { weight: false },
    size: null,
    colour: { weightMode: 'colour' },
    both: { weightMode: 'both' },
    bgcolour: { weightMode: 'bgcolour', padding: 2, bgRadius: 5 },
    bgoutline: { weightMode: 'bgoutline', bgOutlineThickness: 3, padding: 2, bgRadius: 5 },
    outline: { weightMode: 'outline' }
}, gexamples = ["t1", "t1", "t5"];
function TLoad(o) { TagCanvas.Start('tagcanvas', 'weightTags', wOpts[o]); }
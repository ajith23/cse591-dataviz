function loadWordChart(tagName)
{
    $.ajax({
        type: 'GET',
        url: 'http://17edf783.ngrok.io/topic_model?tag=' + tagName,
        //url: 'https://d81a451e.ngrok.io/topic_model?tag=' + tagName,
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
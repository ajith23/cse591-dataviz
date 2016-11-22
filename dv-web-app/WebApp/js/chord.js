var visual = document.getElementById("chordChart");

var year = getRequestParam()["year"];
var queryParamTag = getRequestParam()["tag"];


var labelArray = [];

var rotation = -0;

function Chord(container, options, matrix) {

    // initialize the chord configuration variables
    var config = {
        width: 560,
        height: 460,
        rotation: 0,
        textgap: 26,
        colors: ["#7fc97f", "#beaed4", "#fdc086", "#ffff99", "#386cb0", "#f0027f", "#bf5b17", "#666666"]
    };

    // add options to the chord configuration object
    if (options) {
        extend(config, options);
    }

    // set chord visualization variables from the configuration object
    var offset = Math.PI * config.rotation,
        width = config.width,
        height = config.height,
        textgap = config.textgap
    colors = config.colors;

    // set viewBox and aspect ratio to enable a resize of the visual dimensions 
    var viewBoxDimensions = "0 0 " + width + " " + height,
        aspect = width / height;

    if (config.gnames) {
        gnames = config.gnames;
    } else {
        // make a list of names
        gnames = [];
        for (var i = 97; i < matrix.length; i++) {
            gnames.push(String.fromCharCode(i));
        }
    }

    // start the d3 magic
    var chord = d3.layout.chord()
        .padding(.05)
        .sortSubgroups(d3.descending)
        .matrix(matrix);

    var innerRadius = Math.min(width, height) * .31,
        outerRadius = innerRadius * 1.1;

    var fill = d3.scale.ordinal()
        .domain(d3.range(matrix.length - 1))
        .range(colors);

    var svg = d3.select("#chordChart").append("svg")
        .attr("id", "visual")
        .attr("viewBox", viewBoxDimensions)
        .attr("preserveAspectRatio", "xMinYMid")    // add viewBox and preserveAspectRatio
        .attr("width", width)
        .attr("height", height)
      .append("g")
        .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

    var g = svg.selectAll("g.group")
        .data(chord.groups)
      .enter().append("svg:g")
        .attr("class", "group");

    g.append("svg:path")
        .style("fill", function (d) { return fill(d.index); })
        .style("stroke", function (d) { return fill(d.index); })
        .attr("id", function (d, i) { return "group" + d.index; })
        .attr("d", d3.svg.arc().innerRadius(innerRadius).outerRadius(outerRadius).startAngle(startAngle).endAngle(endAngle))
        .on("mouseover", fade(.1))
        .on("mouseout", fade(1));

    g.append("svg:text")
        .each(function (d) { d.angle = ((d.startAngle + d.endAngle) / 2) + offset; })
        .attr("dy", ".35em")
        .attr("text-anchor", function (d) { return d.angle > Math.PI ? "end" : null; })
        .attr("transform", function (d) {
            return "rotate(" + (d.angle * 180 / Math.PI - 90) + ")"
                + "translate(" + (outerRadius + textgap) + ")"
                + (d.angle > Math.PI ? "rotate(180)" : "");
        })
        .text(function (d) { return gnames[d.index]; });

    svg.append("g")
        .attr("class", "chord")
      .selectAll("path")
        .data(chord.chords)
      .enter().append("path")
        .attr("d", d3.svg.chord().radius(innerRadius).startAngle(startAngle).endAngle(endAngle))
        .style("fill", function (d) { return fill(d.source.index); })
        .style("opacity", 1)
      .append("svg:title")
        .text(function (d) {
            return d.source.value + " people from " + gnames[d.source.index] + " commute to " + gnames[d.target.index];
        });

    // helper functions start here

    function startAngle(d) {
        return d.startAngle + offset;
    }

    function endAngle(d) {
        return d.endAngle + offset;
    }

    function extend(a, b) {
        for (var i in b) {
            a[i] = b[i];
        }
    }

    // Returns an event handler for fading a given chord group.
    function fade(opacity) {
        return function (g, i) {
            svg.selectAll(".chord path")
                .filter(function (d) { return d.source.index != i && d.target.index != i; })
                .transition()
                .style("opacity", opacity);
        };
    }


    window.onresize = function () {
        var targetWidth = (window.innerWidth < width) ? window.innerWidth : width;

        var svg = d3.select("#chordChart")
            .attr("width", targetWidth)
            .attr("height", targetWidth / aspect);
    }


}

window.onload = function () {
    $('#detailsHeader').html(queryParamTag);
    $('#tagRelationsTitle').html('Tag Relations for ' + queryParamTag + ' in ' + year);
    loadForumSupportQandAA(queryParamTag);
    loadForumSupportPostCount(queryParamTag);

    var dataFile = 'data/' + year + 'json.json';
    d3.json(dataFile, function (error, data) {
        if (error) throw error;
        var matrix = buildMatrix(data);

        var chord_options = {
            "gnames": labelArray,
            "rotation": rotation,
            "colors": ["#98abc5", "#8a89a6", "#7b6888", "#6b486b", "#a05d56", "#d0743c", "#ff8c00"]
        };

        Chord(visual, chord_options, matrix);
    });
    loadWordChart(queryParamTag);
}

function buildMatrix(data) {
    var indexes = $.map(data.nodes, function (obj, index) {
        if (obj.id == queryParamTag) {
            return index;
        }
    })
    var tagIndex = indexes[0];

    var targets = $.grep(data.edges, function (e) { return e.source == tagIndex });
    var sources = $.grep(data.edges, function (e) { return e.target == tagIndex });

    var pointArray = [];
    var dataArray = [];
    for (var i = 0; i < targets.length; i++) {
        dataArray.push({ 'label': data.nodes[targets[i].target].id, 'value': parseFloat(data.nodes[targets[i].target].actualValue) });
    }
    for (var i = 0; i < sources.length; i++) {
        dataArray.push({ 'label': data.nodes[sources[i].source].id, 'value': parseFloat(data.nodes[sources[i].source].actualValue) });
    }

    dataArray.sort(function (a, b) {
        return parseFloat(a.value) - parseFloat(b.value);
    });
    dataArray.push({ 'label': queryParamTag, 'value': 0 });

    for (var i = 0; i < dataArray.length; i++) {
        labelArray.push(dataArray[i].label);
        pointArray.push(dataArray[i].value);
    }
    //console.log(pointArray);
    //console.log(labelArray);
    var m = [];
    for (var i = 0; i < labelArray.length; i++) {
        var row = [];
        if (i == labelArray.length - 1)
            m.push(pointArray);
        else {
            for (var j = 0; j < labelArray.length; j++) {
                if (j == labelArray.length - 1) 
                    row.push(pointArray[i]);
                else 
                    row.push(0);
            }
            m.push(row);
        }
    }

    //console.log(tagIndex);
    //console.log(labelArray);
    //console.log(pointArray);
    return m;
}

//d3.select(self.frameElement).style("height", "600px");

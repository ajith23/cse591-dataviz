var w = 1000;
var h = 600;
var linkDistance = 70;

var colors = d3.scale.category10();

function loadNetwork(year) {
    var dataFile = 'data/' + year + 'json.json';
    d3.json(dataFile, function (error, data) {
        if (error) throw error;
        else buildNetwork(data);
    });
}

function buildNetwork(dataset) {
    var svg = d3.select("#networkChart").append("svg")
                                        .attr({ "width": w, "height": h })
                                        .attr("viewBox", "0 0 " + w + " " + h)
                                        .attr("perserveAspectRatio", "xMinYMid");

    var force = d3.layout.force()
        .nodes(dataset.nodes)
        .links(dataset.edges)
        .size([w, h])
        .linkDistance([linkDistance])
        .linkStrength(1)
        .charge([-200])
        .theta(0.1)
        .gravity(0.05)
        .start();

    var edges = svg.selectAll("line")
      .data(dataset.edges)
      .enter()
      .append("line")
      .attr("id", function (d, i) { return 'edge' + i })
      .attr("stroke-width", function(d) { return (d.value); })
      .style("stroke", "#ccc")
      .style("pointer-events", "none");

    var nodes = svg.selectAll("circle")
      .data(dataset.nodes)
      .enter()
      .append("circle")
      .attr({ "r": function (d) { return (d.value); } })
      .on("dblclick", function (d) { window.location.href = "/details.html?year=" + $('#yearSelector button.active').html() + "&tag=" + d.name; })
      .style("fill", function (d, i) { return colors(d.group); })
      .call(force.drag)


    var nodelabels = svg.selectAll(".nodelabel")
       .data(dataset.nodes)
       .enter()
       .append("text")
       .attr({
           "x": function (d) { return d.x; },
           "y": function (d) { return d.y; },
           "class": "nodelabel",
           "stroke": "black",
           "font-weight": 100,
           "font-family": "courier new"
       })
       .text(function (d) { return d.name; });

    var edgepaths = svg.selectAll(".edgepath")
        .data(dataset.edges)
        .enter()
        .append('path')
        .attr({
            'd': function (d) { return 'M ' + d.source.x + ' ' + d.source.y + ' L ' + d.target.x + ' ' + d.target.y },
            'class': 'edgepath',
            'fill-opacity': 0,
            'stroke-opacity': 0,
            'fill': 'blue',
            'stroke': 'red',
            'id': function (d, i) { return 'edgepath' + i }
        })
        .style("pointer-events", "none");

    var edgelabels = svg.selectAll(".edgelabel")
        .data(dataset.edges)
        .enter()
        .append('text')
        .style("pointer-events", "none")
        .attr({
            'class': 'edgelabel',
            'id': function (d, i) { return 'edgelabel' + i },
            'dx': 40,
            'dy': 0,
            'font-size': 10,
            'fill': '#aaa'
        });

    edgelabels.append('textPath')
        .attr('xlink:href', function (d, i) { return '#edgepath' + i })
        .style("pointer-events", "none")
        .text(function (d, i) { return d.actualValue; });

    force.on("tick", function () {
        edges.attr({
            "x1": function (d) { return d.source.x; }, "y1": function (d) { return d.source.y; },
            "x2": function (d) { return d.target.x; }, "y2": function (d) { return d.target.y; }
        });
        nodes.attr({ "cx": function (d) { return d.x; }, "cy": function (d) { return d.y; } });
        nodelabels.attr("x", function (d) { return d.x; }).attr("y", function (d) { return d.y; });
        edgepaths.attr('d', function (d) { var path = 'M ' + d.source.x + ' ' + d.source.y + ' L ' + d.target.x + ' ' + d.target.y; return path });
        edgelabels.attr('transform', function (d, i) {
            if (d.target.x < d.source.x) {
                bbox = this.getBBox();
                rx = bbox.x + bbox.width / 2;
                ry = bbox.y + bbox.height / 2;
                return 'rotate(180 ' + rx + ' ' + ry + ')';
            }
            else {
                return 'rotate(0)';
            }
        });
    });
}
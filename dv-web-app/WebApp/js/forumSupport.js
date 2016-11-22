
function loadForumSupportQandAA(tagName) {

    var margin = { top: 20, right: 20, bottom: 30, left: 40 },
        width = 500 - margin.left - margin.right,
        height = 450 - margin.top - margin.bottom;

    var x0 = d3.scale.ordinal().rangeRoundBands([0, width], .1);
    var x1 = d3.scale.ordinal();
    var y = d3.scale.linear().range([height, 0]);
    var color = d3.scale.ordinal().range(["#98abc5", "#8a89a6", "#7b6888", "#6b486b", "#a05d56", "#d0743c", "#ff8c00"]);
    var xAxis = d3.svg.axis().scale(x0).orient("bottom");
    var yAxis = d3.svg.axis() .scale(y).orient("left").tickFormat(d3.format(".2s"));

    var svg = d3.select("#barChart1").append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
      .append("g")
        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    var dataFile = 'data/forumSupportData.csv';
    d3.csv(dataFile, function (error, data) {
        if (error) throw error;
        data = data.slice(0,10);
        var fieldNames = d3.keys(data[0]).filter(function (key) { return key !== "TagName"; });

        var index = 0;
        data.forEach(function (d) {
            d.ages = fieldNames.map(function (name) { return { name: name, value: +d[name] }; });
        });

        x0.domain(data.map(function (d) { return d.TagName; }));
        x1.domain(fieldNames).rangeRoundBands([0, x0.rangeBand()]);
        y.domain([0, d3.max(data, function (d) { return d3.max(d.ages, function (d) { return d.value; }); })]);

        svg.append("g").attr("class", "x axis").attr("transform", "translate(0," + height + ")").call(xAxis);
        svg.append("g").attr("class", "y axis").call(yAxis)
          .append("text").attr("transform", "rotate(-90)").attr("y", 6).attr("dy", ".71em").style("text-anchor", "end").text("");

        var state = svg.selectAll(".state")
            .data(data)
          .enter().append("g")
            .attr("class", "state")
            .attr("transform", function (d) { return "translate(" + x0(d.TagName) + ",0)"; });

        state.selectAll("rect")
            .data(function (d) { return d.ages; })
          .enter().append("rect")
            .attr("width", x1.rangeBand())
            .attr("x", function (d) { return x1(d.name); })
            .attr("y", function (d) { return y(d.value); })
            .attr("height", function (d) { return height - y(d.value); })
            .style("fill", function (d) { return color(d.name); });

        var legend = svg.selectAll(".legend")
            .data(fieldNames.slice().reverse())
          .enter().append("g")
            .attr("class", "legend")
            .attr("transform", function (d, i) { return "translate(0," + i * 20 + ")"; });

        legend.append("rect")
            .attr("x", width - 18)
            .attr("width", 18)
            .attr("height", 18)
            .style("fill", color);

        legend.append("text")
            .attr("x", width - 24)
            .attr("y", 9)
            .attr("dy", ".35em")
            .style("text-anchor", "end")
            .text(function (d) { return d; });

    });
}

function loadForumSupportPostCount(tagName) {
    var margin = { top: 20, right: 20, bottom: 30, left: 100 },
    width = 700 - margin.left - margin.right,
    height = 250 - margin.top - margin.bottom;
    var color = d3.scale.ordinal().range(["#7b6888", "#6b486b", "#a05d56", "#d0743c", "#ff8c00"]);
    var parseDate = d3.time.format("%d-%b-%y").parse;
    var x = d3.time.scale().range([0, width]);
    var y = d3.scale.linear().range([height, 0]);
    var xAxis = d3.svg.axis().scale(x).orient("bottom");
    var yAxis = d3.svg.axis().scale(y).orient("left");

    var area = d3.svg.area()
        .x(function (d) { return x(d.date); })
        .y0(height).y1(function (d) { return y(d.count); });

    var svg = d3.select("#areaChart1").append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
      .append("g")
        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    d3.csv("data/tagPostCountYearWise.csv", function (error, data) {
        if (error) throw error;


        var select = $.grep(data, function (e) { return e.tagName == tagName });
        var temp = [];
        //console.log(select);
        var year = 2008;
        for (var j = 1; j <=9; j++)
            temp.push({ date: '1-Jan-' + (year.toString().substring(2)), count: parseInt(select[0][year++]) });
                
        data = temp;
        //console.log(data);
        data.forEach(function (d) {
            d.date = parseDate(d.date);
            d.count = +d.count;
        });

        x.domain(d3.extent(data, function (d) { return d.date; }));
        y.domain([0, d3.max(data, function (d) { return d.count; })]);

        svg.append("path")
            .datum(data)
            .attr("class", "area")
            .attr("d", area).style("fill", function (d) { return color(d.name); });;

        svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + height + ")")
            .call(xAxis);

        svg.append("g")
            .attr("class", "y axis")
            .call(yAxis)
          .append("text")
            .attr("transform", "rotate(-90)")
            .attr("y", 6)
            .attr("dy", ".71em")
            .style("text-anchor", "end")
            .text("Count");
    });
}
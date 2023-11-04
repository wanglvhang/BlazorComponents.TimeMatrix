// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

//export function showPrompt(message) {
//  return prompt(message, 'Type anything here');
//}

//configurations
//cell 宽高 间隔   mark 高度 间隔   line 每行个数
var matrix_padding = 3;

//var cell_width = 36;
//var cell_height = 36;
//var cell_gap = 2;
//var cell_rx = 3;
var cell_width = 26;
var cell_height = 26;
var cell_gap = 1;
var cell_rx = 2;

//var timeline_margin_top = 4;//time line margin top
//var timeline_height = 6; //时间线高度
//var timeline_gap = 2;
var timeline_margin_top = 3;//time line margin top
var timeline_height = 4; //时间线高度
var timeline_gap = 2;

var instant_width = 8;
var instant_height = 6;
var instant_rx = 1;
var instant_gap = 1;


//是否有cell选中
var is_cell_selected = false;



//创建 time marix  (list)
export function drawTimeMatrix(svgid, matrixData) {
    //绘制 time matrix
    //console.log(svgid);
    //console.log(matrixData);
    //console.log(d3)

    var svg_width = matrixData.matrix_width * (cell_width + cell_gap) + 10;
    var svg_height = matrixData.matrix_lineCount * (cell_height + cell_gap) + 10;

    var svg = d3.select(`#${svgid}`).attr("width", svg_width).attr("height", svg_height).style("background", "white");

    var cellg = svg.selectAll("g").data(matrixData.cells).enter().append("g");

    cellg.each(function (c, i) {

        var toAppend = d3.select(this);

        //cell 对应对象 rect
        var cell_rec = toAppend.append("rect").attr("x", c => getCellX(c)).attr("y", c => getCellY(c))
            .attr("width", cell_width).attr("height", cell_height).attr("rx", cell_rx)
            .attr("id", `cell_${c.val}`)
            .attr("pointer-events", "all")
            .style("cursor", "pointer")
            .on("mouseover", (e, d) => {

                //若启用选中则不触发高亮
                console.log(is_cell_selected);
                if (is_cell_selected) {
                    return;
                }

                //hover事件，启用事件高亮，并显示简要信息
                highlightCell(e);
                //
                var summary_x = Math.round(e.srcElement.getBoundingClientRect().left + cell_width * 2 / 3);
                var summary_y = Math.round(e.srcElement.getBoundingClientRect().top + cell_width * 2 / 3);
                dotnet_matrixview_objref.invokeMethodAsync('triggerHoverEvent', d.val, summary_x, summary_y);

            })
            .on("mouseout", (e, d) => {

                if (is_cell_selected) {
                    return;
                }

                //离开事件，取消高亮，移除对应简要信息
                deHighlightCell(e)

                dotnet_matrixview_objref.invokeMethodAsync('triggerHoverOutEvent', d.val);

            })
            .on("click", (e, d) => {

                //
                if (is_cell_selected) {
                    return;
                }

                //获取对应cell坐标
                var summary_x = Math.round(e.srcElement.getBoundingClientRect().left + cell_width * 2 / 3);
                var summary_y = Math.round(e.srcElement.getBoundingClientRect().top + cell_width * 2 / 3);

                dotnet_matrixview_objref.invokeMethodAsync('triggerClickEvent', d.val, summary_x, summary_y);

                is_cell_selected = true;
                selectCell(e);

            });

        if (c.bg_events.length == 0) {

            cell_rec.style("stroke-width", "0px").style("fill", "#f7f7f7");

        } else if (c.bg_events.length == 1) {

            cell_rec.style("stroke-width", "0px").style("stroke", "#000000")
                .style("fill", `${c.bg_events[0].color}`);

        } else if (c.bg_events.length == 2) {

            //console.log(toAppend);

            toAppend.append("polygon").attr("points", c => getBGPolygonPoints(c, 0)).attr("pointer-events", "none")
                .style("stroke-width", "0px").style("fill", `${c.bg_events[0].color}`);

            toAppend.append("polygon").attr("points", c => getBGPolygonPoints(c, 1)).attr("pointer-events", "none")
                .style("stroke-width", "0px").style("fill", `${c.bg_events[1].color}`);
        }

    });



    //事件
    var marks_enter = cellg.selectAll('.mark').data(d => { return d.marks }).enter();

    marks_enter.each(function (m, i) {

        var toAppend = d3.select(this);

        if (m.type == "ongoing") {
            //
            toAppend.append('rect').classed('mark', true).classed('esid', true)
                .attr("x", m => getTimelineX(m)).attr("y", m => getTimelineY(m)).attr("width", cell_width).attr("height", timeline_height).style("fill", m => m.color)
                .attr("pointer-events", "none");
        }
        else if (m.type == "start") {
            //
            toAppend.append("polygon").classed('mark', true)
                .attr("points", m => getStartMarkPoints(m)).style("fill", m => m.color)
                .attr("pointer-events", "none");
        }
        else if (m.type == "end") {
            //
            toAppend.append('rect').classed('mark', true)
                .attr("x", m => getTimelineX(m)).attr("y", m => getTimelineY(m)).attr("width", (cell_width / 2)).attr("height", timeline_height).style("fill", m => m.color)
                .attr("pointer-events", "none");
        }
        else if (m.type == "instant") {
            //
            toAppend.append('rect').classed('mark', true)
                .attr("x", m => getInstantEventX(m)).attr("y", m => getInstantEventY(m)).attr("height", instant_height).attr("width", instant_width).attr("rx", instant_rx).style("fill", m => m.color)
                .attr("pointer-events", "none");

        }

    });

}

export function unselectCell(cell_val) {
    //console.log("unselected called");
    //console.log(this);
    //console.log(globalThis);
    is_cell_selected = false;
    d3.select(`#cell_${cell_val}`).style("stroke-width", "0px").style("stroke", "#000000");
}


function getCellX(cell) {
    var x = cell.c * (cell_width + cell_gap) + matrix_padding;
    return x;
}

function getCellY(cell) {
    var y = cell.l * (cell_height + cell_gap) + matrix_padding;
    return y;
}

function getTimelineX(mark) {

    var x = getCellX(mark);
    if (mark.type == "start") {
        x = x + cell_width / 2;
    }
    return x;

}

function getTimelineY(mark) {

    var y = getCellY(mark) + timeline_margin_top + (mark.seq * (timeline_height + timeline_gap));

    return y;

}

function getInstantEventX(mark) {

    var x = getCellX(mark);
    if (mark.slot) {
        x += (mark.slot * (instant_width + instant_gap));
    }
    return x;

}

function getInstantEventY(mark) {
    var y = getCellY(mark);
    y += cell_height - instant_height;
    return y;
}

function getStartMarkPoints(mark) {
    var x = getCellX(mark);
    var y = getCellY(mark);

    var points = `${x + cell_width / 2},${y + timeline_margin_top + mark.seq * (timeline_height + timeline_gap) + timeline_height / 2}`;
    points += ` ${x + cell_width / 2 + cell_width / 5},${y + timeline_margin_top + mark.seq * (timeline_height + timeline_gap)}`;
    points += ` ${x + cell_width},${y + timeline_margin_top + mark.seq * (timeline_height + timeline_gap)}`;
    points += ` ${x + cell_width},${y + timeline_margin_top + mark.seq * (timeline_height + timeline_gap) + timeline_height}`;
    points += ` ${x + cell_width / 2 + cell_width / 5},${y + timeline_margin_top + mark.seq * (timeline_height + timeline_gap) + timeline_height}`;

    return points;
}

function getBGPolygonPoints(cell, idx) {

    var x = getCellX(cell);
    var y = getCellY(cell);

    var points = ""

    if (idx == 0) {
        //points += `${x},${y} `;
        //points += ` ${x + cell_width},${y}`;
        //points += ` ${x},${y + cell_height}`;
        //=============
        points += `${x},${y} `;
        points += `${x + cell_width},${y} `;
        points += `${x + cell_width},${y + cell_height / 2} `;
        points += `${x},${y + cell_height / 2} `;
        //=============
        //points += `${x},${y} `;
        //points += `${x + cell_width / 2},${y} `;
        //points += `${x + cell_width / 2},${y + cell_height} `;
        //points += `${x},${y + cell_height} `;
    } else {
        //points += `${x + cell_width},${y} `;
        //points += ` ${x + +cell_width},${y + cell_height}`;
        //points += ` ${x},${y + cell_height}`;
        //=============
        points += `${x},${y + cell_height / 2} `;
        points += `${x + cell_width},${y + cell_height / 2} `;
        points += `${x + cell_width},${y + cell_height} `;
        points += `${x},${y + cell_height} `;
        //=============
        //points += `${x + cell_width / 2},${y} `;
        //points += `${x + cell_width},${y} `;
        //points += `${x + cell_width},${y + cell_height} `;
        //points += `${x + cell_width / 2},${y + cell_height} `;
    }

    return points;

}


function highlightCell(event) {

    d3.select(event.srcElement).style("stroke-width", "1px").style("stroke", "#929292");

}

function deHighlightCell(event) {

    d3.select(event.srcElement).style("stroke-width", "0px").style("stroke", "#000000");

}


function selectCell(event) {
    //设置选中颜色
    d3.select(event.srcElement).style("stroke-width", "1px").style("stroke", "#ff551d");
}


function showCellSummary(point, cell) {

}


function hideCellSummary(cell) {

}






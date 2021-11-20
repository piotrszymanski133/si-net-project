
function ConvertToCSV(objArray) {
    var array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
    var str = '';

    str += 'value,datetime,hiveId,databaseId,type' + '\r\n';

    for (var i = 0; i < array.length; i++) {
        var line = '';
        for (var index in array[i]) {
            if (line != '') line += ','

            line += array[i][index];
        }

        str += line + '\r\n';
    }

    return str;
}

function createChart(Viewdata){
    let div;
    div = document.createElement('div');
    div.setAttribute('id', 'chartContainer');
    div.style.width = '100%';
    div.style.height = '300px';
    document.getElementById('toAdd').appendChild(div);

    var data = Viewdata;
    var values = data.map(a => a.value)
    var dates = data.map(a => a.dateTime)

    var dps = []

    for (var i = dps.length; i < dates.length; i++)
        dps.push({
            x: new Date(Date.parse(dates[i])),
            y: values[i]
        });

    var chart = new CanvasJS.Chart("chartContainer", {
        theme: "light2",
        title:{
            text: "Chart"
        },
        axisX: {
            title: "Dates"
        },
        axisY: {
            title: "Values"
        },
        data: [{
            type: "line",
            dataPoints: dps
        }]
    });
    chart.render();
}

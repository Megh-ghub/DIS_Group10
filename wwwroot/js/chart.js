function displaychart() {
    var aID = $('#activities').val();
    let f = new FormData();
    f.append("id", aID);
    $.ajax({
        method: "POST",
        url: "/Park/Explore",
        cache: false,
        contentType: false,
        processData: false,
        data: f,
        success: function (chartdata) {
            let states = chartdata[0];
            let parkcount = chartdata[1];
            let activityname = chartdata[2];
            if (activityname == null) {
                activityname = "All Activities"
            }
            let ctx = $("#barchart").get(0).getContext("2d");
            if (window.bar != undefined)
                window.bar.destroy(); 
            window.bar = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: states,
                    datasets: [
                        {
                            label: "Count of Parks",
                            backgroundColor: Array(10).fill(["#3e95cd", "#8e5ea2", "#3cba9f", "#e8c3b9", "#c45850"]).flat(),
                            data: parkcount
                        }
                    ]
                },
                options: {
                    responsive: true,
                    legend: { display: false },
                    title: {
                        display: true,
                        text: 'State-wise count of parks for ' + activityname
                    },
                    scales: {
                        xAxes: [{
                            ticks: {
                                autoSkip: true,
                                minRotation: 90,
                                maxRotation: 90
                            }
                        }],
                        yAxes: [{
                            ticks: {
                                beginAtZero: true,
                                stepSize: 1,
                                precision: 0
                            }
                        }]
                    }
                }
            });
        },
        error: function (req, status, error) {
            console.log(error);
        }
    });
}

$('#activities').change(displaychart);
$(document).ready(displaychart);





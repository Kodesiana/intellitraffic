export function renderVehicleVolumeTimeSeries(data) {
    const dataProj = data.map(x => [new Date(x.date).getTime(), x.total]);
    const minDate = Math.min(...dataProj.map(x => x[0]));
    const maxDate = Math.max(...dataProj.map(x => x[0]));

    new ApexCharts(document.querySelector("#chart-line2"), {
        series: [{
            name: 'Total Vehicles',
            data: dataProj
        }],
        chart: {
            id: 'chart2',
            type: 'line',
            height: 230,
            dropShadow: {
                enabled: true,
                enabledOnSeries: [1]
            },
            toolbar: {
                autoSelected: 'pan',
                show: false
            }
        },
        colors: ['#008FFB', '#00E396'],
        dataLabels: {
            enabled: false
        },
        stroke: {
            width: [2, 6],
            curve: ['straight', 'monotoneCubic']
        },
        fill: {
            opacity: [1, 0.75],
        },
        markers: {
            size: 0
        },
        yaxis: [
            {
                seriesName: 'Total Vehicles',
                axisTicks: {
                    show: true,
                    color: '#008FFB'
                },
                axisBorder: {
                    show: true,
                    color: '#008FFB'
                },
                labels: {
                    style: {
                        colors: '#008FFB',
                    }
                },
                title: {
                    text: "Total Vehicles",
                    style: {
                        color: '#008FFB'
                    }
                },
            }
        ],
        xaxis: {
            type: 'datetime'
        }
    }).render();

    new ApexCharts(document.querySelector("#chart-line"), {
        series: [{
            name: 'Total Vehicles',
            data: dataProj
        }],
        chart: {
            id: 'chart1',
            height: 130,
            type: 'area',
            brush: {
                target: 'chart2',
                enabled: true
            },
            selection: {
                enabled: true,
                xaxis: {
                    min: minDate,
                    max: maxDate,
                }
            },
        },
        colors: ['#008FFB', '#00E396'],
        stroke: {
            width: [1, 3],
            curve: ['straight', 'monotoneCubic']
        },
        fill: {
            type: 'gradient',
            gradient: {
                opacityFrom: 0.91,
                opacityTo: 0.1,
            }
        },
        xaxis: {
            type: 'datetime',
            tooltip: {
                enabled: false
            }
        },
        yaxis: {
            max: 100,
            tickAmount: 2
        }
    }).render();
}

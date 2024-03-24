$(() => {

    var connection = new signalR.HubConnectionBuilder().withUrl("/signalrServer").build();

    connection.start();
    LoadDashboard();
    connection.on("LoadDashboards", function () {
        LoadDashboard();
    });
});

function LoadDashboard() {
    $.ajax({
        url: '/Admin/Dashboards/DashboardData',
        method: 'GET',
        success: (result) => {
            $('#OrderToday').text(result.orderToday);
            $('#ProfitToday').text('$ ' + result.profitToday);
            $('#IncreasePercentProfit').text('+' + result.increasePercentProfit + '%');
            $('#ProductToday').text(result.productToday);
            $('#IncreasePercentProduct').text('+' + result.increasePercentProduct + '%');
            OrderStatics(result.orderStaticsObjects);
            OrderStaticsTable(result.orderStaticsObjects);
            $('#TotalOrderDetails').text(result.totalOrderDetails);
            $('#TotalIncomeInYear').text('$' + result.totalIncomeInYear);
            TotalOrderInYear(result.ordersInMonth);

        },
        error: (error) => {
            console.log(error)
        }
    });
}

function OrderStatics(statics) {
    const chartOrderStatistics = document.querySelector('#orderStatisticsChart'),
        orderChartConfig = {
            chart: {
                height: 165,
                width: 130,
                type: 'donut'
            },
            labels: statics.name,
            series: statics.value,
            stroke: {
                width: 5,
                colors: ['#fff']
            },
            dataLabels: {
                enabled: true,
                formatter: function (val, opt) {
                    return parseInt(val) + '%';
                }
            },
            legend: {
                show: false
            },
            grid: {
                padding: {
                    top: 0,
                    bottom: 0,
                    right: 15
                }
            },
            states: {
                hover: {
                    filter: { type: 'none' }
                },
                active: {
                    filter: { type: 'none' }
                }
            },
            plotOptions: {
                pie: {
                    donut: {
                        size: '75%',
                        labels: {
                            show: true,
                            value: {
                                fontSize: '1.5rem',
                                fontFamily: 'Public Sans',
                                color: '#566a7f',
                                offsetY: -15,
                                formatter: function (val) {
                                    return parseInt(val) + ' p';
                                }
                            },
                            name: {
                                offsetY: 20,
                                fontFamily: 'Public Sans'
                            },
                            total: {
                                show: true,
                                fontSize: '0.8125rem',
                                color: '',
                                label: 'Total',
                            }
                        }
                    }
                }
            }
        };
    if (typeof chartOrderStatistics !== undefined && chartOrderStatistics !== null) {
        const statisticsChart = new ApexCharts(chartOrderStatistics, orderChartConfig);
        statisticsChart.render();
    }
}
function OrderStaticsTable(statics) {
    let result = statics.name.map((item, index) => {
        return {
            name: item,
            price: statics.price[index],
            number: statics.value[index]
        };
    });
    var li = '';
    $.each(result, (k, item) => {
        li += `
            <li class="d-flex mb-4 pb-1">
                        <div class="d-flex w-100 flex-wrap align-items-center justify-content-between gap-2">
                            <div class="me-2">
                                <h6 class="mb-0">${item.name}</h6>
                            </div>
                            <div class="user-progress">
                                <small class="fw-medium">$ ${item.price}</small>
                            </div>
                        </div>
            </li>
        `;
    })
    $("#OrderList").html(li);
}

function TotalOrderInYear(statics) {
    let keys = statics.map(item => item.key);
    let values = statics.map(item => item.value);
    const incomeChartEl = document.querySelector('#incomeChart'),
        incomeChartConfig = {
            series: [
                {
                    data: values
                }
            ],
            chart: {
                height: 215,
                parentHeightOffset: 0,
                parentWidthOffset: 0,
                toolbar: {
                    show: false
                },
                type: 'area'
            },
            dataLabels: {
                enabled: false
            },
            stroke: {
                width: 2,
                curve: 'smooth'
            },
            legend: {
                show: false
            },
            markers: {
                size: 6,
                colors: 'transparent',
                strokeColors: 'transparent',
                strokeWidth: 4,
                discrete: [
                    {
                        fillColor: config.colors.white,
                        seriesIndex: 0,
                        dataPointIndex: 100,
                        strokeColor: config.colors.primary,
                        strokeWidth: 2,
                        size: 6,
                        radius: 8
                    }
                ],
                hover: {
                    size: 7
                }
            },
            colors: [config.colors.primary],
            fill: {
                type: 'gradient',
                gradient: {
                    shadeIntensity: 0.6,
                    opacityFrom: 0.5,
                    opacityTo: 0.25,
                    stops: [0, 95, 100]
                }
            },
            grid: {
                borderColor: '#eceef1',
                strokeDashArray: 3,
                padding: {
                    top: -20,
                    bottom: -8,
                    left: -10,
                    right: 8
                }
            },
            xaxis: {
                categories: keys,
                axisBorder: {
                    show: false
                },
                axisTicks: {
                    show: false
                },
                labels: {
                    show: true,
                    style: {
                        fontSize: '13px',
                    }
                }
            },
            yaxis: {
                labels: {
                    show: false
                },
                min: 0,
                max: 1000,
                tickAmount: 4
            }
        };
    if (typeof incomeChartEl !== undefined && incomeChartEl !== null) {
        const incomeChart = new ApexCharts(incomeChartEl, incomeChartConfig);
        incomeChart.render();
    }
}
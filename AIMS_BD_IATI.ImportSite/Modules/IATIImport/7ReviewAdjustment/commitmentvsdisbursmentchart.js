
angular.module('iatiDataImporter').directive('commitmentvsdisbursmentchart', function () {
    return {
        restrict: 'EC',
        template: '<div></div>',
        replace: true,
        link: function (scope, elem, attrs) {
            var renderChart = function () {
                elem.html('');
                var series = scope.$eval(attrs.series);
                //if (angular.isUndefined(attrs.series)) {
                //    return;
                //}
                var aimsCommitment = scope.$eval(attrs.aimsCommitment);
                var aimsDisbursment = scope.$eval(attrs.aimsDisbursment);
                var iatiCommitment = scope.$eval(attrs.iatiCommitment);
                var iatiDisbursment = scope.$eval(attrs.iatiDisbursment);

                var config = {
                    chart: {
                        type: 'bar',
                        height: 100,
                        //spacing: [0, 0, 0, 0],
                        margin: [0, 50, 0, 100],
                        renderTo : elem[0]
                    },
                    title: {
                        text: 'Comparison for Financial Data',
                        style: { display: 'none' },
                        visible: false
                    },
                    //subtitle: {
                    //    text: 'between AIMS and IATI'
                    //},
                    xAxis: {
                        categories: ['Commitment', 'Disbursement'],
                        title: {
                            text: null
                        }
                    },
                    yAxis: {
                        gridLineWidth: 0,
                        "startOnTick": true,
                        title: null,
                        labels: {
                            enabled: false
                        },
                        visible: false

                    },

                    tooltip: {
                        valueSuffix: ' USD'
                    },
                    plotOptions: {
                        //bar: {
                        //    dataLabels: {
                        //        enabled: true
                        //    }
                        //}
                    },
                    legend: {
                        layout: 'vertical',
                        align: 'right',
                        verticalAlign: 'top',
                        x: 0,
                        y: 0,
                        floating: true,
                        borderWidth: 0,
                        backgroundColor: ((Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'),
                        shadow: true
                    },
                    credits: {
                        enabled: false
                    },
                    series: [{
                        name: 'AIMS',
                        data: [aimsCommitment, aimsDisbursment],
                        color: '#a94442'
                    }, {
                        name: 'IATI',
                        data: [iatiCommitment, iatiDisbursment],
                        color: '#3071a9'
                    }]
                };

                var chart = new Highcharts.Chart(config);

                chart.redraw();
            };

            renderChart();
        }
    };
});

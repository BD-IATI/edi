angular.module('iatiDataImporter').controller("7ReviewAdjustmentController", function ($rootScope, $scope, $http, $uibModal, $timeout) {

    $http({
        method: 'POST',
        url: apiprefix + '/api/IATIImport/GetProjectsToMap',
        data: JSON.stringify($rootScope.GeneralPreference)
    }).success(function (result) {
        $scope.models = result;
    });

    $scope.disbursmentDiff = function (m) {
        var numerator = m.iatiActivity.TotalDisbursmentThisDPOnly >= m.aimsProject.TotalDisbursmentThisDPOnly ? m.iatiActivity.TotalDisbursmentThisDPOnly : m.aimsProject.TotalDisbursmentThisDPOnly;

        var denominator = m.iatiActivity.TotalDisbursmentThisDPOnly < m.aimsProject.TotalDisbursmentThisDPOnly ? m.iatiActivity.TotalDisbursmentThisDPOnly : m.aimsProject.TotalDisbursmentThisDPOnly;

        return (numerator / denominator) * 100;
    }
    $scope.commitmentDiff = function (m) {
        var numerator = m.iatiActivity.TotalCommitmentThisDPOnly >= m.aimsProject.TotalCommitmentThisDPOnly ? m.iatiActivity.TotalCommitmentThisDPOnly : m.aimsProject.TotalCommitmentThisDPOnly;

        var denominator = m.iatiActivity.TotalCommitmentThisDPOnly < m.aimsProject.TotalCommitmentThisDPOnly ? m.iatiActivity.TotalCommitmentThisDPOnly : m.aimsProject.TotalCommitmentThisDPOnly;

        return (numerator / denominator) * 100;

    }
    $scope.isDiffGT5 = function (mkl) {

        return Math.abs(($scope.disbursmentDiff(mkl) - $scope.commitmentDiff(mkl)) / 2) > 5;
    }

    $scope.OpenProjectSpecificAdjustment = function (MatchedProject) {
        var modalInstance = $uibModal.open({
            animation: true,
            backdrop: false,
            templateUrl: '8ProjectSpecificAdjustment/8ProjectSpecificAdjustmentView.html',
            controller: '8ProjectSpecificAdjustmentController',
            size: 'lg',
            resolve: {
                MatchedProject: function () {

                    return MatchedProject;
                }
            }
        });

        //modalInstance.result.then(function (selectedItem) {
        //    $scope.selected = selectedItem;
        //}, function () {
        //    //$log.info('Modal dismissed at: ' + new Date());
        //});
    };

    $scope.OpenTransactionDetail = function (MatchedProject) {
        var modalInstance = $uibModal.open({
            animation: true,
            backdrop: false,
            templateUrl: 'Transaction/TransactionView.html',
            controller: 'TransactionController',
            size: 'lg',
            resolve: {
                MatchedProject: function () {

                    return MatchedProject;
                }
            }
        });

        //modalInstance.result.then(function (selectedItem) {
        //    $scope.selected = selectedItem;
        //}, function () {
        //    //$log.info('Modal dismissed at: ' + new Date());
        //});
    };

    $scope.ImportProjects = function () {
        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/ImportProjects',
            data: JSON.stringify($scope.models)
        }).success(function (result) {
            $timeout(function () {
                alert("Projects are imported.");
                document.getElementById('btnGoDashboard').click(); //redirect
            });
        });

    }

});


iatiDataImporterApp.directive('commitmentvsdisbursmentchart', function () {
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
                    }
                };

                //config.series = series;
                config.series = [{
                    name: 'AIMS',
                    data: [aimsCommitment, aimsDisbursment],
                    color: '#a94442'
                }, {
                    name: 'IATI',
                    data: [iatiCommitment, iatiDisbursment],
                    color: '#3071a9'
                }];

                config.chart.renderTo = elem[0];

                var chart = new Highcharts.Chart(config);

                chart.redraw();
            };

            renderChart();
            //scope.$watch(attrs.uiChart, function () {
            //    renderChart();
            //}, true);

            //scope.$watch(attrs.chartOptions, function () {
            //    renderChart();
            //});
        }
    };
});


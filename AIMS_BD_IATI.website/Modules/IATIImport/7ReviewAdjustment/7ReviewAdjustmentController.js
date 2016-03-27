angular.module('iatiDataImporter').controller("7ReviewAdjustmentController", function ($rootScope, $scope, $http, $uibModal) {
    $http({
        method: 'POST',
        url: apiprefix + '/api/IATIImport/GetProjectsToMap',
        data: JSON.stringify($rootScope.GeneralPreference)
    }).success(function (result) {
        $scope.models = result;
    });


    $scope.disbursmentDiff = function (m) {
        return ((m.iatiActivity.TotalDisbursment + 1) / (m.aimsProject.TotalDisbursment + 1)) * 100;
    }
    $scope.commitmentDiff = function (m) {
        return ((m.iatiActivity.TotalCommitment + 1) / (m.aimsProject.TotalCommitment + 1)) * 100;
    }
    $scope.isDiffGT5 = function (mkl) {
        
        return Math.abs($scope.disbursmentDiff(mkl) - $scope.commitmentDiff(mkl)) > 5;
    }


    $scope.myChartOpts = { 
      seriesDefaults: {
        // Make this a pie chart.
        renderer: jQuery.jqplot.PieRenderer, 
        rendererOptions: {
          // Put data labels on the pie slices.
          // By default, labels show the percentage of the slice.
          showDataLabels: true
        }
      }, 
      legend: { show:true, location: 'e' }
    };

    $scope.someData = [[
      ['Heavy Industry', 12],['Retail', 9], ['Light Industry', 14], 
      ['Out of home', 16],['Commuting', 7], ['Orientation', 9]
    ]];

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

    $scope.ImportProjects = function () {
        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/ImportProjects',
            data: JSON.stringify($scope.models)
        }).success(function (result) {
            alert("Projects are imported.");
        });

    }
});

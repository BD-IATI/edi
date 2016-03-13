angular.module('iatiDataImporter').controller("7ReviewAdjustmentController", function ($rootScope, $scope, $http, $uibModal) {
    $http({
        method: 'POST',
        url: apiprefix + '/api/IATIImport/GetProjectsToMap',
        data: JSON.stringify($rootScope.GeneralPreference)
    }).success(function (result) {
        $scope.models = result;
    });

    $scope.isDiffGT5 = function (mkl) {
        return ((mkl.iatiActivity.TotalDisbursment + 1) / (mkl.aimsProject.TotalDisbursment + 1)) * 100 > 5;
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


});

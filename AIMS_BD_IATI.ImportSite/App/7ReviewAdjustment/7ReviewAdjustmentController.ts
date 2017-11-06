/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Modules/Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />


angular.module('iatiDataImporter').controller("7ReviewAdjustmentController", function ($rootScope: RootScopeModel, $scope, $http, $uibModal, $timeout) {
    //$('#divView').slimScroll({ scrollTo: '0px' });

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

        var diff = (numerator / denominator) * 100;
        return diff;
    }
    $scope.commitmentDiff = function (m) {
        var numerator = m.iatiActivity.TotalCommitmentThisDPOnly >= m.aimsProject.TotalCommitmentThisDPOnly ? m.iatiActivity.TotalCommitmentThisDPOnly : m.aimsProject.TotalCommitmentThisDPOnly;

        var denominator = m.iatiActivity.TotalCommitmentThisDPOnly < m.aimsProject.TotalCommitmentThisDPOnly ? m.iatiActivity.TotalCommitmentThisDPOnly : m.aimsProject.TotalCommitmentThisDPOnly;

        var diff = (numerator / denominator) * 100;
        return diff;
    }
    $scope.isDiffGT5 = function (mkl) {

        var cDiff = $scope.commitmentDiff(mkl);
        var dDiff = $scope.disbursmentDiff(mkl);
        var avgDiff = ((dDiff + cDiff) / 2);

        return avgDiff > 120; //difference tolerance %
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

    $scope.UnlinkProject = function (MatchedProject) {

        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/UnlinkProject',
            data: JSON.stringify(MatchedProject)
        }).success(function (result) {

            var index = $scope.models.MatchedProjects.indexOf(MatchedProject, 0);
            if (index > -1) {
                $scope.models.MatchedProjects.splice(index, 1);
            }

        });


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




/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />


angular.module('iatiDataImporter').controller("8ProjectSpecificAdjustmentController", function ($rootScope: RootScopeModel, $timeout, $uibModalInstance, $scope, $http, MatchedProject) {

    $scope.model = MatchedProject;

    $scope.ok = function () {

        $uibModalInstance.close();
    };

        $scope.Save = function () {
        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/SaveActivityPreferences',
            data: JSON.stringify($scope.model),
            dataType: 'json'
        }).then(function (result) {
            if (result.data != null || result.data != undefined) {
                $timeout(function () {
                    $uibModalInstance.close();
                });
            }
            else {
                alert('Something wrong happening!');
            }
        },
    function (response) {
        //deferred.reject(response);
    });


    }

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});

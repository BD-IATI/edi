/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />


angular.module('iatiDataImporter').controller("9CofinancingController", function ($rootScope : RootScopeModel, $timeout, $scope, $http, $uibModalInstance, model) {
        $scope.AssignedActivities = $rootScope.AssignedActivities;
    if (model == undefined || model == null) {

        $http({
            method: 'POST',
            url: apiprefix + '/api/CFnTF/SubmitAssignedActivities',
            data: JSON.stringify($rootScope.AssignedActivities)
        }).success(function (result) {
            $scope.model = result;
        });
    } else {
        $scope.model = model;
    }
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };


    $scope.SavePreferences = function () {
        $http({
            method: 'POST',
            url: apiprefix + '/api/CFnTF/SavePreferences',
            data: JSON.stringify($scope.model)
        }).success(function (result) {
            alert('saved')
            //$scope.model = result;
        });
    }


    $scope.GetSumOfPlannedDisbursments = function (prjArray) {
        var sum = 0;
        for (var i in prjArray) {
            sum = sum + Number(prjArray[i].TotalPlannedDisbursment);
        }
        return sum;
    };

    $scope.ChangeIsCommitmentIncluded = function (prjArray) {
        for (var i = 1; i < prjArray.length; i++) {
            prjArray[i].IsCommitmentIncluded = !prjArray[i].IsCommitmentIncluded;
        }
    };
    $scope.ChangeIsDisbursmentIncluded = function (prjArray) {
        for (var i = 1; i < prjArray.length; i++) {
            prjArray[i].IsDisbursmentIncluded = !prjArray[i].IsDisbursmentIncluded;
        }
    };
    $scope.ChangeIsPlannedDisbursmentIncluded = function (prjArray) {
        for (var i = 1; i < prjArray.length; i++) {
            prjArray[i].IsPlannedDisbursmentIncluded = !prjArray[i].IsPlannedDisbursmentIncluded;
        }
    };

});



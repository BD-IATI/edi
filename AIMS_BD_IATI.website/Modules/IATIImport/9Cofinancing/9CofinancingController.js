angular.module('iatiDataImporter').controller("9CofinancingController", function ($rootScope, $timeout, $scope, $http) {
    $scope.AssignedActivities = $rootScope.AssignedActivities;
    //$scope.Projects = $rootScope.Projects;
    //$scope.TrustFunds = $rootScope.TrustFunds;

    //$http({
    //    method: 'GET',
    //    url: apiprefix + '/api/CFnTF/GetTrustFundDetails',
    //    params: { trustFundId: Activity.MappedTrustFundId }
    //}).success(function (result) {
    //    $scope.TrustFundDetails = result;
    //});

    $http({
        method: 'POST',
        url: apiprefix + '/api/CFnTF/SubmitAssignedActivities',
        data: JSON.stringify($rootScope.AssignedActivities)
    }).success(function (result) {
        $scope.model = result;
    });

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
    $scope.GetSumOfCommitments = function (prjArray) {
        var sum = 0;
        for (var i in prjArray) {
            sum = sum + Number(prjArray[i].TotalCommitment);
        }
        return sum;
    };
    $scope.GetSumOfDisbursments = function (prjArray) {
        var sum = 0;
        for (var i in prjArray) {
            sum = sum + Number(prjArray[i].TotalDisbursment);
        }
        return sum;
    };
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



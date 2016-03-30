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

});



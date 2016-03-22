angular.module('iatiDataImporter').controller("DashboardController", function ($rootScope, $scope, $http) {
    $http({
        method: 'GET',
        url: apiprefix + '/api/Dashboard/GetDashboardData',
        //data: { applicationId: 3 }
    }).success(function (result) {
        $scope.model = result;
    });

});

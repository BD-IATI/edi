angular.module('iatiDataImporter').controller("3FilterDPController", function ($rootScope, $scope, $http) {
    $scope.ImplementingOrgs = [];
    $scope.RelevantActivities = [];

    $http({
        url: apiprefix + '/api/ApiHome/GetAllImplementingOrg',
        method: 'POST',
        dataType: 'json',
        data: JSON.stringify($rootScope.filterBDModel)
    }).then(function (result) {
        $scope.ImplementingOrgs = result.data.Orgs;
        $scope.FundSources = result.data.FundSources;
        $rootScope.filterBDModel = null;
        //deferred.resolve(result);
    },
    function (response) {
        //deferred.reject(response);
    });

    $scope.FilterDP = function () {

        $http({
            url: apiprefix + '/api/ApiHome/FilterDP',
            method: 'POST',
            data: JSON.stringify($scope.ImplementingOrgs),
            dataType: 'json'
        }).then(function (result) {
            $rootScope.RelevantActivities = $scope.RelevantActivities = result.data;
            //deferred.resolve(result);
        },
        function (response) {
            //deferred.reject(response);
        });

    };

});

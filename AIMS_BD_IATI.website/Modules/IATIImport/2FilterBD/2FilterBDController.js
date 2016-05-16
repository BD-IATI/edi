angular.module('iatiDataImporter').controller("2FilterBDController", function ($rootScope, $scope, $http) {
    $scope.activeTabIndex = 0;
    $scope.setTabIndex = function (index) { $scope.activeTabIndex = index; };

    $http({
        url: apiprefix + '/api/IATIImport/SubmitHierarchy',
        method: 'POST',
        data: JSON.stringify($rootScope.hierarchyModel),
        dataType: 'json'
    }).then(function (result) {
        $rootScope.filterBDModel = $scope.model = result.data;
        $rootScope.hierarchyModel = null;
        //deferred.resolve(result);
    },
    function (response) {
        //deferred.reject(response);
    });


});

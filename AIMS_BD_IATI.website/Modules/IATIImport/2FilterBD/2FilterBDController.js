angular.module('iatiDataImporter').controller("2FilterBDController", function ($rootScope, $scope, $http) {

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

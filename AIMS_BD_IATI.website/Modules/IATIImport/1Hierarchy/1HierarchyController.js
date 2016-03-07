angular.module('iatiDataImporter').controller("1HierarchyController", function ($rootScope, $scope, $http, $timeout) {

    $rootScope.HasRelatedActivity = false;
    $http({
        method: 'POST',
        url: apiprefix + '/api/ApiHome/GetHierarchyData',
        dataType: 'json',
        data: JSON.stringify($rootScope.selectedFundSource)
    }).success(function (result) {
        if (result == null || result == undefined) {
            $timeout(function () {
                document.getElementById('btn2FilterBD').click(); //redirect
            });
        }
        else {
            $rootScope.hierarchyModel = $scope.model = result;
        }
    });

});

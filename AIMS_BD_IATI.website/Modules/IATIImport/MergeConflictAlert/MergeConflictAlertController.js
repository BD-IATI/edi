angular.module('iatiDataImporter').controller("MergeConflictAlertController", function ($rootScope, $timeout, $uibModalInstance, $scope, $http, iatiIdentifier) {


    $http({
        method: 'GET',
        url: apiprefix + '/api/IATIImport/GetMatchedProjectByIatiIdentifier',
        dataType: 'json',
        params: { iatiIdentifier: iatiIdentifier }
    }).success(function (result) {
        $scope.model = result;
    });


    $scope.ok = function () {

        $uibModalInstance.close();
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});
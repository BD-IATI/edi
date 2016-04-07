angular.module('iatiDataImporter').controller("MergeConflictAlertController", function ($rootScope, $timeout, $uibModalInstance, $scope, $http, iatiIdentifier) {


    $http({
        method: 'GET',
        url: apiprefix + '/api/IATIImport/GetMatchedProjectByIatiIdentifier',
        dataType: 'json',
        params: { iatiIdentifier: iatiIdentifier }
    }).then(function (result) {
        
    },
    function (response) {
        
    });


    $scope.ok = function () {

        $uibModalInstance.close();
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});
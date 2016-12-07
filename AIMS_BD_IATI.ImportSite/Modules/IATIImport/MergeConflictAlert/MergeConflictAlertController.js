angular.module('iatiDataImporter').controller("MergeConflictAlertController", function ($rootScope, $timeout, $uibModalInstance, $scope, $http, log) {
    $scope.ActionType = '';

    $http({
        method: 'GET',
        url: apiprefix + '/api/IATIImport/GetMatchedProjectByIatiIdentifier',
        dataType: 'json',
        params: { iatiIdentifier: log.IatiIdentifier }
    }).success(function (result) {
        $scope.model = result;
    });

    $scope.resolve = function () {
        if ($scope.ActionType == 'option1') {
            $http({
                method: 'POST',
                url: apiprefix + '/api/IATIImport/UpdateTransactionByForce',
                dataType: 'json',
                data: JSON.stringify(log)
            }).success(function (result) {
                
            });
        }
        else if ($scope.ActionType = 'option2')
        {
            $http({
                method: 'POST',
                url: apiprefix + '/api/IATIImport/SetIgnoreActivity',
                dataType: 'json',
                data: JSON.stringify(log)
            }).success(function (result) {
                

            });

        }
        else
        {

        }
        $uibModalInstance.close(log);
    };

    $scope.ok = function () {

        $uibModalInstance.close();
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});
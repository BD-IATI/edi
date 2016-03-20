angular.module('iatiDataImporter').controller("9TFnCFController", function ($rootScope,$uibModalInstance, $timeout, $scope, $http,Activity,Project) {
    $scope.Activity = Activity;
    $scope.Project = Project;

    $scope.ok = function () {
        $uibModalInstance.close();
    };


    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});

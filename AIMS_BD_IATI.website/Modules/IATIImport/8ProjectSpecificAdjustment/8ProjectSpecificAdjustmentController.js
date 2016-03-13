angular.module('iatiDataImporter').controller("8ProjectSpecificAdjustmentController", function ($rootScope,$uibModalInstance, $scope, MatchedProject) {

    $scope.model = MatchedProject;

    $scope.ok = function () {
        $uibModalInstance.close();
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});

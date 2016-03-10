angular.module('iatiDataImporter').controller("8ProjectSpecificAdjustmentController", function ($rootScope,$uibModalInstance, $scope, mkl) {

    $scope.mkl = mkl;

    $scope.ok = function () {
        $uibModalInstance.close($scope.selected.item);
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});

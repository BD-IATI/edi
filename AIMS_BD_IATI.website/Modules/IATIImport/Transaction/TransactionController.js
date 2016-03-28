angular.module('iatiDataImporter').controller("TransactionController", function ($rootScope,$timeout,$uibModalInstance, $scope,$http, MatchedProject) {

    $scope.model = MatchedProject;

    $scope.ok = function () {

        $uibModalInstance.close();
    };


    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});

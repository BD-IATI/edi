/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Modules/Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />


angular.module('iatiDataImporter').controller("DownloadLogViewerController", function ($rootScope: RootScopeModel, $scope, $http, $timeout, $uibModalInstance, model) {
    $scope.model = model;


    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});

angular.module('iatiDataImporter')
    .filter('toTrusted', ['$sce', function ($sce) {
        return function (text) {
            return $sce.trustAsHtml(text);
        };
    }]);
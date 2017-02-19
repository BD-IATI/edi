/// <reference path="../../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../IatiImportApp.ts" />
/// <reference path="../../authentication/definitions.d.ts" />

angular.module('iatiDataImporter').controller("1HierarchyController", function ($rootScope : RootScopeModel, $scope, $http, $timeout) {
    //$rootScope.hierarchyModel = null;
    //$rootScope.HasChildActivity = false;
    //$('#divView').slimScroll({ scrollTo: '0px' });


    $http({
        method: 'POST',
        url: apiprefix + '/api/Dashboard/CheckSession',

        data: JSON.stringify($rootScope.getCookie('selectedFundSource'))

    }).success(function (result) {
        $timeout(function () {
            if (result == '/0Begin') {
                $scope.GetHierarchyData();
            }
            else {
                location.hash = result;
            }
        });

    });

    $scope.GetHierarchyData = () => {
        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/GetHierarchyData',
            dataType: 'json',
            data: JSON.stringify($rootScope.getCookie('selectedFundSource'))
        }).success(function (result) {
            if (result == null || result == undefined) {
                $rootScope.hierarchyModel = null;
                $rootScope.HasChildActivity = false;

                $timeout(function () {
                    document.getElementById('btn2FilterBD').click(); //redirect
                });
            }
            else {
                $rootScope.hierarchyModel = $scope.model = result;
                $rootScope.HasChildActivity = true;

            }
        });
    };

});

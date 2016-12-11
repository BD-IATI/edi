/// <reference path="../../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../IatiImportApp.ts" />
/// <reference path="../../../scripts/typings/jquery.slimscroll/jquery.slimscroll.d.ts" />

angular.module('iatiDataImporter').controller("2FilterBDController", function ($rootScope : RootScopeModel, $scope, $http, $timeout) {
    $('#divView').slimScroll({ scrollTo: '0px' });

    $scope.activeTabIndex = 0;
    $scope.setTabIndex = function (index) { $scope.activeTabIndex = index; };
    $scope.nextFromTab0 = function () {
        if ($rootScope.HasChildActivity)
            $scope.activeTabIndex = 1;
        else $timeout(function () {
            document.getElementById('btn3FilterDP').click(); //redirect
        });

    };

    //var selectedHierarchy = $rootScope.hierarchyModel ? $rootScope.hierarchyModel.SelectedHierarchy : null;

    $http({
        url: apiprefix + '/api/IATIImport/SubmitHierarchy',
        method: 'POST',
        data: JSON.stringify($rootScope.hierarchyModel),
        dataType: 'json'
    }).then(function (result) {
        if (result.data != null) {
            if (result.data.AllExtendingOrgs)
                result.data.AllExtendingOrgs.forEach(e => e.IsRelevant = true);
            $rootScope.filterBDModel = $scope.model = result.data;
        }
        else
            $scope.model = $rootScope.filterBDModel;

        //$rootScope.hierarchyModel = null;
        //deferred.resolve(result);
    },
        function (response) {
            //deferred.reject(response);
        });

    $scope.filterByExtendingOrg = function () {
        var acts = $scope.model.iatiActivities as Array<any>;
        var selectedExtOrgs = $scope.model.AllExtendingOrgs.filter(s => s.IsRelevant == true);
        for (var act of acts) {
            act.IsRelevant = false;
            if (act.ExtendingOrgs) {
                for (var eOrg of selectedExtOrgs) {
                    if (act.ExtendingOrgs.filter(fe => fe.Name == eOrg.Name).length > 0) {
                        act.IsRelevant = act.PercentToBD >= 20 && act.ActivityStatus == "Implementation";
                    }
                }

            }
        }
    };
});

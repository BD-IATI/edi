/// <reference path="../../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../iatiimportapp.ts" />
/// <reference path="../../../scripts/typings/jquery.slimscroll/jquery.slimscroll.d.ts" />
angular.module('iatiDataImporter').controller("2FilterBDController", function ($rootScope, $scope, $http, $timeout) {
    $('#divView').slimScroll({ scrollTo: '0px' });
    $scope.activeTabIndex = 0;
    $scope.setTabIndex = function (index) { $scope.activeTabIndex = index; };
    $scope.nextFromTab0 = function () {
        if ($rootScope.HasChildActivity)
            $scope.activeTabIndex = 1;
        else
            $timeout(function () {
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
                result.data.AllExtendingOrgs.forEach(function (e) { return e.IsRelevant = true; });
            $rootScope.filterBDModel = $scope.model = result.data;
        }
        else
            $scope.model = $rootScope.filterBDModel;
        //$rootScope.hierarchyModel = null;
        //deferred.resolve(result);
    }, function (response) {
        //deferred.reject(response);
    });
    $scope.filterByExtendingOrg = function () {
        var acts = $scope.model.iatiActivities;
        var selectedExtOrgs = $scope.model.AllExtendingOrgs.filter(function (s) { return s.IsRelevant == true; });
        for (var _i = 0, acts_1 = acts; _i < acts_1.length; _i++) {
            var act = acts_1[_i];
            act.IsRelevant = false;
            if (act.ExtendingOrgs) {
                for (var _a = 0, selectedExtOrgs_1 = selectedExtOrgs; _a < selectedExtOrgs_1.length; _a++) {
                    var eOrg = selectedExtOrgs_1[_a];
                    if (act.ExtendingOrgs.filter(function (fe) { return fe.Name == eOrg.Name; }).length > 0) {
                        act.IsRelevant = act.PercentToBD >= 20 && act.ActivityStatus == "Implementation";
                    }
                }
            }
        }
    };
});

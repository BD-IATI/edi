angular.module('iatiDataImporter').controller("2FilterBDController", function ($rootScope, $scope, $http, $timeout) {
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
        if (result.data != null)
            $rootScope.filterBDModel = $scope.model = result.data;
        else
            $scope.model = $rootScope.filterBDModel;

        //$rootScope.hierarchyModel = null;
        //deferred.resolve(result);
    },
    function (response) {
        //deferred.reject(response);
    });


});

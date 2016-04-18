angular.module('iatiDataImporter').controller("3FilterDPController", function ($rootScope, $scope, $http, $timeout) {
    $scope.ImplementingOrgs = [];
    $scope.RelevantActivities = [];

    $http({
        url: apiprefix + '/api/IATIImport/GetAllImplementingOrg',
        method: 'POST',
        dataType: 'json',
        data: JSON.stringify($rootScope.filterBDModel)
    }).then(function (result) {
        $scope.ImplementingOrgs = result.data.Orgs;
        $scope.FundSources = result.data.FundSources;
        $rootScope.filterBDModel = null;
        //deferred.resolve(result);
    },
    function (response) {
        //deferred.reject(response);
    });

    $scope.FilterDP = function () {

        $http({
            url: apiprefix + '/api/IATIImport/FilterDP',
            method: 'POST',
            data: JSON.stringify($scope.ImplementingOrgs),
            dataType: 'json'
        }).then(function (result) {
            $rootScope.RelevantActivities = $scope.RelevantActivities = result.data;
            //deferred.resolve(result);
        },
        function (response) {
            //deferred.reject(response);
        });

    };

    $scope.hasOtherDPsProject = false;
    $scope.onFundSourceChanged = function () {
        var hasOtherDPsProject = false;
        for (var i = 0; i < $scope.RelevantActivities.length; i++) {
            if ($scope.RelevantActivities[i].FundSourceIDnIATICode != $rootScope.getCookie('selectedFundSource').IDnIATICode)
            {
                hasOtherDPsProject = true;
                break;
            }
        }

        $scope.hasOtherDPsProject = hasOtherDPsProject;
    };

    $scope.SaveAndNext = function () {
        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/AssignActivities',
            data: JSON.stringify($scope.RelevantActivities),
            dataType: 'json'
        }).then(function (result) {
            if (result.data != null || result.data != undefined) {
                $timeout(function () {
                    document.getElementById('btn4Projects').click(); //redirect
                });
            }
            else {
                alert('Something wrong happening!');
            }
        },
    function (response) {
        //deferred.reject(response);
    });

    }
    $scope.NextWithoutSaving = function () {
        $timeout(function () {
            document.getElementById('btn4Projects').click(); //redirect
        });
    }

});

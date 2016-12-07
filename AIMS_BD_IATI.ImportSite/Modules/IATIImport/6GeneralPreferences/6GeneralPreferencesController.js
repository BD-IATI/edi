angular.module('iatiDataImporter').controller("6GeneralPreferencesController", function ($rootScope, $scope, $http, $timeout,$uibModalInstance) {
    $rootScope.GeneralPreference = {};
    $http({
        method: 'GET',
        url: apiprefix + '/api/IATIImport/GetGeneralPreferences',
    }).success(function (result) {
        $rootScope.GeneralPreference = $scope.model = result;
    });

    $scope.Save = function () {
        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/SaveGeneralPreferences',
            data: JSON.stringify($rootScope.GeneralPreference),
            dataType: 'json'
        }).then(function (result) {
            if (result.data != null || result.data != undefined) {
                $timeout(function () {
                    $uibModalInstance.dismiss('cancel');
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

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.SaveAndNext = function () {
        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/SaveGeneralPreferences',
            data: JSON.stringify($rootScope.GeneralPreference),
            dataType: 'json'
        }).then(function (result) {
            if (result.data != null || result.data != undefined) {
                $timeout(function () {
                    document.getElementById('btn7ReviewAdjustment').click(); //redirect
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
            document.getElementById('btn7ReviewAdjustment').click(); //redirect
        });

    }

});

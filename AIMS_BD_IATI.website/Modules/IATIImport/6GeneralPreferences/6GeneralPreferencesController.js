angular.module('iatiDataImporter').controller("6GeneralPreferencesController", function ($rootScope, $scope, $http) {
    $rootScope.GeneralPreference = {};
    $http({
        method: 'GET',
        url: apiprefix + '/api/IATIImport/GetGeneralPreferences',
    }).success(function (result) {
        $rootScope.GeneralPreference = $scope.model = result;
    });

});

angular.module('iatiDataImporter').controller("6GeneralPreferencesController", function ($rootScope, $scope, $http) {
    $http({
        method: 'GET',
        url: apiprefix + '/api/IATIImport/GetGeneralPreferences',
        //data: { applicationId: 3 }
    }).success(function (result) {
        $scope.model = result;
    });


});

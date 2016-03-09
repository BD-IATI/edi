angular.module('iatiDataImporter').controller("6GeneralPreferencesController", function ($rootScope, $scope, $http) {
    $http({
        method: 'GET',
        url: apiprefix + '/api/IATIImport/GetGeneralPreferences',
        //data: { applicationId: 3 }
    }).success(function (result) {
        $scope.model = result;
    });

    $scope.FieldClick = function (e,field) {
        var tJqObj = $(e.currentTarget);

        if (field.Source == 'IATI')
        {
            field.Source = 'AIMS';
        }
        else
        {
            field.Source = 'IATI';
        }

        if (tJqObj.hasClass("alert-success")) {
            tJqObj.removeClass("alert-success").addClass("alert-danger");
            tJqObj.siblings().removeClass("alert-danger").addClass("alert-success");
        } else {
            tJqObj.removeClass("alert-danger").addClass("alert-success");
            tJqObj.siblings().removeClass("alert-success").addClass("alert-danger");
        }
    }

});

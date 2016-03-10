angular.module('iatiDataImporter').controller("5MatchController", function ($rootScope, $scope, $http) {
    $scope.models = $rootScope.models;

    $scope.Commands = {
        saveData: function () {
            $http({
                url: apiprefix + '/api/IATIImport/PostData',
                method: 'POST',
                data: JSON.stringify($scope.models),
                dataType: 'json'
            }).then(function (result) {
                //deferred.resolve(result);
            },
            function (response) {
                //deferred.reject(response);
            });

        }

    };


});

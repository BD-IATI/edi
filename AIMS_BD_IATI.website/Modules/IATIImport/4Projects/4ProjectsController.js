angular.module('iatiDataImporter').controller("4ProjectsController", function ($rootScope, $scope, $http) {

    $http({
        url: apiprefix + '/api/ApiHome/SubmitActivities',
        method: 'POST',
        data: JSON.stringify($rootScope.RelevantActivities),
        dataType: 'json'
    }).then(function (result) {
        $rootScope.models = $scope.models = result.data;
        $rootScope.RelevantActivities = null;
        //deferred.resolve(result);
    },
    function (response) {
        //deferred.reject(response);
    });

});

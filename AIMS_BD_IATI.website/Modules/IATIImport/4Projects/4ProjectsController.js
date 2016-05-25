angular.module('iatiDataImporter').controller("4ProjectsController", function ($rootScope, $scope, $http) {

    $http({
        url: apiprefix + '/api/IATIImport/SubmitActivities',
        method: 'POST',
        data: JSON.stringify($rootScope.RelevantActivities),
        dataType: 'json'
    }).then(function (result) {
        var projects = result.data.AimsProjectsNotInIati;
        $scope.AimsProjectsDrpSrc = [];

        for (var i = 0; i < projects.length; i++) {

            $scope.AimsProjectsDrpSrc.push({ ProjectId: projects[i].ProjectId, Title: projects[i].Title });

        }

        $scope.AimsProjectsDrpSrc.push({ ProjectId: -2, Title: 'Create New' });

        $rootScope.models = $scope.models = result.data;

        $rootScope.RelevantActivities = null;
        //deferred.resolve(result);
    },
    function (response) {
        //deferred.reject(response);
    });

});

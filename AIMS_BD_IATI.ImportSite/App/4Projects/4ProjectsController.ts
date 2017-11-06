/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Modules/authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />


angular.module('iatiDataImporter').controller("4ProjectsController", function ($rootScope: RootScopeModel, $scope, $http, $timeout) {
    //$('#divView').slimScroll({ scrollTo: '0px' });

    $http({
        url: apiprefix + '/api/IATIImport/SubmitActivities',
        method: 'POST',
        data: JSON.stringify($rootScope.RelevantActivities),
        dataType: 'json'
    }).then(function (result) {
        //var projects = result.data.AimsProjectsNotInIati;
        //var matchedProjects = result.data.MatchedProjects;

        $scope.AimsProjectsDrpSrc = result.data.AimsProjectsDrpSrc;

        //for (var i = 0; i < projects.length; i++) {

        //    $scope.AimsProjectsDrpSrc.push({ ProjectId: projects[i].ProjectId, Title: projects[i].Title });
        //}

        //for (var i = 0; i < matchedProjects.length; i++) {
        //    $scope.AimsProjectsDrpSrc.push({ ProjectId: matchedProjects[i].aimsProject.ProjectId, Title: matchedProjects[i].aimsProject.Title });
        //}

        $scope.AimsProjectsDrpSrc.push({ ID: -2, Name: 'Create New' });

        $rootScope.models = $scope.models = result.data;

        //$rootScope.RelevantActivities = null;
        //deferred.resolve(result);
    },
    function (response) {
        //deferred.reject(response);
    });


    $scope.saveData= function () {
            $http({
                url: apiprefix + '/api/IATIImport/SubmitManualMatchingUsingDropdown',
                method: 'POST',
                data: JSON.stringify($scope.models),
                dataType: 'json'
            }).then(function (result) {
                $timeout(function () {
                    document.getElementById('btn6GeneralPreferences').click(); //redirect
                });

                //deferred.resolve(result);
            },
            function (response) {
                //deferred.reject(response);
            });

        };
});

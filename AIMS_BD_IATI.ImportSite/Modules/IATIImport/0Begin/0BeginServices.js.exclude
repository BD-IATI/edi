'use strict';

angular.module('iatiDataImporter').controller("0BeginController", function ($rootScope, $scope, $http) {
    $rootScope.models = {
        selected: false,
        IatiActivities: [],
        MatchedProjects: [],
        IatiActivitiesNotInAims: [],
        AimsProjectsNotInIati: [],
        NewProjectsToAddInAims: [],
        ProjectsOwnedByOther: []
    };

    $scope.FundSources = [];

    $http({
        method: 'GET',
        url: apiprefix + '/api/IATIImport/GetFundSources',
        //data: { applicationId: 3 }
    }).success(function (result) {
        $scope.FundSources = result;
    });

});


angular.module('Authentication', []);

angular.module('Authentication')

.factory('AuthenticationService', 
    ['Base64', '$http', '$cookieStore', '$rootScope',
    function (Base64, $http, $cookieStore, $rootScope) {
        var service = {};

        service.Login = function (username, password, callback) {
            $http.post(apiprefix + '/authenticate', { username: username, password: password })
                .success(function (response) {
                    callback(response);
                });
        };

        service.SetCredentials = function (username, password) {
            var authdata = Base64.encode(username + ':' + password);

            $rootScope.globals = {
                currentUser: {
                    username: username,
                    authdata: authdata
                }
            };

            $http.defaults.headers.common['Authorization'] = 'Basic ' + authdata; // jshint ignore:line
            $cookieStore.put('globals', $rootScope.globals);
        };

        service.ClearCredentials = function () {
                $http.get(apiprefix + '/logout')
                .success(function (response) {
                    //callback(response);
                });

            $rootScope.globals = {};
            $cookieStore.remove('globals');
            $http.defaults.headers.common.Authorization = 'Basic ';
        };

        return service;
    }])
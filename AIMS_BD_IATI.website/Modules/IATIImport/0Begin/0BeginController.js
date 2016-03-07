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
        url: apiprefix + '/api/ApiHome/GetFundSources',
        //data: { applicationId: 3 }
    }).success(function (result) {
        $scope.FundSources = result;
    });

});

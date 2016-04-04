angular.module('iatiDataImporter').controller("0BeginController", function ($rootScope, $scope, $http, $timeout) {
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

        $timeout(function () {
            if($scope.FundSources.length == 1)
                document.getElementById('btnDashboard').click(); //redirect
            });
    });

});

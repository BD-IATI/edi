angular.module('iatiDataImporter').controller("3FilterDPController", function ($rootScope, $scope, $http, $timeout) {
    $scope.ImplementingOrgs = [];
    $scope.RelevantActivities = [];

    $http({
        url: apiprefix + '/api/IATIImport/GetAllImplementingOrg',
        method: 'POST',
        dataType: 'json',
        data: JSON.stringify($rootScope.filterBDModel)
    }).then(function (result) {
        $scope.ImplementingOrgs = result.data.Orgs;
        $scope.FundSources = result.data.FundSources;
        $scope.ExecutingAgencyTypes = result.data.ExecutingAgencyTypes;
        $scope.ExecutingAgencies = result.data.ExecutingAgencies;
        $rootScope.filterBDModel = null;
        //$timeout(function () {
        //    $("select").select2({ width: 'resolve' });
        //});
    
        //deferred.resolve(result);
    },
    function (response) {
        //deferred.reject(response);
    });
    $scope.activeTabIndex = 0;

    $scope.DetermineOrgType = function () {
        $scope.activeTabIndex = 1;
        $('#divView').slimScroll({ scrollTo: '0px' });
    };

    $scope.FilterDP = function () {

        $http({
            url: apiprefix + '/api/IATIImport/FilterDP',
            method: 'POST',
            data: JSON.stringify($scope.ImplementingOrgs),
            dataType: 'json'
        }).then(function (result) {
            $rootScope.RelevantActivities = $scope.RelevantActivities = result.data;
            $scope.activeTabIndex = 2;
            $('#divView').slimScroll({ scrollTo: '0px' });

            //deferred.resolve(result);
        },
        function (response) {
            //deferred.reject(response);
        });

    };

    $scope.hasOtherDPsProject = false;
    $scope.onFundSourceChanged = function () {
        var hasOtherDPsProject = false;
        for (var i = 0; i < $scope.RelevantActivities.length; i++) {
            if ($scope.RelevantActivities[i].FundSourceIDnIATICode != $rootScope.getCookie('selectedFundSource').IDnIATICode) {
                hasOtherDPsProject = true;
                break;
            }
        }

        $scope.hasOtherDPsProject = hasOtherDPsProject;
    };

    $scope.SaveAndNext = function () {
        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/AssignActivities',
            data: JSON.stringify($scope.RelevantActivities),
            dataType: 'json'
        }).then(function (result) {
            if (result.data != null || result.data != undefined) {
                $timeout(function () {
                    document.getElementById('btn4Projects').click(); //redirect
                });
            }
            else {
                alert('Something wrong happening!');
            }
        },
    function (response) {
        //deferred.reject(response);
    });

    }
    $scope.NextWithoutSaving = function () {
        $timeout(function () {
            document.getElementById('btn4Projects').click(); //redirect
        });
    }


    $scope.GuessAgency = function () {
        //try to set executing agency
        for (var org in $scope.ImplementingOrgs) {
            var agencyGuessed = null;
            var minDistance = 1000;
            for (var agency in $scope.ExecutingAgencies) {
                var distance = Levenshtein.iLD(org.Name, agency.Name);
                if (minDistance > distance) {
                    minDistance = distance;
                    agencyGuessed = agency;
                }

            }
            if (minDistance < 7 && agencyGuessed != null) {
                org.AllID = agencyGuessed.AllID;
            }
        }
    }

    // Compute the edit distance between the two given strings
    $scope.getEditDistance = function (a, b) {
        if (a.length == 0) return b.length;
        if (b.length == 0) return a.length;

        var matrix = [];

        // increment along the first column of each row
        var i;
        for (i = 0; i <= b.length; i++) {
            matrix[i] = [i];
        }

        // increment each column in the first row
        var j;
        for (j = 0; j <= a.length; j++) {
            matrix[0][j] = j;
        }

        // Fill in the rest of the matrix
        for (i = 1; i <= b.length; i++) {
            for (j = 1; j <= a.length; j++) {
                if (b.charAt(i - 1) == a.charAt(j - 1)) {
                    matrix[i][j] = matrix[i - 1][j - 1];
                } else {
                    matrix[i][j] = Math.min(matrix[i - 1][j - 1] + 1, // substitution
                                            Math.min(matrix[i][j - 1] + 1, // insertion
                                                     matrix[i - 1][j] + 1)); // deletion
                }
            }
        }

        return matrix[b.length][a.length];
    };

});

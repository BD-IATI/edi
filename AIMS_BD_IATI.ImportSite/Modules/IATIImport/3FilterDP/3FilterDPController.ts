/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />

 
angular.module('iatiDataImporter').controller("3FilterDPController", function ($rootScope : RootScopeModel, $scope, $http, $timeout, $filter, $uibModal) {
    //$('#divView').slimScroll({ scrollTo: '0px' });
    $scope.ImplementingOrgs = [];
    $scope.RelevantActivities = [];
    $scope.FundSources = [];
    $scope.activeTabIndex = 0;

    $http({
        url: apiprefix + '/api/IATIImport/GetAllImplementingOrg',
        method: 'POST',
        dataType: 'json',
        data: JSON.stringify($rootScope.filterBDModel)
    }).then(function (result) {
        $scope.ImplementingOrgs = result.data.Orgs;
        $scope.ManagingDPs = result.data.ManagingDPs;
        $scope.ExecutingAgencyTypes = result.data.ExecutingAgencyTypes;
        $scope.ExecutingAgencies = result.data.ExecutingAgencies;
        //$rootScope.filterBDModel = null;

        //for (var i = 0; i < $scope.ImplementingOrgs.length; i++) {
        //    var dis = $scope.GuessAgency($scope.ImplementingOrgs[i], false);
        //}

    },
        function (response) {
        });
    $scope.GotoTab = function (indx) {
        $scope.activeTabIndex = indx;
        //$('#divView').slimScroll({ scrollTo: '0px' });

    }
    $scope.FilterDP = function () {

        $http({
            url: apiprefix + '/api/IATIImport/FilterDP',
            method: 'POST',
            data: JSON.stringify($scope.ImplementingOrgs),
            dataType: 'json'
        }).then(function (result) {
            $rootScope.RelevantActivities = $scope.RelevantActivities = result.data;
            $scope.onFundSourceChanged();
            $scope.activeTabIndex = 1;
            //$('#divView').slimScroll({ scrollTo: '0px' });

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
            if (!$scope.RelevantActivities[i].AllID.startsWith($rootScope.getCookie('selectedFundSource').IDnIATICode)) {
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

    $scope.AddNewImplementingOrg = function (org) {
        var exAgencies = $scope.ExecutingAgencies;
        for (var i = 0; i < exAgencies.length; i++) {
            var distance = $scope.getEditDistance(org.Name.toLowerCase(), exAgencies[i].Name.toLowerCase());
            exAgencies[i].editDistance = distance;
        }

        $uibModal.open({
            templateUrl: 'AddNewImplementingOrgView.html',
            controller: 'AddNewImplementingOrgController',
            resolve: { parentScope: $scope, org: org }

        }).result.then(function (selectedOrg) {
            if (selectedOrg != null) {
                org.ExecutingAgencyTypeId = selectedOrg.ExecutingAgencyTypeId;
                org.AllID = selectedOrg.AllID;
            }
            else {
                //org.AllID = org.ExecutingAgencyOrganizationId + "~"
                //+ (org.ref || "") + "~"
                //+ org.ExecutingAgencyTypeId + "~" 
                //+ org.ExecutingAgencyOrganizationTypeId + "~New~" + org.Name;
                //org.IATICode = org.ref;

                //$scope.ExecutingAgencies.push(org);

                $http({
                    url: apiprefix + '/api/IATIImport/CreateNewExecutingAgency',
                    method: 'POST',
                    dataType: 'json',
                    data: JSON.stringify(org)
                }).then(function (result) {

                    var newIorg = result.data;
                    $scope.ExecutingAgencies.push(newIorg);
                    if (org.ExecutingAgencyTypeId == 2) //2 for DP
                        $scope.FundSources.push(newIorg);

                    org.AllID = newIorg.AllID;

                },
                    function (response) {
                    });
            }
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });
    };


    $scope.GuessAgency = function (org, isFilterByType) {
        var IsNotFoundInAims = true;
        var exAgencies = isFilterByType ? $filter('filter')($scope.ExecutingAgencies, { ExecutingAgencyTypeId: org.ExecutingAgencyTypeId }) : $scope.ExecutingAgencies;

        if (org['ref'] != null || org['ref'] != undefined) {
            var exa = $filter('filter')(exAgencies, { IATICode: org['ref'] });
            if (exa.length > 0) {
                org.AllID = exa[0].AllID;
                IsNotFoundInAims = false;
                if (!isFilterByType) org.ExecutingAgencyTypeId = exa[0].ExecutingAgencyTypeId;
            }
        }

        if (IsNotFoundInAims) {
            //try to set executing agency
            var agencyGuessed = null;
            var minDistance = 1000;
            for (var i = 0; i < exAgencies.length; i++) {
                var distance = $scope.getEditDistance(org.Name.toLowerCase(), exAgencies[i].Name.toLowerCase());
                if (minDistance > distance) {
                    minDistance = distance;
                    agencyGuessed = exAgencies[i];
                }
            }

            if (agencyGuessed != null) {
                var tolaratedDistance = isFilterByType ? (org.Name.length + agencyGuessed.Name.length) / 2 : ((org.Name.length + agencyGuessed.Name.length) / 2) * 50 / 100;
                if (minDistance < tolaratedDistance) {
                    org.AllID = agencyGuessed.AllID;

                    if (!isFilterByType) org.ExecutingAgencyTypeId = agencyGuessed.ExecutingAgencyTypeId;
                }
            }
            else {
                org.ExecutingAgencyTypeId = 4;
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

angular.module('iatiDataImporter').controller("AddNewImplementingOrgController", function ($rootScope : RootScopeModel, $scope, $http, $timeout, $filter, $uibModalInstance, parentScope, org) {

    org.ExecutingAgencyType = org.ExecutingAgencyTypeId == 1 ? 'Govt.' :
        org.ExecutingAgencyTypeId == 2 ? 'DP' :
            org.ExecutingAgencyTypeId == 3 ? 'NGO' : 'Other';

    $scope.ExecutingAgencies = parentScope.ExecutingAgencies;
    $scope.org = org;
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.Ok = function () {
        $uibModalInstance.close();
    };

    $scope.selectOrg = function (Org) {
        $uibModalInstance.close(Org);
    };

});

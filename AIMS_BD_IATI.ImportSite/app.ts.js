/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
var apiprefix = '../../../IATIImport';
var iatiDataImporterApp = angular.module('iatiDataImporter', ['Authentication', 'ngCookies', 'ngRoute', 'dndLists', 'ngLoadingSpinner', 'smart-table', 'ngAnimate', 'ui.bootstrap', 'angular.filter']);
iatiDataImporterApp.config(function ($routeProvider) {
    $routeProvider
        .when('/login', {
        controller: 'LoginController',
        templateUrl: '../Authentication/LoginView.html'
    })
        .when('/restart', {
        controller: 'DashboardController',
        templateUrl: 'Dashboard/DashboardView.html'
    })
        .when('/:name*', {
        templateUrl: function (params) {
            return params.name + '/' + params.name + 'View.html';
        }
    })
        .otherwise({ redirectTo: '/login' });
});
iatiDataImporterApp.run(['$rootScope', '$location', '$cookieStore', '$http', '$timeout',
    function ($rootScope, $location, $cookieStore, $http, $timeout) {
        $rootScope.IsImportFromOtherDP = false;
        $rootScope.location = $location;
        $rootScope.getCookie = function (key) { return $cookieStore.get(key) || {}; };
        $rootScope.putCookie = function (key, val) { $cookieStore.put(key, val) || {}; };
        // keep user logged in after page refresh
        $rootScope.globals = $cookieStore.get('globals') || {};
        if ($rootScope.globals.currentUser) {
            $http.defaults.headers.common['Authorization'] = 'Basic ' + $rootScope.globals.currentUser.authdata; // jshint ignore:line
        }
        $rootScope.$on('$locationChangeStart', function (event, next, current) {
            // redirect to login page if not logged in
            if ($location.path() !== '/login' && !$rootScope.globals.currentUser) {
                $location.path('/login');
            }
            else if ($location.path() == '/restart') {
                $rootScope.hierarchyModel = null;
                $rootScope.HasChildActivity = false;
                $rootScope.hierarchyModel = null;
                $rootScope.filterBDModel = null;
                $rootScope.RelevantActivities = null;
                $http({
                    method: 'POST',
                    url: apiprefix + '/api/Dashboard/RestartSession',
                    data: JSON.stringify($rootScope.getCookie('selectedFundSource'))
                }).success(function (result) {
                    $timeout(function () {
                        location.hash = '/Dashboard';
                    });
                });
            }
        });
    }]);
iatiDataImporterApp.directive('resolveController', ['$controller', function ($controller) {
        return {
            scope: true,
            link: function (scope, elem, attrs) {
                var resolve = scope.$eval(attrs.resolve);
                angular.extend(resolve, { $scope: scope });
                $controller(attrs.resolveController, resolve);
            }
        };
    }]);
iatiDataImporterApp.directive('navigation', function ($rootScope, $location) {
    return {
        template: '<li ng-repeat="option in options" ng-class="{active: isActive(option)}">' +
            '    <a> <span ng-class="option.glyphicon" aria-hidden="true"></span> {{option.label}}</a>' +
            '</li>',
        link: function (scope, element, attr) {
            scope.options = [
                { label: "Begin import", glyphicon: 'glyphicon glyphicon-home', href: "#/0Begin" },
                { label: "1. Select project structure", glyphicon: 'glyphicon glyphicon-th-list', href: "#/1Hierarchy" },
                { label: "2. Filter relevant projects", glyphicon: 'glyphicon glyphicon-filter', href: "#/2FilterBD" },
                { label: "3. Implementing organisations", glyphicon: 'glyphicon glyphicon-filter', href: "#/3FilterDP" },
                { label: "4. Review matched projects", glyphicon: 'glyphicon glyphicon-link', href: "#/4Projects" },
                //{ label: "5. Map unmatched projects", glyphicon: 'glyphicon glyphicon-resize-small', href: "#/5Match" },
                { label: "5. Set import preferences", glyphicon: 'glyphicon glyphicon-link', href: "#/6GeneralPreferences" },
                { label: "6. Review and import", glyphicon: 'glyphicon glyphicon-link', href: "#/7ReviewAdjustment" }
            ];
            scope.isActive = function (option) {
                return option.href.indexOf(scope.location) === 1;
            };
            $rootScope.$on("$locationChangeSuccess", function (event, next, current) {
                scope.location = $location.path();
            });
        }
    };
});
iatiDataImporterApp.directive('navigationOtherDp', function ($rootScope, $location) {
    return {
        template: '<li ng-repeat="option in options" ng-class="{active: isActive(option)}">' +
            '    <a>{{option.label}}</a>' +
            '</li>',
        link: function (scope, element, attr) {
            scope.options = [
                { label: "Activities from other DPs", href: "#/9OtherDPsActivities" },
                { label: "Trust Funds and Co-financing", href: "#/9TFnCF" },
            ];
            scope.isActive = function (option) {
                return option.href.indexOf(scope.location) == 1;
            };
            $rootScope.$on("$locationChangeSuccess", function (event, next, current) {
                scope.location = $location.path();
            });
        }
    };
});
/// <reference path="../IatiImportApp.ts" />
/// <reference path="../../authentication/definitions.d.ts" />
angular.module('iatiDataImporter').controller("0BeginController", function ($rootScope, $scope, $http, $timeout) {
    //$('#divView').slimScroll({ scrollTo: '0px' });
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
    }).success(function (result) {
        $scope.FundSources = result;
        $timeout(function () {
            if ($scope.FundSources.length == 1) {
                $rootScope.putCookie('selectedFundSource', $scope.FundSources[0]);
                document.getElementById('btnDashboard').click(); //redirect
            }
        });
    });
});
/// <reference path="../../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../IatiImportApp.ts" />
/// <reference path="../../authentication/definitions.d.ts" />
angular.module('iatiDataImporter').controller("1HierarchyController", function ($rootScope, $scope, $http, $timeout) {
    //$rootScope.hierarchyModel = null;
    //$rootScope.HasChildActivity = false;
    //$('#divView').slimScroll({ scrollTo: '0px' });
    $http({
        method: 'POST',
        url: apiprefix + '/api/Dashboard/CheckSession',
        data: JSON.stringify($rootScope.getCookie('selectedFundSource'))
    }).success(function (result) {
        $timeout(function () {
            if (result == '/0Begin') {
                $scope.GetHierarchyData();
            }
            else {
                location.hash = result;
            }
        });
    });
    $scope.GetHierarchyData = function () {
        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/GetHierarchyData',
            dataType: 'json',
            data: JSON.stringify($rootScope.getCookie('selectedFundSource'))
        }).success(function (result) {
            if (result == null || result == undefined) {
                $rootScope.hierarchyModel = null;
                $rootScope.HasChildActivity = false;
                $timeout(function () {
                    document.getElementById('btn2FilterBD').click(); //redirect
                });
            }
            else {
                $rootScope.hierarchyModel = $scope.model = result;
                $rootScope.HasChildActivity = true;
            }
        });
    };
});
/// <reference path="../../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../IatiImportApp.ts" />
angular.module('iatiDataImporter').controller("2FilterBDController", function ($rootScope, $scope, $http, $timeout) {
    ////$('#divView').slimScroll({ scrollTo: '0px' });
    $scope.activeTabIndex = 0;
    $scope.setTabIndex = function (index) { $scope.activeTabIndex = index; };
    $scope.nextFromTab0 = function () {
        if ($rootScope.HasChildActivity)
            $scope.activeTabIndex = 1;
        else
            $timeout(function () {
                document.getElementById('btn3FilterDP').click(); //redirect
            });
    };
    //var selectedHierarchy = $rootScope.hierarchyModel ? $rootScope.hierarchyModel.SelectedHierarchy : null;
    $http({
        url: apiprefix + '/api/IATIImport/SubmitHierarchy',
        method: 'POST',
        data: JSON.stringify($rootScope.hierarchyModel),
        dataType: 'json'
    }).then(function (result) {
        if (result.data != null) {
            if (result.data.AllExtendingOrgs)
                result.data.AllExtendingOrgs.forEach(function (e) { return e.IsRelevant = true; });
            $rootScope.filterBDModel = $scope.model = result.data;
        }
        else
            $scope.model = $rootScope.filterBDModel;
        //$rootScope.hierarchyModel = null;
        //deferred.resolve(result);
    }, function (response) {
        //deferred.reject(response);
    });
    $scope.filterByExtendingOrg = function () {
        var acts = $scope.model.iatiActivities;
        var selectedExtOrgs = $scope.model.AllExtendingOrgs.filter(function (s) { return s.IsRelevant == true; });
        for (var _i = 0, acts_1 = acts; _i < acts_1.length; _i++) {
            var act = acts_1[_i];
            act.IsRelevant = false;
            if (act.ExtendingOrgs) {
                for (var _a = 0, selectedExtOrgs_1 = selectedExtOrgs; _a < selectedExtOrgs_1.length; _a++) {
                    var eOrg = selectedExtOrgs_1[_a];
                    if (act.ExtendingOrgs.filter(function (fe) { return fe.Name == eOrg.Name; }).length > 0) {
                        act.IsRelevant = act.PercentToBD >= 20 && act.ActivityStatus == "Implementation";
                    }
                }
            }
        }
    };
    $scope.IncludeAll = function () {
        var acts = $scope.model.iatiActivities;
        for (var _i = 0, acts_2 = acts; _i < acts_2.length; _i++) {
            var act = acts_2[_i];
            act.IsRelevant = true;
        }
    };
    $scope.ExcludeAll = function () {
        var acts = $scope.model.iatiActivities;
        for (var _i = 0, acts_3 = acts; _i < acts_3.length; _i++) {
            var act = acts_3[_i];
            act.IsRelevant = false;
        }
    };
});
/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />
angular.module('iatiDataImporter').controller("3FilterDPController", function ($rootScope, $scope, $http, $timeout, $filter, $uibModal) {
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
        $scope.ManagingDPs = result.data.FundSources;
        $scope.ExecutingAgencyTypes = result.data.ExecutingAgencyTypes;
        $scope.ExecutingAgencies = result.data.ExecutingAgencies;
        //$rootScope.filterBDModel = null;
        //for (var i = 0; i < $scope.ImplementingOrgs.length; i++) {
        //    var dis = $scope.GuessAgency($scope.ImplementingOrgs[i], false);
        //}
    }, function (response) {
    });
    $scope.GotoTab = function (indx) {
        $scope.activeTabIndex = indx;
        //$('#divView').slimScroll({ scrollTo: '0px' });
    };
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
        }, function (response) {
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
        }, function (response) {
            //deferred.reject(response);
        });
    };
    $scope.NextWithoutSaving = function () {
        $timeout(function () {
            document.getElementById('btn4Projects').click(); //redirect
        });
    };
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
                    if (org.ExecutingAgencyTypeId == 2)
                        $scope.FundSources.push(newIorg);
                    org.AllID = newIorg.AllID;
                }, function (response) {
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
                if (!isFilterByType)
                    org.ExecutingAgencyTypeId = exa[0].ExecutingAgencyTypeId;
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
                    if (!isFilterByType)
                        org.ExecutingAgencyTypeId = agencyGuessed.ExecutingAgencyTypeId;
                }
            }
            else {
                org.ExecutingAgencyTypeId = 4;
            }
        }
    };
    // Compute the edit distance between the two given strings
    $scope.getEditDistance = function (a, b) {
        if (a.length == 0)
            return b.length;
        if (b.length == 0)
            return a.length;
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
                }
                else {
                    matrix[i][j] = Math.min(matrix[i - 1][j - 1] + 1, // substitution
                    Math.min(matrix[i][j - 1] + 1, // insertion
                    matrix[i - 1][j] + 1)); // deletion
                }
            }
        }
        return matrix[b.length][a.length];
    };
});
angular.module('iatiDataImporter').controller("AddNewImplementingOrgController", function ($rootScope, $scope, $http, $timeout, $filter, $uibModalInstance, parentScope, org) {
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
/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />
angular.module('iatiDataImporter').controller("4ProjectsController", function ($rootScope, $scope, $http, $timeout) {
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
    }, function (response) {
        //deferred.reject(response);
    });
    $scope.saveData = function () {
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
        }, function (response) {
            //deferred.reject(response);
        });
    };
});
/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />
angular.module('iatiDataImporter').controller("5MatchController", function ($rootScope, $scope, $http, $timeout) {
    $scope.models = $rootScope.models;
    //$('#divView').slimScroll({ scrollTo: '0px' });
    $scope.Commands = {
        saveData: function () {
            $http({
                url: apiprefix + '/api/IATIImport/SubmitManualMatching',
                method: 'POST',
                data: JSON.stringify($scope.models),
                dataType: 'json'
            }).then(function (result) {
                $timeout(function () {
                    document.getElementById('btn6GeneralPreferences').click(); //redirect
                });
                //deferred.resolve(result);
            }, function (response) {
                //deferred.reject(response);
            });
        }
    };
});
/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />
angular.module('iatiDataImporter').controller("6GeneralPreferencesController", function ($rootScope, $scope, $http, $timeout, $uibModalInstance) {
    $rootScope.GeneralPreference = {};
    $http({
        method: 'GET',
        url: apiprefix + '/api/IATIImport/GetGeneralPreferences',
    }).success(function (result) {
        $rootScope.GeneralPreference = $scope.model = result;
    });
    $scope.Save = function () {
        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/SaveGeneralPreferences',
            data: JSON.stringify($rootScope.GeneralPreference),
            dataType: 'json'
        }).then(function (result) {
            if (result.data != null || result.data != undefined) {
                $timeout(function () {
                    $uibModalInstance.dismiss('cancel');
                });
            }
            else {
                alert('Something wrong happening!');
            }
        }, function (response) {
            //deferred.reject(response);
        });
    };
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    $scope.SaveAndNext = function () {
        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/SaveGeneralPreferences',
            data: JSON.stringify($rootScope.GeneralPreference),
            dataType: 'json'
        }).then(function (result) {
            if (result.data != null || result.data != undefined) {
                $timeout(function () {
                    document.getElementById('btn7ReviewAdjustment').click(); //redirect
                });
            }
            else {
                alert('Something wrong happening!');
            }
        }, function (response) {
            //deferred.reject(response);
        });
    };
    $scope.NextWithoutSaving = function () {
        $timeout(function () {
            document.getElementById('btn7ReviewAdjustment').click(); //redirect
        });
    };
});
/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />
angular.module('iatiDataImporter').controller("7ReviewAdjustmentController", function ($rootScope, $scope, $http, $uibModal, $timeout) {
    //$('#divView').slimScroll({ scrollTo: '0px' });
    $http({
        method: 'POST',
        url: apiprefix + '/api/IATIImport/GetProjectsToMap',
        data: JSON.stringify($rootScope.GeneralPreference)
    }).success(function (result) {
        $scope.models = result;
    });
    $scope.disbursmentDiff = function (m) {
        var numerator = m.iatiActivity.TotalDisbursmentThisDPOnly >= m.aimsProject.TotalDisbursmentThisDPOnly ? m.iatiActivity.TotalDisbursmentThisDPOnly : m.aimsProject.TotalDisbursmentThisDPOnly;
        var denominator = m.iatiActivity.TotalDisbursmentThisDPOnly < m.aimsProject.TotalDisbursmentThisDPOnly ? m.iatiActivity.TotalDisbursmentThisDPOnly : m.aimsProject.TotalDisbursmentThisDPOnly;
        var diff = (numerator / denominator) * 100;
        return diff;
    };
    $scope.commitmentDiff = function (m) {
        var numerator = m.iatiActivity.TotalCommitmentThisDPOnly >= m.aimsProject.TotalCommitmentThisDPOnly ? m.iatiActivity.TotalCommitmentThisDPOnly : m.aimsProject.TotalCommitmentThisDPOnly;
        var denominator = m.iatiActivity.TotalCommitmentThisDPOnly < m.aimsProject.TotalCommitmentThisDPOnly ? m.iatiActivity.TotalCommitmentThisDPOnly : m.aimsProject.TotalCommitmentThisDPOnly;
        var diff = (numerator / denominator) * 100;
        return diff;
    };
    $scope.isDiffGT5 = function (mkl) {
        var cDiff = $scope.commitmentDiff(mkl);
        var dDiff = $scope.disbursmentDiff(mkl);
        var avgDiff = ((dDiff + cDiff) / 2);
        return avgDiff > 120; //difference tolerance %
    };
    $scope.OpenProjectSpecificAdjustment = function (MatchedProject) {
        var modalInstance = $uibModal.open({
            animation: true,
            backdrop: false,
            templateUrl: '8ProjectSpecificAdjustment/8ProjectSpecificAdjustmentView.html',
            controller: '8ProjectSpecificAdjustmentController',
            size: 'lg',
            resolve: {
                MatchedProject: function () {
                    return MatchedProject;
                }
            }
        });
        //modalInstance.result.then(function (selectedItem) {
        //    $scope.selected = selectedItem;
        //}, function () {
        //    //$log.info('Modal dismissed at: ' + new Date());
        //});
    };
    $scope.OpenTransactionDetail = function (MatchedProject) {
        var modalInstance = $uibModal.open({
            animation: true,
            backdrop: false,
            templateUrl: 'Transaction/TransactionView.html',
            controller: 'TransactionController',
            size: 'lg',
            resolve: {
                MatchedProject: function () {
                    return MatchedProject;
                }
            }
        });
        //modalInstance.result.then(function (selectedItem) {
        //    $scope.selected = selectedItem;
        //}, function () {
        //    //$log.info('Modal dismissed at: ' + new Date());
        //});
    };
    $scope.UnlinkProject = function (MatchedProject) {
        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/UnlinkProject',
            data: JSON.stringify(MatchedProject)
        }).success(function (result) {
            var index = $scope.models.MatchedProjects.indexOf(MatchedProject, 0);
            if (index > -1) {
                $scope.models.MatchedProjects.splice(index, 1);
            }
        });
    };
    $scope.ImportProjects = function () {
        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/ImportProjects',
            data: JSON.stringify($scope.models)
        }).success(function (result) {
            $timeout(function () {
                alert("Projects are imported.");
                document.getElementById('btnGoDashboard').click(); //redirect
            });
        });
    };
});
/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />
angular.module('iatiDataImporter').controller("8ProjectSpecificAdjustmentController", function ($rootScope, $timeout, $uibModalInstance, $scope, $http, MatchedProject) {
    $scope.model = MatchedProject;
    $scope.ok = function () {
        $uibModalInstance.close();
    };
    $scope.Save = function () {
        $http({
            method: 'POST',
            url: apiprefix + '/api/IATIImport/SaveActivityPreferences',
            data: JSON.stringify($scope.model),
            dataType: 'json'
        }).then(function (result) {
            if (result.data != null || result.data != undefined) {
                $timeout(function () {
                    $uibModalInstance.close();
                });
            }
            else {
                alert('Something wrong happening!');
            }
        }, function (response) {
            //deferred.reject(response);
        });
    };
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});
/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />
angular.module('iatiDataImporter').controller("9CofinancingController", function ($rootScope, $timeout, $scope, $http, $uibModalInstance, model) {
    $scope.AssignedActivities = $rootScope.AssignedActivities;
    if (model == undefined || model == null) {
        $http({
            method: 'POST',
            url: apiprefix + '/api/CFnTF/SubmitAssignedActivities',
            data: JSON.stringify($rootScope.AssignedActivities)
        }).success(function (result) {
            $scope.model = result;
        });
    }
    else {
        $scope.model = model;
    }
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    $scope.SavePreferences = function () {
        $http({
            method: 'POST',
            url: apiprefix + '/api/CFnTF/SavePreferences',
            data: JSON.stringify($scope.model)
        }).success(function (result) {
            alert('saved');
            //$scope.model = result;
        });
    };
    $scope.GetSumOfPlannedDisbursments = function (prjArray) {
        var sum = 0;
        for (var i in prjArray) {
            sum = sum + Number(prjArray[i].TotalPlannedDisbursment);
        }
        return sum;
    };
    $scope.ChangeIsCommitmentIncluded = function (prjArray) {
        for (var i = 1; i < prjArray.length; i++) {
            prjArray[i].IsCommitmentIncluded = !prjArray[i].IsCommitmentIncluded;
        }
    };
    $scope.ChangeIsDisbursmentIncluded = function (prjArray) {
        for (var i = 1; i < prjArray.length; i++) {
            prjArray[i].IsDisbursmentIncluded = !prjArray[i].IsDisbursmentIncluded;
        }
    };
    $scope.ChangeIsPlannedDisbursmentIncluded = function (prjArray) {
        for (var i = 1; i < prjArray.length; i++) {
            prjArray[i].IsPlannedDisbursmentIncluded = !prjArray[i].IsPlannedDisbursmentIncluded;
        }
    };
});
/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />
angular.module('iatiDataImporter').controller("9OtherDPsActivitiesController", function ($rootScope, $timeout, $scope, $http, $uibModal, $filter) {
    $rootScope.IsImportFromOtherDP = true;
    $http({
        method: 'GET',
        url: apiprefix + '/api/CFnTF/GetAssignedActivities',
        params: { dp: $rootScope.getCookie('selectedFundSource').ID }
    }).success(function (result) {
        $rootScope.AssignedActivities = $scope.AssignedActivities = result.AssignedActivities;
        $rootScope.AimsProjects = $scope.AimsProjects = result.AimsProjects;
        $rootScope.TrustFunds = $scope.TrustFunds = result.TrustFunds;
    });
    $scope.OpenTFnCF = function (activity) {
        var modalInstance = $uibModal.open({
            animation: true,
            backdrop: false,
            templateUrl: '9TFnCF/9TFnCFView.html',
            controller: '9TFnCFController',
            size: 'lg',
            windowClass: 'full-modal-window',
            resolve: {
                Activity: function () {
                    return activity;
                },
                Project: function () {
                    var project = $filter('filter')($scope.Projects, { ProjectId: activity.MappedProjectId })[0];
                    return project;
                }
            }
        });
        //modalInstance.result.then(function (selectedItem) {
        //    $scope.selected = selectedItem;
        //}, function () {
        //    //$log.info('Modal dismissed at: ' + new Date());
        //});
    };
});
/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />
angular.module('iatiDataImporter').controller("9TFnCFController", function ($rootScope, $uibModalInstance, $timeout, $scope, $http, Activity, Project) {
    $scope.Activity = Activity;
    $scope.Project = Project;
    $http({
        method: 'GET',
        url: apiprefix + '/api/IATIImport/GetTrustFundDetails',
        params: { trustFundId: Activity.MappedTrustFundId }
    }).success(function (result) {
        $scope.TrustFundDetails = result;
    });
    $scope.ok = function () {
        $uibModalInstance.close();
    };
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});
/// <reference path="../../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../iatiimportapp.ts" />
angular.module('iatiDataImporter').controller("DashboardController", function ($rootScope, $scope, $http, $uibModal, $timeout) {
    $http({
        method: 'POST',
        url: apiprefix + '/api/Dashboard/GetDashboardData',
        data: JSON.stringify($rootScope.getCookie('selectedFundSource'))
    }).success(function (result) {
        $scope.model = result;
    });
    $scope.OpenAdjustImportPreferences = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            backdrop: false,
            templateUrl: 'Dashboard/ImportPreferenceModal.html',
            controller: '6GeneralPreferencesController',
            size: 'lg',
        });
    };
    $scope.OpenTrustFund = function (trustfunddetail) {
        var modalInstance = $uibModal.open({
            animation: true,
            backdrop: false,
            templateUrl: 'Dashboard/TrustFundModal.html',
            controller: '9CofinancingController',
            size: 'lg',
            resolve: {
                model: function () {
                    return { TrustFundDetails: [trustfunddetail] };
                }
            }
        });
    };
    $scope.OpenCofinancing = function (aimsproject) {
        var modalInstance = $uibModal.open({
            animation: true,
            backdrop: false,
            templateUrl: 'Dashboard/CofinancingModal.html',
            controller: '9CofinancingController',
            size: 'lg',
            resolve: {
                model: function () {
                    return {
                        AimsProjects: [aimsproject],
                    };
                }
            }
        });
    };
    $scope.OpenMergeConflictAlert = function (log) {
        var modalInstance = $uibModal.open({
            animation: true,
            backdrop: false,
            templateUrl: 'MergeConflictAlert/MergeConflictAlertView.html',
            controller: 'MergeConflictAlertController',
            size: 'lg',
            resolve: {
                log: function () {
                    return log;
                }
            }
        });
        modalInstance.result.then(function (selectedItem) {
            var index = $scope.model.FinancialDataMismatchedLogs.indexOf(selectedItem, 0);
            if (index > -1) {
                $scope.model.FinancialDataMismatchedLogs.splice(index, 1);
            }
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });
    };
    $scope.RecallDelegatedActivity = function (da) {
        if (da.IsProccessed == true) {
            document.getElementById('btnRecallExplanation').click(); //Show explanation
        }
        else {
            $http({
                method: 'POST',
                url: apiprefix + '/api/Dashboard/RecallDelegatedActivity',
                data: JSON.stringify(da)
            }).success(function (result) {
                $scope.model.DelegatedActivities = result;
            });
        }
    };
    $scope.timeSince = function (date) {
        if (date == null || date == undefined)
            return '';
        if (typeof date !== 'object') {
            date = new Date(date);
        }
        var di = new Date();
        var seconds = Math.floor(Math.abs(di - date) / 1000);
        var intervalType;
        var interval = Math.floor(seconds / 31536000);
        if (interval >= 1) {
            intervalType = 'year';
        }
        else {
            interval = Math.floor(seconds / 2592000);
            if (interval >= 1) {
                intervalType = 'month';
            }
            else {
                interval = Math.floor(seconds / 86400);
                if (interval >= 1) {
                    intervalType = 'day';
                }
                else {
                    interval = Math.floor(seconds / 3600);
                    if (interval >= 1) {
                        intervalType = "hour";
                    }
                    else {
                        interval = Math.floor(seconds / 60);
                        if (interval >= 1) {
                            intervalType = "minute";
                        }
                        else {
                            interval = seconds;
                            intervalType = "second";
                        }
                    }
                }
            }
        }
        if (interval > 1 || interval === 0) {
            intervalType += 's';
        }
        return interval + ' ' + intervalType + ' ago';
    };
});
angular.module('iatiDataImporter').filter('sumByKey', function () {
    return function (data, key) {
        if (typeof (data) === 'undefined' || typeof (key) === 'undefined') {
            return 0;
        }
        var sum = 0;
        for (var i = data.length - 1; i >= 0; i--) {
            sum += parseFloat(data[i][key]);
        }
        return sum;
    };
});
/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />
angular.module('iatiDataImporter').controller("FinishImportController", function ($rootScope, $scope, $http, $timeout) {
});
/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Authentication/definitions.d.ts" />
/// <reference path="../IatiImportApp.ts" />
angular.module('iatiDataImporter').controller("MergeConflictAlertController", function ($rootScope, $timeout, $uibModalInstance, $scope, $http, log) {
    $scope.ActionType = '';
    $http({
        method: 'GET',
        url: apiprefix + '/api/IATIImport/GetMatchedProjectByIatiIdentifier',
        dataType: 'json',
        params: { iatiIdentifier: log.IatiIdentifier }
    }).success(function (result) {
        $scope.model = result;
    });
    $scope.resolve = function () {
        if ($scope.ActionType == 'option1') {
            $http({
                method: 'POST',
                url: apiprefix + '/api/IATIImport/UpdateTransactionByForce',
                dataType: 'json',
                data: JSON.stringify(log)
            }).success(function (result) {
            });
        }
        else if ($scope.ActionType = 'option2') {
            $http({
                method: 'POST',
                url: apiprefix + '/api/IATIImport/SetIgnoreActivity',
                dataType: 'json',
                data: JSON.stringify(log)
            }).success(function (result) {
            });
        }
        else {
        }
        $uibModalInstance.close(log);
    };
    $scope.ok = function () {
        $uibModalInstance.close();
    };
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});
angular.module('iatiDataImporter').controller("TransactionController", function ($rootScope, $timeout, $uibModalInstance, $scope, $http, MatchedProject) {
    $scope.model = MatchedProject;
    $scope.ok = function () {
        $uibModalInstance.close();
    };
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});
//# sourceMappingURL=app.ts.js.map
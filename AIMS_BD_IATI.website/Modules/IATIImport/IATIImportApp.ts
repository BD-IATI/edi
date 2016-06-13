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
iatiDataImporterApp.run(['$rootScope', '$location', '$cookieStore', '$http','$timeout',
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
            else if ($location.path() == '/restart')
            {
                
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
                { label: "1. Select Project structure", glyphicon: 'glyphicon glyphicon-th-list', href: "#/1Hierarchy" },
                { label: "2. Filter country relevant activities", glyphicon: 'glyphicon glyphicon-filter', href: "#/2FilterBD" },
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

declare namespace angular {

    interface IScope extends IRootScopeService {
        options: any;
        isActive: any;
        location: string;

    }
}

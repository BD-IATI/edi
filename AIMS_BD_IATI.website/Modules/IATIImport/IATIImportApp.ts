/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />


var apiprefix = '../../../IATIImport';


var iatiDataImporterApp = angular.module('iatiDataImporter',
    ['Authentication', 'ngCookies', 'ngRoute', 'dndLists', 'ngLoadingSpinner', 'smart-table', 'ui.select', 'ngAnimate', 'ui.bootstrap', 'angular.filter']);

iatiDataImporterApp.config(function ($routeProvider) {
    $routeProvider
        .when('/login', {
            controller: 'LoginController',
            templateUrl: '../Authentication/LoginView.html'
        })
        .when('/:name*', {
            templateUrl: function (params) {
                return params.name + '/' + params.name + 'View.html';
            }
        })
        .otherwise({ redirectTo: '/login' });
})

iatiDataImporterApp.run(['$rootScope', '$location', '$cookieStore', '$http',
    function ($rootScope, $location, $cookieStore, $http) {
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
                  '    <a ng-href="{{option.href}}">{{option.label}}</a>' + //'    <a>{{option.label}}</a>' + //
                  '</li>',
        link: function (scope, element, attr) {
            scope.options = [
                { label: "Begin import", href: "#/0Begin" },
                { label: "1. Project structure", href: "#/1Hierarchy" },
                { label: "2. Filter Bangladesh-relevant activities", href: "#/2FilterBD" },
                { label: "3. Determine managing DP", href: "#/3FilterDP" },
                { label: "4. Review matched projects", href: "#/4Projects" },
                { label: "5. Match unmatched projects", href: "#/5Match" },
                { label: "6. Set import preferences", href: "#/6GeneralPreferences" },
                { label: "7. Review and import", href: "#/7ReviewAdjustment" }
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
                  '    <a ng-href="{{option.href}}">{{option.label}}</a>' + //'    <a>{{option.label}}</a>' + //
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

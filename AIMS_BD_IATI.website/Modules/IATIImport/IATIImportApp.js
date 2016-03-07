var apiprefix = '../../../AIMS_DB_IATI.WebAPI';
var iatiDataImporterApp = angular.module('iatiDataImporter', ['ngRoute', 'dndLists', 'ngLoadingSpinner', 'smart-table']);
iatiDataImporterApp.config(function ($routeProvider) {
    $routeProvider
        .when('/login', {
            controller: 'LoginController',
            templateUrl: 'modules/authentication/views/login.html'
        })
        .when('/0Begin', {
            templateUrl: '0Begin/0BeginView.html',
            controller: '0BeginController'
        })
        .when('/1Hierarchy', {
            templateUrl: '1Hierarchy/1HierarchyView.html',
            controller: '1HierarchyController'
        })
        .when('/2FilterBD', {
            templateUrl: '2FilterBD/2FilterBDView.html',
            controller: '2FilterBDController'
        })
        .when('/3FilterDP', {
            templateUrl: '3FilterDP/3FilterDPView.html',
            controller: '3FilterDPController'
        })
        .when('/4Projects', {
            templateUrl: '4Projects/4ProjectsView.html',
            controller: '4ProjectsController'
        })
        .when('/5Match', {
            templateUrl: '5Match/5MatchView.html',
            controller: '5MatchController'
        })
        .otherwise({ redirectTo: '/0Begin' });
})

iatiDataImporterApp.directive('navigation', function ($rootScope, $location) {
    return {
        template: '<li ng-repeat="option in options" ng-class="{active: isActive(option)}">' +
                  '    <a>{{option.label}}</a>' + //'    <a ng-href="{{option.href}}">{{option.label}}</a>'
                  '</li>',
        link: function (scope, element, attr) {
            scope.options = [
                { label: "Begin import", href: "#/0Begin" },
                { label: "1. Hierarchy", href: "#/1Hierarchy" },
                { label: "2. Filter Bangladesh-relevant activities", href: "#/2FilterBD" },
                { label: "3. Filter DP activities", href: "#/3FilterDP" },
                { label: "4. Show projects", href: "#/4Projects" },
                { label: "5. Match unmatched projects", href: "#/5Match" }
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
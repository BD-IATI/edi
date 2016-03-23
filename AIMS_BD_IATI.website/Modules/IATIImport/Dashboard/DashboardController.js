angular.module('iatiDataImporter').controller("DashboardController", function ($rootScope, $scope, $http, $uibModal) {

    $http({
        method: 'GET',
        url: apiprefix + '/api/Dashboard/GetDashboardData',
        params: {
            dp: $rootScope.getCookie('selectedFundSource').ID
        }
    }).success(function (result) {
        $scope.model = result;
    });


    $scope.OpenAdjustImportPreferences = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            backdrop: false,
            templateUrl: '6GeneralPreferences/ImportPreferenceModal.html',
            controller: '6GeneralPreferencesController',
            size: 'lg',
            //resolve: {
            //    MatchedProject: function () {

            //        return MatchedProject;
            //    }
            //}
        });

        //modalInstance.result.then(function (selectedItem) {
        //    $scope.selected = selectedItem;
        //}, function () {
        //    //$log.info('Modal dismissed at: ' + new Date());
        //});
    };



    $scope.timeSince = function (date) {
        if (date == null || date == undefined) return '';

        if (typeof date !== 'object') {
            date = new Date(date);
        }

        var seconds = Math.floor((new Date() - date) / 1000);
        var intervalType;

        var interval = Math.floor(seconds / 31536000);
        if (interval >= 1) {
            intervalType = 'year';
        } else {
            interval = Math.floor(seconds / 2592000);
            if (interval >= 1) {
                intervalType = 'month';
            } else {
                interval = Math.floor(seconds / 86400);
                if (interval >= 1) {
                    intervalType = 'day';
                } else {
                    interval = Math.floor(seconds / 3600);
                    if (interval >= 1) {
                        intervalType = "hour";
                    } else {
                        interval = Math.floor(seconds / 60);
                        if (interval >= 1) {
                            intervalType = "minute";
                        } else {
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

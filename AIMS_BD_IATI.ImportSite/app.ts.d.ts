/// <reference path="scripts/typings/angularjs/angular.d.ts" />
/// <reference path="Modules/Authentication/definitions.d.ts" />
/// <reference path="scripts/typings/jquery.slimscroll/jquery.slimscroll.d.ts" />
declare var apiprefix: string;
declare var iatiDataImporterApp: ng.IModule;
declare namespace angular {
    interface IScope extends IRootScopeService {
        options: any;
        isActive: any;
        location: string;
    }
}

/// <reference path="Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="Modules/Authentication/definitions.d.ts" />
declare var apiprefix: string;
declare var iatiDataImporterApp: ng.IModule;
declare namespace angular {
    interface IScope extends IRootScopeService {
        options: any;
        isActive: any;
        location: string;
    }
}

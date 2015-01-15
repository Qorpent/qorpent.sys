define([],function(){
    return '<input class="dropdown-toggle form-control" ng-focus="__acFocus()" ng-model="${bindQuery}" ng-change="__acChange(1)"\
    on-enter="__acChange(2)"/> \
<ul  ng-if="!(\'${templateUrl}\')" class="dropdown-menu"> \
    <li class="default"  ng-repeat="i in __fixdata"> \
        <a ng-click="__acClick($event,i)">${template}</a> \
    </li> \
    <li ng-if="__fixdata.length!=0 && __data.length!=0" class="delimiter"></li> \
    <li ng-repeat="i in __data"> \
        <a ng-click="__acClick($event,i)">${template}</a> \
    </li> \
</ul>\
<ul ng-if="(\'${templateUrl}\')" class="dropdown-menu" ng-include="\'${templateUrl}\'"> \
</ul>'
});
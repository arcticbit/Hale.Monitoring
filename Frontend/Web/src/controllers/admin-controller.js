(function() {
  'use strict';

  angular.module('HaleGUI')
    .controller('AdminController', ['$scope', '$routeParams', function($scope, $routeParams) {
      $scope.title = 'Administration';
      $scope.description = '';
      $scope.renderSection = function(name) {
        return ($routeParams.section === name);
      }
    }])
})();

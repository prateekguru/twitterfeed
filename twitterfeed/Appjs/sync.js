/// <reference path="C:\Works\twitterfeed\twitterfeed\Scripts/_references.js" />

angular.module('myapp', ['ui.bootstrap', 'ngSanitize']);

angular.module('myapp')
    .filter('to_trusted', ['$sce', function ($sce) {
        return function (text) {
            return $sce.trustAsHtml(text);
        };
    }]);


angular.module('myapp')
    .controller('MainController', ['$http', function ($http) {

        var tweetsList = [];
        this.tweets = tweetsList;


        var url = './api/timeline/';

        $http.get(url)
            .success(function (data) {
                console.log("INFO: MainController **** Success calling TimelineController, Tweets received #" + data.length);
                angular.forEach(data, function(tweet) {
                    tweetsList.push(tweet);
                }); 

            })
            .error(function (data) {
                console.log("ERORR: MainController **** Error calling TimelineController. Error message is: " + data);
            });

    }]);



/// <reference path="C:\Works\twitterfeed\twitterfeed\Scripts/_references.js" />

angular.module('myapp', ['ui.bootstrap', 'SignalR', 'ngSanitize']);

angular.module('myapp')
    .filter('to_trusted', ['$sce', function ($sce) {
        return function (text) {
            return $sce.trustAsHtml(text);
        };
    }]);

angular.module('myapp')
    .factory('tweetsService', [
        '$rootScope', 'Hub', '$sce', function ($rootScope, hubService, $sce) {

            var tweetsService = this;
            tweetsService.tweets = [];

            var sotwInit = false;

            var addTweet = function (tweetStatus) {
                var existingTweets = tweetsService.tweets;
                if (existingTweets.length > 0) {
                    if (tweetStatus.id > existingTweets[0].id)
                        tweetsService.tweets.unshift(tweetStatus);
                    else
                        tweetsService.tweets.push(tweetStatus);
                } else {
                    tweetsService.tweets.push(tweetStatus);
                }

                console.log("INFO: tweetsFactory *** numTweets: " + tweetsService.tweets.length + " addTweet: " + tweetStatus.id_str);
                $rootScope.$apply();
            };

            var getSotw = function () {
                hub.stateOfTheWorld()
                    .done(function (tweetStatuses) {
                        sotwInit = true;
                        console.log("INFO: tweetsFactory *** SOTW done. Number of tweets received: " + tweetStatuses.length);
                        angular.forEach(tweetStatuses, function (tweetStatus) {
                            console.log("INFO: tweetsFactory *** tweet received #1 " + tweetStatus);
                            addTweet(tweetStatus);
                        });
                    });
            };

            var hub = new hubService('twitterFeedHub', {

                // client side functions
                listeners: {
                    'updateReadyState': function (state) {
                        console.log("INFO: tweetsFactory *** updateReadyState: " + state + " and sotwInit: " + sotwInit);
                        if (!sotwInit && state) {
                            getSotw();
                        }
                    },
                    'updateTweet': function (tweetStatus) {
                        console.log("INFO: tweetsFactory ***  Tweet Received");
                        addTweet(tweetStatus);
                    }
                },

                // server side methods
                methods: ['readyStateStatus', 'stateOfTheWorld'],

                queryParams: {}, // connection parameters
                errorHandler: function (err) { console.log('ERROR: tweetsFactory *** ' + err); },
                hubDisconnected: function () {
                    console.log('Client disconnected.');
                }
            });

            hub.connect().done(function () {
                hub.readyStateStatus()
                    .done(function (state) {
                        if (state) { // if server is ready with state and has tweets, then get sotw
                            console.log("INFO: tweetsFactory *** Ready State Status on connection is " + state);
                            getSotw();
                        }
                    });

            });


            return tweetsService;


        }
    ]);


angular.module('myapp')
    .controller('MainController', ['tweetsService', function (tweetsService) {

        this.tweets = tweetsService.tweets;

    }]);



using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using twitterfeed.Hubs;
using Twitterizer;

namespace twitterfeed.Services
{
    public class TimelineService
    {
        public static Lazy<TimelineService> ServiceInstance = new Lazy<TimelineService>(() => new TimelineService(GlobalHost.ConnectionManager.GetHubContext<TwitterHub>().Clients));

        private readonly ConcurrentDictionary<string, TwitterStatus> _tweets = new ConcurrentDictionary<string, TwitterStatus>();
        private decimal _lastTweetId;
        private Timer _timer;

        public static TimelineService Instance
        {
            get { return ServiceInstance.Value; }
        }

        private TimelineService(IHubConnectionContext<dynamic> clients)
        {

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;


            Clients = clients;
            Tokens = HttpContext.Current.Application["AuthTokens"] as OAuthTokens;


            UpdateTweets(null);
            //_timer = new Timer(UpdateTweets, LastTweetId, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30));

        }

        private void UpdateTweets(object state)
        {
            TwitterTimelineAsync.HomeTimeline(Tokens, new TimelineOptions() { Count = 200, SinceStatusId = LastTweetId, UseSSL = true }, new TimeSpan(0, 5, 0),
                f =>
                {
                    var statusCollection = f.ResponseObject;
                    foreach (var twitterStatus in statusCollection.Where(twitterStatus => _tweets.TryAdd(twitterStatus.StringId, twitterStatus)))
                    {
                        if (state != null)
                        {
                            // called from timer, so need to broadcast the update to clients
                            Clients.All.updateTweet(twitterStatus);
                        }
                        LastTweetId = twitterStatus.Id;
                    }
                    if (_timer == null)
                    {
                        ReadyState = true;
                        Clients.All.updateReadyState(ReadyState);
                        _timer = new Timer(UpdateTweets, LastTweetId, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30));
                    }

                });

        }

        public IEnumerable<TwitterStatus> StateOfTheWorld()
        {
            return _tweets.Values;
        }

        public bool ReadyState { get; set; }

        private OAuthTokens Tokens { get; set; }

        private decimal LastTweetId
        {
            get { return _lastTweetId; }
            set { if (value > _lastTweetId) _lastTweetId = value; }
        }

        public IHubConnectionContext<dynamic> Clients { get; set; }
    }
}
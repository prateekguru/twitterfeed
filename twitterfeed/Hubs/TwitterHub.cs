using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using twitterfeed.Services;
using Twitterizer;


namespace twitterfeed.Hubs
{
    [HubName("twitterFeedHub")]
    public class TwitterHub : Hub
    {
        private readonly TimelineService _service;

        public TwitterHub() : this(TimelineService.Instance)
        {
        }

        public TwitterHub(TimelineService service)
        {
            _service = service;
        }

        public IEnumerable<TwitterStatus> StateOfTheWorld()
        {
            return _service.StateOfTheWorld();
        }

        public bool ReadyStateStatus()
        {
            return _service.ReadyState;
        }

    }
}
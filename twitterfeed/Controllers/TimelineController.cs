using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Twitterizer;

namespace twitterfeed.Controllers
{
    public class TimelineController : ApiController
    {

        public TimelineController()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;

            Tokens = HttpContext.Current.Application["AuthTokens"] as OAuthTokens;
        }

        private OAuthTokens Tokens { get; set; }


        public IEnumerable<TwitterStatus> Get()
        {
            return GetTimeline(default(decimal));
        }


        public IEnumerable<TwitterStatus> Get(decimal lastTweetId)
        {
            return GetTimeline(lastTweetId);
        }

        private IEnumerable<TwitterStatus> GetTimeline(decimal lastTweetId)
        {
            var timelineOptions = new TimelineOptions
            {
                Count = 200,
                UseSSL = true,
                SinceStatusId = lastTweetId
            };

            var collection = TwitterTimeline.HomeTimeline(Tokens, timelineOptions);
            return collection.ResponseObject.AsEnumerable();
        }

        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}
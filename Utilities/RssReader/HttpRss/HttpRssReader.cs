using System;
using System.Net.Http;
using System.Threading.Tasks;
using CodeHollow.FeedReader;

namespace RssToSiteCreator.Utilities.RssReader.HttpRss
{
    /// <summary>
    /// Web上の RSS の読み取りを行う
    /// </summary>
    public class HttpRssReader : IRssReader
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HttpRssReader(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// フィードを読み込み、パースして返す
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<RssReadResult> ReadAsync(string url)
        {
            HttpResponseMessage response = null;
            var client = httpClientFactory.CreateClient();

            try
            {
                response = await client.GetAsync(url);
            }
            catch(Exception e)
            {
                return RssReadResult.CreateFailed(e.Message);
            }

            if (!response.IsSuccessStatusCode)
            {
                return RssReadResult.CreateFailed($"StatusCode: {response.StatusCode}");
            }

            var feedString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(feedString))
            {
                return RssReadResult.CreateFailed("Rss feed empty");
            }

            // RSSをパース
            var feed = FeedReader.ReadFromString(feedString);

            return RssReadResult.CreateSuccess(feed);
        }
    }
}
using CodeHollow.FeedReader;

namespace RssToSiteCreator.Utilities.RssReader
{
    /// <summary>
    /// RSS の読み込み結果を保持する
    /// </summary>
    public readonly struct RssReadResult
    {
        private RssReadResult(bool success, Feed feed, string errorMessage)
        {
            Success = success;
            Feed = feed;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; }

        public Feed Feed { get; }

        public string ErrorMessage { get; }

        public static RssReadResult CreateSuccess(Feed feed)
        {
            return new RssReadResult(true,feed, null);
        }

        public static RssReadResult CreateFailed(string errorMessage)
        {
            return new RssReadResult(false, null, errorMessage);
        }
    }
}

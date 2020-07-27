using CodeHollow.FeedReader.Feeds;

namespace RssToSiteCreator.Utilities.SiteCreator.FeedReaderExtensions
{
    public static class BaseFeedItemExtensions
    {
        /// <summary>
        /// Enclosureタグから画像URLを取得する
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetImageUrl(this BaseFeedItem item)
        {
            FeedItemEnclosure enclosure = null;

            switch (item)
            {
                case Rss092FeedItem i:
                    enclosure = i.Enclosure;
                    break;
                case Rss20FeedItem i:
                    enclosure = i.Enclosure;
                    break;
                default:
                    return null;
            }

            if (enclosure?.MediaType == null)
            {
                return null;
            }

            return enclosure.MediaType.StartsWith("image") ? enclosure.Url : null;
        }
    }
}
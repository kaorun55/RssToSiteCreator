using System;

namespace RssToSiteCreator.Utilities.SiteCreator
{
    public class SitePost
    {
        public string Author { get; set; }
        public string AuthorUrl { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PublishDate { get; set; }
    }
}
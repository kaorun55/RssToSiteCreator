namespace RssToSiteCreator.Utilities.SiteCreator.SiteDeployer.Netlify
{
    public class NetlifyDeployerSettings
    {
        /// <summary>
        /// Netlifyのアクセストークン
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Netlifyのデプロイ先サイトID
        /// </summary>
        public string SiteId { get; set; } = string.Empty;
    }
}
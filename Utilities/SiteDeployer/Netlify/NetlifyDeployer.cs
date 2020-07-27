using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NetlifySharp;

namespace RssToSiteCreator.Utilities.SiteCreator.SiteDeployer.Netlify
{
    /// <summary>
    /// Netlifyへサイトをデプロイする
    /// </summary>
    public class NetlifyDeployer : ISiteDeployer
    {
        private readonly NetlifyDeployerSettings settings;
        private readonly IHttpClientFactory httpClientFactory;

        public NetlifyDeployer(IOptions<NetlifyDeployerSettings> options, IHttpClientFactory httpClientFactory)
        {
            settings = options.Value;
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// サイトをデプロイする
        /// </summary>
        /// <param name="sourceDirectoryPath"></param>
        /// <returns></returns>
        public async Task DeployAsync(string sourceDirectoryPath)
        {
            var client = new NetlifyClient(settings.AccessToken, httpClientFactory.CreateClient());

            await client.UpdateSiteAsync(sourceDirectoryPath, settings.SiteId);
        }
    }
}

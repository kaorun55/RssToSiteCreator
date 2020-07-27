using System.Threading.Tasks;

namespace RssToSiteCreator.Utilities.SiteCreator.SiteDeployer
{
    public interface ISiteDeployer
    {
        /// <summary>
        /// サイトをデプロイする
        /// </summary>
        /// <param name="sourceDirectoryPath"></param>
        /// <returns></returns>
        Task DeployAsync(string sourceDirectoryPath);
    }
}
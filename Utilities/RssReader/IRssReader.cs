using System.Threading.Tasks;

namespace RssToSiteCreator.Utilities.RssReader
{
    public interface IRssReader
    {
        /// <summary>
        /// フィードを読み込み、パースして返す
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<RssReadResult> ReadAsync(string url);
    }
}
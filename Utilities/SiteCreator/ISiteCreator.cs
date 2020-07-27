using System.Collections.Generic;
using System.Threading.Tasks;

namespace RssToSiteCreator.Utilities.SiteCreator
{
    public interface ISiteCreator
    {
        /// <summary>
        /// テンプレートにしてしたポストを埋め込み、静的ファイルをコピーしてサイトを生成する
        /// 生成したサイトが保存されたパスを返す
        /// </summary>
        /// <param name="posts"></param>
        /// <returns></returns>
        Task<string> CreateAsync(IEnumerable<SitePost> posts);
    }
}
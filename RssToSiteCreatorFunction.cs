using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RssToSiteCreator.Utilities.RssReader;
using RssToSiteCreator.Utilities.SiteCreator;
using RssToSiteCreator.Utilities.SiteCreator.FeedReaderExtensions;
using RssToSiteCreator.Utilities.SiteCreator.SiteDeployer;

namespace RssToSiteCreator
{
    public class RssToSiteCreatorFunction
    {
        // タグを除去する正規表現
        // https://stackoverflow.com/questions/787932/using-c-sharp-regular-expressions-to-remove-html-tags
        private static readonly Regex removeHtmlTagRegex = new Regex(@"<[^>]*>");

        private readonly IRssReader rssReader;
        private readonly ISiteCreator siteCreator;
        private readonly ISiteDeployer siteDeployer;
        private readonly FunctionSettings settings;

        public RssToSiteCreatorFunction(IRssReader rssReader,ISiteCreator siteCreator, ISiteDeployer siteDeployer, IOptions<FunctionSettings> options)
        {
            this.rssReader = rssReader;
            this.siteCreator = siteCreator;
            this.siteDeployer = siteDeployer;

            // 環境変数を取得
            // https://docs.microsoft.com/ja-jp/azure/azure-functions/functions-dotnet-dependency-injection#working-with-options-and-settings
            settings = options.Value;
        }

        [FunctionName("RssToSiteCreatorFunction")]
        public async Task Run([TimerTrigger("%SCHEDULE_EXPRESSION%", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var rssReadResults = await Task.WhenAll(settings.RssSiteUrlsArray.Select(url => rssReader.ReadAsync(url)));

            foreach (var rssReadResult in rssReadResults.Where(result => !result.Success))
            {
                log.LogError($"Rss read failed: {rssReadResult.ErrorMessage}");
            }

            var convertPosts = 
                rssReadResults
                    .Where(result => result.Success)
                    .SelectMany(
                        result => result.Feed.Items, 
                        (result, item) =>
                            new SitePost
                            {
                                Author = result.Feed.Title,
                                AuthorUrl = result.Feed.Link,
                                Title = item.Title,
                                Summary = FormatSummary(item.Description),
                                Url = item.Link,
                                ImageUrl = item.SpecificItem.GetImageUrl(),
                                PublishDate = item.PublishingDate ?? DateTime.MinValue
                            }
                        );

            var siteDirectory = await siteCreator.CreateAsync(convertPosts);

            // サイトをデプロイ
            await siteDeployer.DeployAsync(siteDirectory);
        }

        /// <summary>
        /// Summary を表示用に変換する
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string FormatSummary(string source)
        {
            var withoutTag = removeHtmlTagRegex.Replace(source, string.Empty);

            if (withoutTag.Length > settings.SummaryLimit)
            {
                withoutTag = withoutTag.Substring(0, settings.SummaryLimit);
            }

            return withoutTag;
        }
    }

    public class FunctionSettings
    {
        /// <summary>
        /// RSS取得先サイトのURL（セミコロン区切りで複数指定可能）
        /// </summary>
        public string RssSiteUrls { get; set; } = string.Empty;

        private string[] rssSiteUrlsArray = null;
        /// <summary>
        /// RSS取得先サイトのURL配列
        /// </summary>
        public string[] RssSiteUrlsArray
        {
            get
            {
                if (rssSiteUrlsArray != null)
                {
                    return rssSiteUrlsArray;
                }

                return rssSiteUrlsArray = RssSiteUrls.Split(';');
            }
        }

        /// <summary>
        /// サマリの文字数上限
        /// </summary>
        public int SummaryLimit { get; set; } = 200;
    }
}

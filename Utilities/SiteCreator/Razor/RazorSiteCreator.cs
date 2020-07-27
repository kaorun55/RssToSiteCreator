using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Options;
using RazorEngine;
using RazorEngine.Templating;
using Encoding = System.Text.Encoding;

namespace RssToSiteCreator.Utilities.SiteCreator.Razor
{
    /// <summary>
    /// サイトの生成を行う
    /// </summary>
    public class RazorSiteCreator : ISiteCreator
    {
        private const string TemplatesDirectoryName = "Templates";
        private const string StaticDirectoryName = "Static";
        private const string OutputDirectoryName = "Output";

        private readonly SiteCreatorSettings settings;
        private readonly string appDirectory;

        public RazorSiteCreator(IOptions<SiteCreatorSettings> settingsOptions, IOptions<ExecutionContextOptions> contextOptions)
        {
            settings = settingsOptions.Value;
            appDirectory = contextOptions.Value.AppDirectory;
        }


        /// <summary>
        /// テンプレートにしてしたポストを埋め込み、静的ファイルをコピーしてサイトを生成する
        /// 生成したサイトが保存されたパスを返す
        /// </summary>
        /// <param name="posts"></param>
        /// <returns></returns>
        public async Task<string> CreateAsync(IEnumerable<SitePost> posts)
        {
            var templatesDirectoryPath = Path.Combine(appDirectory, TemplatesDirectoryName);
            var staticDirectoryPath = Path.Combine(appDirectory, StaticDirectoryName);
            var outputDirectoryPath = Path.Combine(appDirectory, OutputDirectoryName);

            // テンプレートを読み込む
            var templateFiles = Directory.Exists(templatesDirectoryPath) ? Directory.GetFiles(templatesDirectoryPath, "*", SearchOption.AllDirectories) : new string[0];

            // テンプレートに適用する
            var postsArray = posts as SitePost[] ?? posts.ToArray();
            var sortedPosts = postsArray.OrderByDescending(x => x.PublishDate).Take(settings.PostsLimit).ToArray();
            var authors = postsArray.Select(x => new {x.Author, x.AuthorUrl}).Distinct().ToArray();

            Directory.CreateDirectory(outputDirectoryPath);

            var model = new {Authors =  authors, Posts = sortedPosts};

            foreach (var templateFile in templateFiles)
            {
                var templateFileName = Path.GetFileName(templateFile);
                var template = await File.ReadAllTextAsync(templateFile, Encoding.UTF8);
                var output = Engine.Razor.RunCompile(
                    template, 
                    templateFileName,
                    null,
                    model);

                await File.WriteAllTextAsync(Path.Combine(outputDirectoryPath, templateFileName), output, Encoding.UTF8);
            }

            // 静的ファイルを展開する
            if (Directory.Exists(staticDirectoryPath))
            {
                DirectoryCopy(staticDirectoryPath, outputDirectoryPath, true);
            }

            return outputDirectoryPath;
        }

        /// <summary>
        /// ディレクトリのコピー
        /// https://docs.microsoft.com/ja-jp/dotnet/standard/io/how-to-copy-directories
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            var dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    var temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}

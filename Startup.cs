using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RssToSiteCreator.Utilities.RssReader;
using RssToSiteCreator.Utilities.RssReader.HttpRss;
using RssToSiteCreator.Utilities.SiteCreator;
using RssToSiteCreator.Utilities.SiteCreator.Razor;
using RssToSiteCreator.Utilities.SiteCreator.SiteDeployer;
using RssToSiteCreator.Utilities.SiteCreator.SiteDeployer.Netlify;

[assembly: FunctionsStartup(typeof(RssToSiteCreator.Startup))]
namespace RssToSiteCreator
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<FunctionSettings>()
                .Configure<IConfiguration>((settings, configuration) => configuration.Bind(settings));
            builder.Services.AddOptions<SiteCreatorSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                    configuration.GetSection("Site").Bind(settings));
            builder.Services.AddOptions<NetlifyDeployerSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                    configuration.GetSection("Netlify").Bind(settings));
            

            builder.Services.AddHttpClient();

            builder.Services.AddTransient<IRssReader, HttpRssReader>();
            builder.Services.AddTransient<ISiteCreator, RazorSiteCreator>();
            builder.Services.AddTransient<ISiteDeployer, NetlifyDeployer>();
        }
    }
}

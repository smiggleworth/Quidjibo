using Quidjibo.WebProxy.Clients;
using Quidjibo.WebProxy.Configurations;
using Quidjibo.WebProxy.Factories;

namespace Quidjibo.WebProxy.Extensions
{
    public static class QuidjiboBuilderExtensions
    {
        /// <summary>
        ///     Use WebProxy for Work, Progress, and Scheduled Jobs, and sets Quidjibo configuration
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static QuidjiboBuilder UseWebProxyServer(this QuidjiboBuilder builder, WebProxyQuidjiboConfiguration config)
        {
            var client = new WebProxyClient(config.Url, config.ClientId, config.ClientSecret);

            return builder.Configure(config)
                          .ConfigureWorkProviderFactory(new WebProxyWorkProviderFactory(builder.LoggerFactory, client))
                          .ConfigureProgressProviderFactory(new WebProxyProgressProviderFactory(builder.LoggerFactory, client))
                          .ConfigureScheduleProviderFactory(new WebProxyScheduleProviderFactory(builder.LoggerFactory, client));
        }
    }
}
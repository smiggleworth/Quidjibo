using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Constants;
using Quidjibo.Models;
using Quidjibo.WebProxy.Clients;
using Quidjibo.WebProxy.Models;
using Quidjibo.WebProxy.Providers;
using Quidjibo.WebProxy.Requests;

namespace Quidjibo.WebProxy.Tests.Providers
{
    [TestClass]
    public class WebProxyWorkProviderTests
    {
        private WebProxyWorkProvider _sut;
        private ILogger _logger;
        private IWebProxyClient _webProxyClient;

        [TestInitialize]
        public void Initialize()
        {
            _logger = Substitute.For<ILogger>();
            _webProxyClient = Substitute.For<IWebProxyClient>();
            _sut = new WebProxyWorkProvider(_logger, _webProxyClient, Default.Queues);
        }

        [TestMethod]
        public async Task SendAsync()
        {
            // Arrange
            var item = new WorkItem();

            _webProxyClient.PostAsync(
                Arg.Is<WebProxyRequest>(x => x.Path == "/work-items" && ((RequestData<WorkItem>)x.Data).Data == item),
                CancellationToken.None).Returns(Task.FromResult(new WebProxyResponse()));

            // Act
            await _sut.SendAsync(item, 0, CancellationToken.None);

            // Assert
            _logger.DidNotReceive().LogDebug(Arg.Any<string>());
        }
    }
}

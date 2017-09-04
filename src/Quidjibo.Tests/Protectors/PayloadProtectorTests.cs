using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Protectors;

namespace Quidjibo.Tests.Protectors
{
    [TestClass]
    public class PayloadProtectorTests
    {
        private PayloadProtector _sut;

        [TestInitialize]
        public void Init()
        {
            _sut = new PayloadProtector();
        }

        [TestMethod]
        public async Task ProtectAsync_PassThroughTest()
        {
            // Arrange
            var payload = Guid.NewGuid().ToByteArray();

            // Act
            var result = await _sut.ProtectAsync(payload, CancellationToken.None);

            // Assert
            result.SequenceEqual(payload).Should().BeTrue("payload protector is just a pass through");
        }

        [TestMethod]
        public async Task UnprotectAsync_PassThroughTest()
        {
            // Arrange
            var payload = Guid.NewGuid().ToByteArray();

            // Act
            var result = await _sut.UnprotectAsync(payload, CancellationToken.None);

            // Assert
            result.SequenceEqual(payload).Should().BeTrue("payload protector is just a pass through");
        }
    }
}
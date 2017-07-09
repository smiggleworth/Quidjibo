using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Protectors;
using Quidjibo.Serializers;
using Quidjibo.Tests.Samples;

namespace Quidjibo.Tests.Serializers
{
    [TestClass]
    public class PayloadSerializerTests
    {
        private PayloadSerializer _sut;
        private IPayloadProtector _protector;

        [TestInitialize]
        public void Init()
        {
            _protector = Substitute.For<IPayloadProtector>();
            _sut = new PayloadSerializer(_protector);
        }

        [TestMethod]
        public async Task SerializeAsync()
        {
            // Arrange
            var modelData = GenFu.GenFu.New<ModelData>();
            var command = new ComplexCommand(modelData);
            _protector.ProtectAsync(Arg.Any<byte[]>(),CancellationToken.None).Returns(x => x.Arg<byte[]>());

            // Act
            var payload = await _sut.SerializeAsync(command, CancellationToken.None);

            // Assert

        }
    }
}

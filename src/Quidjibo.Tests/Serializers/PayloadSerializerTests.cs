using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
        private IPayloadProtector _protector;
        private PayloadSerializer _sut;

        [TestInitialize]
        public void Init()
        {
            _protector = Substitute.For<IPayloadProtector>();
            _sut = new PayloadSerializer(_protector);
        }

        [TestMethod]
        public async Task Serializer_Should_Serialize_Then_Deserialize()
        {
            // Arrange
            var id = Guid.NewGuid();
            var modelData = GenFu.GenFu.New<ModelData>();
            var command = new ComplexCommand(id, modelData);
            _protector.ProtectAsync(Arg.Any<byte[]>(), CancellationToken.None).Returns(x => Task.FromResult(x.Arg<byte[]>()));
            _protector.UnprotectAysnc(Arg.Any<byte[]>(), CancellationToken.None).Returns(x => Task.FromResult(x.Arg<byte[]>()));

            // Act
            var serialized = await _sut.SerializeAsync(command, CancellationToken.None);
            var deserialized = await _sut.DeserializeAsync(serialized, CancellationToken.None);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized.Should().BeOfType<ComplexCommand>();
            var deserializedCommand = (ComplexCommand)deserialized;
            deserializedCommand.Id.Should().Be(id);
            deserializedCommand.Data.CreatedOn.Should().Be(modelData.CreatedOn);
            deserializedCommand.Data.Sequence.Should().Be(modelData.Sequence);
            deserializedCommand.Data.Name.Should().Be(modelData.Name);
        }
    }
}
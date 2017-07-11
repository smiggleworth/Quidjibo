using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Commands;
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

        [TestMethod]
        public async Task Serializer_Should_Serialize_Then_Deserialize_Worflow()
        {
            // Arrange
            var id1 = Guid.NewGuid();
            var modelData1 = GenFu.GenFu.New<ModelData>();
            var command1 = new ComplexCommand(id1, modelData1);

            var id2 = Guid.NewGuid();
            var modelData2 = GenFu.GenFu.New<ModelData>();
            var command2 = new ComplexCommand(id2, modelData2);

            var id3 = Guid.NewGuid();
            var modelData3 = GenFu.GenFu.New<ModelData>();
            var command3 = new ComplexCommand(id3, modelData3);

            var workflow = new WorkflowCommand(command1).Then(i => new[]
                {
                    command2,
                    command3
                });

            _protector.ProtectAsync(Arg.Any<byte[]>(), CancellationToken.None).Returns(x => Task.FromResult(x.Arg<byte[]>()));
            _protector.UnprotectAysnc(Arg.Any<byte[]>(), CancellationToken.None).Returns(x => Task.FromResult(x.Arg<byte[]>()));

            // Act
            var serialized = await _sut.SerializeAsync(workflow, CancellationToken.None);
            var deserialized = await _sut.DeserializeAsync(serialized, CancellationToken.None);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized.Should().BeOfType<WorkflowCommand>();

            var deserializedWorkflow = (WorkflowCommand)deserialized;

            var deserializedCommand1 = (ComplexCommand)deserializedWorkflow.Entries.First().Value.First();
            deserializedCommand1.Id.Should().Be(id1);
            deserializedCommand1.Data.CreatedOn.Should().Be(modelData1.CreatedOn);
            deserializedCommand1.Data.Sequence.Should().Be(modelData1.Sequence);
            deserializedCommand1.Data.Name.Should().Be(modelData1.Name);

            var deserializedCommand2 = (ComplexCommand)deserializedWorkflow.Entries.Last().Value.First();
            deserializedCommand2.Id.Should().Be(id2);
            deserializedCommand2.Data.CreatedOn.Should().Be(modelData2.CreatedOn);
            deserializedCommand2.Data.Sequence.Should().Be(modelData2.Sequence);
            deserializedCommand2.Data.Name.Should().Be(modelData2.Name);

            var deserializedCommand3 = (ComplexCommand)deserializedWorkflow.Entries.Last().Value.Last();
            deserializedCommand3.Id.Should().Be(id3);
            deserializedCommand3.Data.CreatedOn.Should().Be(modelData3.CreatedOn);
            deserializedCommand3.Data.Sequence.Should().Be(modelData3.Sequence);
            deserializedCommand3.Data.Name.Should().Be(modelData3.Name);
        }

       
    }
}
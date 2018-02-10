using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Commands;
using Quidjibo.Tests.Samples;

namespace Quidjibo.Tests.Commands
{
    [TestClass]
    public class WorkflowCommandTests
    {
        [TestMethod]
        public void WorkflowCommand_ConstructWithCommands()
        {
            // Arrange
            var command1 = new BasicCommand();
            var command2 = new BasicCommand();

            // Act
            var workflow = new WorkflowCommand(command1, command2);

            // Assert
            workflow.Step.Should().Be(1);
            workflow.Entries.Should().HaveCount(1);
            workflow.Entries.SelectMany(x => x.Value).Should().ContainInOrder(command1, command2);
        }


        [TestMethod]
        public void Then_ShouldAddCommandsToDictionaryInOrder()
        {
            // Arrange
            var workflow = new WorkflowCommand();
            var command1 = new BasicCommand();
            var command2 = new BasicCommand();
            var command3 = new BasicCommand();
            var command4 = new BasicCommand();
            var command5 = new BasicCommand();
            var command6 = new BasicCommand();

            // Act
            workflow.Then(i => command1)
                    .Then(i => new[]
                    {
                        command2,
                        command3
                    })
                    .Then(i => new[]
                    {
                        command4,
                        command5,
                        command6
                    });

            // Assert
            workflow.Entries.Should().HaveCount(3);
            workflow.Entries.Where(x => x.Key == 0).SelectMany(x => x.Value).Should().ContainInOrder(command1);
            workflow.Entries.Where(x => x.Key == 1).SelectMany(x => x.Value).Should().ContainInOrder(command2, command3);
            workflow.Entries.Where(x => x.Key == 2).SelectMany(x => x.Value).Should().ContainInOrder(command4, command5, command6);
        }


        [TestMethod]
        public void Then_ShouldNotAddNestedWorkflows()
        {
            // Arrange
            var workflow1 = new WorkflowCommand(new BasicCommand());
            var workflow2 = new WorkflowCommand(new BasicCommand());

            // Act
            Action then = () => workflow1.Then(i => workflow2);

            // Assert
            then.Should().Throw<InvalidOperationException>("Workflow commands cannot be nested.");
        }

        [TestMethod]
        public void NextStep_ShouldIncrementTheCurrentStepByOne()
        {
            // Arrange
            var workflow = new WorkflowCommand();
            var beginingStep = workflow.CurrentStep;

            // Act
            workflow.NextStep();

            // Assert
            workflow.CurrentStep.Should().Be(beginingStep + 1);
        }
    }
}
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Attributes;
using Quidjibo.Misc;
using Quidjibo.Models;

namespace Quidjibo.Tests.Attributes
{
    [TestClass]
    public class ScheduleAttributeTests
    {
        [TestMethod]
        public void ScheduleAttributeTest_WithExpression()
        {
            // Arrange
            var name = "Test";
            var expression = "* * * * *";

            // Act
            var attribute = new ScheduleAttribute(name, expression);

            // Assert
            attribute.Name.Should().Be(name);
            attribute.Cron.Expression.Should().Be(expression);
            attribute.Queue.Should().Be("default");
            attribute.ClientKey.Should().Be(typeof(DefaultClientKey));
        }

        [TestMethod]
        public void ScheduleAttributeTest_WithCron()
        {
            // Arrange
            var name = "Test";
            var expression = "* * * * *";
            var queue = "custom";

            // Act
            var attribute = new ScheduleAttribute(name, expression, queue);

            // Assert
            attribute.Name.Should().Be(name);
            attribute.Cron.Expression.Should().Be(expression);
            attribute.Queue.Should().Be(queue);
            attribute.ClientKey.Should().Be(typeof(DefaultClientKey));
        }

        [TestMethod]
        public void ScheduleAttributeTest_WithCustomKey()
        {
            // Arrange
            var name = "Test";
            var expression = "* * * * *";
            var queue = "custom";
            var key = typeof(CustomClientKey);

            // Act
            var attribute = new ScheduleAttribute(name, expression, queue, typeof(CustomClientKey));

            // Assert
            attribute.Name.Should().Be(name);
            attribute.Cron.Expression.Should().Be(expression);
            attribute.Queue.Should().Be(queue);
            attribute.ClientKey.Should().Be(key);
        }


        private class CustomClientKey : IQuidjiboClientKey { }
    }
}
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
            var expression = "* * * * *";

            // Act
            var attribute = new ScheduleAttribute(expression);

            // Assert
            attribute.Cron.Expression.Should().Be(expression);
            attribute.ClientKey.Should().Be(typeof(DefaultClientKey));
        }

        [TestMethod]
        public void ScheduleAttributeTest_WithCron()
        {
            // Arrange
            var cron = new Cron("* * * * *");

            // Act
            var attribute = new ScheduleAttribute(cron);

            // Assert
            attribute.Cron.Should().Be(cron);
            attribute.ClientKey.Should().Be(typeof(DefaultClientKey));
        }

        [TestMethod]
        public void ScheduleAttributeTest_WithCustomKey()
        {
            // Arrange
            var expression = "* * * * *";
            var key = typeof(CustomClientKey);

            // Act
            var attribute = new ScheduleAttribute(expression, key);

            // Assert
            attribute.Cron.Expression.Should().Be(expression);
            attribute.ClientKey.Should().Be(typeof(CustomClientKey));
        }


        private class CustomClientKey : IQuidjiboClientKey { }
    }
}
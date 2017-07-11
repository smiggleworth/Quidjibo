using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Providers;

namespace Quidjibo.Tests.Providers
{
    [TestClass]
    public class CronProviderTests
    {
        private CronProvider _sut;

        [TestInitialize]
        public void Init()
        {
            _sut = new CronProvider();
        }


        [DataTestMethod]
        [DataRow("", DisplayName = "No Parts")]
        [DataRow("*", DisplayName = "One Parts")]
        [DataRow("* *", DisplayName = "Two Parts")]
        [DataRow("* * *", DisplayName = "Three Parts")]
        [DataRow("* * * *", DisplayName = "Four Parts")]
        public void GetSchedule_RequiresFivePartCronExpression(string expression)
        {
            // Act
            Action action = () => _sut.GetSchedule(expression, DateTime.UtcNow);

            // Assert
            action.ShouldThrow<InvalidOperationException>().WithMessage("Expression must contain 5 parts");
        }

        [DataTestMethod]
        [DataRow("* * * * *", "2017-07-11 09:14:00", DisplayName = "Every minute")]
        public void GetNextScheduleTests(string expression, string expected)
        {
            // Arrange
            var start = new DateTime(2017, 7, 11, 9, 13, 27);

            // Act
            var result = _sut.GetNextSchedule(expression, start);

            // Assert
            var expectedDateTime = DateTime.Parse(expected);
            result.Should().Be(expectedDateTime);
        }


        [DataTestMethod]
        [DataRow("* * * * *", "2017-07-11 09:14:00", "2017-07-11 09:15:00", "2017-07-11 09:16:00", "2017-07-11 09:17:00", "2017-07-11 09:18:00", DisplayName = "Every minute")]
        public void GetScheduleTests(string expression, string expected1, string expected2, string expected3, string expected4, string expected5)
        {
            // Arrange
            var start = new DateTime(2017, 7, 11, 9, 13, 27);

            // Act
            var results = _sut.GetSchedule(expression, start).Take(5).ToList();

            // Assert
            var expectedDateTime1 = DateTime.Parse(expected1);
            var expectedDateTime2 = DateTime.Parse(expected2);
            var expectedDateTime3 = DateTime.Parse(expected3);
            var expectedDateTime4 = DateTime.Parse(expected4);
            var expectedDateTime5 = DateTime.Parse(expected5);

            results.Should().ContainInOrder(
                expectedDateTime1,
                expectedDateTime2,
                expectedDateTime3,
                expectedDateTime4,
                expectedDateTime5);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Models;
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

//        [TestMethod]
//        public void GetSchedule_ScheduleBeginsAtStart()
//        {
//            // Arrange
//            var expression = "0 0 * * 0";
//            var start = DateTime.UtcNow;
//
//            // Act
//            var result = _sut.GetSchedule(expression,start).Take(10);
//
//            // Assert
//            result.Should().BeEquivalentTo();
//        }

    }
}

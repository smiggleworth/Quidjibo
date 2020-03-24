using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Models;
using Quidjibo.SqlServer.Extensions;
using Quidjibo.SqlServer.Providers;
using Quidjibo.SqlServer.Utils;

namespace Quidjibo.SqlServer.Tests.Extensions
{
    [TestClass]
    public class SqlCommandExtensionsTests
    {
        private Random _random = new Random();

        [TestMethod]
        public async Task PrepareForSendAsync_SetCommandParameters()
        {
            // Arrange
            WorkItem item = GenFu.GenFu.New<WorkItem>();
            item.ScheduleId = Guid.NewGuid();
            item.Payload = new byte[128];
            _random.NextBytes(item.Payload);
            SqlCommand cmd = new SqlCommand();

            // Act
            await cmd.PrepareForSendAsync(item, 0, CancellationToken.None);

            // Assert
            cmd.CommandText.Should().Be(await SqlLoader.GetScript("Work.Send"));
            cmd.Parameters.Count.Should().Be(12);
            cmd.Parameters["@Id"].Value.Should().Be(item.Id);
            cmd.Parameters["@ScheduleId"].Value.Should().Be(item.ScheduleId);
            cmd.Parameters["@CorrelationId"].Value.Should().Be(item.CorrelationId);
            cmd.Parameters["@Name"].Value.Should().Be(item.Name);
            cmd.Parameters["@Worker"].Value.Should().Be(item.Worker);
            cmd.Parameters["@Queue"].Value.Should().Be(item.Queue);
            cmd.Parameters["@Attempts"].Value.Should().Be(item.Attempts);
            cmd.Parameters["@CreatedOn"].Value.Should().Match<DateTime>(
                x => x > DateTime.UtcNow.AddMinutes(-1) && x < DateTime.UtcNow.AddMinutes(1),
                "because it should always be set to UtcNow");
            cmd.Parameters["@ExpireOn"].Value.Should().Be(item.ExpireOn);
            cmd.Parameters["@VisibleOn"].Value.Should().Be(
                cmd.Parameters["@CreatedOn"].Value,
                "because there is no delay");
            cmd.Parameters["@Status"].Value.Should().Be(SqlWorkProvider.StatusFlags.New);
            cmd.Parameters["@Payload"].Value.Should().Be(item.Payload);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(100)]
        public async Task PrepareForSendAsync_AdjustVisibleOnAccordingToDelay(int delay)
        {
            // Arrange
            WorkItem item = GenFu.GenFu.New<WorkItem>();
            SqlCommand cmd = new SqlCommand();

            // Act
            await cmd.PrepareForSendAsync(item, delay, CancellationToken.None);

            // Assert
            DateTime createdOn = (DateTime)cmd.Parameters["@CreatedOn"].Value;
            cmd.Parameters["@VisibleOn"].Value.Should().Be(createdOn.AddSeconds(delay));
        }

        [TestMethod]
        public async Task PrepareForSendAsync_ExpireOnIsNotSet_ShouldSetExpireOnToDefault()
        {
            // Arrange
            WorkItem item = GenFu.GenFu.New<WorkItem>();
            item.ExpireOn = default;
            SqlCommand cmd = new SqlCommand();

            // Act
            await cmd.PrepareForSendAsync(item, 0, CancellationToken.None);

            // Assert
            cmd.Parameters["@ExpireOn"].Value.Should().Match<DateTime>(x => x.Date == DateTime.UtcNow.AddDays(SqlWorkProvider.DEFAULT_EXPIRE_DAYS).Date);
        }
        
        [TestMethod]
        public async Task PrepareForFaultAsync_SetCommandParameters()
        {
            // Arrange
            WorkItem item = GenFu.GenFu.New<WorkItem>();
            SqlCommand cmd = new SqlCommand();

            // Act
            await cmd.PrepareForFaultAsync(item, 60, CancellationToken.None);

            // Assert
            cmd.CommandText.Should().Be(await SqlLoader.GetScript("Work.Fault"));
            cmd.Parameters.Count.Should().Be(3);
            cmd.Parameters["@Id"].Value.Should().Be(item.Id);
            cmd.Parameters["@VisibleOn"].Value.Should().Match<DateTime>(
                x => x > DateTime.UtcNow.AddSeconds(55) && x < DateTime.UtcNow.AddSeconds(65));
            cmd.Parameters["@Faulted"].Value.Should().Be(SqlWorkProvider.StatusFlags.Faulted);
        }
        
        [TestMethod]
        public async Task PrepareForFaultAsync_VisibilityTimeoutUnderThirtySeconds_VisibleOnShouldBeAtLeastThirtySeconds()
        {
            // Arrange
            WorkItem item = GenFu.GenFu.New<WorkItem>();
            SqlCommand cmd = new SqlCommand();

            // Act
            await cmd.PrepareForFaultAsync(item, 15, CancellationToken.None);

            // Assert
            cmd.CommandText.Should().Be(await SqlLoader.GetScript("Work.Fault"));
            cmd.Parameters.Count.Should().Be(3);
            cmd.Parameters["@Id"].Value.Should().Be(item.Id);
            cmd.Parameters["@VisibleOn"].Value.Should().Match<DateTime>(
                x => x > DateTime.UtcNow.AddSeconds(25) && x < DateTime.UtcNow.AddSeconds(35));
            cmd.Parameters["@VisibleOn"].Value.Should().Match<DateTime>(x => x < DateTime.UtcNow.AddSeconds(30));
            cmd.Parameters["@Faulted"].Value.Should().Be(SqlWorkProvider.StatusFlags.Faulted);
        }
                
        [TestMethod]
        public async Task PrepareForReneweAsync_SetCommandParameters()
        {
            // Arrange
            WorkItem item = GenFu.GenFu.New<WorkItem>();
            DateTime lockExpireOn = DateTime.UtcNow.AddSeconds(60);
            SqlCommand cmd = new SqlCommand();

            // Act
            await cmd.PrepareForRenewAsync(item, lockExpireOn, CancellationToken.None);

            // Assert
            cmd.CommandText.Should().Be(await SqlLoader.GetScript("Work.Renew"));
            cmd.Parameters.Count.Should().Be(2);
            cmd.Parameters["@Id"].Value.Should().Be(item.Id);
            cmd.Parameters["@VisibleOn"].Value.Should().Be(lockExpireOn);
        }
                
        [TestMethod]
        public async Task PrepareForCompleteAsync_SetCommandParameters()
        {
            // Arrange
            WorkItem item = GenFu.GenFu.New<WorkItem>();
            SqlCommand cmd = new SqlCommand();

            // Act
            await cmd.PrepareForCompleteAsync(item, CancellationToken.None);

            // Assert
            cmd.CommandText.Should().Be(await SqlLoader.GetScript("Work.Complete"));
            cmd.Parameters.Count.Should().Be(2);
            cmd.Parameters["@Id"].Value.Should().Be(item.Id);
            cmd.Parameters["@Complete"].Value.Should().Be(SqlWorkProvider.StatusFlags.Complete);
        }

    }
}

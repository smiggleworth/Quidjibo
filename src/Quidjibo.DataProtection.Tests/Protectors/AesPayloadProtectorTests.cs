using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quidjibo.DataProtection.Tests.Protectors
{
    [TestClass]
    public class AesPayloadProtectorTests
    {
        private AesPayloadProtector _sut = new AesPayloadProtector();




        [TestMethod]
        public void ProtectTests()
        {
            var test = "test";
            test.Should().Be("test");
        }


        public void UnprotectTests()
        {
        }
    }
}

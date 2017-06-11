using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Misc;

namespace Quidjibo.Tests.Misc
{
    [TestClass]
    public class ProviderCacheKeyTests
    {
        [TestMethod]
        public void When_Same_KeyType_And_Name()
        {
            var key1 = new ProviderCacheKey<TestClientKey1>("test");
            var key2 = new ProviderCacheKey<TestClientKey1>("test");
            key2.Should().Be(key1);
        }

        [TestMethod]
        public void When_Same_KeyType_And_Different_Name()
        {
            var key1 = new ProviderCacheKey<TestClientKey1>("test1");
            var key2 = new ProviderCacheKey<TestClientKey1>("test2");
            key2.Should().NotBe(key1);
        }

        [TestMethod]
        public void When_Different_KeyType_And_Same_Name()
        {
            var key1 = new ProviderCacheKey<TestClientKey1>("test");
            var key2 = new ProviderCacheKey<TestClientKey2>("test");
            key2.Should().NotBe(key1);
        }

        [TestMethod]
        public void When_Different_KeyType_And_Different_Name()
        {
            var key1 = new ProviderCacheKey<TestClientKey1>("test1");
            var key2 = new ProviderCacheKey<TestClientKey2>("test2");
            key2.Should().NotBe(key1);
        }

        public class TestClientKey1 : IQuidjiboClientKey
        {
        }

        public class TestClientKey2 : IQuidjiboClientKey
        {
        }
    }
}
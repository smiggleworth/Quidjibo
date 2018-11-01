using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Quidjibo.DataProtection.Protectors;
using Quidjibo.DataProtection.Providers;

namespace Quidjibo.DataProtection.Tests.Protectors
{
    [TestClass]
    public class AesPayloadProtectorTests
    {
        private PayloadModel _model;
        private AesPayloadProtector _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _model = GenFu.GenFu.New<PayloadModel>();
            using (var aes = Aes.Create())
            {
                _sut = new AesPayloadProtector(new KeyProvider(aes.Key));
            }
        }


        [TestMethod]
        public async Task ProtectAndUnprotectTest()
        {
            var payload = _model.GetPayload();

            var protectedPayload = await _sut.ProtectAsync(payload, CancellationToken.None);
            protectedPayload.SequenceEqual(payload).Should().BeFalse();

            var unprotectedPayload = await _sut.UnprotectAsync(protectedPayload, CancellationToken.None);
            unprotectedPayload.SequenceEqual(payload).Should().BeTrue();
        }


        [TestMethod]
        public async Task ShouldVerifyTest()
        {
            var payload = _model.GetPayload();

            var protectedPayload = await _sut.ProtectAsync(payload, CancellationToken.None);

            for(var i = 1; i < protectedPayload.Length; i++)
            {
                var payloadCopy = new byte[protectedPayload.Length];
                protectedPayload.AsSpan().CopyTo(payloadCopy);
                payloadCopy[i] ^= 255;

                Func<Task> sut = async () => await _sut.UnprotectAsync(payloadCopy, CancellationToken.None);

                sut.Should().Throw<Exception>().WithMessage("MAC mismatch");
            }
        }


        public class PayloadModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public DateTime Date { get; set; }
            public int Number { get; set; }

            public byte[] GetPayload()
            {
                return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
            }
        }
    }
}
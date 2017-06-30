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
                _sut = new AesPayloadProtector(aes.Key);
            }
        }


        [TestMethod]
        public async Task ProtectAndUnprotectTest()
        {
            var payload = _model.GetPayload();

            var protectedPayload = await _sut.ProtectAsync(payload, CancellationToken.None);
            protectedPayload.SequenceEqual(payload).Should().BeFalse();

            var unprotectedPayload = await _sut.UnprotectAysnc(protectedPayload, CancellationToken.None);
            unprotectedPayload.SequenceEqual(payload).Should().BeTrue();
        }


        [TestMethod]
        public async Task ShouldVerifyTest()
        {
            var payload = _model.GetPayload();

            var protectedPayload = await _sut.ProtectAsync(payload, CancellationToken.None);

            protectedPayload[49] = (byte)(protectedPayload[49] ^ (byte)255);

            Func<Task> sut = async () => await _sut.UnprotectAysnc(protectedPayload, CancellationToken.None);

            sut.ShouldThrow<Exception>().WithMessage("MAC mismatch");
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
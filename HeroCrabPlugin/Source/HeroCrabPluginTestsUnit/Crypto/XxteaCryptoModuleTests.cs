// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.
using System.Text;
using HeroCrabPlugin.Crypto;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Crypto
{
    [TestFixture]
    public class XxteaCryptoModuleTests
    {
        private ICryptoModule _cryptoModule;
        private string _message = "Secret test message!";
        private string _key = "1234Key54321";

        [SetUp]
        public void Setup()
        {
            _cryptoModule = new XxteaCryptoModule();
        }

        [Test]
        public void Encrypt_EncryptStringWithStringAndDecrypt_ReturnsString()
        {
            var bytes = _cryptoModule.Encrypt(_message, _key);
            var message = _cryptoModule.Decrypt(bytes, _key);

            Assert.That(message, Is.EqualTo(_message));
        }

        [Test]
        public void Encrypt_EncryptBytesWithStringAndDecrypt_ReturnsString()
        {
            var messageBytes = Encoding.UTF8.GetBytes(_message);
            var bytes = _cryptoModule.Encrypt(messageBytes, _key);
            var message = _cryptoModule.Decrypt(bytes, _key);

            Assert.That(message, Is.EqualTo(_message));
        }
    }
}

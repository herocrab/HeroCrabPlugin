/* Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
Distributed under the MIT license. See the LICENSE.md file in the project root for more information. */
using HeroCrabPlugin.Core;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Core
{
    [TestFixture]
    public class NetServicesTests
    {
        private class FakeService
        {
        }

        [Test]
        public void Add_AddAnAObject_GetAnObjectVerifySame()
        {
            NetServices.Registry.Clear();

            var fakeService = new FakeService();
            NetServices.Registry.Add(fakeService);
            var service = NetServices.Registry.Get<FakeService>();
            Assert.That(service, Is.EqualTo(fakeService));
        }

        [Test]
        public void Get_GetAnObjectThatDoesNotExist_ThrowsNullReferenceException()
        {
            NetServices.Registry.Clear();
            var service = NetServices.Registry.Get<FakeService>();
            Assert.That(service, Is.Null);
        }

        [Test]
        public void Get_RemoveAnExistingObject_ObjectIsRemovedAndThrowsNullReferenceException()
        {
            NetServices.Registry.Clear();

            var fakeService = new FakeService();
            NetServices.Registry.Add(fakeService);
            NetServices.Registry.Remove<FakeService>();

            var service = NetServices.Registry.Get<FakeService>();

            Assert.That(NetServices.Registry.Count, Is.EqualTo(0));
            Assert.That(service, Is.Null);
        }

        [Test]
        public void Remove_RemoveANonExistentObject_VerifyNoExceptionIsThrown()
        {
            NetServices.Registry.Clear();
            Assert.DoesNotThrow(NetServices.Registry.Remove<FakeService>);
        }

        [Test]
        public void Add_AddAnObjectOfTypeThatAlreadyExists_OverwriteTheObjectType()
        {
            NetServices.Registry.Clear();

            var fakeServiceA = new FakeService();
            var fakeServiceB = new FakeService();
            NetServices.Registry.Add(fakeServiceA);
            NetServices.Registry.Add(fakeServiceB);

            var service = NetServices.Registry.Get<FakeService>();
            Assert.That(service, Is.EqualTo(fakeServiceB));
            Assert.That(NetServices.Registry.Count, Is.EqualTo(1));
        }
    }
}

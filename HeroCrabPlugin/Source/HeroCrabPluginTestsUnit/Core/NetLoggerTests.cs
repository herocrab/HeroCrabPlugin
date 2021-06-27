using HeroCrabPlugin.Core;
using Moq;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Core
{
    [TestFixture]
    public class NetLoggerTests
    {
        [Test]
        public void Add_AddTwoNetworkLoggersAndWrite_VerifyTwoLoggersWriteMethodsCalled()
        {
            var loggerA = new Mock<INetLogger>();
            loggerA.SetupAllProperties();

            var loggerB = new Mock<INetLogger>();
            loggerB.SetupAllProperties();

            var networkLogger = new NetLogger(loggerA.Object)
            {
                Mask = NetLogger.LoggingGroup.Custom
            };

            networkLogger.Add(loggerB.Object);

            networkLogger.Write(NetLogger.LoggingGroup.Custom,this, "Test message...");
            loggerA.Verify(a => a.Write(It.IsAny<object>(),
                It.IsAny<string>()), Times.Once());

            loggerB.Verify(a => a.Write(It.IsAny<object>(),
                It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void Add_AddTheSameLoggerTwoTimes_VerifyOnlyOneLoggerExists()
        {
            var loggerA = new Mock<INetLogger>();
            loggerA.SetupAllProperties();

            var networkLogger = new NetLogger(loggerA.Object)
            {
                Mask = NetLogger.LoggingGroup.Custom
            };

            networkLogger.Add(loggerA.Object);

            networkLogger.Write(NetLogger.LoggingGroup.Custom,this, "Test message...");
            loggerA.Verify(a => a.Write(It.IsAny<object>(),
                It.IsAny<string>()), Times.Once());

            Assert.That(networkLogger.Count, Is.EqualTo(1));
        }

        [Test]
        public void Add_AddANetworkLoggerRemoveSameLogger_VerifyLoggerIsRemoved()
        {
            var loggerA = new Mock<INetLogger>();
            loggerA.SetupAllProperties();

            var networkLogger = new NetLogger(loggerA.Object)
            {
                Mask = NetLogger.LoggingGroup.Custom
            };

            networkLogger.Remove(loggerA.Object);
            networkLogger.Write(NetLogger.LoggingGroup.Custom,this, "Test message...");
            Assert.That(networkLogger.Count, Is.EqualTo(0));
        }

        [Test]
        public void Write_WriteLogEntry_VerifyDelegateIsCalled()
        {
            var logWriteCount = 0;
            void OnLogWrite(object sender, string message)
            {
                logWriteCount++;
            }

            var loggerA = new Mock<INetLogger>();
            loggerA.SetupAllProperties();

            var networkLogger = new NetLogger(loggerA.Object)
            {
                Mask = NetLogger.LoggingGroup.Custom
            };

            networkLogger.LogWrite += OnLogWrite;
            networkLogger.Write(NetLogger.LoggingGroup.Custom,this, "Test message...");
            Assert.That(logWriteCount, Is.EqualTo(1));
        }

        [Test]
        public void Write_WriteLogEntryFromGroupNotInMask_VerifyDelegateIsNotCalled()
        {
            var logWriteCount = 0;
            void OnLogWrite(object sender, string message)
            {
                logWriteCount++;
            }

            var loggerA = new Mock<INetLogger>();
            loggerA.SetupAllProperties();

            var networkLogger = new NetLogger(loggerA.Object)
            {
                Mask = NetLogger.LoggingGroup.Custom
            };

            networkLogger.LogWrite += OnLogWrite;
            networkLogger.Write(NetLogger.LoggingGroup.Session,this, "Test message...");
            Assert.That(logWriteCount, Is.EqualTo(0));
        }

        [Test]
        public void Write_WriteLogEntryFromNullSender_VerifyDelegateIsCalledWithNullStart()
        {
            var logWriteCount = 0;
            var senderName = string.Empty;
            void OnLogWrite(object sender, string message)
            {
                logWriteCount++;
                senderName = sender.ToString();
            }

            var loggerA = new Mock<INetLogger>();
            loggerA.SetupAllProperties();

            var networkLogger = new NetLogger(loggerA.Object)
            {
                Mask = NetLogger.LoggingGroup.Custom
            };

            networkLogger.LogWrite += OnLogWrite;
            networkLogger.Write(NetLogger.LoggingGroup.Custom,null, "Test message...");
            Assert.That(logWriteCount, Is.EqualTo(1));
            Assert.That(senderName, Is.EqualTo("Null"));
        }
    }
}

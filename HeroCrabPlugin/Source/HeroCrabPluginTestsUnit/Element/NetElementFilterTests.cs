// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using HeroCrabPlugin.Element;
using HeroCrabPlugin.Stream;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Element
{
    [TestFixture]
    public class NetElementFilterTests
    {
        [Test]
        public void Add_ValueAssignmentTeamOne_VerifyBitMaskValues()
        {
            var elementFilter = new NetElementFilter();
            elementFilter.StreamGroup |= NetStreamGroup.Team1;

            Assert.That(elementFilter.Contains(NetStreamGroup.Team1));
            Assert.That(elementFilter.Contains(NetStreamGroup.Team2), Is.Not.True);
        }

        [Test]
        public void Add_ValueAssignmentTeamOne_VerifyByGroupValue()
        {
            var elementFilter = new NetElementFilter();
            elementFilter.StreamGroup |= NetStreamGroup.Team1;

            Assert.That(elementFilter.Contains(NetStreamGroup.Team1));
        }

        [Test]
        public void Add_ValueAssignmentTeam1TeamTwo_VerifyByGroupValueMinusTeamTwo()
        {
            var elementFilter = new NetElementFilter();
            elementFilter.StreamGroup |= NetStreamGroup.Team1;
            elementFilter.StreamGroup |= NetStreamGroup.Team2;

            Assert.That(elementFilter.Contains(NetStreamGroup.Team1));
        }

        [Test]
        public void Add_ValueAssignmentTeamOne_VerifyByGroupValuePlusTeamTwo()
        {
            var elementFilter = new NetElementFilter();
            elementFilter.StreamGroup |= NetStreamGroup.Team1;

            Assert.That(elementFilter.Contains(NetStreamGroup.Team1 | NetStreamGroup.Team2), Is.False);
        }

        [Test]
        public void Add_ValueAssignmentTeamOne_VerifyByGroupValueThatElementContainsMask()
        {
            var elementFilter = new NetElementFilter();
            elementFilter.StreamGroup |= NetStreamGroup.Team1;
            elementFilter.StreamGroup |= NetStreamGroup.Team2;

            Assert.That(elementFilter.Contains(NetStreamGroup.Team1));
        }

        [Test]
        public void Add_ValueAssignmentTeamOneTwice_VerifyBitMaskValues()
        {
            var elementFilter = new NetElementFilter();
            elementFilter.StreamGroup |= NetStreamGroup.Team1;
            elementFilter.StreamGroup |= NetStreamGroup.Team1;

            Assert.That(elementFilter.Contains(NetStreamGroup.Team1));
            Assert.That(elementFilter.Contains(NetStreamGroup.Team2), Is.Not.True);
        }

        [Test]
        public void Remove_ValueAssignmentTeamOneRemoveSeenByTeamOne_VerifyBitMaskValues()
        {
            var elementFilter = new NetElementFilter();
            elementFilter.StreamGroup |= NetStreamGroup.Team1;
            elementFilter.StreamGroup -= NetStreamGroup.Team1;

            Assert.That(elementFilter.Contains(NetStreamGroup.Team1), Is.Not.True);
        }

        [Test]
        public void Constructor_DefaultConstructor_VerifyBitMaskValues()
        {
            var elementFilter = new NetElementFilter();

            Assert.That(elementFilter.Contains(NetStreamGroup.Default));
            Assert.That(elementFilter.Contains(NetStreamGroup.Team1), Is.Not.True);
        }
    }
}

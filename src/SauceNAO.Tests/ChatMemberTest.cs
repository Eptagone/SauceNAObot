// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using SauceNAO.Core.Extensions;

namespace SauceNAO.Tests
{
    public sealed class ChatMemberTest
    {
        [Fact]
        public void IsNotMemberOrAdministrator()
        {
            ChatMember member1 = new ChatMemberAdministrator();
            ChatMember member2 = new ChatMemberMember();
            ChatMember member3 = new ChatMemberRestricted();

            bool Test(ChatMember member) => !member.IsMemberOrAdmin();

            Assert.False(Test(member1));
            Assert.False(Test(member2));
            Assert.True(Test(member3));
        }
    }
}

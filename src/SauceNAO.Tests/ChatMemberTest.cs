// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Extensions;
using Telegram.BotAPI.AvailableTypes;
using Xunit;

namespace SauceNAO.Tests;

public sealed class ChatMemberTest
{
	[Fact]
	public void IsNotMemberOrAdministrator()
	{
		ChatMember member1 = new ChatMemberAdministrator();
		ChatMember member2 = new ChatMemberMember();
		ChatMember member3 = new ChatMemberRestricted();

		bool Test(ChatMember member)
		{
			return !member.IsMemberOrAdmin();
		}

		Assert.False(Test(member1));
		Assert.False(Test(member2));
		Assert.True(Test(member3));
	}
}

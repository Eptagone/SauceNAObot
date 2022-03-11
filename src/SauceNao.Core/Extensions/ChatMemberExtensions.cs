// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Core.Extensions
{
    public static class ChatMemberExtensions
    {
        public static bool IsOwner(this ChatMember member)
        {
            return member is ChatMemberOwner;
        }

        public static bool IsAdmin(this ChatMember member)
        {
            return member is ChatMemberAdministrator;
        }

        public static bool IsAdminOrOwner(this ChatMember member)
        {
            return member.IsAdmin() || member.IsOwner();
        }

        public static bool IsMember(this ChatMember member)
        {
            return member is ChatMemberMember;
        }

        public static bool IsMemberOrAdmin(this ChatMember member)
        {
            return member is ChatMemberMember || member.IsAdmin();
        }

        public static bool IsMemberOrAdminOrOwner(this ChatMember member)
        {
            return member is ChatMemberMember || member.IsAdminOrOwner();
        }

        public static bool IsRestricted(this ChatMember member)
        {
            return member is ChatMemberRestricted;
        }

        public static bool IsBanned(this ChatMember member)
        {
            return member is ChatMemberBanned;
        }

        public static bool IsLeft(this ChatMember member)
        {
            return member is ChatMemberLeft;
        }
    }
}

using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Features.AntiCheats;

class AlreadyRemovedAntiCheatsException(Message message, User target) : AntiCheatsException(message)
{
    public User Target => target;
    public override string DisplayErrorKey => "AnticheatsAlreadyRemoved";
}

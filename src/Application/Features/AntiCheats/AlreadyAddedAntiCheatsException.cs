using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Features.AntiCheats;

class AlreadyAddedAntiCheatsException(Message message, User target) : AntiCheatsException(message)
{
    public User Target => target;
    public override string DisplayErrorKey => "AnticheatsAlreadyAdded";
}

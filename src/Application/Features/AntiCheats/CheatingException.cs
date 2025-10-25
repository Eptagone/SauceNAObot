using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Features.AntiCheats;

class CheatingException(Message message) : AntiCheatsException(message)
{
    public override string DisplayErrorKey => "AnticheatsNoCheats";
}

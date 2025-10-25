using SauceNAO.Application.Features.AntiCheats;
using Telegram.BotAPI.AvailableTypes;

class NotABotException(Message message) : AntiCheatsException(message)
{
    public override string DisplayErrorKey => "AntiCheatsNotABot";
}

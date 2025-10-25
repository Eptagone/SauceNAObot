using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Features.Search;

class NoPublicKeysException(Message message) : SauceException(message)
{
    public override string DisplayErrorKey => "NoPublicKeys";
}

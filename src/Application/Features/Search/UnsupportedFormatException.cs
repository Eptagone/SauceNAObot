using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Features.Search;

class UnsupportedFormatException(Message message) : SauceException(message)
{
    public override string DisplayErrorKey => "UnsupportedFormat";
}

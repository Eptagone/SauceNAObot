using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Features.Search;

class TooBigFileException(Message message, Message? sentMessage = null) : SauceException(message)
{
    public Message? SentMessage => sentMessage;
    public override string DisplayErrorKey => "TooBigFile";
}

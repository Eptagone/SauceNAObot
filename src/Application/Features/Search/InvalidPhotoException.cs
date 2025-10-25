using SauceNAO.Core.Exceptions;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Features.Search;

class InvalidPhotoException(Message message) : BotMessageException(message)
{
    public override string DisplayErrorKey => "InvalidPhoto";
}

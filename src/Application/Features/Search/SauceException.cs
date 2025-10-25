using SauceNAO.Core.Exceptions;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Features.Search;

abstract class SauceException(Message message) : BotMessageException(message) { }

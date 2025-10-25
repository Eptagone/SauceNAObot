using SauceNAO.Core.Exceptions;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Application.Features.AntiCheats;

/// <summary>
/// Represents an exception related to anti-cheats.
/// </summary>
abstract class AntiCheatsException(Message message) : BotMessageException(message) { }

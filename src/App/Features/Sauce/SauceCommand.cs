// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Exceptions.Media;
using SauceNAO.Core.Services;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace SauceNAO.App.Features.Sauce;

[
    BotCommand("sauce", "Look for the image sauce", ["source", "snao", "search"]),
    LocalizedBotCommand("es", "salsa", "Buscar la salsa de la imagen"),
]
class SauceCommand(IMediaExtractor mediaExtractor, ISauceHandler handler) : ICommandHandler
{
    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        var similarity = 55;
        if (args.Length > 0)
        {
            if (int.TryParse(args[0], out var sim))
            {
                similarity = Math.Clamp(sim, 10, 90);
            }
        }

        var target =
            await mediaExtractor.DeepExtractAsync(message, cancellationToken)
            ?? throw new InvalidPhotoException(message) { MediaMessage = message };
        await handler.HandleAsync(target, similarity, cancellationToken);
    }
}

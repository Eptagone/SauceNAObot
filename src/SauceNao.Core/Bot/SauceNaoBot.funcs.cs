// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Logging;
using SauceNAO.Core.API;
using SauceNAO.Core.Entities;
using SauceNAO.Core.Models;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.UpdatingMessages;
using File = System.IO.File;

namespace SauceNAO.Core
{
    public partial class SauceNaoBot : AsyncTelegramBotBase<SnaoBotProperties>
    {
        private async Task UpdateSearchMessageAsync(Message message, string newtext, [Optional] InlineKeyboardMarkup keyboard)
        {
            EditMessageTextArgs args = new()
            {
                ChatId = message.Chat.Id,
                MessageId = message.MessageId,
                Text = string.IsNullOrEmpty(newtext) ? "unknown name" : newtext,
                ParseMode = ParseMode.HTML,
                DisableWebPagePreview = true,
                ReplyMarkup = keyboard
            };

            try
            {
                await Api.EditMessageTextAsync<Message>(args).ConfigureAwait(false);
            }
            catch (BotRequestException exp)
            {
                if (exp.Message == "Bad Request: message must be non-empty")
                {
                    args.Text = "unknown name";
                    await Api.EditMessageTextAsync<Message>(args).ConfigureAwait(false);
                }
                else
                {
                    OnBotExceptionAsync(exp, default).Wait();
                }
            }
            catch (Exception exp)
            {
                OnExceptionAsync(exp, default).Wait();
            }
        }
        private bool TryGetFilePath(TargetMedia targetMedia, out string ext)
        {
            var okey = true;
            ext = string.Empty;
            try
            {
                var file = Api.GetFile(targetMedia.TargetFileId);
                targetMedia.FilePath = string.Format(BotClient.BaseFilesUrl, Api.Token, file.FilePath);
                ext = Path.GetExtension(file.FilePath);
            }
            catch (BotRequestException exp)
            {
                okey = false;
                OnBotExceptionAsync(exp, default).Wait();
            }
            return okey;
        }

        [SuppressMessage("Usage", "CA2253:Named placeholders should not be numeric values")]
        private async Task<bool> TryDownloadAsync(TargetMedia target, CancellationToken cancellationToken)
        {
            var okey = true;
            if (TryGetFilePath(target, out string ext))
            {
                var fileName = target.FileUniqueId + ext;
                var tempFile = cache.Files.GetFile(target.FileUniqueId);
                if (tempFile == default)
                {
                    var fileBytes = await httpClient.GetByteArrayAsync(target.FilePath, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var item = new CachedTelegramFile(target.FileUniqueId, fileName, target.ContentType, fileBytes);
                        await cache.Files.InsertAsync(item, cancellationToken).ConfigureAwait(false);
                        target.TemporalFilePath = string.Format(Properties.TempFilesUrl, target.FileUniqueId);
                    }
                    catch (Exception e)
                    {
                        okey = false;
                        logger.LogError("Can't insert new temporal file. Error message. {0}", e.InnerException?.Message ?? e.Message);
                    }
                }
                else
                {
                    target.TemporalFilePath = string.Format(Properties.TempFilesUrl, target.FileUniqueId);
                }
            }
            else
            {
                okey = false;
            }
            return okey;
        }
        private async Task<bool> TryGetImageFromVideo(TargetMedia targetMedia, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetMedia.FilePath))
            {
                if (!TryGetFilePath(targetMedia, out _))
                {
                    return false;
                }
            }
            var fileName = $"{targetMedia.FileUniqueId}.jpg";
            string outputPath = $"{Path.GetTempPath()}{fileName}";
            var tempFile = cache.Files.GetFile(targetMedia.FileUniqueId);

            if (tempFile == default)
            {
                var ffmpegArgs = "-vf \"select=eq(n\\,0)\" -frames:v 1";
                if (Properties.FFmpeg.Run(targetMedia.FilePath, outputPath, ffmpegArgs))
                {
                    targetMedia.TemporalFilePath = string.Format(Properties.TempFilesUrl, targetMedia.FileUniqueId);

                    var fileBytes = await File.ReadAllBytesAsync(outputPath, cancellationToken).ConfigureAwait(false);
                    var item = new CachedTelegramFile(targetMedia.FileUniqueId, fileName, targetMedia.ContentType, fileBytes);
                    await cache.Files.InsertAsync(item, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                targetMedia.TemporalFilePath = string.Format(Properties.TempFilesUrl, targetMedia.FileUniqueId);
            }
            return true;
        }

        [SuppressMessage("Usage", "CA2253:Named placeholders should not be numeric values")]
        private async Task<SauceBowl> CookSauceAsync(string filePath, CancellationToken cancellationToken)
        {
            SauceBowl sauce;
            try
            {
                // Select api key
                var snao = string.IsNullOrEmpty(User.ApiKey)
                    ? Properties.SnaoService // Shared Api Key
                    : new SauceNaoApiService(OutputType.JsonApi, User.ApiKey, db: 999, dedupe: Dedupe.AllImplementedDedupeMethodsSuchAsBySeriesName); // user Api Key
                // Get raw sauce
                var response = await snao.SearchAsync(filePath, cancellationToken).ConfigureAwait(false);
                // Cook sauce
                sauce = new SauceBowl(response);
            }
            catch (SearchResponseException exp)
            {
                logger.LogError("Cooking Error: {0}", exp.InnerException?.Message ?? exp.Message);
                // Cook sauce
                sauce = new SauceBowl(exp);
            }
            return sauce;
        }
    }
}

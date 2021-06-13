// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Telegram.BotAPI;

namespace SauceNao.Services
{
    public partial class SauceNaoBot : TelegramBotAsync
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override async Task OnBotExceptionAsync(BotRequestException exp, CancellationToken cancellationToken)
        {
            if (dateTime == default)
            {
                dateTime = DateTime.Now;
            }
            var st = new StackTrace(exp, true);
            var frames = st.GetFrames()
                            .Select(frame => new
                            {
                                FileName = frame.GetFileName(),
                                LineNumber = (short)frame.GetFileLineNumber()
                            });
            var expData = frames.FirstOrDefault(f => f.FileName != null);
            var jsonUpdate = JsonSerializer.Serialize(cUpdate, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault });
            if (expData == default)
            {
                Logger.LogError(exp, "An exception was generated while the update was being processed with id: {0}.\nUpdate: {4}", cUpdate.UpdateId, jsonUpdate);
            }
            else
            {
                Logger.LogError(exp, "An exception was generated while the update was being processed with id: {0}. File name: {1}, Line number: {2}.\nUpdate: {4}", cUpdate.UpdateId, expData.FileName, expData.LineNumber, jsonUpdate);
            }
            await Task.CompletedTask;
        }
        protected override async Task OnExceptionAsync(Exception exp, CancellationToken cancellationToken)
        {
            if (dateTime == default)
            {
                dateTime = DateTime.Now;
            }
            var st = new StackTrace(exp, true);
            var frames = st.GetFrames()
                            .Select(frame => new
                            {
                                FileName = frame.GetFileName(),
                                LineNumber = (short)frame.GetFileLineNumber()
                            });
            var expData = frames.FirstOrDefault(f => f.FileName != null);
            var jsonUpdate = JsonSerializer.Serialize(cUpdate, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault });
            if (expData == default)
            {
                Logger.LogError(exp, "An exception was generated while the update was being processed with id: {0}.\nUpdate: {1}", cUpdate.UpdateId, jsonUpdate);
            }
            else
            {
                Logger.LogError(exp, "An exception was generated while the update was being processed with id: {0}. File name: {1}, Line number: {2}.\nUpdate: {4}", cUpdate.UpdateId, expData.FileName, expData.LineNumber, jsonUpdate);
            }
            await Task.CompletedTask;
        }
        protected void OnSauceError(string messageText)
        {
            if (!(messageText.Contains("Daily Search Limit Exceeded") || messageText.Contains("Too many failed search attempts") || messageText.Contains("Search Rate Too High")))
            {
                Logger.LogCritical("Sauce Error! Message: {0}", messageText);
            }
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

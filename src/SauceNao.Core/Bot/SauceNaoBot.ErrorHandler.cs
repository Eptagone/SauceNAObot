// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Telegram.BotAPI;

namespace SauceNAO.Core;

public partial class SauceNaoBot : AsyncTelegramBotBase<SnaoBotProperties>
{
	protected override async Task OnBotExceptionAsync(BotRequestException exp, CancellationToken cancellationToken)
	{
		if (this.Date == default)
		{
			this.Date = DateTime.Now;
		}
		var st = new StackTrace(exp, true);
		var frames = st.GetFrames()
						.Select(frame => new
						{
							FileName = frame.GetFileName(),
							LineNumber = (short)frame.GetFileLineNumber()
						});
		var expData = frames.FirstOrDefault(f => f.FileName != null);
		var jsonUpdate = JsonSerializer.Serialize(this.Update, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault });
		if (expData == default)
		{
			this._logger.LogError(exp, "An exception was generated while the update was being processed with id: {updateId}.\nUpdate: {rawUpdate}", this.Update?.UpdateId, jsonUpdate);
		}
		else
		{
			this._logger.LogError(exp, "An exception was generated while the update was being processed with id: {updateId}. File name: {filename}, Line number: {lineNumber}.\nUpdate: {rawUpdate}", this.Update?.UpdateId, expData.FileName, expData.LineNumber, jsonUpdate);
		}

		await Task.CompletedTask;
	}
	protected override async Task OnExceptionAsync(Exception exp, CancellationToken cancellationToken)
	{
		if (this.Date == default)
		{
			this.Date = DateTime.Now;
		}
		var st = new StackTrace(exp, true);
		var frames = st.GetFrames()
						.Select(frame => new
						{
							FileName = frame.GetFileName(),
							LineNumber = (short)frame.GetFileLineNumber()
						});
		var expData = frames.FirstOrDefault(f => f.FileName != null);
		var jsonUpdate = JsonSerializer.Serialize(this.Update, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault });
		if (expData == default)
		{
			this._logger.LogError(exp, "An exception was generated while the update was being processed with id: {updateId}.\nUpdate: {rawUpdate}", this.Update?.UpdateId, jsonUpdate);
		}
		else
		{
			this._logger.LogError(exp, "An exception was generated while the update was being processed with id: {updateId}. File name: {filename}, Line number: {lineNumber}.\nUpdate: {rawUpdate}", this.Update?.UpdateId, expData.FileName, expData.LineNumber, jsonUpdate);
		}

		await Task.CompletedTask;
	}

	protected void OnSauceError(string messageText)
	{
		if (!(messageText.Contains("Daily Search Limit Exceeded") || messageText.Contains("Too many failed search attempts") || messageText.Contains("Search Rate Too High")))
		{
			this._logger.LogCritical("Sauce Error! Message: {message}", messageText);
		}
	}
}

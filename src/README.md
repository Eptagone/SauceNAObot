# Setup

This bot works using a webhook. You can run the webhook locally using [NGROK](https://ngrok.com/).

This solution is composed by **6** projects:

| Name                    | Description                                                             |
| :---------------------- | :---------------------------------------------------------------------- |
| SauceNAO.Core           | Contains the main functionality of the bot                              |
| SauceNAO.Infrastructure | Implements clases for data access                                       |
| SauceNAO.Webhook        | The webhook application. Run the bot using a webhook.                   |
| SauceNAO.Service        | Runs the bot using Long Polling. Some features will be disabled.        |
| SauceNAO.LocalWebhook   | The webhook application (like `SauceNAO.Webhook` but for local running) |
| SauceNAO.Tests          | Unit Tests                                                              |

In order to run anyone of webhook projects, you need to set the following application settings via **json file** (`secrets.json`, `application.json`) or using **enviroment variables**. See the table below that shows the available settings.

| JSON property name        | Enviroment variable          | Apply to         | Description                                                 |
| :------------------------ | :--------------------------- | :--------------- | :---------------------------------------------------------- |
| AccessToken               | AccessToken                  | Both webhooks    | Your webhook secret token specified by you.                 |
| FFmpegExec                | FFmpegExec                   | Both webhooks    | The ffmpeg path executable.                                 |
| SauceNAO:ApiKey           | SauceNAO\_\_ApiKey           | All              | You apikey for SauceNAO API.                                |
| Telegram:BotToken         | Telegram\_\_BotToken         | All              | You bot token.                                              |
| Telegram:SupportChatLink  | Telegram\_\_SupportChatLink  | All              | Support chat link. (<https://t.me/+8NJMCbRmiTk2Yjkx>)       |
| ConnectionStrings:Default | ConnectionStrings\_\_Default | All              | The connection string to database.                          |
| DbProvider                | DbProvider                   | All              | Database provider. Can be 'sqlite' (default) or 'sqlserver' |
| ApplicationUrl            | ApplicationUrl               | SauceNAO.Webhook | Your webhook base url. (<https://example.com>)              |
| Certificate               | Certificate                  | SauceNAO.Webhook | Optional. Certificate path                                  |

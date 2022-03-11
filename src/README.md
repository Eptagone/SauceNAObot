# Setup

This bot works using a webhook. You can run the webhook locally using [NGROK](https://ngrok.com/).

This solution is composed by **6** projects:

| Name                    | Description                                                             |
| :---------------------- | :---------------------------------------------------------------------- |
| SauceNAO.Core           | Contains the main functionality of the bot                              |
| SauceNAO.Infrastructure | Implements clases for data access                                       |
| SauceNAO.Webhook        | The webhook application. Run the bot using a webhook.                   |
| SauceNAO.Servie         | Runs the bot using Long Polling. Some features will be disabled.        |
| SauceNAO.LocalWebhook   | The webhook application (like `SauceNAO.Webhook` but for local running) |
| SauceNAO.Tests          | Unit Tests                                                              |

In order to run anyone of webhook projects, you need to set the following application settings via **json file** (`secrets.json`, `application.json`) or using **enviroment variables**. See the table below that shows the available settings.

| JSON property name         | Enviroment variable           | Apply to              | Description                                           |
| :------------------------- | :---------------------------- | :-------------------- | :---------------------------------------------------- |
| AccessToken                | AccessToken                   | Both webhooks         | Your webhook secret token specified by you.           |
| FFmpegExec                 | FFmpegExec                    | Both webhooks         | The ffmpeg path executable.                           |
| SauceNAO:ApiKey            | SauceNAO\_\_ApiKey            | All                   | You apikey for SauceNAO API.                          |
| Telegram:BotToken          | Telegram\_\_BotToken          | All                   | You bot token.                                        |
| Telegram:SupportChatLink   | Telegram\_\_SupportChatLink   | All                   | Support chat link. (<https://t.me/+8NJMCbRmiTk2Yjkx>) |
| ConnectionStrings:SauceNAO | ConnectionStrings\_\_SauceNAO | All                   | The connection string to database.                    |
| ApplicationUrl             | ApplicationUrl                | SauceNAO.Webhook      | Your webhook base url. (<https://example.com>)        |
| Certificate                | Certificate                   | SauceNAO.Webhook      | Optional. Certificate path                            |
| Ngrok:Port                 | Ngrok\_\_Port                 | SauceNAO.LocalWebhook | Port where your app is running. (7161)                |
| Ngrok:TunnelName           | Ngrok\_\_TunnelName           | SauceNAO.LocalWebhook | Optional. The tunnel name. (SnaoTunnel)               |

> Before running the **SauceNAO.LocalWebhook** project, you have to start **ngrok** in the background. Otherwise, your application will not be able to start. You can use the following command: `ngrok start --none`.

> Currently the bot uses **SQLite** for data storage and caching. If you want to change it to another provider then modify the `Program.cs` file from the Service or Webhooks projects to use another provider compatible with **Entity Framework**. If you don't want to use **Entity Framework**, you must implement a class based on the `ISauceDatabase` interface and register the class in `Program.cs` from your Webhook projects. You can take the `Data/SauceDatabase.cs` class located in the `SauceNAO.Infrastructure` project as an example.

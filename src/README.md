# Setup

This bot works using a webhook. You can run the webhook locally using [NGROK](https://ngrok.com/).

This solution is composed by **5** projects:

| Name                    | Description                                                             |
| :---------------------- | :---------------------------------------------------------------------- |
| SauceNAO.Core           | Contains the main functionality of the bot                              |
| SauceNAO.Infrastructure | Implements clases for data access                                       |
| SauceNAO.Webhook        | The webhook application                                                 |
| SauceNAO.LocalWebhook   | The webhook application (like `SauceNAO.Webhook` but for local running) |
| SauceNAO.Tests          | Unit Tests                                                              |

In order to run anyone of webhook projects, you need to set the following application settings via **json file** (`secrets.json`, `application.json`) or using **enviroment variables**. See the table below that shows the available settings.

| JSON property name         | Enviroment variable           | Apply to              | Description                                         |
| :------------------------- | :---------------------------- | :-------------------- | :-------------------------------------------------- |
| AccessToken                | AccessToken                   | Both                  | Your webhook secret token specified by you.         |
| FFmpegExec                 | FFmpegExec                    | Both                  | The ffmpeg path executable.                         |
| SauceNAO:ApiKey            | SauceNAO\_\_ApiKey            | Both                  | You apikey for SauceNAO API.                        |
| Telegram:BotToken          | Telegram\_\_BotToken          | Both                  | You bot token.                                      |
| Telegram:SupportChatLink   | Telegram\_\_SupportChatLink   | Both                  | Support chat link. (https://t.me/+8NJMCbRmiTk2Yjkx) |
| ConnectionStrings:SauceNAO | ConnectionStrings\_\_SauceNAO | Both                  | The connection string to database.                  |
| AplicationUrl              | AplicationUrl                 | SauceNAO.Webhook      | Your webhook base url. (https://example.com)        |
| Ngrok:Port                 | Ngrok\_\_Port                 | SauceNAO.LocalWebhook | Port where your app is running. (7161)              |
| Ngrok:TunnelName           | Ngrok\_\_TunnelName           | SauceNAO.LocalWebhook | Optional. The tunnel name. (SnaoTunnel)             |

> Before running the **SauceNAO.LocalWebhook** project, you have to start **ngrok** in the background. Otherwise, your application will not be able to start. You can use the following command: `ngrok start --none `.

> Currently the bot uses **SQLite** for data storage and also for caching information. If you want to change it to another provider, delete the `Migrations` folder from `SauceNAO.Infrastructure`, modify the `Program.cs` file from the Webhooks projects to use another provider compatible with **Entity Framework**, and finally create new migrations. If you don't want to use **Entity Framework**, you must implement a class based on the `IBotDb` interface and register the class in `Program.cs` from your Webhook projects. You can take the `Data/BotDb.cs` class located in the `SauceNAO.Infrastructure` project as an example.

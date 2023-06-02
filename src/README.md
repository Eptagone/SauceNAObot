# Setup

This bot works using a webhook. You can run the webhook locally using [NGROK](https://ngrok.com/).

This solution is composed by **6** projects:

| Name                    | Description                                                      |
| :---------------------- | :--------------------------------------------------------------- |
| SauceNAO.Core           | Contains the main functionality of the bot                       |
| SauceNAO.Infrastructure | Implements clases for data access                                |
| SauceNAO.WebApp         | The web application. Allows you to configure a webhook.          |
| SauceNAO.AppService     | Runs the bot using Long Polling. Some features will be disabled. |
| SauceNAO.Tests          | Unit Tests                                                       |

In order to run anyone of webhook projects, you need to set the following application settings via **json file** (`secrets.json`, `application.json`) or using **enviroment variables**. See the table below that shows the available settings.

| JSON property name        | Enviroment variable          | Apply to | Description                                                  |
| :------------------------ | :--------------------------- | :------- | :----------------------------------------------------------- |
| ConnectionStrings:Default | ConnectionStrings\_\_Default | All      | The connection string to database.                           |
| DbProvider                | DbProvider                   | All      | Database provider. Can be 'sqlite' (default) or 'sqlserver'  |
| SNAO:ApiKey               | SNAO\_\_ApiKey               | All      | You apikey for SauceNAO API.                                 |
| SNAO:BotToken             | SNAO\_\_BotToken             | All      | You bot token.                                               |
| SNAO:SupportChatLink      | SNAO\_\_SupportChatLink      | All      | Support chat link. (<https://t.me/+8NJMCbRmiTk2Yjkx>)        |
| SNAO:ApplicationUrl       | SNAO\_\_ApplicationUrl       | WebApp   | Optional. Your application base url. (<https://example.com>) |
| SNAO:FFmpegExec           | SNAO\_\_FFmpegExec           | WebApp   | Optional. The ffmpeg path executable.                        |
| SNAO:SecretToken          | SNAO\_\_SecretToken          | WebApp   | Optional. Your secret token to enable webhook.               |
| SNAO:CertificatePath      | SNAO\_\_CertificatePath      | WebApp   | Optional. Certificate path                                   |
| SNAO:DropPendingUpdates   | SNAO\_\_DropPendingUpdates   | WebApp   | Optional. Drop pending updates.                              |

## Local Mode

Local mode is a special mode that allows you to run the bot without a web server. This mode is useful for testing purposes.

> The AppService project will always run in local mode.
> The WebApp will run in local mode if the `ApplicationUrl`, `SecretToken` or `FFmpegExec` settings are not set.

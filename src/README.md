# How to run the bot

The bot use a webhook to receive messages by default and also for generating temporal files. If you decide to not use a webhook, the bot can still work using long polling but with some limitations. See the [Settings](#settings) section to see the required settings for the bot.

## Settings

This bot can work using long polling or a webhook. If you want to run the bot at it's full capacity, it's recommended to run it as a webhook. It can also work with the [Local Bot API Server](https://core.telegram.org/bots/api#using-a-local-bot-api-server) (LBAS)

In order to configure the bot, you must set the following application settings.

> Replace `:` with `__` when using enviroment variables.

| Property name                     | Required           | Description                                          |
| :-------------------------------- | :----------------- | :--------------------------------------------------- |
| ConnectionStrings:Default         | Yes                | The connection string to the SQL database.           |
| TelegramBot:BotToken              | Yes                | Your bot token.                                      |
| TelegramBot:ServerAddress         | When using LBAS    | The local server URL for bot requests.               |
| TelegramBot:WebhookUrl            | When using webhook | Your webhook url accessible by the bot api server.   |
| TelegramBot:SecretToken           | When using webhook | Secret token for the webhook.                        |
| General:ApplicationUrl            | When using webhook | Your application url. (<https://example.com>)        |
| General:SupportChatInvitationLink | Yes                | Support chat invite link.                            |
| General:FilesPath                 | No                 | The path where the files are stored when using LBAS. |
| General:FFmpegPath                | No                 | The ffmpeg path executable.                          |

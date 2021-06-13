# SauceNAObot

The bot works using a webhook.

If you want to try this bot locally, I recommend using a tool like [NGROK](https://ngrok.com/).

# Setup

Before you test this project and deploy it to your own bot, you'll need to edit appsettings.json file or your user secrets.json file to make it work.

Open appsettings.json or your secrets.json and specify the follow informatión:

- LiteDB is your connection string to your sqlite database. Please specify a valid filename.
- ApiKey is the api key of your Saucenao account, you can keep it empty or include your key to increase reverse searches.
- BotToken is the bot token, provided by BotFathe.
- SecretToken is a secret path, your webhook's token. We recommend that you use random characters and not provide this key to anyone.
- WebhookUrl is the Webhook's URL. Example: https://saucenao.com/

## appsettings.json, secrets.json

```json
{
  "ConnectionStrings": {
    "LiteDB": "Filename=saucenaobot-SECRET-GUID.db"
  },
  "SauceNao": {
    "ApiKey": "<SAUCENAO-API-KEY>",
    "BotToken": "<BOT-TOKEN>",
    "SecretToken": "<SECRET-TOKEN>",
    "WebhookUrl": "https://example.com/"
  }
}
```

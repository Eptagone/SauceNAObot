# SauceNAObot

The bot is separated into two projects:

- SauceNAO Bot DLL (Bot Logic)
- SauceNAO Bot Webhook (Webhook)

The bot works using a webhook. You cannot use polling without removing features from your bot.

If you want to try this bot locally, I recommend using a tool like [NGROK](https://ngrok.com/).

# Setup

Before you test this project and deploy it to your own bot, you'll need to edit some files to make it work.

Open the SauceNAObot.cs file, located in "SauceNAO Bot DLL". Locates the BotToken and PublicApiKey properties.

- BotToken is the bot token, provided by BotFather, you need to replace it with your token.
- PublicApiKey is the api key of your Saucenao account, you can leave it empty or include your key to increase reverse searches.

Open the Settings.cs file, located in "SauceNAO Bot Webhook". Locates the SecurityToken and BaseURL properties.

- SecurityToken is a secret path, your webhook's token. We recommend that you use random characters and not provide this key to anyone.
- BaseURL is the Webhook's URL. Example: https://saucenaobot.com/

# SauceNAObot

El bot esta separado en dos proyectos:

- SauceNAO Bot DLL (Logica del bot)
- SauceNAO Bot Webhook (Webhook)

El bot funciona usando un webhook. No es posible utilizar sondeo sin remover características al bot.

Si quiere probar este bot localmente, recomiendo utilizar una herramienta como [NGROK](https://ngrok.com/).

# Configurar

Antes de probar el proyecto e implementarlo en tu propio bot, deberás editar algunos archivos para que funcione.

Abra el archivo SauceNAObot.cs, ubicado en "SauceNAO Bot DLL". Busca las propiedades BotToken y PublicApiKey.

- BotToken es el token del bot, proporcionado por BotFather, debe reemplazarlo por el suyo.
- PublicApiKey es la clave api de tu cuenta de Saucenao, puedes dejarla vacía o incluir tu clave para aumentar las búsquedas inversas.

Abra el archivo Settings.cs, ubicado en "SauceNAO Bot Webhook". Busca las propiedades SecurityToken y BaseURL.

- SecurityToken es una ruta de acceso secreta, el token del webhook. Es recomendable que utilice caracteres aleatorios y no proporcione esta clave a nadie.
- BaseURL es la URL del webhook. Ejemplo: https://saucenaobot.com/

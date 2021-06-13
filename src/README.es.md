# SauceNAObot

El bot funciona usando un webhook. 

Si quiere probar este bot localmente, recomiendo utilizar una herramienta como [NGROK](https://ngrok.com/).

# Configurar

Antes de probar el proyecto e implementarlo en tu propio bot, deberás editar el archivo appsettings.json or tu archivo secrets.json de usuario paraa hacer que el bot funcione.

Abre appsettings.json o secrets.json y especifica la siguiente información:

- LiteDB es tu cadena de conexión a la base de datos de sqlite. Por favor especifica un nombre de archivo valido.
- ApiKey es la clave de api de tu cuenta de Saucenao, puedes dejarla vacia o incluir tu clave para incrementar el numero de busquedas.
- BotToken es el token del bot, proporcionado por BotFather.
- SecretToken es tu ruta secreta, tu token del webhook. Se recomienda usar caracteres aleatorios y no proporcionar la clave a nadie.
- WebhookUrl es la la direccion web de tu Webhook. ejemplo: https://saucenao.com/

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

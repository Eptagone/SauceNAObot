# Configuración

Este bot funciona utilizando un Webhook. Usted puede ejecutar el Webhook localmente usando [NGROK](https://ngrok.com/).

Esta solución esta compuesta por **6** proyectos:

| Nombre                  | Descripción                                                               |
| :---------------------- | :------------------------------------------------------------------------ |
| SauceNAO.Core           | Contiene la funcionalidad principal del bot                               |
| SauceNAO.Infrastructure | Implementa clases para el acceso a datos                                  |
| SauceNAO.Webhook        | La aplicación Webhook. Ejecut el bot usando un webhook.                   |
| SauceNAO.Service        | Ejecuta el bot usando sondeo. Algunas características son deshabilitadas. |
| SauceNAO.LocalWebhook   | La aplicación Webhook (como `SauceNAO.Webhook` pero para ejecución local) |
| SauceNAO.Tests          | Pruebas unitarias                                                         |

Para ejecutar cualquiera de los proyectos, usted necesita establecer la siguiente configuración de la aplicación meidiante un **archivo json** (`secrets.json`, `application.json`) o usando **variables de entorno**.

| Nombre de la propiedad JSON | Variable de entorno          | Aplica a            | Descripción                                                                    |
| :-------------------------- | :--------------------------- | :------------------ | :----------------------------------------------------------------------------- |
| AccessToken                 | AccessToken                  | Webhooks            | El token secreto del webhook especificado por usted.                           |
| FFmpegExec                  | FFmpegExec                   | Webhooks            | La ruta al ejecutable de **ffmpeg**.                                           |
| SauceNAO:ApiKey             | SauceNAO\_\_ApiKey           | Todos los proyectos | La clave de api para usar la SauceNAO API.                                     |
| Telegram:BotToken           | Telegram\_\_BotToken         | Todos los proyectos | El token del bot.                                                              |
| Telegram:SupportChatLink    | Telegram\_\_SupportChatLink  | Todos los proyectos | Link al chat de ayuda. (<https://t.me/+8NJMCbRmiTk2Yjkx>)                      |
| ConnectionStrings:Default   | ConnectionStrings\_\_Default | Todos los proyectos | La cadena de conexión a la base de datos.                                      |
| DbProvider                  | DbProvider                   | All                 | Proveedor de la base de datos. Puede ser 'sqlite' (por defecto) o 'sqlserver'. |
| ApplicationUrl              | ApplicationUrl               | SauceNAO.Webhook    | La dirección base del webhook. (<https://example.com>)                         |
| Certificate                 | Certificate                  | SauceNAO.Webhook    | Opcional. Ruta del certificado.                                                |

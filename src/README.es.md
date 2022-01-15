# Configuración

Este bot funciona utilizando un Webhook. Usted puede ejecutar el Webhook localmente usando [NGROK](https://ngrok.com/).

Esta solución esta compuesta por **5** proyectos:

| Nombre                  | Descripción                                                               |
| :---------------------- | :------------------------------------------------------------------------ |
| SauceNAO.Core           | Contiene la funcionalidad principal del bot                               |
| SauceNAO.Infrastructure | Implementa clases para el acceso a datos                                  |
| SauceNAO.Webhook        | La aplicación Webhook                                                     |
| SauceNAO.LocalWebhook   | La aplicación Webhook (como `SauceNAO.Webhook` pero para ejecución local) |
| SauceNAO.Tests          | Pruebas unitarias                                                         |

Para ejecutar cualquiera de los proyectos de Webhook, usted necesita establecer la siguiente configuración de la aplicación meidiante un **archivo json** (`secrets.json`, `application.json`) o usando **variables de entorno**.

| Nombre de la propiedad JSON | Variable de entorno           | Aplica a              | Descripción                                             |
| :-------------------------- | :---------------------------- | :-------------------- | :------------------------------------------------------ |
| AccessToken                 | AccessToken                   | Ambos proyectos       | El token secreto del webhook especificado por usted.    |
| FFmpegExec                  | FFmpegExec                    | Ambos proyectos       | La ruta al ejecutable de **ffmpeg**.                    |
| SauceNAO:ApiKey             | SauceNAO\_\_ApiKey            | Ambos proyectos       | La clave de api para usar la SauceNAO API.              |
| Telegram:BotToken           | Telegram\_\_BotToken          | Ambos proyectos       | El token del bot.                                       |
| Telegram:SupportChatLink    | Telegram\_\_SupportChatLink   | Ambos proyectos       | Link al chat de ayuda. (https://t.me/+8NJMCbRmiTk2Yjkx) |
| ConnectionStrings:SauceNAO  | ConnectionStrings\_\_SauceNAO | Ambos proyectos       | La cadena de conexión a la base de datos.               |
| AplicationUrl               | AplicationUrl                 | SauceNAO.Webhook      | La dirección base del webhook. (https://example.com)    |
| Ngrok:Port                  | Ngrok\_\_Port                 | SauceNAO.LocalWebhook | Puerto donde se ejecuta la aplicación. (7161)           |
| Ngrok:TunnelName            | Ngrok\_\_TunnelName           | SauceNAO.LocalWebhook | Opcional. Nombre del tunnel. (SnaoTunnel)               |

> Antes de ejecutar el proyecto **SauceNAO.LocalWebhook**, usted necesita iniciar **ngrok** en segundo plano. De lo contrario, la aplicación no podrá iniciar. Puede usar el siguiente comando: `ngrok start --none `.

> Actualmente, el bot usa **SQLite** para el almacenamiento de datos y también para almacenar información en caché. Si desea cambiarlo a otro proveedor, elimine la carpeta `Migrations` de `SauceNAO.Infraestructura`, modifique el archivo `Program.cs` de los proyectos de Webhooks para usar otro proveedor compatible con **Entity Framework**, y finalmente crear nuevas migraciones. Si no desea utilizar **Entity Framework**, debe implementar una clase basada en la interfaz `IBotDb` y registrar la clase en `Program.cs` desde sus proyectos Webhook. Puede tomar la clase `Data/BotDb.cs` ubicada en el proyecto `SauceNAO.Infrastructure` como ejemplo.

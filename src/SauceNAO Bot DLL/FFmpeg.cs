// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SauceNAO
{
    public static class FFmpeg
    {
        // Convierte
        internal static string Mp4toGIF(string mp4filepath, [Optional] string args)
        {
            string tmppath = Path.GetFileName(mp4filepath).Replace(".mp4", ".gif");
            string outputpath = $"{Path.GetTempPath()}{tmppath}";
            // Si el archivo ya existe entonces regresar la ruta al mismo.
            if (File.Exists(outputpath))
            {
                return string.Format(Utilities.TempFilesPath, tmppath);
            }

            using Process convtask = new Process();
            string video = mp4filepath;
            convtask.StartInfo.FileName = $"FFmpeg\\ffmpeg.exe";
            if (string.IsNullOrEmpty(args))
            {
                convtask.StartInfo.Arguments = $" -i \"{video}\" -loop 0 \"{outputpath}\"";
            }
            else
            {
                convtask.StartInfo.Arguments = $" -i \"{video}\" -loop 0 {args} \"{outputpath}\"";
            }

            convtask.StartInfo.UseShellExecute = false;
            convtask.StartInfo.RedirectStandardInput = true;
            convtask.StartInfo.RedirectStandardOutput = true;
            convtask.Start(); // start !
            convtask.WaitForExit();
            if (File.Exists(outputpath))
            {
                return string.Format(Utilities.TempFilesPath, tmppath);
            }
            else
            {
                return string.Empty;
            }
        }
        internal static async Task<string> VideoToImage(string vfilepath, string nfilename)
        {
            string ext = Path.GetExtension(vfilepath);
            string inputpath = $"{Path.GetTempPath()}{nfilename}.{ext}";
            string outputpath = $"{Path.GetTempPath()}{nfilename}.jpg";
            // Si el archivo ya existe entonces regresar la ruta al mismo.
            if (File.Exists(outputpath))
            {
                return string.Format(Utilities.TempFilesPath, $"{nfilename}.jpg");
            }
            // Descargar archivo y guardar
            byte[] targetfile = await Utilities.DownloadFileAsync(new System.Uri(vfilepath)).ConfigureAwait(false);
            await Utilities.WriteOnDisk(inputpath, targetfile).ConfigureAwait(false);
            using Process convtask = new Process();
            convtask.StartInfo.FileName = $"FFmpeg\\ffmpeg.exe";
            convtask.StartInfo.Arguments = $" -i \"{inputpath}\" -vf \"select=eq(n\\,0)\" -frames:v 1 \"{outputpath}\"";
            convtask.StartInfo.UseShellExecute = false;
            convtask.StartInfo.RedirectStandardInput = true;
            convtask.StartInfo.RedirectStandardOutput = true;
            convtask.Start(); // start !
            convtask.WaitForExit();
            if (File.Exists(outputpath))
            {
                return string.Format(Utilities.TempFilesPath, $"{nfilename}.jpg");
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

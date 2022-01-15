using System.Diagnostics;

namespace SauceNAO.Core.Tools
{
    /// <summary>That class allows you to use the ffmpeg tool.</summary>
    public sealed class FFmpeg
    {
        private readonly string _ffmpegPath;
        /// <summary>Initialize a new instance of FFmpeg class.</summary>
        /// <param name="ffmpegPath">The ffmpeg.exe file path.</param>
        public FFmpeg(string ffmpegPath)
        {
            _ffmpegPath = ffmpegPath;
        }
        /// <summary>Run ffmpeg command</summary>
        /// <param name="input">Input file name.</param>
        /// <param name="output">Output file name.</param>
        /// <param name="arguments">Arguments</param>
        public bool Run(string input, string output, params string[] arguments)
        {
            return Run($"-i \"{input}\" {string.Join(' ', arguments)} \"{output}\"");
        }

        /// <summary>Run ffmpeg command.</summary>
        /// <param name="arguments">Arguments.</param>
        public bool Run(string arguments)
        {
            return Run(arguments, out _);
        }

        /// <summary>Run ffmpeg command.</summary>
        /// <param name="arguments">Arguments.</param>
        /// <param name="outputLog">Output log.</param>
        public bool Run(string arguments, out string outputLog)
        {
            outputLog = string.Empty;
            using Process convtask = new();
            convtask.StartInfo.FileName = _ffmpegPath;
            convtask.StartInfo.Arguments = arguments;
            convtask.StartInfo.UseShellExecute = false;
            convtask.StartInfo.RedirectStandardOutput = true;
            try
            {
                convtask.Start(); // start !
                convtask.WaitForExit();
                outputLog = convtask.StandardOutput.ReadToEnd();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FacebookProducer
{
    public class ScriptExecutor
    {
        public static Task<string> Execute(
            string command,
            string fileName,
            List<string> args)
        {
            args.Insert(0, fileName);

            var startInfo = CreateProcessStartInfo(command, args);
            
            using Process process = Process.Start(startInfo);

            return process?.StandardOutput.ReadToEndAsync();
        }

        private static ProcessStartInfo CreateProcessStartInfo(
            string command, IEnumerable<string> args)
        {
            return new()
            {
                FileName = command,
                Arguments = string.Join(' ', args),
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
        }
    }
}
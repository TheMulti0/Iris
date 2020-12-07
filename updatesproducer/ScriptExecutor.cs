using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace UpdatesProducer
{
    public static class ScriptExecutor
    {
        public static Task<string> ExecutePython(
            string fileName,
            params object[] parameters)
        {
            return Execute("python", fileName, parameters);
        }
        
        public static async Task<string> Execute(
            string command,
            string fileName,
            params object[] parameters)
        {
            var arguments = new[] { fileName }
                .Concat(
                    parameters.Select(o => o.ToString()));
            
            ProcessStartInfo startInfo = CreateProcessStartInfo(
                command,
                arguments);
            
            using Process process = Process.Start(startInfo);

            string output = await process?.StandardOutput?.ReadToEndAsync();
            if (string.IsNullOrEmpty(output))
            {
                throw new InvalidOperationException("Failed to execute script (no output)");
            }

            return output;
        }

        private static ProcessStartInfo CreateProcessStartInfo(
            string command,
            IEnumerable<string> args)
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
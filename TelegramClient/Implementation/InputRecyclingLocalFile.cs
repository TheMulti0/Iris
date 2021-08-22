using System;
using System.IO;
using TdLib;

namespace TelegramClient
{
    public sealed class InputRecyclingLocalFile : TdApi.InputFile.InputFileLocal, IDisposable
    {
        public InputRecyclingLocalFile(string filePath)
        {
            Path = filePath;
        }

        public void Dispose()
        {
            File.Delete(Path);
        }
    }
}
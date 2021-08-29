namespace TelegramClient
{
    internal record StartupState(bool ParametersSet, bool DatabaseEncryptionKeyChecked, bool IsReady)
    {
        public bool IsComplete => ParametersSet && DatabaseEncryptionKeyChecked && IsReady;
    }
}
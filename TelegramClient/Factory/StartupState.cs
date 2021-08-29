namespace TelegramClient
{
    internal record StartupState(bool ParametersSet, bool DatabaseEncryptionKeyChecked)
    {
        public bool IsComplete => ParametersSet && DatabaseEncryptionKeyChecked;
    }
}
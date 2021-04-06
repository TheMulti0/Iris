using System.Linq;
using System.Threading.Tasks;
using TdLib;

namespace TelegramClient
{
    public class TelegramClientFactory
    {
        private readonly TelegramClientConfig _config;
        private readonly TdClient _client = new();
        private readonly TdApi.TdlibParameters _tdlibParameters;

        public TelegramClientFactory(TelegramClientConfig config)
        {
            _config = config;
            
            _tdlibParameters = new TdApi.TdlibParameters
            {
                ApiId = _config.AppId,
                ApiHash = _config.AppHash,
                ApplicationVersion = "1.6.0",
                DeviceModel = "PC",
                SystemLanguageCode = "en",
                SystemVersion = "Win 10.0"
            };
        }

        public async Task<ITelegramClient> CreateAsync()
        {
            await _client.SetLogStreamAsync(new TdApi.LogStream.LogStreamEmpty());
            await _client.SetLogVerbosityLevelAsync(0);

            var startupState = new StartupState(false, false);
            
            await foreach (TdApi.Update update in _client.OnUpdateReceived().ToAsyncEnumerable())
            {
                startupState = await UpdateStartupStateAsync(update, startupState);
                
                if (startupState.IsComplete)
                {
                    break;
                }
            }
            
            await AuthenticateAsync();

            return new TelegramClient(_client);
        }

        private async Task<StartupState> UpdateStartupStateAsync(TdApi.Update update, StartupState state)
        {
            switch (update)
            {
                case TdApi.Update.UpdateAuthorizationState authState when authState.AuthorizationState.GetType() == typeof(TdApi.AuthorizationState.AuthorizationStateWaitTdlibParameters):
                    await _client.SetTdlibParametersAsync(_tdlibParameters);
                    
                    return state with { ParametersSet = true };
                
                case TdApi.Update.UpdateAuthorizationState authState when authState.AuthorizationState.GetType() == typeof(TdApi.AuthorizationState.AuthorizationStateWaitEncryptionKey):
                    await _client.CheckDatabaseEncryptionKeyAsync();
                    
                    return state with { DatabaseEncryptionKeyChecked = true };
            }

            return state;
        }

        private async Task AuthenticateAsync() => await _client.CheckAuthenticationBotTokenAsync(_config.BotToken);
    }
}
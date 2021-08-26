using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TdLib;

namespace TelegramClient
{
    public class TelegramClientFactory
    {
        private readonly TelegramClientConfig _config;
        private readonly TdClient _client = new();
        private readonly TdApi.TdlibParameters _tdlibParameters;
        private readonly ILogger<TelegramClientFactory> _logger;

        public TelegramClientFactory(
            TelegramClientConfig config,
            ILogger<TelegramClientFactory> logger)
        {
            _config = config;
            _logger = logger;

            _tdlibParameters = new TdApi.TdlibParameters
            {
                ApiId = _config.AppId,
                ApiHash = _config.AppHash,
                ApplicationVersion = "1.6.0",
                DeviceModel = "PC",
                SystemLanguageCode = "en",
                SystemVersion = "Win 10.0"
            };

            _client.SetLogStreamAsync(new TdApi.LogStream.LogStreamEmpty()).Wait();
            _client.SetLogVerbosityLevelAsync(0).Wait();
        }

        public async Task<ITelegramClient> CreateAsync()
        {
            try
            {
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

                TdApi.User me = await _client.GetMeAsync();
                _logger.LogInformation("Logged in as {}", me.Username);

                return new TelegramClient(_client, _config);
            }
            catch (TdException e)
            {
                _logger.LogError(e, "Failed to authenticate");
                throw;
            }
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
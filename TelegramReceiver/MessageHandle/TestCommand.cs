using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramReceiver.Data;

namespace TelegramReceiver
{
    internal class TestCommand : INewCommand
    {
        private readonly IConnectionsRepository _repository;
        private readonly ITelegramBotClient _client;

        public TestCommand(
            IConnectionsRepository repository,
            Context context)
        {
            _repository = repository;
            _client = context.Client;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            await Task.Delay(-1);
            Console.WriteLine("Yeah");
            return new RedirectResult(Route.Test);
        }
    }
}